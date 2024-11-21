using FluentAssertions;

namespace MyProject.Tests;

public class PersonsControllerIntegrationTest
{
    private readonly HttpClient _client;

    public PersonsControllerIntegrationTest()
    {
        var factory = new CustomWebApplicationFactory();
        _client = factory.CreateClient();
    }

    #region Index
    [Fact]
    public async Task Index_Should_ToReturnView()
    {
        //Arrange

        //Act
        HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

        //Assert
        response.Should().BeSuccessful();

        string responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Person Name");
    }

    #endregion
}
