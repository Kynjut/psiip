using EducatITion.Models.User;

namespace EducatITion.Models.Combined
{
    public class CombinedIndexModel
    {
        public AuthModel AuthModel { get; set; } = null!;
        public RegModel RegModel { get; set; } = null!;
        public IndexViewModel IndexViewModel { get; set; } = null!;
    }
}
