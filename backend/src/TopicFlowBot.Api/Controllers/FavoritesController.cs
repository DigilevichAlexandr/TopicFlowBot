using Microsoft.AspNetCore.Mvc;
using TopicFlowBot.Application;

namespace TopicFlowBot.Api.Controllers;

[ApiController]
[Route("favorites")]
public class FavoritesController(IFavoriteService favoriteService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFavoriteRequest request, CancellationToken ct)
    {
        await favoriteService.AddAsync(request, ct);
        return Ok();
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<IReadOnlyList<FavoriteDto>>> GetByUser([FromRoute] int userId, CancellationToken ct)
    {
        return Ok(await favoriteService.GetByUserAsync(userId, ct));
    }
}
