using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.DTOs.Ratings;
using MovieHub.API.Services;
using System.Security.Claims;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api")]
public class RatingsController(RatingService ratingService) : ControllerBase
{
    private readonly RatingService _ratingService = ratingService;

    [HttpPost("movies/{movieId:int}/ratings")]
    [Authorize]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RatingResponse>> Create(
        int movieId,
        CreateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException());
        var rating = await _ratingService.CreateAsync(movieId, userId, request, cancellationToken);
        return CreatedAtAction(nameof(Create), rating);
    }
    [HttpPut("ratings/{ratingId:int}")]
    [Authorize]
    [ProducesResponseType(typeof(RatingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<RatingResponse>> Update(
        int ratingId,
        UpdateRatingRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null) return Unauthorized(new { message = "Unauthorized" });
        var userId = int.Parse(userIdClaim);
        var rating = await _ratingService.UpdateAsync(userId, ratingId, request, cancellationToken);
            return Ok(rating);
    }
}