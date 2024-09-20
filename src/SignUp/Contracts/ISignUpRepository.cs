using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider.Model;

namespace SignUp.Contracts;

public record RegisterResponse(string Username, string CreatedDate);

public record RegisterRequest(string Username, string Password, string Email, string Name);

public interface ISignUpRepository
{
    public Task<RegisterResponse> Register(RegisterRequest request);
}