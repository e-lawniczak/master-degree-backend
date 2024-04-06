namespace PizzeriaAPI.Models
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public bool IsFirstLogin {  get; set; }
    }
}
