using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using MessagePack;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class FreelancerExperience
    {
        public int FreelancerExperienceID { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int? EmploymentTypeId { get; set; }
        [ForeignKey("EmploymentTypeId")]
        public EmploymentType EmploymentType { get; set; }

        public string? CompanyName { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
}
