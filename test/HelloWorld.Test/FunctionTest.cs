using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

namespace HelloWorld.Tests
{
  public class FunctionTest
  {
    private static readonly HttpClient client = new HttpClient();

    [Fact]
    public async Task TestCreateUserFunctionHandler()
    {
        // Arrange
        var request = new APIGatewayProxyRequest();
        var context = new TestLambdaContext();
        request.Body = @"
        {
            ""Cpf"": ""53469738009"",
            ""Name"": ""junior"",
        }";
            
        // Act
        var function = new Function();
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        var expectedResponse = new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
            
        Assert.Equal(expectedResponse.Headers, response.Headers);
        Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
    }
  }
}