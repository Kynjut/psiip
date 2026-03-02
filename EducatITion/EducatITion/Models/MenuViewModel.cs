namespace EducatITion.Models
{
	public class MenuViewModel : BaseViewModel
	{
		public Dictionary<string, string> MenuSectionsNames = new Dictionary<string, string>
		{
			{ "support", "" },
			{ "courses", "" },
			{ "comments", "" },
			{ "exit", "" },
		};

		public Dictionary<int, string> GreetingInfo = new Dictionary<int, string>
		{
			{ 1, "" },
			{ 2, "" },
			{ 3, "" },
			{ 4, "" },
			{ 5, "" },
			{ 6, "" },
		};

		public string ExitButtonName;
		public string StayButtonName;
		public string AreYouSureTextPart1, AreYouSureTextPart2;

		public string[] CoursesDescriptions;
		public string[] CoursesTitles;
		public string[] CoursesPrices;
		public CourseType[] CoursesTypes;
		public string[] CoursesImgPaths;
		public MenuPage PageType;

		public MenuViewModel(ISession session, MenuPage type) : base(session) 
		{
			PageType = type;
		}

		protected override void LoadData(Localization localization)
		{
			var data = LoadCoursesLocalizedInfo(localization, 3);
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
				Title = "Меню";
				ExitButtonName = "Выйти";
				StayButtonName = "Остаться";
				AreYouSureTextPart1 = "Вы действительно хотите";
				AreYouSureTextPart2 = "выйти из аккаунта?";
				MenuSectionsNames["courses"] = "Курсы";
				MenuSectionsNames["comments"] = "Комментарии";
				MenuSectionsNames["support"] = "Поддержка";
				MenuSectionsNames["exit"] = "Выйти из аккаунта";

				GreetingInfo[0] = "Уважаемый пользователь!";
				GreetingInfo[1] = "Благодарим вас за выбор электронно-образовательного сайта EducatIT! Мы рады приветствовать вас в нашем сообществе, где знания становятся доступнее, а обучение — увлекательнее.";
				GreetingInfo[2] = "Ваше желание развиваться и учиться вдохновляет нас на создание качественного контента и разнообразных образовательных ресурсов. Мы стремимся предоставлять актуальную информацию и полезные инструменты, чтобы поддерживать ваш образовательный путь.";
				GreetingInfo[3] = "Если у вас есть вопросы, пожелания или предложения, пожалуйста, не стесняйтесь обращаться к нам. Мы всегда готовы помочь и сделать ваше обучение еще более эффективным.";
				GreetingInfo[4] = "Спасибо, что учитесь с EducatIT!";
				GreetingInfo[5] = @"С уважением, Команда EducatITon";
			}
			else if (localisation == Localization.en)
			{
				Title = "Menu";
				ExitButtonName = "Exit";
				StayButtonName = "Stay";
				AreYouSureTextPart1 = "Are you really want";
				AreYouSureTextPart2 = "to exit from account?";
				MenuSectionsNames["courses"] = "Courses";
				MenuSectionsNames["comments"] = "Comments";
				MenuSectionsNames["support"] = "Support";
				MenuSectionsNames["exit"] = "Logout";

				GreetingInfo[0] = "Dear user!";
				GreetingInfo[1] = "Thank you for choosing the EducatIT electronic educational site! We are glad to welcome you to our community, where knowledge becomes more accessible and learning becomes more exciting.";
				GreetingInfo[2] = "Your desire to grow and learn inspires us to create quality content and a variety of educational resources. We strive to provide relevant information and useful tools to support your educational journey.";
				GreetingInfo[3] = "If you have any questions, requests or suggestions, please do not hesitate to contact us. We are always ready to help and make your training even more effective.";
				GreetingInfo[4] = "Thank you for studying with EducatIT!";
				GreetingInfo[5] = @"Sincerely, EducatITon Team";
			}
		}
	}
}
