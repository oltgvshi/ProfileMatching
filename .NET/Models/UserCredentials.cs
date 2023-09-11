using System.ComponentModel.DataAnnotations;

namespace ProfileMatching.Models
{
    public class UserCredentials
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public string UserRole {  get; set; }
    }
}
