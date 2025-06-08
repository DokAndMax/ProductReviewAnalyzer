using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public sealed class DatasourcePublisher : IDatasourcePublisher
{
    private const string ApiVersion = "3.25";

    private readonly ITableauAuthenticator authenticator;
    private readonly HttpClient http;
    private readonly IConfiguration cfg;
    private readonly ILogger<DatasourcePublisher> logger;
    private readonly AsyncRetryPolicy retryPolicy;

    public DatasourcePublisher(
        ITableauAuthenticator authenticator,
        IConfiguration cfg,
        ILogger<DatasourcePublisher> logger)
    {
        this.authenticator = authenticator;
        this.cfg = cfg;
        this.logger = logger;

        http = this.authenticator.HttpClient;

        retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                onRetry: (ex, ts, retryCount, ctx) =>
                {
                    this.logger.LogWarning(ex, "DatasourcePublish спроба {RetryCount} завершилась помилкою, чекаємо {Delay}s",
                        retryCount, ts.TotalSeconds);
                });
    }

    public async Task<string> PublishHyperAsync(
        string projectName,
        string datasourceName,
        byte[] hyperBytes,
        CancellationToken cancellationToken)
    {
        await authenticator.EnsureAuthenticatedAsync(cancellationToken);
        var siteId = authenticator.SiteId;
        var token = authenticator.AuthToken;
        var projectId = cfg["Tableau:ProjectId"]
                        ?? throw new ArgumentException("Tableau:ProjectId не конфігурований");

        var boundary = $"----tbl-{Guid.NewGuid():N}";
        var multipart = new MultipartContent("mixed", boundary);

        var xmlPayload = new XElement("tsRequest",
            new XElement("datasource",
                new XAttribute("name", datasourceName),
                new XElement("project", new XAttribute("id", projectId))
            )
        ).ToString(SaveOptions.DisableFormatting);

        var xmlContent = new StringContent(xmlPayload, Encoding.UTF8, "text/xml");
        xmlContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "request_payload"
        };
        multipart.Add(xmlContent);

        var fileContent = new ByteArrayContent(hyperBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "tableau_datasource",
            FileName = $"{datasourceName}.hyper"
        };
        multipart.Add(fileContent);

        var fullBuffer = await multipart.ReadAsByteArrayAsync(cancellationToken);

        var requestUri = $"/api/{ApiVersion}/sites/{siteId}/datasources?overwrite=true";
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
        httpRequest.Headers.Add("X-Tableau-Auth", token);

        httpRequest.Content = new ByteArrayContent(fullBuffer);
        httpRequest.Content.Headers.Remove("Content-Type");
        httpRequest.Content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/mixed; boundary={boundary}");
        httpRequest.Content.Headers.ContentLength = fullBuffer.Length;

        logger.LogInformation("DatasourcePublisher: відправляємо .hyper({Length} байт) до {RequestUri}", fullBuffer.Length, requestUri);

        return await retryPolicy.ExecuteAsync(async () =>
        {
            using var response = await http.SendAsync(httpRequest, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("DatasourcePublisher: помилка під час публікації Datasource: {Status} / {Body}",
                    response.StatusCode, responseBody);
                response.EnsureSuccessStatusCode();
            }

            var doc = XDocument.Parse(responseBody);
            XNamespace ns = "http://tableau.com/api";
            var dsElem = doc.Root.Element(ns + "datasource") ?? throw new InvalidOperationException("У відповіді не знайдено вузол <datasource>.");
            var contentUrl = dsElem.Attribute("contentUrl")?.Value
                             ?? throw new InvalidOperationException("contentUrl у відповіді відсутній.");
            var publishedName = dsElem.Attribute("name")?.Value
                                ?? throw new InvalidOperationException("name у відповіді відсутній.");

            logger.LogInformation("DatasourcePublisher: успішно опубліковано Datasource '{Name}', contentUrl={ContentUrl}",
                publishedName, contentUrl);

            return contentUrl;
        });
    }
}
