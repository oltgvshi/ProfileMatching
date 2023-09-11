using System.ComponentModel.DataAnnotations;

namespace ProfileMatching.Models
{
    public class Slider
    {
        [Key]
        public int SliderId { get; set; }
        public string SliderTitle { get; set; }

        public string SliderFilename { get; set; }
    }
}
