using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }
        public int RatingStars { get; set; }
        public string Comment { get; set; }

        public int? UserRating { get; set; }
        [ForeignKey("UserId")]

        public int? UserRated { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
