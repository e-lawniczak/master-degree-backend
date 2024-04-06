namespace ClothBackend.DAL.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsControlGroup { get; set; }
        public bool FirstLogin { get; set; }
        public int? CurrentPlaytrough { get; set; }
        public int Attempts { get; set; }
        public int Deaths { get; set; }
        public int HighScore { get; set; }
    }
}
