using System;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using FluentAssertions;
using Moq;
using SignUp.Contracts;

namespace SignUp.Tests
{
  public class FunctionTest
  {
    private Mock<ISignUpRepository> _registerService;

    private void BeforeTestStarting()
    {
        _registerService = new Mock<ISignUpRepository>();
    }
    
    [Fact]
    public async Task RegisterUser()
    {
        // Arrange
        BeforeTestStarting();
      
        var request = new APIGatewayProxyRequest();
        var context = new TestLambdaContext();

        var cpf = "53469738009";
        var name = "Mario Sergio Cortela";
        var password = "DefaultPassword!";
        var email = "mario@kart.com";
      
        request.Body = $@"
        {{
            ""Name"": ""{name}"",
            ""Username"": ""{cpf}"",
            ""Email"": ""{email}"",
            ""Password"": ""{password}""
        }}";

        _registerService.Setup(service => service.Register(
                new RegisterRequest(cpf, password, email, name)))
            .ReturnsAsync(
                new RegisterResponse(cpf, DateTime.Now.ToString())
            );
            
        var function = new Function(_registerService.Object);
      
        // Act
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        response.StatusCode.Should().Be(200);
      
        var authResponse = JsonSerializer.Deserialize<SignUpFunctionResponse>(response.Body);
        authResponse.Username.Should().Be(cpf);

    }
    
  }
}