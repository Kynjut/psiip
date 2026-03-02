using System.ComponentModel.DataAnnotations;

namespace EducatITion.DB.Models
{
	public class Course
	{
		[Key]
		public int Id { get; set; }
		public string NameRu { get; set; }
		public string NameEn { get; set; }
		public string DescriptionRu { get; set; }
		public string DescriptionEn { get; set; }
		public string Price { get; set; }
		public CourseType Type { get; set; }
		public string ImgPath { get; set; }
    }
}
