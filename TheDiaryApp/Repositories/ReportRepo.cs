using TheDiaryApp.Helpers;
using TheDiaryApp.Models;

namespace TheDiaryApp.Repositories
{
    public class ReportRepo
    {
        private readonly ExcelParser _excelParser;
        private readonly ReplacementParser _replacementParser;

        public ReportRepo(ExcelParser excelParser, ReplacementParser replacementParser)
        {
            _excelParser = excelParser;
            _replacementParser = replacementParser;
        }

        public async Task<StructuredSchedule> ReportAsync(string groupName, int subGroup)
        {
            // Пути к файлам
            string mainFilePath = Path.Combine(FileSystem.AppDataDirectory, "Main.xlsx");
            string replacementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Replacements.xlsx");

            // Скачиваем файлы
            await DownloadFileAsync("https://newlms.magtu.ru/pluginfile.php/2510356/mod_folder/content/0/%D0%9A%D1%81%D0%9A-21-1.xlsx?forcedownload=1", mainFilePath);
            await DownloadFileAsync("https://newlms.magtu.ru/pluginfile.php/1936755/mod_folder/content/0/13.02.25-15.02.25.xlsx?forcedownload=1", replacementsFilePath);

            // Парсим основной файл
            var mainSchedule = _excelParser.ParseExcel(mainFilePath, groupName, subGroup);
            // Парсим файл замен
            var replacements = _replacementParser.ParseReplacements(replacementsFilePath, groupName);

            // Применяем замены к основному расписанию
            var updatedSchedule = ApplyReplacements(mainSchedule, replacements);

            return updatedSchedule;
        }

        private static async Task DownloadFileAsync(string url, string outputPath)
        {
            using (HttpClient httpClient = new())
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string directory = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }
        }

        public StructuredSchedule ApplyReplacements(
    StructuredSchedule mainSchedule,
    Dictionary<string, Schedule> replacements)
        {
            foreach (var weekPair in mainSchedule.WeekData)
            {
                var weekType = weekPair.Key; // Тип недели (Нечетная/Четная)
                foreach (var dayPair in weekPair.Value)
                {
                    var dayOfWeek = dayPair.Key; // День недели (Понедельник, Вторник и т.д.)
                    var daySchedules = dayPair.Value; // Список пар на этот день

                    // Ищем замены для этого дня недели
                    var dayReplacements = replacements
                        .Where(r => r.Key.StartsWith(dayOfWeek))
                        .ToDictionary(r => r.Key, r => r.Value);

                    if (dayReplacements.Any())
                    {
                        // Очищаем список пар на этот день


                        // Добавляем замены
                        foreach (var replacement in dayReplacements.Values)
                        {
                            var lessonreplays = -1;
                            for (var i = 0; i < daySchedules.Count; i++)
                            {
                                if (daySchedules[i].LessonNumber == replacement.LessonNumber)
                                {
                                    lessonreplays = i;
                                    continue;
                                }
                            }
                            if (lessonreplays > -1)
                            {
                                daySchedules[lessonreplays].DayOfWeek = dayOfWeek;
                                daySchedules[lessonreplays].LessonNumber = replacement.LessonNumber;
                                daySchedules[lessonreplays].Subject = replacement.Subject;
                                daySchedules[lessonreplays].Teacher = replacement.Teacher;
                                daySchedules[lessonreplays].Room = replacement.Room;
                                daySchedules[lessonreplays].GroupName = replacement.GroupName;
                                daySchedules[lessonreplays].SubGroup = replacement.SubGroup;
                            }
                            else
                            {
                                // Добавляем замену в список
                                daySchedules.Add(new Schedule
                                {
                                    DayOfWeek = dayOfWeek,
                                    LessonNumber = replacement.LessonNumber,
                                    Subject = replacement.Subject,
                                    Teacher = replacement.Teacher,
                                    Room = replacement.Room,
                                    GroupName = replacement.GroupName,
                                    SubGroup = replacement.SubGroup
                                });
                            }
                        }

                        // Сортируем пары по номеру
                        daySchedules.Sort((a, b) => a.LessonNumber.CompareTo(b.LessonNumber));
                    }
                }
            }

            return mainSchedule;
        }
    }
}