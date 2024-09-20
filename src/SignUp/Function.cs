using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using SignIn.Contracts;
using SignIn.Repositories;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SignUp
{
    
    public record SignUpBodyRequest(string Username, string Password, string Email, string Name);
    public record SignUpFunctionResponse(string Username, string CreatedDate);

    public class Function
    {
        private readonly CognitoSignUpRepository _signUpService;

        public Function()
        {
            _signUpService = new CognitoSignUpRepository();
        }

        public Function(ISignUpRepository singUpRepository)
        {
            _signUpService = singUpRepository;
        }
        
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {
            var createUserRequest = JsonSerializer.Deserialize<SignUpBodyRequest>(apigProxyEvent.Body);
            
            var client = new AmazonCognitoIdentityProviderClient();
            var response = await client.AdminCreateUserAsync(new AdminCreateUserRequest 
            {
                MessageAction = "SUPPRESS",
                TemporaryPassword = createUserRequest.Password,
                UserAttributes = new List<AttributeType> {
                    new AttributeType {
                        Name = "name",
                        Value = createUserRequest.Name
                    },
                    new AttributeType {
                        Name = "email",
                        Value = createUserRequest.Email
                    },
                    new AttributeType {
                        Name = "custom:CPF",
                        Value = createUserRequest.Username
                    }
                },
                UserPoolId = "us-east-1_DBk6tjf8T",
                Username = createUserRequest.Username
            });

            await client.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest()
            {
                UserPoolId = "us-east-1_DBk6tjf8T",
                Username = createUserRequest.Username,
                Password = createUserRequest.Password,
                Permanent = true
            });
            
            UserType user = response.User;
            
            var userResponse = new SignUpFunctionResponse(user.Username, user.UserCreateDate.ToString());

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(userResponse),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
