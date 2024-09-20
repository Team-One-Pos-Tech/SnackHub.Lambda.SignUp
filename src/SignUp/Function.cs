using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using SignUp.Contracts;
using SignUp.Services;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SignUp
{
    public record SignUpBodyRequest(string Username, string Password, string Email, string Name);

    public record SignUpFunctionResponse(string Username, string CreatedDate);

    public class Function
    {
        private readonly ISignUpRepository _signUpService;

        public Function()
        {
            _signUpService = new CognitoSignUpService();
        }

        public Function(ISignUpRepository singUpRepository)
        {
            _signUpService = singUpRepository;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            var createUserRequest = JsonSerializer.Deserialize<SignUpBodyRequest>(apigProxyEvent.Body);

            var response = await _signUpService.Register(new RegisterRequest(
                createUserRequest.Username, 
                createUserRequest.Password,
                createUserRequest.Email,
                createUserRequest.Name
            ));

            var userResponse = new SignUpFunctionResponse(response.Username, response.CreatedDate);

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(userResponse),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}