using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class JobPost
    {
        [Key]
        public int JobPostId { get; set; }
        public string JobPostName { get; set; }
        public int JobPostBudget { get; set; }
        public DateTime JobLength { get; set; }
        public string JobPostDescription { get; set; }
        public DateTime JobApplicationDeadline { get; set; }
        public bool IsArchived { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public string? CompanyName { get; set; }
        public DateTime CreationDate { get; set; }




    }
}
