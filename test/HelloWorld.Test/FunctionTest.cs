using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;

namespace SignUp.Tests
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
            ""Name"": ""Novo User Teste""
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