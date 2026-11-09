
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Services;

public class ApiService
{
    private readonly HttpClient client;

    public ApiService(HttpClient client)
    {
        this.client = client;
    }

    private static string GetErrorMessage(System.Net.HttpStatusCode status, string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return $"API returned {(int)status} ({status}).";
        try
        {
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            if (root.ValueKind == JsonValueKind.String) return root.GetString() ?? "Request failed.";
            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var key in new[] { "message", "detail", "title" })
                    if (root.TryGetProperty(key, out var value) && value.ValueKind == JsonValueKind.String)
                        return value.GetString() ?? "Request failed.";
            }
        }
        catch (JsonException) { }
        return body.Length > 250 ? $"API returned {(int)status} ({status})." : body;
    }

    public async Task<(bool ok, string message, JsonElement data)> Send(
        HttpContext context,
        HttpMethod method,
        string route,
        object? payload = null,
        IFormFile? file = null)
    {
        using var request = new HttpRequestMessage(method, route);

        if (context.Session.TryGetValue("ApiCookie", out var bytes))
        {
            request.Headers.TryAddWithoutValidation(
                "Cookie",
                System.Text.Encoding.UTF8.GetString(bytes));
        }

        if (file != null)
        {
            var form = new MultipartFormDataContent();

            form.Add(
                new StreamContent(file.OpenReadStream()),
                "file",
                file.FileName);

            request.Content = form;
        }
        else if (payload != null)
        {
            request.Content = JsonContent.Create(payload);
        }

        try
        {
            using var response = await client.SendAsync(request);

            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var cookie = cookies.FirstOrDefault(
                    x => x.StartsWith(
                        ".RaceDay.Session=",
                        StringComparison.Ordinal));

                if (cookie != null)
                {
                    context.Session.SetString(
                        "ApiCookie",
                        cookie.Split(';')[0]);
                }
            }

            var body = await response.Content.ReadAsStringAsync();

            JsonElement data = default;

            try
            {
                data = JsonDocument.Parse(body).RootElement.Clone();
            }
            catch (JsonException)
            {
                // The API response is not JSON.
            }

            return (
                response.IsSuccessStatusCode,
                response.IsSuccessStatusCode
                    ? ""
                    : GetErrorMessage(response.StatusCode, body),
                data
            );
        }
        catch (HttpRequestException)
        {
            return (
                false,
                "The RaceDay API is unavailable. Please try again.",
                default
            );
        }
    }
}

