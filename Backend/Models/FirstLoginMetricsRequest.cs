using System.ComponentModel.DataAnnotations;

namespace ClothBackend.Models
{
    public class FirstLoginMetricsRequest
    {
        [Required]
        public string Country { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string GamesExperience { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string DemographicBackground { get; set; }
        [Required]
        public int UserId { get; set; }
    }
}
