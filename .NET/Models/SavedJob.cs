using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class SavedJob
    {
        [Key]
        public int SavedJobsId { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int? JobPostId { get; set; }
        [ForeignKey("JobPostId")]
        public JobPost JobPost { get; set; }
    }
}
