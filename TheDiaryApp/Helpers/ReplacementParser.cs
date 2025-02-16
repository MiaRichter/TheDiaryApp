using DocumentFormat.OpenXml.Bibliography;
using OfficeOpenXml;
using TheDiaryApp.Models;

namespace TheDiaryApp.Helpers
{
    public class ReplacementParser
    {
        public Dictionary<string, Schedule> ParseReplacements(string filePath, string targetGroup)
        {
            var replacements = new Dictionary<string, Schedule>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден", filePath);
            }

            // Чтение файла в MemoryStream
            byte[] fileBytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, (int)fileStream.Length);
            }

            // Использование MemoryStream
            using (var memoryStream = new MemoryStream(fileBytes))
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int firstRow = worksheet.Dimension.Start.Row;
                int lastRow = worksheet.Dimension.End.Row;
                int firstCol = worksheet.Dimension.Start.Column;
                int lastCol = worksheet.Dimension.End.Column;
                var dayOfWeek = "";
                var rawlesson = 0;
                // Находим столбец с нужной группой
                int groupColumn = -1;
                for (int col = firstCol; col <= lastCol; col++)
                {
                    var cellValue = worksheet.Cells[firstRow, col].Text.Trim();
                    if (cellValue.Equals(targetGroup, StringComparison.OrdinalIgnoreCase))
                    {
                        groupColumn = col;
                        break;
                    }
                }

                if (groupColumn == -1) return replacements;

                // Обходим строки с заменами
                for (int row = firstRow + 1; row <= lastRow; row++)
                {
                    if (worksheet.Cells[row, firstCol].Text.Trim() != "")
                        dayOfWeek = worksheet.Cells[row, firstCol].Text.Trim();
                    var lessonNumberText = worksheet.Cells[row, firstCol + 2].Text.Trim();

                    if (!int.TryParse(lessonNumberText, out int lessonNumber) || string.IsNullOrWhiteSpace(dayOfWeek))
                        continue;

                    var lessonContent = worksheet.Cells[row, groupColumn].Text.Trim();
                    if (string.IsNullOrWhiteSpace(lessonContent))
                        continue;
                    if (lessonContent.Contains("------------"))
                    {
                        // Маркируем пару на удаление
                        replacements[$"{dayOfWeek}_{lessonNumber}"] = new Schedule
                        {
                            DayOfWeek = dayOfWeek,
                            LessonNumber = lessonNumber,
                            Subject = "REMOVE_FLAG",
                            GroupName = targetGroup
                        };
                        continue;
                    }
                    var parts = lessonContent.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim()).ToArray();

                    var schedule = new Schedule
                    {
                        DayOfWeek = dayOfWeek,
                        LessonNumber = lessonNumber,
                        Subject = parts.Length > 0 + rawlesson ? "(" + ExtractSubstring(parts[0 + rawlesson], "(", "  ") : "",
                        Teacher = parts.Length > 1 + rawlesson ? parts[1 + rawlesson] : "",
                        Room = parts.Length > 0 + rawlesson ? ExtractSubstring(parts[0 + rawlesson], "  ", "") : "",
                        GroupName = targetGroup,
                        SubGroup = 1,
                        Time = GetLessonTime(dayOfWeek, lessonNumber)
                    };
                    rawlesson = 0;
                    var key = $"{dayOfWeek}_{lessonNumber}";
                    replacements[key] = schedule;
                }
            }

            return replacements;
        }

        public string GetLessonTime(string dayOfWeek, int lessonNumber)
        {
            if (dayOfWeek == "Суббота")
            {
                return lessonNumber switch
                {
                    1 => "08:30-10:00",
                    2 => "10:10-11:40",
                    3 => "11:50-13:20",
                    4 => "13:30-15:00",
                    5 => "15:10-16:40",
                    6 => "16:50-18:20",
                    _ => "Неизвестное время"
                };
            }
            else
            {
                return lessonNumber switch
                {
                    1 => "08:30-10:00",
                    2 => "10:10-11:40",
                    3 => "12:20-13:50",
                    4 => "14:20-15:50",
                    5 => "16:00-17:30",
                    6 => "17:40-19:10",
                    _ => "Неизвестное время"
                };
            }
        }

        string ExtractSubstring(string input, string startChar, string endChar)
        {
            int startIndex = input.IndexOf(startChar);
            int endIndex = 0;
            if (endChar == "")
                endIndex = input.IndexOf(endChar, startIndex + 1) + 5;
            else
                endIndex = input.IndexOf(endChar, startIndex + 1);

            if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
            {
                return input.Substring(startIndex + 1, endIndex - startIndex - 1);
            }

            return string.Empty; // Если не найдено
        }
    }

}