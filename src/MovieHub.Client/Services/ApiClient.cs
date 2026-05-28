using System.Net.Http;

namespace MovieHub.Client.Services;

public class ApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
}
