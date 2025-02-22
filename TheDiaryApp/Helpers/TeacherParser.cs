using OfficeOpenXml;
using TheDiaryApp.Models;
using System.IO.MemoryMappedFiles;
using DocumentFormat.OpenXml.Bibliography;

namespace TheDiaryApp.Helpers
{
    public class TeacherParser
    {

        public StructuredSchedule ParseTeacher(string filePath)
        {
            Schedule schedule;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Указываем контекст лицензии для использования EPPlus

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Берем первый лист
                int rowCount = worksheet.Dimension.Rows; // Количество строк в файле
                int columnCount = worksheet.Dimension.Columns; // Количество столбцов в файле

                // Перебираем строки, начиная со второй (первая — заголовки)
                for (int row = 2; row <= rowCount; row++)
                {
                    for (int col = 1; col <= columnCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Text; // Получаем текст из ячейки

                       // if (cellValue.Contains( StringComparison.OrdinalIgnoreCase))
                       // {
                            schedule = new Schedule
                            {
                                DayOfWeek = worksheet.Cells[row, 1].Text, // Предполагаем, что первый столбец - день недели
                                LessonNumber = int.TryParse(worksheet.Cells[row, 2].Text, out int lessonNum) ? lessonNum : 0, // Второй столбец - номер пары
                                Subject = worksheet.Cells[row, 3].Text, // Третий столбец - предмет
                                Teacher = worksheet.Cells[row, col].Text, // Преподаватель найден в текущем столбце
                                Room = worksheet.Cells[row, 5].Text, // Пятый столбец - аудитория
                                Time = worksheet.Cells[row, 6].Text  // Шестой столбец - время пары
                            };
                     //   }
                    }
                    var key = $"{DayOfWeek}_{LessonNumber}";
                    replacements[key] = schedule;
                }
                return schedule;
            }
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
