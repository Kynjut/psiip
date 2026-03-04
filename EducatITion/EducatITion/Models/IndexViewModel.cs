namespace EducatITion.Models
{
    public class IndexViewModel : BaseViewModel
    {
        public Dictionary<string, string> HeaderTextBtnsNames = new Dictionary<string, string>
        {
            { "catalogue", "" },
            { "courses", "" },
            { "support", "" },
            { "basket", "" }
        };

		public Dictionary<string, string> PlaceholdersNames = new Dictionary<string, string>
		{
			{ "name", "" },
			{ "login", "" },
			{ "password", "" },
			{ "repeat password", "" },
		};

		public Dictionary<string, string> BtnsNames = new Dictionary<string, string>
        {
            { "signIn", "" },
            { "signUp", "" },
            { "teacher", "" },
            { "buyCourse", "" },
        };

        public string RegText;
        public string RegLikeText;
        public string StudentText;
        public string TeacherText;
        public string ContinueText;
        public string EnterText;
        public string AgreementText;

        public string[] CoursesTitles;
        public string[] CoursesDescriptions;
        public string[] CoursesImgsPaths;

        private int offset;
        public int SelectedCourseInd 
        {
            get => GetCycledIndex(2);
        }

        public string SelectedCourseTitle;
        public string SelectedCourseDescription;

        public IndexViewModel(ISession session) : base(session) { }

        public string GetImgPath(int index) => CoursesImgsPaths[GetCycledIndex(index)];

        private int GetCycledIndex(int index)
        {
            int res;
            var cycled = (index + offset) % CoursesTitles.Length;
            if (cycled < 0)
                res = CoursesTitles.Length + cycled;
            else
                res = cycled;

            return res;
        }

        protected override void LoadData(Localization localization)
		{
			var data = LoadCoursesLocalizedInfo(localization);
			CoursesTitles = data[0];
			CoursesDescriptions = data[1];
            CoursesImgsPaths = data[4];
            if (Session.GetInt32("selectedCourseInd") != null)
                offset = (int)Session.GetInt32("selectedCourseInd");
            SelectedCourseTitle = CoursesTitles[SelectedCourseInd];
            SelectedCourseDescription = CoursesDescriptions[SelectedCourseInd];
		}

		protected override void Localize(Localization localisation)
        {
            if (localisation == Localization.ru)
            {
                Title = "Главная";
                RegText = "Зарегистрироваться";
                RegLikeText = "Зарегистрироваться как";
                StudentText = "Студент";
                TeacherText = "Преподаватель";
                ContinueText = "Продолжить";
                EnterText = "Войти";
                AgreementText = "Нажимая кнопку «Зарегистрироваться», вы даёте своё согласие на обработку персональных данных в соответствии с «Политикой конфиденциальности» и соглашаетесь с «Условиями оказания услуг»";
                PlaceholdersNames["name"] = "Имя";
				PlaceholdersNames["login"] = "Логин"; 
				PlaceholdersNames["password"] = "Пароль";
				PlaceholdersNames["repeat password"] = "Повтор пароля";

				HeaderTextBtnsNames["catalogue"] = "Каталог";
                HeaderTextBtnsNames["support"] = "Поддержка";
                HeaderTextBtnsNames["courses"] = "Курсы";
                HeaderTextBtnsNames["basket"] = "Корзина";

                BtnsNames["signIn"] = "Войти";
                BtnsNames["signUp"] = "Регистрация";
                BtnsNames["teacher"] = "Преподаватель";
                BtnsNames["buyCourse"] = "Купить курс";
            }
            else if (localisation == Localization.en)
            {
                Title = "Main";
                RegText = "Register";
				RegLikeText = "Register like";
                StudentText = "Student";
                TeacherText = "Teacher";
                ContinueText = "Continue";
				EnterText = "Enter";
                AgreementText = "By clicking the «Register» button, you consent to the processing of personal data in accordance with the «Privacy Policy» and agree to the «Terms of Service»";

                PlaceholdersNames["name"] = "Name";
				PlaceholdersNames["login"] = "Login";
				PlaceholdersNames["password"] = "Password";
				PlaceholdersNames["repeat password"] = "Repeat password";

				HeaderTextBtnsNames["catalogue"] = "Catalogue";
				HeaderTextBtnsNames["support"] = "Support";
				HeaderTextBtnsNames["courses"] = "Courses";
                HeaderTextBtnsNames["basket"] = "Basket";

                BtnsNames["signIn"] = "Sign In";
				BtnsNames["signUp"] = "Register";
				BtnsNames["teacher"] = "Teacher";
				BtnsNames["buyCourse"] = "Buy Course";
			}
        }
    }
}
