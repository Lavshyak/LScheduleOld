using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using Xamarin.Forms;

namespace LSchedule.Kernel.Backend
{
    /// <summary>
    /// Все для USURT
    /// </summary>
    static class ScheduleManager_USURT
    {
        /// <summary>
        /// Скачать расписание
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <param name="dt"></param>
        /// <returns>1 - скачалось, 0 - не скачалось, -1 - не скачалось и файла нет</returns>
        public static int DownloadSchedule(ScheduleParams scheduleParams, DateTime dt)
        {
            string path = FileManager.GetPath_Schedule(scheduleParams, dt);
            string url = InstitutionsXmlManager.Get_ScheduleUrl(scheduleParams, TimeUtils.IsChet(dt));
            int ret = Weber.Download(path, url);
            return ret;
        }

        static private System.Data.DataTable GetTable(string path)
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (IExcelDataReader reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    var dataTable = reader.AsDataSet(conf).Tables[0];

                    return dataTable;
                }
            }
        }

        static public string[] GetNamesOfDirections(string path)
        {
            var dataTable = GetTable(path);

            int maxY = dataTable.Rows.Count, maxX = dataTable.Columns.Count;
            int headY = 0, napravlX = 0;

            List<string> namesOfDirections = new List<string>();

            while (headY < maxY)
            {
                if (dataTable.Rows[headY][0].ToString().ToLower().Trim().Contains("день"))
                {
                    break;
                }
                headY++;
            }

            napravlX += 2;
            while (napravlX < maxX)
            {
                if (dataTable.Rows[headY][napravlX].ToString().Trim() != "" || napravlX < maxX)
                {
                    namesOfDirections.Add(dataTable.Rows[headY][napravlX].ToString());
                }
                napravlX++;
            }


            return namesOfDirections.ToArray();
        }

        /// <summary>
        /// Получить конечное расписание
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <param name="dt"></param>
        /// <param name="ret"></param>
        /// <returns></returns>
        static public StackLayout GetSchedule(ScheduleParams scheduleParams, DateTime dt, StackLayout ret)
        {
            bool addedMessage = false;
            string path = Backend.FileManager.GetPath_Schedule(scheduleParams, dt);
            //удостовериться в том, что файл скачан
            if (!File.Exists(path) || (DateTime.Now.DayOfYear - File.GetLastWriteTime(path).DayOfYear >= 1 && DateTime.Now.Hour >= 12))
            {
                int downloadResult = Backend.ScheduleManager_USURT.DownloadSchedule(scheduleParams, dt);

                if (downloadResult == -1)
                    return ScheduleGetter.IfFileNotFound();
                else if (downloadResult == 0)
                {
                    addedMessage = true;
                    ret.Children.Add(new Label() { Text = $"файл с расписанием был обновлен {(DateTime.Now - File.GetLastWriteTime(path)).TotalHours} часов назад" });
                }
            }

            //все остальное
            string[][] scheduleList = GetScheduleSS(scheduleParams, dt);

            string[] couple;
            for (int i = 0; i < scheduleList.Length; i++)
            {
                couple = scheduleList[i];
                if (couple[0] != "" && couple[1] != "")
                {
                    ret.Children.Add(new Label() { TextColor = Color.Red, Text = couple[0], Margin = new Thickness(20, 0, 0, -10), FontSize = 20 });
                    ret.Children.Add(new Editor() { TextColor = Color.Green, IsReadOnly = true, Text = "1) " + couple[1] });
                }
                else if ((i + 1) < scheduleList.Length && couple[0] != "" && scheduleList[i + 1][1] != "" && scheduleList[i + 1][0] == "")
                    ret.Children.Add(new Label() { TextColor = Color.Red, Text = couple[0], Margin = new Thickness(20, 0, 0, -10), FontSize = 20 });
                else if (couple[1] != "")
                    ret.Children.Add(new Editor() { TextColor = Color.Green, IsReadOnly = true, Text = "1) " + couple[1] });
            }

            if ((ret.Children.Count < 1 && addedMessage == false) || (ret.Children.Count < 2 && addedMessage == true))
            {
                ret.Children.Add(new Editor
                {
                    Text = "Скорее всего, в этот день нет пар"
                });
            }

            return ret;
        }

        /// <summary>
        /// Спарсить расписание
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        static private string[][] GetScheduleSS(ScheduleParams scheduleParams, DateTime dt)
        {
            string path = FileManager.GetPath_Schedule(scheduleParams, dt);

            var dataTable = GetTable(path);

            List<string[]> scheduleList = new List<string[]>();

            string dayOfWeek = TimeUtils.Get_DayOfWeekRU_Long(dt).ToLower();
            string direction = scheduleParams.Direction.ToLower();

            int maxY = dataTable.Rows.Count, maxX = dataTable.Columns.Count;
            int headY = 0, napravlX = 0;

            while (headY < maxY)
            {
                if (dataTable.Rows[headY][0].ToString().ToLower().Trim().Contains("день")) break;
                headY++;
            }
            while (napravlX < maxX)
            {
                if (dataTable.Rows[headY][napravlX].ToString().ToLower().Trim().Contains(direction)) break;
                napravlX++;
            }

            for (int i = headY + 1; i < maxY; i++)
            {
                if (dataTable.Rows[i][0].ToString().ToLower().Contains(dayOfWeek))
                {
                    string nowKey, nowValue;

                    nowKey = dataTable.Rows[i][1].ToString().Trim();
                    nowValue = dataTable.Rows[i][napravlX].ToString().Trim();

                    scheduleList.Add(new string[] { nowKey, nowValue });
                    i++;


                    while (i < maxY && !dataTable.Rows[i][1].ToString().Trim().Contains("10:05"))
                    {
                        nowKey = dataTable.Rows[i][1].ToString().Trim();
                        nowValue = dataTable.Rows[i][napravlX].ToString().Trim();

                        scheduleList.Add(new string[] { nowKey, nowValue });

                        i++;
                    }
                    break;
                }
            }

            return scheduleList.ToArray();
        }
    }
}
