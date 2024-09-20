using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using SignUp.Contracts;

namespace SignUp.Services;

public class CognitoSignUpService : ISignUpRepository
{
    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var client = new AmazonCognitoIdentityProviderClient();
        
        var userPoolId = Environment.GetEnvironmentVariable("USER_POOL_ID");
        
        var response = await client.AdminCreateUserAsync(new AdminCreateUserRequest 
        {
            MessageAction = "SUPPRESS",
            TemporaryPassword = request.Password,
            UserAttributes = new List<AttributeType> {
                new AttributeType {
                    Name = "name",
                    Value = request.Name
                },
                new AttributeType {
                    Name = "email",
                    Value = request.Email
                },
                new AttributeType {
                    Name = "custom:CPF",
                    Value = request.Username
                }
            },
            UserPoolId = userPoolId,
            Username = request.Username
        });

        await client.AdminSetUserPasswordAsync(new AdminSetUserPasswordRequest()
        {
            UserPoolId = userPoolId,
            Username = request.Username,
            Password = request.Password,
            Permanent = true
        });
            
        var user = response.User;

        return new RegisterResponse(user.Username, user.UserCreateDate.ToString());
    }
}