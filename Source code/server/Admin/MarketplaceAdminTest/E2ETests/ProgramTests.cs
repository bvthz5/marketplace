using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MarketplaceAdminTest.E2ETests
{
    public class ProgramTests
    {
        private readonly HttpClient _client;

        public ProgramTests()
        {
            var factory = new WebApplicationFactory<Program>();
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task TestSwaggerUI_Loaded()
        {

            // Act
            var response = await _client.GetAsync("/swagger/index.html");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotEmpty(content);

        }
    }

}
