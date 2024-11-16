using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

// This class defines integration tests for the BucketSolver API endpoint
public class BucketSolverIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    // Constructor initializes the WebApplicationFactory and HttpClient
    public BucketSolverIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    // This test method verifies the /solve-bucket endpoint
    [Fact]
    public async Task TestSolveBucketEndpoint()
    {
        // Create a request object with bucket capacities and desired amount
        var request = new BucketRequest(3, 5, 4);
        // Serialize the request object to JSON format
        var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        // Send a POST request to the /solve-bucket endpoint with the JSON content
        var response = await _client.PostAsync("/solve-bucket", jsonContent);

        // Ensure the response status code indicates success
        response.EnsureSuccessStatusCode();
        // Read the response content as a string
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert that the response contains the word "Solved"
        Assert.Contains("Solved", responseString);
    }
}

// This record defines the structure of the request object for the /solve-bucket endpoint
public record BucketRequest(int x_capacity, int y_capacity, int z_amount_wanted);