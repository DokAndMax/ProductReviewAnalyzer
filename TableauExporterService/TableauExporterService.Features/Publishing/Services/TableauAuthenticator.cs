using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProductReviewAnalyzer.TableauExporterService.Features.Publishing.Services;

public sealed class TableauAuthenticator : ITableauAuthenticator
{
    private const string ApiVersion = "3.25";
    private readonly IConfiguration cfg;
    private readonly HttpClient http;
    private readonly ILogger<TableauAuthenticator> logger;

    private string? _authToken;
    private string? _siteId;

    public TableauAuthenticator(HttpClient http, IConfiguration cfg, ILogger<TableauAuthenticator> logger)
    {
        this.http = http;
        this.cfg = cfg;
        this.logger = logger;

        var baseUrl = this.cfg["Tableau:BaseUrl"]
                       ?? throw new ArgumentException("Таблиця: BaseUrl не задана в конфігурації.");
        this.http.BaseAddress = new Uri(baseUrl);
    }

    public HttpClient HttpClient => http;

    public string AuthToken => _authToken
        ?? throw new InvalidOperationException("Авторизація не виконана. Викличте EnsureAuthenticatedAsync().");

    public string SiteId => _siteId
        ?? throw new InvalidOperationException("Авторизація не виконана. Викличте EnsureAuthenticatedAsync().");

    public async Task EnsureAuthenticatedAsync(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_authToken) && !string.IsNullOrEmpty(_siteId))
        {
            return;
        }

        var patName = cfg["Tableau:PATName"]
                      ?? throw new ArgumentException("Tableau:PATName не налаштований");
        var patSecret = cfg["Tableau:PATSecret"]
                        ?? throw new ArgumentException("Tableau:PATSecret не налаштований");
        var siteContentUrl = cfg["Tableau:SiteUrl"] ?? "";

        var signInPayload = new
        {
            credentials = new
            {
                personalAccessTokenName = patName,
                personalAccessTokenSecret = patSecret,
                site = new { contentUrl = siteContentUrl }
            }
        };

        var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(signInPayload);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/{ApiVersion}/auth/signin");
        request.Content = new ByteArrayContent(jsonBytes);
        request.Content.Headers.Remove("Content-Type");
        request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        logger.LogInformation("TableauAuthenticator: запит SignIn...");

        using var response = await http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var bodyString = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(bodyString);
        var creds = doc.RootElement.GetProperty("credentials");

        _authToken = creds.GetProperty("token").GetString()
                     ?? throw new InvalidOperationException("Не отримано token під час SignIn");
        _siteId = creds.GetProperty("site").GetProperty("id").GetString()
                  ?? throw new InvalidOperationException("Не отримано site.id під час SignIn");

        logger.LogInformation("TableauAuthenticator: успішно автентифіковано. SiteId={SiteId}", _siteId);
    }
}
