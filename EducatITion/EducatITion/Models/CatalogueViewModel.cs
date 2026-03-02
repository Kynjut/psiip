using EducatITion.DB.Models;

namespace EducatITion.Models
{
	public class CatalogueViewModel : BaseViewModel
	{
		public string SearchText;

		public string[] CoursesDescriptions;
		public string[] CoursesTitles;
		public string[] CoursesPrices;
		public CourseType[] CoursesTypes;
		public string[] CoursesImgPaths;

		public CatalogueViewModel(ISession session) : base(session) { }

		protected override void LoadData(Localization localization)
		{
			var data = LoadCoursesLocalizedInfo(localization);
			var searched = Session.GetString("searched")?.ToLower() ?? String.Empty;
			var sort = Session.GetString("sort") ?? String.Empty;

			int[] indices = new int[data[0].Length];
			for (int i = 0; i < indices.Length; i++)
				indices[i] = i;

			if (sort != String.Empty)
				Array.Sort(data[0], indices);

			string[] 
				c1 = data[1].ToArray(), 
				c2 = data[2].ToArray(), 
				c3 = data[3].ToArray(), 
				c4 = data[4].ToArray();

			for (int i = 0; i < indices.Length; i++)
			{
				data[1][i] = c1[indices[i]];
				data[2][i] = c2[indices[i]];
				data[3][i] = c3[indices[i]];
				data[4][i] = c4[indices[i]];
			}

			if (searched != String.Empty)
			{
				var indexisToClaim = data[0]
					.Select((x, index) => new { Value = x, Index = index })
					.Where(item => item.Value.ToLower().Contains(searched))
					.Select(item => item.Index)
					.ToArray();

				CoursesTitles = indexisToClaim.Select(index => data[0][index]).ToArray();
				CoursesDescriptions = indexisToClaim.Select(index => data[1][index]).ToArray();
				CoursesPrices = indexisToClaim.Select(index => data[2][index]).ToArray();
				CoursesTypes = indexisToClaim.Select(index => Enum.Parse<CourseType>(data[3][index])).ToArray();
				CoursesImgPaths = indexisToClaim.Select(index => data[4][index]).ToArray();

                return;
			}

			CoursesTitles = data[0];
			CoursesDescriptions = data[1];
			CoursesPrices = data[2];
			CoursesTypes = data[3].Select(course => Enum.Parse<CourseType>(course)).ToArray();
			CoursesImgPaths = data[4];
		}

		protected override void Localize(Localization localisation)
		{ 			
			if (localisation == Localization.ru)
			{
				Title = "Каталог";
				SearchText = "Поиск...";
			}
			else if (localisation == Localization.en)
			{
				Title = "Catalogue";
				SearchText = "Search...";
			}
		}
	}
}
