using Microsoft.AspNetCore.Mvc;
using TopicFlowBot.Application;

namespace TopicFlowBot.Api.Controllers;

[ApiController]
[Route("topics")]
public class TopicsController(ITopicService topicService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TopicDto>>> Search([FromQuery] string query, CancellationToken ct)
    {
        var items = await topicService.SearchAsync(query, ct);
        return Ok(items);
    }
}
