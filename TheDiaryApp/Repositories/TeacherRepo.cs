using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TheDiaryApp.Helpers;
using TheDiaryApp.Models;

namespace TheDiaryApp.Repositories
{
    public class TeacherRepo
    {
        private readonly TeacherParser _teacherParser;
        private readonly ReplacementParser _replacementParser;

        public TeacherRepo(TeacherParser teacherParser, ReplacementParser replacementParser)
        {
            _teacherParser = teacherParser;
            _replacementParser = replacementParser;
        }

        public async Task<List<TeacherShedule>> ReportTAsync(string teacherName)
        {
            List<TeacherShedule> teacherSchedules;
            //Dictionary<string, Schedule> replacements;
            //StructuredSchedule updatedSchedule;

            // Пути к файлам
            string teacherFilePath = Path.Combine(FileSystem.AppDataDirectory, "Teachers.xlsx");
            //string replacementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Replacements.xlsx");

            // Всегда пытаемся использовать локальные файлы
            if (!File.Exists(teacherFilePath))
                await InitializeFilesAsync();

            // Проверка наличия интернета
            var currentNetworkAccess = Connectivity.NetworkAccess;
            try
            {
                if (currentNetworkAccess == NetworkAccess.Internet)
                {
                    //await DownloadFileAsync("https://newlms.magtu.ru/pluginfile.php/541373/mod_folder/content/0/Высписки%20из%20расписания%202%20семестра%20от17.02.2025.xlsx?forcedownload=1",teacherFilePath);

                    if (DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Monday.ToString() || DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Thursday.ToString() || DateTime.Now.DayOfWeek.ToString() == DayOfWeek.Sunday.ToString())
                    {
                        DateTime now = DateTime.Now;
                        string day = now.Day.ToString("D2");
                        string month = now.Month.ToString("D2");
                        int year = now.Year % 100;
                        string endDate = now.AddDays(2).Day.ToString("D2");

                        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                        {
                            day = now.AddDays(1).Day.ToString("D2");
                            endDate = now.AddDays(3).Day.ToString("D2");
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                        {
                            day = now.AddDays(-1).Day.ToString("D2");
                            endDate = now.AddDays(1).Day.ToString("D2");
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday && DateTime.Now.Hour >= 21)
                        {
                            day = now.AddDays(1).Day.ToString("D2");
                            endDate = now.AddDays(3).Day.ToString("D2");
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                        {
                            day = now.AddDays(-1).Day.ToString("D2");
                            endDate = now.AddDays(1).Day.ToString("D2");
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                        {
                            day = now.AddDays(-2).Day.ToString("D2");
                            endDate = now.AddDays(0).Day.ToString("D2");
                        }
                        if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour >= 21)
                        {
                            day = now.AddDays(2).Day.ToString("D2");
                            endDate = now.AddDays(4).Day.ToString("D2");
                        }

                        //string query = $"https://newlms.magtu.ru/pluginfile.php/1936755/mod_folder/content/0/{day}.{month}.{year}-{endDate}.{month}.{year}.xlsx?forcedownload=1";
                        //await DownloadFileAsync(query, replacementsFilePath);
                    }
                }
            }
            catch { }

            teacherSchedules = _teacherParser.ParseTeacher(teacherFilePath, teacherName);
            //replacements = _replacementParser.ParseReplacements(replacementsFilePath, groupName, subGroup);
            //updatedSchedule = ApplyReplacements(teacherSchedules, replacements);
            SaveLocalCopy(teacherSchedules);
            return teacherSchedules;
        }

        private void SaveLocalCopy(List<TeacherShedule> schedules)
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "offline_schedule.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(schedules));
        }

        public List<TeacherShedule> LoadLocalSchedule()
        {
            string path = Path.Combine(FileSystem.AppDataDirectory, "offline_schedule.json");

            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<List<TeacherShedule>>(File.ReadAllText(path));
            }

            return null;
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
                // Обработка ошибок
            }
        }

        public List<TeacherShedule> ApplyReplacements(List<TeacherShedule> teacherSchedules, Dictionary<string, Schedule> replacements)
        {
            // Логика применения замен
            // В зависимости от структуры ваших данных, нужно адаптировать этот метод
            return teacherSchedules;
        }

        private async Task InitializeFilesAsync()
        {
            string teacherFilePath = Path.Combine(FileSystem.AppDataDirectory, "Teachers.xlsx");
            string replacementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "Replacements.xlsx");

            // Если файлы уже существуют, пропустим копирование
            if (File.Exists(teacherFilePath) && File.Exists(replacementsFilePath))
                return;

            // Копирование Teacher.xlsx
            using (var teacherStream = await FileSystem.OpenAppPackageFileAsync("Teachers.xlsx"))
            using (var teacherFileStream = new FileStream(teacherFilePath, FileMode.Create, FileAccess.Write))
            {
                await teacherStream.CopyToAsync(teacherFileStream);
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