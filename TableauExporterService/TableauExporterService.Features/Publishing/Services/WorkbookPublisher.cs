using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public sealed class WorkbookPublisher : IWorkbookPublisher
{
    private const string ApiVersion = "3.25";

    private readonly ITableauAuthenticator authenticator;
    private readonly HttpClient http;
    private readonly IConfiguration cfg;
    private readonly ILogger<WorkbookPublisher> logger;
    private readonly AsyncRetryPolicy retryPolicy;

    public WorkbookPublisher(
        ITableauAuthenticator authenticator,
        IConfiguration cfg,
        ILogger<WorkbookPublisher> logger)
    {
        this.authenticator = authenticator;
        this.cfg = cfg;
        this.logger = logger;

        http = this.authenticator.HttpClient;

        retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2),
                (ex, ts, retryCount, ctx) =>
                {
                    this.logger.LogWarning(ex, "WorkbookPublish спроба {RetryCount} завершилась помилкою, ждемо {Delay}s",
                        retryCount, ts.TotalSeconds);
                });
    }

    public async Task<string> PublishWorkbookAsync(
        string workbookName,
        string datasourceContentUrl,
        string datasourceName,
        bool overwrite,
        CancellationToken cancellationToken)
    {
        await authenticator.EnsureAuthenticatedAsync(cancellationToken);
        var siteId = authenticator.SiteId;
        var token = authenticator.AuthToken;
        var projectId = cfg["Tableau:ProjectId"]
                        ?? throw new ArgumentException("Tableau:ProjectId не конфігурований");

        var twbTemplatePath = cfg["Tableau:TwbTemplatePath"]
                              ?? throw new ArgumentException("Tableau:TwbTemplatePath не налаштований");
        if (!File.Exists(twbTemplatePath))
            throw new FileNotFoundException($"Шаблон .twb не знайдено: {twbTemplatePath}");

        var twbTemplate = await File.ReadAllTextAsync(twbTemplatePath, cancellationToken);

        var escapedWorkbookName = SecurityElement.Escape(workbookName);
        var escapedDataBaseLink = SecurityElement.Escape(datasourceContentUrl);
        var escapedDataBaseName = SecurityElement.Escape(datasourceName);

        var twbXml = twbTemplate
            .Replace("{WorkbookName}", escapedWorkbookName)
            .Replace("{DataBaseLink}", escapedDataBaseLink)
            .Replace("{DataBaseName}", escapedDataBaseName);

        var twbBytes = Encoding.UTF8.GetBytes(twbXml);

        var tsRequest = new XElement("tsRequest",
            new XElement("workbook",
                new XAttribute("name", workbookName),
                new XElement("project", new XAttribute("id", projectId))
            )
        ).ToString(SaveOptions.DisableFormatting);

        var boundary = $"----tbl-{Guid.NewGuid():N}";
        var multipart = new MultipartContent("mixed", boundary);

        var xmlContent = new StringContent(tsRequest, Encoding.UTF8, "text/xml");
        xmlContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "request_payload"
        };
        multipart.Add(xmlContent);

        var fileContent = new ByteArrayContent(twbBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "tableau_workbook",
            FileName = $"{workbookName}.twb"
        };
        multipart.Add(fileContent);

        var fullBuffer = await multipart.ReadAsByteArrayAsync(cancellationToken);

        var uri = $"/api/{ApiVersion}/sites/{siteId}/workbooks?skipConnectionCheck=true";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri);
        httpRequest.Headers.Add("X-Tableau-Auth", token);

        httpRequest.Content = new ByteArrayContent(fullBuffer);
        httpRequest.Content.Headers.Remove("Content-Type");
        httpRequest.Content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/mixed; boundary={boundary}");
        httpRequest.Content.Headers.ContentLength = fullBuffer.Length;

        logger.LogInformation("WorkbookPublisher: відправляємо .twb ({Length} байт) до {Uri}", fullBuffer.Length, uri);

        return await retryPolicy.ExecuteAsync(async () =>
        {
            using var response = await http.SendAsync(httpRequest, cancellationToken);
            var respBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("WorkbookPublisher: помилка публікації воркбука {WorkbookName}. Статус {Status}, Тіло:{Body}",
                    workbookName, response.StatusCode, respBody);
                response.EnsureSuccessStatusCode();
            }

            var xdoc = XDocument.Parse(respBody);
            XNamespace ts = "http://tableau.com/api";

            var workbookElem = xdoc.Root.Element(ts + "workbook")
                              ?? throw new InvalidOperationException("У відповіді не знайдено елемент <workbook>.");
            var workbookContentUrl = workbookElem.Attribute("contentUrl")?.Value
                                     ?? throw new InvalidOperationException("workbook.contentUrl не знайдено!");

            var viewsElem = workbookElem.Element(ts + "views")
                            ?? throw new InvalidOperationException("workbook.views не знайдено!");
            var sheetElem = viewsElem.Elements(ts + "view")
                             .FirstOrDefault(v => (string)v.Attribute("name") == "Дашборд")
                             ?? throw new InvalidOperationException("Не знайдено <view name='Дашборд'>!");
            var viewUrlName = sheetElem.Attribute("viewUrlName")?.Value
                              ?? throw new InvalidOperationException("view.viewUrlName не знайдено!");

            var webpageUrl = workbookElem.Attribute("webpageUrl")?.Value
                             ?? throw new InvalidOperationException("webpageUrl у відповіді не знайдено!");

            var sitePart = "";
            var match = System.Text.RegularExpressions.Regex.Match(webpageUrl, @"/site/([^/]+)/");
            if (match.Success)
                sitePart = match.Groups[1].Value;

            var dashboardViewUrl = $"https://{cfg["Tableau:BaseUrl"]?.Replace("https://", "").Split('/')[0]}" +
                                   $"/#/site/{sitePart}/views/{workbookContentUrl}/{viewUrlName}";

            logger.LogInformation("WorkbookPublisher: успішно опубліковано воркбук '{Name}', Dashboard URL={DashboardUrl}",
                workbookName, dashboardViewUrl);

            return dashboardViewUrl;
        });
    }
}
