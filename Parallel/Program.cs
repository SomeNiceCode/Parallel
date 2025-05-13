namespace Parallel
{
    internal class Program
    {
        public class Candidate
        {
            public string Name { get; set; }
            public int ExperienceYears { get; set; }
            public string City { get; set; }
            public List<string> Technologies { get; set; }
            public List<string> Certifications { get; set; }
            public bool KnowsPLINQ { get; set; }
            public int ExpectedSalary { get; set; }
        }
        public static async Task<List<Candidate>> LoadResumesAsync(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.txt");
            var tasks = files.Select(file => Task.Run(() => ParseResume(file)));
            return (await Task.WhenAll(tasks)).ToList();
        }

        public static Candidate ParseResume(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            return new Candidate
            {
                Name = lines[0],
                ExperienceYears = int.Parse(lines[1]),
                City = lines[2],
                Technologies = lines[3].Split(',').ToList(),
                Certifications = lines[4].Split(',').ToList(),
                KnowsPLINQ = lines[5].Contains("PLINQ"),
                ExpectedSalary = int.Parse(lines[6])
            };
        }
        public static void GenerateReports(List<Candidate> resumes)
        {
            var tasks = new List<Task>
    {
        Task.Run(() => Console.WriteLine($"Самый опытный: {resumes.MaxBy(c => c.ExperienceYears).Name}")),
        Task.Run(() => Console.WriteLine($"Наименее опытный: {resumes.MinBy(c => c.ExperienceYears).Name}")),
        Task.Run(() => Console.WriteLine($"Кандидаты из Одессы: {string.Join(", ", resumes.Where(c => c.City == "Одесса").Select(c => c.Name))}")),
        Task.Run(() => Console.WriteLine($"Макс. технологий: {resumes.MaxBy(c => c.Technologies.Count).Name}")),
        Task.Run(() => Console.WriteLine($"Макс. сертификатов: {resumes.MaxBy(c => c.Certifications.Count).Name}")),
        Task.Run(() => Console.WriteLine($"Кандидаты с PLINQ: {string.Join(", ", resumes.Where(c => c.KnowsPLINQ).Select(c => c.Name))}")),
        Task.Run(() => Console.WriteLine($"Мин. зарплата: {resumes.MinBy(c => c.ExpectedSalary).Name}")),
        Task.Run(() => Console.WriteLine($"Макс. зарплата: {resumes.MaxBy(c => c.ExpectedSalary).Name}"))
    };

            Task.WaitAll(tasks.ToArray());
        }


        static async Task Main()
        {
            Console.WriteLine("Загрузка резюме...");
            var resumes = await LoadResumesAsync("C:\\Resumes");

            Console.WriteLine("Формирование отчетов...");
            GenerateReports(resumes);
        }
    }
}
