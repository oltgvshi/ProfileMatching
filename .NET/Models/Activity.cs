using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public string? ActivityDescription { get; set; }
        public DateTime ActivityDate { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}