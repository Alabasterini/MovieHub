using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.DTOs.Movies;
using MovieHub.API.Exceptions;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly MovieService _movieService;

    public MoviesController(MovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MovieSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieSummaryResponse>>> GetAll(
        [FromQuery] MovieListQueryRequest query,
        CancellationToken cancellationToken)
    {
        var movies = await _movieService.GetAllAsync(query, cancellationToken);
        return Ok(movies);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MovieDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var movie = await _movieService.GetByIdAsync(id, cancellationToken);
            return Ok(movie);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MovieSummaryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MovieSummaryResponse>> Create(
        CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _movieService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(MovieSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<MovieSummaryResponse>> Update(
        int id,
        UpdateMovieRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _movieService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _movieService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

