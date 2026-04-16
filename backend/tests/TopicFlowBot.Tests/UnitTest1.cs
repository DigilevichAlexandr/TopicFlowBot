using FluentAssertions;
using Moq;
using TopicFlowBot.Application;
using TopicFlowBot.Domain;

namespace TopicFlowBot.Tests;

public class TopicServiceTests
{
    [Fact]
    public async Task SearchAsync_ReturnsMappedTopics()
    {
        var repo = new Mock<ITopicRepository>();
        repo.Setup(x => x.SearchAsync("dotnet", It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Topic { Id = 5, Title = ".NET", Description = "Runtime" }]);
        var service = new TopicService(repo.Object);

        var result = await service.SearchAsync("dotnet", CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(5);
        result[0].Title.Should().Be(".NET");
    }
}
