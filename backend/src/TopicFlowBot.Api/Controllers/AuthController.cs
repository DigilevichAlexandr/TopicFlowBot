using Microsoft.AspNetCore.Mvc;
using TopicFlowBot.Application;

namespace TopicFlowBot.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ITelegramAuthService authService) : ControllerBase
{
    [HttpPost("telegram")]
    public async Task<ActionResult<TelegramAuthResponse>> Telegram([FromBody] TelegramAuthRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await authService.AuthenticateAsync(request.InitData, ct));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
