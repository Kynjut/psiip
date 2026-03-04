using EducatITion.DB;
using EducatITion.DB.Models;

namespace EducatITion.Models
{
    public class BaseViewModel
    {
        public Course[] Courses;
        public string Title { get; protected set; }
        public bool IsLoggedIn { get; }
        public Role Role { get; }
        private Localization Localization { get; }

        protected ISession Session { get; }
        protected ApplicationContext _context = new ApplicationContext();

        public BaseViewModel(ISession session)
        {
            var localization = session.GetString("localization") ?? Localization.ru.ToString();
            var role = session.GetString("role") ?? Role.student.ToString();

            Session = session;
            Localization = (Localization)Enum.Parse(typeof(Localization), localization);
            Role = (Role)Enum.Parse(typeof(Role), role);

            IsLoggedIn = session.Keys.Contains("username");
            Localize(Localization);
            LoadData(Localization);
        }

        protected string[][] LoadCoursesLocalizedInfo(Localization localization, int? take = null)
        {
			Course[] courses = _context.Courses.ToArray();
            Courses = courses;
            take = take ?? courses.Length;

            courses = courses.Take((int)take).ToArray();
            string[] coursesTitles, coursesDescriptions, coursesPrices, coursesTypes, coursesImgsPaths;

			if (localization == Localization.ru)
			{
				coursesTitles = courses.Select(course => course.NameRu).ToArray();
				coursesDescriptions = courses.Select(course => course.DescriptionRu).ToArray();
			}
			else
			{
				coursesTitles = courses.Select(course => course.NameEn).ToArray();
				coursesDescriptions = courses.Select(course => course.DescriptionEn).ToArray();
			}

            coursesPrices = courses.Select(course => course.Price).ToArray();
            coursesImgsPaths = courses.Select(course => course.ImgPath).ToArray();

			coursesTypes = courses.Select(course => course.Type.ToString()).ToArray();

			return new string[][] { coursesTitles, coursesDescriptions, coursesPrices, coursesTypes, coursesImgsPaths };
		}

        protected virtual void LoadData(Localization localization)
        {
            throw new NotImplementedException();
        }

        protected virtual void Localize(Localization localisation)
        {
            throw new NotImplementedException();
        }
    }
}
