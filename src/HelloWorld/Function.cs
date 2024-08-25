using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using HelloWorld.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HelloWorld
{

    public class Function
    {

        private static readonly HttpClient client = new HttpClient();
        
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {

            CreateUserRequest createUserRequest = JsonSerializer.Deserialize<CreateUserRequest>(apigProxyEvent.Body);
            
            var client = new AmazonCognitoIdentityProviderClient();
            var response = await client.AdminCreateUserAsync(new AdminCreateUserRequest 
            {
                MessageAction = "SUPPRESS",
                TemporaryPassword = "Default-password-99!",
                UserAttributes = new List<AttributeType> {
                    new AttributeType {
                        Name = "name",
                        Value = createUserRequest.Name
                    },
                    new AttributeType {
                        Name = "custom:CPF",
                        Value = createUserRequest.Cpf
                    }
                },
                UserPoolId = "us-east-1_DBk6tjf8T",
                Username = createUserRequest.Cpf
            });
            
            UserType user = response.User;

            return new APIGatewayProxyResponse
            {
                Body = JsonSerializer.Serialize(user),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}
