using System.ComponentModel.DataAnnotations;

namespace ProfileMatching.Models
{
    public class EmploymentType
    {
        [Key]
        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; }

    }
}
