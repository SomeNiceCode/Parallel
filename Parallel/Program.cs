namespace Parallel
{
    internal class Program
    {
        public class Candidate
        {
            public string Name { get; set; }
            public int ExperienceYears { get; set; }
            public string City { get; set; }
            public List<string> Technologies { get; set; } = new List<string>();
            public List<string> Certifications { get; set; } = new List<string>();
            public bool KnowsPLINQ { get; set; }
            public int ExpectedSalary { get; set; }


            public static async Task<List<Candidate>> LoadResumesAsync(string folderPath)
            {
                if (!Directory.Exists(folderPath))
                {
                    Console.WriteLine("Ошибка: указанная папка не существует.");
                    return new List<Candidate>(); // Исправлено: возвращаем пустой список, а не `null`
                }

                var files = Directory.GetFiles(folderPath, "*.txt");
                var tasks = files.Select(file => Task.Run(() => ParseResume(file)));

                var results = await Task.WhenAll(tasks);
                return results.Where(c => c != null).ToList(); // Исправлено: фильтрация `null`
            }

            public static Candidate ParseResume(string filePath)
            {
                var lines = File.ReadAllLines(filePath);
                if (lines.Length < 7) return null; // Исправлено: проверка корректности файла

                return new Candidate
                {
                    Name = lines[0],
                    ExperienceYears = int.TryParse(lines[1], out var exp) ? exp : 0,
                    City = lines[2],
                    Technologies = lines[3]?.Split(',').ToList() ?? new List<string>(),
                    Certifications = lines[4]?.Split(',').ToList() ?? new List<string>(),
                    KnowsPLINQ = lines[5]?.Contains("PLINQ") ?? false,
                    ExpectedSalary = int.TryParse(lines[6], out var salary) ? salary : 0
                };
            }
            public static void GenerateReports(List<Candidate> resumes)
            {
                if (resumes == null || resumes.Count == 0)
                {
                    Console.WriteLine("Ошибка: резюме не загружены!");
                    return;
                }

                var tasks = new List<Task>
    {
        Task.Run(() => Console.WriteLine($"Самый опытный: {resumes.Where(c => c != null).OrderByDescending(c => c.ExperienceYears).FirstOrDefault()?.Name ?? "Нет данных"}")),
        Task.Run(() => Console.WriteLine($"Наименее опытный: {resumes.Where(c => c != null).OrderBy(c => c.ExperienceYears).FirstOrDefault()?.Name ?? "Нет данных"}")),
        Task.Run(() => Console.WriteLine($"Кандидаты из Одессы: {string.Join(", ", resumes.Where(c => c?.City == "Одесса").Select(c => c.Name))}")),
        Task.Run(() => Console.WriteLine($"Макс. технологий: {resumes.Where(c => c?.Technologies != null).OrderByDescending(c => c.Technologies.Count).FirstOrDefault()?.Name ?? "Нет данных"}")),
        Task.Run(() => Console.WriteLine($"Макс. сертификатов: {resumes.Where(c => c?.Certifications != null).OrderByDescending(c => c.Certifications.Count).FirstOrDefault()?.Name ?? "Нет данных"}")),
        Task.Run(() => Console.WriteLine($"Кандидаты с PLINQ: {string.Join(", ", resumes.Where(c => c?.KnowsPLINQ == true).Select(c => c.Name))}")),
        Task.Run(() => Console.WriteLine($"Мин. зарплата: {resumes.Where(c => c != null).OrderBy(c => c.ExpectedSalary).FirstOrDefault()?.Name ?? "Нет данных"}")),
        Task.Run(() => Console.WriteLine($"Макс. зарплата: {resumes.Where(c => c != null).OrderByDescending(c => c.ExpectedSalary).FirstOrDefault()?.Name ?? "Нет данных"}"))
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
}
