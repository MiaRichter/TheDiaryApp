using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using TheDiaryApp.Models;

namespace TheDiaryApp.Helpers
{
    public class TeacherParser
    {
        public List<TeacherShedule> ParseTeacher(string filePath, string teacherName)
        {
            List<TeacherShedule> result = new List<TeacherShedule>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Указываем контекст лицензии для использования EPPlus

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets["Преподаватели"]; // Указываем имя листа

                if (worksheet == null)
                {
                    throw new Exception("Лист 'Преподаватели' не найден.");
                }

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                bool isTeacherFound = false;

                for (int row = 1; row <= rowCount; row++)
                {
                    var cellValue = worksheet.Cells[row, 2].Text; // Проверяем ячейку в столбце B (имя преподавателя)

                    if (cellValue.Contains(teacherName))
                    {
                        isTeacherFound = true;

                        // Начинаем парсинг расписания
                        for (int i = row + 1; i <= rowCount; i++)
                        {
                            var dayOfWeek = worksheet.Cells[i, 1].Text; // День недели
                            var time = worksheet.Cells[i, 2].Text; // Время
                            var subject = worksheet.Cells[i, 3].Text; // Предмет
                            var room = worksheet.Cells[i, 4].Text; // Аудитория
                            var groupName = worksheet.Cells[i, 5].Text; // Название группы

                            if (string.IsNullOrEmpty(dayOfWeek) && string.IsNullOrEmpty(time) && string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(room) && string.IsNullOrEmpty(groupName))
                            {
                                break; // Прерываем цикл, если достигли конца расписания
                            }

                            result.Add(new TeacherShedule
                            {
                                Teacher = teacherName,
                                LessenNumber = GetLessenNumber(time), // Получаем номер пары из времени
                                Subject = subject,
                                Room = room,
                                GroupName = groupName,
                                Time = time,
                                IsToday = IsToday(dayOfWeek) // Проверяем, сегодня ли этот день
                            });
                        }

                        break; // Прерываем цикл, так как нашли преподавателя
                    }
                }

                if (!isTeacherFound)
                {
                    throw new Exception($"Преподаватель '{teacherName}' не найден в файле.");
                }
            }

            return result;
        }

        private string GetLessenNumber(string time)
        {
            // Пример: если время "08:30-10:00", то это 1 пара
            // В зависимости от вашего формата времени, можно добавить логику
            if (time.Contains("08:30-10:00")) return "1";
            if (time.Contains("10:10-11:40")) return "2";
            if (time.Contains("12:20-13:50")) return "3";
            if (time.Contains("14:00-15:30")) return "4";
            if (time.Contains("15:40-17:10")) return "5";
            if (time.Contains("17:20-18:50")) return "6";
            if (time.Contains("19:00-20:30")) return "7";
            return "Unknown";
        }

        private bool IsToday(string dayOfWeek)
        {
            // Проверяем, совпадает ли день недели с текущим днем
            var today = DateTime.Now.DayOfWeek.ToString();
            return dayOfWeek.Contains(today, StringComparison.OrdinalIgnoreCase);
        }
    }
}