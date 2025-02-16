﻿using OfficeOpenXml;
using TheDiaryApp.Models;
using System.IO.MemoryMappedFiles;

namespace TheDiaryApp.Helpers
{
    public class ExcelParser
    {
        public StructuredSchedule ParseExcel(string filePath, string groupName, int subGroup)
        {
            var result = new StructuredSchedule();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
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
                int totalRows = worksheet.Dimension.Rows;

                string currentWeekType = "";
                var dayColumns = new Dictionary<string, int>();

                for (int rowIdx = 1; rowIdx <= totalRows; rowIdx++)
                {
                    var firstCellValue = worksheet.Cells[rowIdx, 1].Text.Trim();

                    // Определение типа недели
                    if (firstCellValue is "Нечетная неделя" or "Четная неделя")
                    {
                        currentWeekType = firstCellValue;
                        dayColumns.Clear();
                        result.WeekData[currentWeekType] = new Dictionary<string, List<Schedule>>();
                        continue;
                    }

                    // Поиск заголовков дней недели
                    if (IsDayOfWeek(firstCellValue))
                    {
                        dayColumns.Clear(); // Очистка перед новым блоком дней
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            var cellValue = worksheet.Cells[rowIdx, col].Text.Trim();
                            if (IsDayOfWeek(cellValue) && !dayColumns.ContainsKey(cellValue))
                            {
                                dayColumns[cellValue] = col;
                                result.WeekData[currentWeekType][cellValue] = new List<Schedule>();
                            }
                        }
                        continue;
                    }

                    // Парсинг данных для каждого дня
                    foreach (var day in dayColumns)
                    {
                        int dayCol = day.Value;
                        var lessonCell = worksheet.Cells[rowIdx, dayCol];

                        // Проверка жирного шрифта (номер пары)
                        if (lessonCell.Style.Font.Bold && int.TryParse(lessonCell.Text.Trim(), out int lessonNumber))
                        {
                            // Предмет через 1 столбец
                            var subject = worksheet.Cells[rowIdx, dayCol + 1].Text.Trim();

                            // Преподаватель и аудитория через 1 строку и 4 столбца
                            if (rowIdx + 1 <= totalRows)
                            {
                                var teacher = worksheet.Cells[rowIdx + 1, dayCol + 1].Text.Trim();
                                var room = worksheet.Cells[rowIdx + 1, dayCol + 4].Text.Trim();

                                if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(teacher))
                                {
                                    // Определение времени проведения пары
                                    string time = GetLessonTime(day.Key, lessonNumber);

                                    result.WeekData[currentWeekType][day.Key].Add(new Schedule
                                    {
                                        GroupName = groupName,
                                        SubGroup = subGroup,
                                        WeekType = currentWeekType,
                                        DayOfWeek = day.Key,
                                        LessonNumber = lessonNumber,
                                        Subject = subject,
                                        Teacher = teacher,
                                        Room = room,
                                        Time = time
                                    });
                                }
                            }
                        }
                    }
                }
            }

            // Сортировка пар по номеру
            foreach (var week in result.WeekData.Values)
            {
                foreach (var day in week.Values)
                {
                    day.Sort((a, b) => a.LessonNumber.CompareTo(b.LessonNumber));
                }
            }

            return result;
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

        private bool IsDayOfWeek(string value) =>
            value is "Понедельник" or "Вторник" or "Среда"
                or "Четверг" or "Пятница" or "Суббота";
    }
}