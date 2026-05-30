using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieHub.API.DTOs.Directors;
using MovieHub.API.Services;

namespace MovieHub.API.Controllers;

[ApiController]
[Route("api/directors")]
public class DirectorsController(DirectorService directorService) : ControllerBase
{
    private readonly DirectorService _directorService = directorService;

    [HttpGet]
    [ProducesResponseType(typeof(List<DirectorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DirectorResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var directors = await _directorService.GetAllAsync(cancellationToken);
        return Ok(directors);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(DirectorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<DirectorResponse>> Create(
        CreateDirectorRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _directorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Create), response);
    }
}

