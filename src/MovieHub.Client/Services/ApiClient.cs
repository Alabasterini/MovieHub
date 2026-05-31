using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using MovieHub.Client.Models.Auth;
using MovieHub.Client.Models.Movies;
using MovieHub.Client.Models.Directors;
using MovieHub.Client.Models.Genres;
using MovieHub.Client.Models.Ratings;

namespace MovieHub.Client.Services;

public class ApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    public void ClearAuthToken()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    // ── Auth ────────────────────────────────────────────────────────────────

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
        response.EnsureSuccessStatusCode();
    }

    // ── Movies ──────────────────────────────────────────────────────────────

    public async Task<List<MovieSummaryResponse>> GetMoviesAsync(
        string? title = null,
        int? genreId = null,
        int? directorId = null)
    {
        var url = "/api/movies";
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(title))  query.Add($"title={Uri.EscapeDataString(title)}");
        if (genreId.HasValue)                    query.Add($"genreId={genreId}");
        if (directorId.HasValue)                 query.Add($"directorId={directorId}");
        if (query.Count > 0)                     url += "?" + string.Join("&", query);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<MovieSummaryResponse>>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<MovieDetailResponse> GetMovieAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/movies/{id}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieDetailResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<MovieDetailResponse> CreateMovieAsync(CreateMovieRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/movies", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieDetailResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<MovieDetailResponse> UpdateMovieAsync(int id, UpdateMovieRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/movies/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieDetailResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task DeleteMovieAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/movies/{id}");
        response.EnsureSuccessStatusCode();
    }

    // ── Ratings ─────────────────────────────────────────────────────────────

    public async Task<RatingResponse> CreateRatingAsync(int movieId, CreateRatingRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/movies/{movieId}/ratings", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RatingResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<RatingResponse> UpdateRatingAsync(int ratingId, UpdateRatingRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/ratings/{ratingId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RatingResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    // ── Genres ──────────────────────────────────────────────────────────────

    public async Task<List<GenreResponse>> GetGenresAsync()
    {
        var response = await _httpClient.GetAsync("/api/genres");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<GenreResponse>>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<GenreResponse> CreateGenreAsync(CreateGenreRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/genres", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<GenreResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }

    // ── Directors ────────────────────────────────────────────────────────────

    public async Task<List<DirectorResponse>> GetDirectorsAsync()
    {
        var response = await _httpClient.GetAsync("/api/directors");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<DirectorResponse>>()
            ?? throw new InvalidOperationException("API returned null");
    }

    public async Task<DirectorResponse> CreateDirectorAsync(CreateDirectorRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/directors", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DirectorResponse>()
            ?? throw new InvalidOperationException("API returned null");
    }
}