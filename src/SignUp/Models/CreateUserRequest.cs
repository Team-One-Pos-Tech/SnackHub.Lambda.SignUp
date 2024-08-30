namespace SignUp.Models;

public class CreateUserRequest
{
    public string Name { get; set; }
    public string Cpf { get; set; }
    
    public string Password { get; set; }
}