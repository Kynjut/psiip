using EducatITion.DB.Models;

namespace EducatITion.Models
{
    public class CatalogueViewModel : BaseViewModel
    {
        public string SearchText;

        // Основные данные курсов (все)
        public string[] AllCoursesTitles { get; private set; }
        public string[] AllCoursesDescriptions { get; private set; }
        public string[] AllCoursesPrices { get; private set; }
        public CourseType[] AllCoursesTypes { get; private set; }
        public string[] AllCoursesImgPaths { get; private set; }

        // Данные для текущей страницы
        public string[] CoursesTitles { get; private set; }
        public string[] CoursesDescriptions { get; private set; }
        public string[] CoursesPrices { get; private set; }
        public CourseType[] CoursesTypes { get; private set; }
        public string[] CoursesImgPaths { get; private set; }

        // Свойства для пагинации
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; } = 9; // 9 курсов на странице (3x3)
        public int TotalItems { get; private set; }

        // Свойства для навигации
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public CatalogueViewModel(ISession session) : base(session) { }

        protected override void LoadData(Localization localization)
        {
            var data = LoadCoursesLocalizedInfo(localization);
            var searched = Session.GetString("searched")?.ToLower() ?? string.Empty;
            var sort = Session.GetString("sort") ?? string.Empty;

            // Получаем номер страницы из сессии или параметров
            CurrentPage = Session.GetInt32("currentPage") ?? 1;

            int[] indices = new int[data[0].Length];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = i;

            // Сортировка
            if (sort == "true")
                Array.Sort(data[0], indices);

            // Перестановка всех массивов согласно сортировке
            string[] temp1 = data[1].ToArray();
            string[] temp2 = data[2].ToArray();
            string[] temp3 = data[3].ToArray();
            string[] temp4 = data[4].ToArray();

            for (int i = 0; i < indices.Length; i++)
            {
                data[1][i] = temp1[indices[i]];
                data[2][i] = temp2[indices[i]];
                data[3][i] = temp3[indices[i]];
                data[4][i] = temp4[indices[i]];
            }

            // Фильтрация по поиску
            if (!string.IsNullOrEmpty(searched))
            {
                var indexesToClaim = data[0]
                    .Select((x, index) => new { Value = x, Index = index })
                    .Where(item => item.Value.ToLower().Contains(searched))
                    .Select(item => item.Index)
                    .ToArray();

                AllCoursesTitles = indexesToClaim.Select(index => data[0][index]).ToArray();
                AllCoursesDescriptions = indexesToClaim.Select(index => data[1][index]).ToArray();
                AllCoursesPrices = indexesToClaim.Select(index => data[2][index]).ToArray();
                AllCoursesTypes = indexesToClaim.Select(index => Enum.Parse<CourseType>(data[3][index])).ToArray();
                AllCoursesImgPaths = indexesToClaim.Select(index => data[4][index]).ToArray();
            }
            else
            {
                AllCoursesTitles = data[0];
                AllCoursesDescriptions = data[1];
                AllCoursesPrices = data[2];
                AllCoursesTypes = data[3].Select(course => Enum.Parse<CourseType>(course)).ToArray();
                AllCoursesImgPaths = data[4];
            }

            // Настройка пагинации
            TotalItems = AllCoursesTitles.Length;
            TotalPages = (int)Math.Ceiling((double)TotalItems / PageSize);

            // Корректировка текущей страницы
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > TotalPages && TotalPages > 0) CurrentPage = TotalPages;

            // Получение данных для текущей страницы
            int startIndex = (CurrentPage - 1) * PageSize;
            int itemsToTake = Math.Min(PageSize, TotalItems - startIndex);

            if (itemsToTake > 0)
            {
                CoursesTitles = AllCoursesTitles.Skip(startIndex).Take(itemsToTake).ToArray();
                CoursesDescriptions = AllCoursesDescriptions.Skip(startIndex).Take(itemsToTake).ToArray();
                CoursesPrices = AllCoursesPrices.Skip(startIndex).Take(itemsToTake).ToArray();
                CoursesTypes = AllCoursesTypes.Skip(startIndex).Take(itemsToTake).ToArray();
                CoursesImgPaths = AllCoursesImgPaths.Skip(startIndex).Take(itemsToTake).ToArray();
            }
            else
            {
                CoursesTitles = Array.Empty<string>();
                CoursesDescriptions = Array.Empty<string>();
                CoursesPrices = Array.Empty<string>();
                CoursesTypes = Array.Empty<CourseType>();
                CoursesImgPaths = Array.Empty<string>();
            }

            // Сохраняем текущую страницу в сессии
            Session.SetInt32("currentPage", CurrentPage);
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

        public int[] GetPageRange(int maxVisiblePages = 5)
        {
            var pages = new List<int>();

            int start = Math.Max(1, CurrentPage - maxVisiblePages / 2);
            int end = Math.Min(TotalPages, start + maxVisiblePages - 1);

            if (end - start + 1 < maxVisiblePages)
                start = Math.Max(1, end - maxVisiblePages + 1);

            for (int i = start; i <= end; i++)
                pages.Add(i);

            return pages.ToArray();
        }
    }
}