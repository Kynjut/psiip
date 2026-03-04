using System.ComponentModel.DataAnnotations;

namespace EducatITion.DB.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseNameRu { get; set; }
        public string CourseNameEn { get; set; }
        public string Price { get; set; }
        public string ImgPath { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
