using System.Threading.Tasks;

namespace SignUp.Contracts;

public interface ISignUpRepository
{
    public Task<SingUpResponse> Register(SignUpRequest request);
}