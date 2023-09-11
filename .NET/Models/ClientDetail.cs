using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class ClientDetail
    {
        [Key]
        public int ClientDetailsId { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public string Position { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }

        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }


    }
}
