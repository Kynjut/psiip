using EducatITion.DB;
using EducatITion.DB.Models;
using EducatITion.Models;
using EducatITion.Models.Combined;
using EducatITion.Models.Send;
using EducatITion.Models.User;
using EducatITion.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Reflection;

namespace EducatITion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ISession session { get => HttpContext.Session; }
        private ApplicationContext context = new ApplicationContext();
		private string localization { get => session.GetString("localization"); }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string localization, string direction)
        {
            //DEBUG_DB_ADD_DATA();

            if (localization != null)
                session.SetString("localization", localization);
            if (direction == null)
                direction = "0";

            int lastInd = session.GetInt32("selectedCourseInd") ?? 0;
			session.SetInt32("selectedCourseInd", lastInd + int.Parse(direction));

			var viewModel = new CombinedIndexModel()
            {
                RegModel = new RegModel(),
                AuthModel = new AuthModel(),
                IndexViewModel = new IndexViewModel(session)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CombinedIndexModel model)
        {
            RegUser(model.RegModel);
            AuthUser(model.AuthModel);
           
            model = new CombinedIndexModel()
            {
                RegModel = new RegModel(),
                AuthModel = new AuthModel(),
                IndexViewModel = new IndexViewModel(session)
            };

            return View(model);
        }

        private bool RegUser(RegModel model)
        {
            bool isValid = IsValid(model);

            if (isValid)
            {
                if (model.Password != model.RepeatPassword)
                    return false;

                if (GetUser(model.Email) != null)
                    return false;

                var user = new User()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                    Role = (Role)Enum.Parse(typeof(Role), session.GetString("role"))
                };

                using (var context = new ApplicationContext())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
               
                WriteUserToSession(user);
                EmailService.SendEmail(model.Email, "You were registered to EducatIT", "Welcome to our platform");
            }

            return isValid;
        }

        private bool AuthUser(AuthModel model)
        {
            bool isValid = IsValid(model);

            if (isValid)
            {
                var user = GetUser(model.Email);
                if (user != null && user.Password == model.Password)
                    WriteUserToSession(user);
                else
                    return false;
            }

            return isValid;
        }

        private User? GetUser(string email)
        {
            return context.Users.Where(x => x.Email == email).FirstOrDefault();
        }

        private void WriteUserToSession(User user)
        {
            session.SetString("username", user.Name);
            session.SetString("login", user.Email);
            session.SetString("role", user.Role.ToString());
        }

        private bool IsValid<T>(T model)
        {
            if (model == null)
                return false;

            Type modelType = typeof(T);

            bool isValid = true;
            var propertiesNames = modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name != "Id")
                .Select(x => $"{modelType.Name}.{x.Name}")
                .ToList();

            foreach (var name in propertiesNames)
                isValid &= ModelState[name]?.ValidationState == ModelValidationState.Valid;

            return isValid;
        }

        [HttpPost]
        [Route("Home/SaveChoice")]
        public IActionResult SaveChoice([FromBody] ChoiceModel model)
        {
            session.SetString("role", model.choice);
            return Ok();
        }

        private void DEBUG_DB_ADD_DATA()
        {
            var users = new List<User>
            {
                new User { Name="teacher", Email = "t@gmail.com", Password = "123", Role = Role.teacher },
                new User { Name="student 1", Email = "s@gmail.com", Password = "123", Role = Role.student },
            };

            var courses = new List<Course>
            {
                // Набор 1: Языки программирования (Python, Java, C#, JavaScript, C++)
                new Course { NameRu = "Python для начинающих", NameEn = "Python for Beginners", DescriptionRu = "Изучите основы Python с нуля. Подходит для новичков.", DescriptionEn = "Learn Python basics from scratch. Suitable for beginners.", Price = "$100-$150", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Продвинутый Python", NameEn = "Advanced Python", DescriptionRu = "Декораторы, генераторы, многопоточность и асинхронность в Python.", DescriptionEn = "Decorators, generators, multithreading and asynchrony in Python.", Price = "$120-$180", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Java для профессионалов", NameEn = "Java for Professionals", DescriptionRu = "Глубокое погружение в экосистему Java и Spring Framework.", DescriptionEn = "Deep dive into Java ecosystem and Spring Framework.", Price = "$150-$220", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Основы Java", NameEn = "Java Fundamentals", DescriptionRu = "Введение в объектно-ориентированное программирование на Java.", DescriptionEn = "Introduction to object-oriented programming in Java.", Price = "$90-$140", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "C# и .NET Core", NameEn = "C# and .NET Core", DescriptionRu = "Разработка веб-приложений и API на платформе .NET.", DescriptionEn = "Web applications and API development on .NET platform.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "ASP.NET MVC", NameEn = "ASP.NET MVC", DescriptionRu = "Создание веб-сайтов по паттерну MVC на C#.", DescriptionEn = "Building websites using MVC pattern in C#.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "JavaScript с нуля", NameEn = "JavaScript from Zero", DescriptionRu = "Полный курс по современному JavaScript (ES6+).", DescriptionEn = "Complete course on modern JavaScript (ES6+).", Price = "$80-$130", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Продвинутый JavaScript", NameEn = "Advanced JavaScript", DescriptionRu = "Замыкания, прототипы, асинхронность и промисы.", DescriptionEn = "Closures, prototypes, asynchrony and promises.", Price = "$100-$160", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "TypeScript в деталях", NameEn = "TypeScript in Depth", DescriptionRu = "Строгая типизация и современные возможности JavaScript.", DescriptionEn = "Strong typing and modern JavaScript features.", Price = "$90-$150", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "C++ для начинающих", NameEn = "C++ for Beginners", DescriptionRu = "Изучите основы C++: переменные, циклы, функции и классы.", DescriptionEn = "Learn C++ basics: variables, loops, functions and classes.", Price = "$95-$145", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 2: Веб-разработка (HTML, CSS, React, Vue, Angular)
                new Course { NameRu = "HTML и CSS для новичков", NameEn = "HTML and CSS for Beginners", DescriptionRu = "Создание адаптивных сайтов с использованием Flexbox и Grid.", DescriptionEn = "Building responsive websites using Flexbox and Grid.", Price = "$70-$110", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Продвинутый CSS", NameEn = "Advanced CSS", DescriptionRu = "Анимации, препроцессоры и методология БЭМ.", DescriptionEn = "Animations, preprocessors and BEM methodology.", Price = "$85-$130", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "React для начинающих", NameEn = "React for Beginners", DescriptionRu = "Компонентный подход, хуки и состояние в React.", DescriptionEn = "Component approach, hooks and state in React.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Продвинутый React", NameEn = "Advanced React", DescriptionRu = "Redux, контекст, оптимизация и кастомные хуки.", DescriptionEn = "Redux, context, optimization and custom hooks.", Price = "$130-$190", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Vue.js основы", NameEn = "Vue.js Basics", DescriptionRu = "Изучите Vue 3, Composition API и Vue Router.", DescriptionEn = "Learn Vue 3, Composition API and Vue Router.", Price = "$100-$155", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "Angular полный курс", NameEn = "Angular Complete Course", DescriptionRu = "TypeScript, компоненты, сервисы и DI в Angular.", DescriptionEn = "TypeScript, components, services and DI in Angular.", Price = "$140-$210", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Веб-дизайн основы", NameEn = "Web Design Fundamentals", DescriptionRu = "Принципы UX/UI, типографика и цветовые схемы.", DescriptionEn = "UX/UI principles, typography and color schemes.", Price = "$75-$120", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Адаптивная верстка", NameEn = "Responsive Layout", DescriptionRu = "Создание сайтов, которые отлично выглядят на всех устройствах.", DescriptionEn = "Creating websites that look great on all devices.", Price = "$80-$125", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Next.js для React", NameEn = "Next.js for React", DescriptionRu = "SSR, SSG и развертывание Next.js приложений.", DescriptionEn = "SSR, SSG and deployment of Next.js applications.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Svelte с нуля", NameEn = "Svelte from Scratch", DescriptionRu = "Новый подход к созданию реактивных интерфейсов.", DescriptionEn = "New approach to building reactive interfaces.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 3: Базы данных и бэкенд
                new Course { NameRu = "SQL для начинающих", NameEn = "SQL for Beginners", DescriptionRu = "Основы реляционных баз данных и запросы SQL.", DescriptionEn = "Relational database basics and SQL queries.", Price = "$80-$130", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Продвинутый SQL", NameEn = "Advanced SQL", DescriptionRu = "Оптимизация запросов, индексы и хранимые процедуры.", DescriptionEn = "Query optimization, indexes and stored procedures.", Price = "$100-$160", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "PostgreSQL мастер", NameEn = "PostgreSQL Mastery", DescriptionRu = "Работа с JSON, полнотекстовый поиск и репликация.", DescriptionEn = "JSON handling, full-text search and replication.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "MongoDB NoSQL", NameEn = "MongoDB NoSQL", DescriptionRu = "Документо-ориентированные базы данных и агрегация.", DescriptionEn = "Document-oriented databases and aggregation.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Node.js API", NameEn = "Node.js API", DescriptionRu = "Создание REST API с Express и MongoDB.", DescriptionEn = "Building REST API with Express and MongoDB.", Price = "$105-$165", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "PHP и MySQL", NameEn = "PHP and MySQL", DescriptionRu = "Серверная разработка на PHP и работа с базами данных.", DescriptionEn = "Server-side development with PHP and database work.", Price = "$85-$135", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Laravel фреймворк", NameEn = "Laravel Framework", DescriptionRu = "Современная разработка на PHP с Laravel.", DescriptionEn = "Modern PHP development with Laravel.", Price = "$100-$160", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Django для Python", NameEn = "Django for Python", DescriptionRu = "Высокоуровневый веб-фреймворк на Python.", DescriptionEn = "High-level Python web framework.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "FastAPI основы", NameEn = "FastAPI Basics", DescriptionRu = "Современные и быстрые веб-приложения на Python.", DescriptionEn = "Modern and fast web applications in Python.", Price = "$90-$145", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "GraphQL API", NameEn = "GraphQL API", DescriptionRu = "Гибкие API с использованием GraphQL и Apollo.", DescriptionEn = "Flexible APIs using GraphQL and Apollo.", Price = "$105-$165", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 4: Мобильная разработка
                new Course { NameRu = "iOS разработка Swift", NameEn = "iOS Development Swift", DescriptionRu = "Создание приложений для iPhone и iPad на Swift.", DescriptionEn = "Building apps for iPhone and iPad with Swift.", Price = "$150-$230", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Android Kotlin", NameEn = "Android Kotlin", DescriptionRu = "Разработка Android приложений на современном Kotlin.", DescriptionEn = "Android app development with modern Kotlin.", Price = "$140-$220", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Flutter для начинающих", NameEn = "Flutter for Beginners", DescriptionRu = "Кроссплатформенная разработка на Dart.", DescriptionEn = "Cross-platform development with Dart.", Price = "$120-$185", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "React Native", NameEn = "React Native", DescriptionRu = "Мобильные приложения на JavaScript и React.", DescriptionEn = "Mobile apps with JavaScript and React.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Xamarin C#", NameEn = "Xamarin C#", DescriptionRu = "Кроссплатформенная разработка на C# и .NET.", DescriptionEn = "Cross-platform development with C# and .NET.", Price = "$125-$190", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "SwiftUI интерфейсы", NameEn = "SwiftUI Interfaces", DescriptionRu = "Декларативное создание интерфейсов для Apple платформ.", DescriptionEn = "Declarative UI building for Apple platforms.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Jetpack Compose", NameEn = "Jetpack Compose", DescriptionRu = "Современный UI тулкит для Android.", DescriptionEn = "Modern UI toolkit for Android.", Price = "$125-$190", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "iOS продвинутый", NameEn = "Advanced iOS", DescriptionRu = "CoreData, многопоточность и анимации в iOS.", DescriptionEn = "CoreData, multithreading and animations in iOS.", Price = "$160-$240", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Android продвинутый", NameEn = "Advanced Android", DescriptionRu = "Room, WorkManager и Material Design.", DescriptionEn = "Room, WorkManager and Material Design.", Price = "$150-$230", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Flutter продвинутый", NameEn = "Advanced Flutter", DescriptionRu = "Анимации, кастомные виджеты и BLoC паттерн.", DescriptionEn = "Animations, custom widgets and BLoC pattern.", Price = "$135-$200", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 5: Data Science и аналитика
                new Course { NameRu = "Python для Data Science", NameEn = "Python for Data Science", DescriptionRu = "NumPy, Pandas, Matplotlib для анализа данных.", DescriptionEn = "NumPy, Pandas, Matplotlib for data analysis.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Машинное обучение", NameEn = "Machine Learning", DescriptionRu = "Scikit-learn, регрессия и классификация.", DescriptionEn = "Scikit-learn, regression and classification.", Price = "$150-$230", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Глубокое обучение", NameEn = "Deep Learning", DescriptionRu = "Нейронные сети с TensorFlow и Keras.", DescriptionEn = "Neural networks with TensorFlow and Keras.", Price = "$170-$260", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "SQL для аналитики", NameEn = "SQL for Analytics", DescriptionRu = "Оконные функции и аналитические запросы.", DescriptionEn = "Window functions and analytical queries.", Price = "$90-$140", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Tableau визуализация", NameEn = "Tableau Visualization", DescriptionRu = "Создание дашбордов и визуализации данных.", DescriptionEn = "Creating dashboards and data visualization.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "Power BI", NameEn = "Power BI", DescriptionRu = "Бизнес-аналитика и отчеты в Power BI.", DescriptionEn = "Business intelligence and reports in Power BI.", Price = "$100-$155", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Статистика для DS", NameEn = "Statistics for DS", DescriptionRu = "Математические основы для Data Science.", DescriptionEn = "Mathematical foundations for Data Science.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "NLP обработка текста", NameEn = "NLP Text Processing", DescriptionRu = "Обработка естественного языка на Python.", DescriptionEn = "Natural language processing with Python.", Price = "$140-$210", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Компьютерное зрение", NameEn = "Computer Vision", DescriptionRu = "OpenCV и нейросети для анализа изображений.", DescriptionEn = "OpenCV and neural networks for image analysis.", Price = "$145-$220", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Excel для аналитики", NameEn = "Excel for Analytics", DescriptionRu = "Сводные таблицы, Power Query и макросы.", DescriptionEn = "Pivot tables, Power Query and macros.", Price = "$70-$110", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 6: DevOps и инфраструктура
                new Course { NameRu = "Docker контейнеризация", NameEn = "Docker Containerization", DescriptionRu = "Упаковка приложений в контейнеры Docker.", DescriptionEn = "Packaging applications into Docker containers.", Price = "$100-$160", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Kubernetes оркестрация", NameEn = "Kubernetes Orchestration", DescriptionRu = "Управление контейнерами в кластере K8s.", DescriptionEn = "Container management in K8s cluster.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "CI/CD pipelines", NameEn = "CI/CD pipelines", DescriptionRu = "Автоматизация сборки и развертывания с Jenkins.", DescriptionEn = "Build and deployment automation with Jenkins.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Linux администрирование", NameEn = "Linux Administration", DescriptionRu = "Командная строка, скрипты и управление серверами.", DescriptionEn = "Command line, scripts and server management.", Price = "$90-$145", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Git и GitHub", NameEn = "Git and GitHub", DescriptionRu = "Системы контроля версий и совместная работа.", DescriptionEn = "Version control systems and collaboration.", Price = "$60-$100", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "Ansible автоматизация", NameEn = "Ansible Automation", DescriptionRu = "Управление конфигурациями и автоматизация задач.", DescriptionEn = "Configuration management and task automation.", Price = "$105-$165", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Terraform инфраструктура", NameEn = "Terraform Infrastructure", DescriptionRu = "Инфраструктура как код на Terraform.", DescriptionEn = "Infrastructure as code with Terraform.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "AWS облачные сервисы", NameEn = "AWS Cloud Services", DescriptionRu = "EC2, S3, Lambda и другие сервисы AWS.", DescriptionEn = "EC2, S3, Lambda and other AWS services.", Price = "$140-$210", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Azure платформа", NameEn = "Azure Platform", DescriptionRu = "Облачные решения от Microsoft Azure.", DescriptionEn = "Cloud solutions from Microsoft Azure.", Price = "$135-$205", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "GCP основы", NameEn = "GCP Basics", DescriptionRu = "Google Cloud Platform для разработчиков.", DescriptionEn = "Google Cloud Platform for developers.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 7: Тестирование и QA
                new Course { NameRu = "Ручное тестирование", NameEn = "Manual Testing", DescriptionRu = "Основы QA и тест-дизайна.", DescriptionEn = "QA basics and test design.", Price = "$70-$115", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Автоматизация на Python", NameEn = "Automation with Python", DescriptionRu = "Selenium и Pytest для автотестов.", DescriptionEn = "Selenium and Pytest for automation.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Автоматизация на Java", NameEn = "Automation with Java", DescriptionRu = "JUnit, TestNG и Selenium WebDriver.", DescriptionEn = "JUnit, TestNG and Selenium WebDriver.", Price = "$100-$155", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Jira и тест-менеджмент", NameEn = "Jira and Test Management", DescriptionRu = "Управление задачами и тест-кейсами в Jira.", DescriptionEn = "Task and test case management in Jira.", Price = "$65-$105", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Нагрузочное тестирование", NameEn = "Load Testing", DescriptionRu = "JMeter и Gatling для тестирования производительности.", DescriptionEn = "JMeter and Gatling for performance testing.", Price = "$90-$145", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "API тестирование", NameEn = "API Testing", DescriptionRu = "Postman, REST Assured и SOAP UI.", DescriptionEn = "Postman, REST Assured and SOAP UI.", Price = "$85-$135", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Mobile тестирование", NameEn = "Mobile Testing", DescriptionRu = "Тестирование iOS и Android приложений.", DescriptionEn = "iOS and Android app testing.", Price = "$90-$140", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "BDD с Cucumber", NameEn = "BDD with Cucumber", DescriptionRu = "Поведенчески-ориентированная разработка тестов.", DescriptionEn = "Behavior-driven test development.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Безопасность приложений", NameEn = "Application Security", DescriptionRu = "Тестирование на проникновение и уязвимости.", DescriptionEn = "Penetration testing and vulnerabilities.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "CI/CD для тестирования", NameEn = "CI/CD for Testing", DescriptionRu = "Интеграция автотестов в пайплайны.", DescriptionEn = "Integrating automated tests into pipelines.", Price = "$105-$160", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 8: Soft Skills и управление
                new Course { NameRu = "Agile и Scrum", NameEn = "Agile and Scrum", DescriptionRu = "Гибкие методологии управления проектами.", DescriptionEn = "Flexible project management methodologies.", Price = "$80-$130", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "IT项目管理", NameEn = "IT Project Management", DescriptionRu = "Планирование, риски и коммуникация в IT.", DescriptionEn = "Planning, risks and communication in IT.", Price = "$95-$150", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Английский для IT", NameEn = "English for IT", DescriptionRu = "Технический английский для разработчиков.", DescriptionEn = "Technical English for developers.", Price = "$70-$115", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Публичные выступления", NameEn = "Public Speaking", DescriptionRu = "Как выступать на конференциях и митапах.", DescriptionEn = "How to speak at conferences and meetups.", Price = "$60-$100", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Переговоры в IT", NameEn = "Negotiations in IT", DescriptionRu = "Навыки эффективных переговоров с заказчиками.", DescriptionEn = "Effective negotiation skills with clients.", Price = "$75-$120", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "Тайм-менеджмент", NameEn = "Time Management", DescriptionRu = "Как успевать больше и не выгорать.", DescriptionEn = "How to get more done and avoid burnout.", Price = "$55-$95", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Лидерство в IT", NameEn = "Leadership in IT", DescriptionRu = "Управление командами и развитие сотрудников.", DescriptionEn = "Team management and employee development.", Price = "$85-$135", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Карьера в IT", NameEn = "Career in IT", DescriptionRu = "Как построить успешную карьеру в IT.", DescriptionEn = "How to build a successful career in IT.", Price = "$65-$105", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Резюме и собеседования", NameEn = "Resume and Interviews", DescriptionRu = "Подготовка к собеседованиям в IT компании.", DescriptionEn = "Preparation for interviews in IT companies.", Price = "$60-$100", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Фриланс для IT", NameEn = "Freelance for IT", DescriptionRu = "Как начать и развивать карьеру фрилансера.", DescriptionEn = "How to start and grow a freelance career.", Price = "$70-$115", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },

                // Набор 9: Дополнительные технологии
                new Course { NameRu = "Rust язык программирования", NameEn = "Rust Programming Language", DescriptionRu = "Безопасный и быстрый системный язык.", DescriptionEn = "Safe and fast systems programming language.", Price = "$120-$185", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Go для микросервисов", NameEn = "Go for Microservices", DescriptionRu = "Создание высоконагруженных сервисов на Go.", DescriptionEn = "Building high-load services with Go.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Kotlin Multiplatform", NameEn = "Kotlin Multiplatform", DescriptionRu = "Общий код для iOS, Android и веба.", DescriptionEn = "Shared code for iOS, Android and web.", Price = "$125-$190", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "WebAssembly", NameEn = "WebAssembly", DescriptionRu = "Высокопроизводительный код в браузере.", DescriptionEn = "High-performance code in the browser.", Price = "$110-$170", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Blockchain разработка", NameEn = "Blockchain Development", DescriptionRu = "Создание смарт-контрактов и DApps.", DescriptionEn = "Creating smart contracts and DApps.", Price = "$150-$230", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" },
                new Course { NameRu = "VR/AR разработка", NameEn = "VR/AR Development", DescriptionRu = "Unity и Unreal для виртуальной реальности.", DescriptionEn = "Unity and Unreal for virtual reality.", Price = "$140-$215", ImgPath="imgs/courses-imgs/Rectangle 7.png" },
                new Course { NameRu = "Игровая разработка", NameEn = "Game Development", DescriptionRu = "Создание игр на Unity и C#.", DescriptionEn = "Game development with Unity and C#.", Price = "$130-$200", ImgPath="imgs/courses-imgs/Rectangle 7-1.png" },
                new Course { NameRu = "Unreal Engine", NameEn = "Unreal Engine", DescriptionRu = "Разработка игр на C++ в Unreal Engine.", DescriptionEn = "Game development with C++ in Unreal Engine.", Price = "$135-$210", ImgPath="imgs/courses-imgs/Rectangle 7-2.png" },
                new Course { NameRu = "Elixir и Phoenix", NameEn = "Elixir and Phoenix", DescriptionRu = "Функциональное программирование для веба.", DescriptionEn = "Functional programming for the web.", Price = "$115-$175", ImgPath="imgs/courses-imgs/Rectangle 7-3.png" },
                new Course { NameRu = "Scala для Big Data", NameEn = "Scala for Big Data", DescriptionRu = "Функциональный подход к обработке данных.", DescriptionEn = "Functional approach to data processing.", Price = "$125-$190", ImgPath="imgs/courses-imgs/Rectangle 7-4.png" }
            };

            foreach (var item in courses)
            {
                item.ImgPath = "/" + item.ImgPath;
            }

            using (var context = new ApplicationContext())
            {
                //context.Courses.AddRange(courses);
                //context.Users.AddRange(users);
                //context.Courses.AddRange(courses);
                context.SaveChanges();
            }
        }

        public IActionResult Menu(string page)
        {
            page = page ?? "Сourses";
            MenuPage type = (MenuPage)Enum.Parse(typeof(MenuPage), page);

            var currentUser = GetUser(session.GetString("login"));

            if (type == MenuPage.Exit)
            {
                session.Clear();
                return RedirectToAction("Index");
            }

			if (!currentUser.WasInMenu)
            {
                type = MenuPage.Greetings;
                currentUser.WasInMenu = true;
                context.SaveChanges();
            }

			var model = new MenuViewModel(session, type);
            return View(model);
        }

        [HttpGet]
        public IActionResult Catalogue(int page = 1)
        {
            session.SetInt32("currentPage", page);

            var viewModel = new CombinedCatalogueModel
            {
                SearchModel = new SearchModel(),
                CatalogueViewModel = new CatalogueViewModel(session)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Catalogue(CombinedCatalogueModel model, string sort, int page = 1)
        {
            session.SetString("sort", sort ?? "");
            session.SetString("searched", model.SearchModel?.Searched ?? "");
            session.SetInt32("currentPage", page);

            var viewModel = new CombinedCatalogueModel
            {
                SearchModel = new SearchModel(),
                CatalogueViewModel = new CatalogueViewModel(session)
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult DeleteCourse(int courseId)
        {
            using (var context = new ApplicationContext())
            {
                var course = context.Courses.Find(courseId);
                if (course != null)
                {
                    context.Courses.Remove(course);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Catalogue");
        }

        [HttpPost]
        public IActionResult ChangePage(int page)
        {
            session.SetInt32("currentPage", page);
            return RedirectToAction("Catalogue", new { page = page });
        }

        // Просмотр корзины
        [HttpGet]
        public IActionResult Cart()
        {
            var viewModel = new CartViewModel(session);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddToCart(int courseId)
        {
            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var cartJson = session.GetString("Cart");
            var cart = EducatITion.DB.Models.Cart.FromJson(cartJson);

            var cartItem = new CartItem
            {
                CourseId = course.Id,
                CourseNameRu = course.NameRu,
                CourseNameEn = course.NameEn,
                Price = course.Price,
                ImgPath = course.ImgPath,
                Quantity = 1
            };

            cart.AddItem(cartItem);
            session.SetString("Cart", cart.ToJson());

            TempData["SuccessMessage"] = localization == "ru" ? "Курс добавлен в корзину" : "Course added to cart";

            return RedirectToAction("Catalogue");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int courseId)
        {
            var cartJson = session.GetString("Cart");
            var cart = EducatITion.DB.Models.Cart.FromJson(cartJson);

            cart.RemoveItem(courseId);
            session.SetString("Cart", cart.ToJson());

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveCompletely(int courseId)
        {
            var cartJson = session.GetString("Cart");
            var cart = EducatITion.DB.Models.Cart.FromJson(cartJson);

            cart.RemoveItemCompletely(courseId);
            session.SetString("Cart", cart.ToJson());

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            session.Remove("Cart");
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int courseId, int quantity)
        {
            if (quantity < 1)
            {
                return RedirectToAction("RemoveCompletely", new { courseId });
            }

            var cartJson = session.GetString("Cart");
            var cart = EducatITion.DB.Models.Cart.FromJson(cartJson);

            var item = cart.Items.FirstOrDefault(i => i.CourseId == courseId);
            if (item != null)
            {
                item.Quantity = quantity;
                cart.UpdatedAt = DateTime.Now;
            }

            session.SetString("Cart", cart.ToJson());

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            var cartJson = session.GetString("Cart");
            var cart = EducatITion.DB.Models.Cart.FromJson(cartJson);

            return Json(new { count = cart.TotalItems });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
