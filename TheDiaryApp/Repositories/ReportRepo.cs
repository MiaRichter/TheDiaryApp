using TheDiaryApp.Helpers;
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
            StructuredSchedule mainSchedule;
            Dictionary<string, Schedule> replacements;
            StructuredSchedule updatedSchedule;
            // Пути к файлам
            await InitializeFilesAsync();
            string mainFilePath = Path.Combine(FileSystem.AppDataDirectory, "Main.xlsx");
            string copymainFilePath = Path.Combine(FileSystem.AppDataDirectory, "Main.xlsx");
            string replacementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Replacements.xlsx");

            // Проверка наличия интернета 2510358 2510356 1936751 2510365
            var currentNetworkAccess = Connectivity.NetworkAccess;
            if (currentNetworkAccess == NetworkAccess.Internet)
            {
                int n = 0;
                while (true)
                {
                    switch (n)
                    {
                        case 0:
                            await DownloadFileAsync(
                                $"https://newlms.magtu.ru/pluginfile.php/1936751/mod_folder/content/0/{groupName}.xlsx?forcedownload=1",
                                mainFilePath);
                            n++;
                            continue;
                        case 1:
                            await DownloadFileAsync(
                                $"https://newlms.magtu.ru/pluginfile.php/2510356/mod_folder/content/0/{groupName}.xlsx?forcedownload=1",
                                mainFilePath);
                            n++;
                            continue;
                        case 2:
                            await DownloadFileAsync(
                                $"https://newlms.magtu.ru/pluginfile.php/2510358/mod_folder/content/0/{groupName}.xlsx?forcedownload=1",
                                mainFilePath);
                            n++;
                            continue;
                        case 3:
                            await DownloadFileAsync(
                                $"https://newlms.magtu.ru/pluginfile.php/2510365/mod_folder/content/0/{groupName}.xlsx?forcedownload=1",
                                mainFilePath);
                            n++;
                            continue;
                    }
                    if (n > 3)
                        break;
                }
                if (DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Monday.ToString() || DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Thursday.ToString() || DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Sunday.ToString())
                {
                    DateTime now = DateTime.Now;
                    // Форматируем день, чтобы он был в формате "02"
                    string day = now.Day.ToString("D2");
                    // Форматируем месяц, чтобы он был в формате "02"
                    string month = now.Month.ToString("D2");
                    // Форматируем год, чтобы он был в формате "25"
                    int year = now.Year % 100;
                    string endDate = now.AddDays(2).Day.ToString("D2");
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) //если сегодня воскресенье то покажи расписание с понедельника по среду
                    {
                        day = now.AddDays(1).Day.ToString("D2");
                        endDate = now.AddDays(3).Day.ToString("D2");
                    }
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday) // если сегодня вторник то покажи расписание с понедельника по среду
                    {
                        day = now.AddDays(-1).Day.ToString("D2");
                        endDate = now.AddDays(1).Day.ToString("D2");
                    }
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour >= 21) // если сегодня среда и время 21:00 то покажи расписание с четверга по субботу
                    {
                        day = now.AddDays(1).Day.ToString("D2");
                        endDate = now.AddDays(3).Day.ToString("D2");
                    }
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Friday) // если сегодня пятница то покажи расписание с четверга по субботу
                    {
                        day = now.AddDays(-1).Day.ToString("D2");
                        endDate = now.AddDays(1).Day.ToString("D2");
                    }
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) // если сегодня суббота то покажи расписание с четверга по субботу
                    {
                        day = now.AddDays(-2).Day.ToString("D2");
                        endDate = now.AddDays(0).Day.ToString("D2");
                    }
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour >= 21) // если сегодня суббота то покажи расписание с понедельника по среду
                    {
                        day = now.AddDays(2).Day.ToString("D2");
                        endDate = now.AddDays(4).Day.ToString("D2");
                    }

                    string query = $"https://newlms.magtu.ru/pluginfile.php/1936755/mod_folder/content/0/{day}.{month}.{year}-{endDate}.{month}.{year}.xlsx?forcedownload=1";
                    await DownloadFileAsync(query, replacementsFilePath);
                }
            }
            else
            {
                Console.WriteLine("Внимание нет интернета! Расписание может быть не актуальным!"); //заглушка под вывод сообщения на экране
            }
            mainSchedule = _excelParser.ParseExcel(mainFilePath, groupName, subGroup);
            replacements = _replacementParser.ParseReplacements(replacementsFilePath, groupName, subGroup);
            updatedSchedule = ApplyReplacements(mainSchedule, replacements);


            return updatedSchedule;
        }

        private static async Task DownloadFileAsync(string url, string outputPath)
        {
            try
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
            catch (Exception ex)
            {
            }
        }

        public StructuredSchedule ApplyReplacements(StructuredSchedule mainSchedule, Dictionary<string, Schedule> replacements)
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
                        // Удаляем пары, помеченные на удаление
                        var removals = dayReplacements
                            .Where(r => r.Value.Subject == "REMOVE_FLAG")
                            .Select(r => r.Value.LessonNumber)
                            .ToList();

                        daySchedules.RemoveAll(schedule => removals.Contains(schedule.LessonNumber));

                        // Применяем замены
                        foreach (var replacement in dayReplacements.Values)
                        {
                            if (replacement.Subject == "REMOVE_FLAG")
                                continue; // Пропускаем удаленные пары

                            var existing = daySchedules.FirstOrDefault(s => s.LessonNumber == replacement.LessonNumber);
                            if (existing != null)
                            {
                                // Обновляем существующую пару
                                existing.Subject = replacement.Subject;
                                existing.Teacher = replacement.Teacher;
                                existing.Room = replacement.Room;
                                existing.Time = replacement.Time;
                            }
                            else
                            {
                                // Добавляем новую пару
                                daySchedules.Add(replacement);
                            }
                        }

                        // Сортируем пары по номеру
                        daySchedules.Sort((a, b) => a.LessonNumber.CompareTo(b.LessonNumber));
                    }
                }
            }

            return mainSchedule;
        }

        private async Task InitializeFilesAsync()
        {
            string mainFilePath = Path.Combine(FileSystem.AppDataDirectory, "Main.xlsx");
            string replacementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Replacements.xlsx");

            // Если файлы уже существуют, пропустим копирование
            if (File.Exists(mainFilePath) && File.Exists(replacementsFilePath))
                return;

            // Копирование Main.xlsx
            using (var mainStream = await FileSystem.OpenAppPackageFileAsync("Main.xlsx"))
            using (var mainFileStream = new FileStream(mainFilePath, FileMode.Create, FileAccess.Write))
            {
                await mainStream.CopyToAsync(mainFileStream);
            }

            // Копирование Replacements.xlsx
            using (var replacementsStream = await FileSystem.OpenAppPackageFileAsync("Replacements.xlsx"))
            using (var replacementsFileStream = new FileStream(replacementsFilePath, FileMode.Create, FileAccess.Write))
            {
                await replacementsStream.CopyToAsync(replacementsFileStream);
            }
        }

    }
}