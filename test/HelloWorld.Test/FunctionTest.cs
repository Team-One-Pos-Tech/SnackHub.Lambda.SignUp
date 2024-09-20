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
    private Mock<ISignUpRepository> _registerRepository;

    private void BeforeTestStarting()
    {
        _registerRepository = new Mock<ISignUpRepository>();
    }
    
    [Fact]
    public async Task RegisterUserIfDoesNotExists()
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

        _registerRepository.Setup(repository => repository.Register(new SignUpRequest(cpf, password, "email@email.com")))
            .ReturnsAsync(
                new SingUpResponse(null, true)
            );
            
        var function = new Function(_registerRepository.Object);
      
        // Act
        var response = await function.FunctionHandler(request, context);
        
        // Assert
        response.StatusCode.Should().Be(200);
      
        var authResponse = JsonSerializer.Deserialize<SignUpFunctionResponse>(response.Body);
        authResponse.Username.Should().Be(cpf);

    }
    
  }
}