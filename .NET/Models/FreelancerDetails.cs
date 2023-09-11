using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class FreelancerDetails
    {
        public int FreelancerDetailsId { get; set; }
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
        public string HourlyRate{ get; set; }
        public string Languages { get; set; }
        public string Overview { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
