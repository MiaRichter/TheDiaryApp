using OfficeOpenXml;
using TheDiaryApp.Models;
using System.Collections.Generic;
using System.IO;

namespace TheDiaryApp.Helpers
{
    public class TeacherParser
    {
        public StructuredSchedule ParseTeacher(string filePath)
        {
            var result = new StructuredSchedule
            {
                WeekData = new Dictionary<string, Dictionary<string, List<Schedule>>>()
            };

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Указываем контекст лицензии для использования EPPlus

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Берем первый лист
                int rowCount = worksheet.Dimension.Rows; // Количество строк в файле
                int columnCount = worksheet.Dimension.Columns; // Количество столбцов в файле

                // Перебираем строки, начиная со второй (первая — заголовки)
                for (int row = 2; row <= rowCount; row++)
                {
                    // Получаем данные из строки
                    string dayOfWeek = worksheet.Cells[row, 1].Text; // День недели
                    int lessonNumber = int.TryParse(worksheet.Cells[row, 2].Text, out int lessonNum) ? lessonNum : 0; // Номер пары
                    string subject = worksheet.Cells[row, 3].Text; // Предмет
                    string teacher = worksheet.Cells[row, 4].Text; // Преподаватель
                    string room = worksheet.Cells[row, 5].Text; // Аудитория
                    string time = worksheet.Cells[row, 6].Text; // Время пары

                    // Создаем объект Schedule
                    var schedule = new Schedule
                    {
                        DayOfWeek = dayOfWeek,
                        LessonNumber = lessonNumber,
                        Subject = subject,
                        Teacher = teacher,
                        Room = room,
                        Time = time
                    };

                    // Добавляем данные в структуру WeekData
                    if (!result.WeekData.ContainsKey(dayOfWeek))
                    {
                        result.WeekData[dayOfWeek] = new Dictionary<string, List<Schedule>>();
                    }

                    if (!result.WeekData[dayOfWeek].ContainsKey(teacher))
                    {
                        result.WeekData[dayOfWeek][teacher] = new List<Schedule>();
                    }

                    result.WeekData[dayOfWeek][teacher].Add(schedule);
                }
            }

            return result;
        }
    }
}