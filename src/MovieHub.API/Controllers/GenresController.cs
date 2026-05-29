using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.DTOs.Genres;
using MovieHub.API.Exceptions;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/genres")]
public class GenresController(GenreService genreService) : ControllerBase
{
    private readonly GenreService _genreService = genreService;


    [HttpGet]
    [ProducesResponseType(typeof(List<GenreResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GenreResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var genres = await _genreService.GetAllAsync(cancellationToken);
        return Ok(genres);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(GenreResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<GenreResponse>> Create(
        CreateGenreRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _genreService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Create), response);
        }
        catch (ConflictException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
