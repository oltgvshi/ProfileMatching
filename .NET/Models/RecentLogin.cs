using ASP.NETCoreIdentityCustom.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileMatching.Models
{
    public class RecentLogin
    {
        public int RecentLoginID { get; set; }
        public DateTime LoginDate { get; set; }

        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}