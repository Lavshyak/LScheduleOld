using System;
using System.Collections.Generic;

namespace LSchedule.Kernel.Backend
{
    class TimeUtils
    {
        /// <summary>
        /// Четная или нечетная неделя сейчас
        /// </summary>
        public static int IsChetNow()
        {
            return IsChet(DateTime.Now);
        }

        /// <summary>
        /// Четная или нечетная выбранная неделя
        /// </summary>
        /// <param name="dt">Дата</param>
        public static int IsChet(DateTime dt)
        {
            if (GetNumberOfWeek(dt) % 2 == 0) return 2;
            else return 1;
        }

        /// <summary>
        /// Получить номер выбранной недели с 2021.08.30
        /// </summary>
        /// <param name="dt">Дата</param>
        public static int GetNumberOfWeek(DateTime dt)
        {
            //TimeSpan raz = dt - new DateTime(2021, 8, 30);
            TimeSpan raz = dt - new DateTime(2020, 12, 28);
            return ((int)(raz.Days / 7));
        }

        /// <summary>
        /// Получить номер настоящей недели с 2021.08.30
        /// </summary>
        public static int GetNumberOfWeekNow()
        {
            return GetNumberOfWeek(DateTime.Now);
        }

        public static string Get_DayOfWeekRU_Long(DateTime dt)
        {
            string[] daysOfWeekRU = { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье" };

            int index = (((int)dt.DayOfWeek) - 1) % 7;

            while (index < 0)
            {
                index += 7;
            }

            return daysOfWeekRU[index];
        }
        public static string Get_DayOfWeekRU_Short(DateTime dt)
        {
            string[] daysOfWeekRU = { "ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС" };

            int index = (((int)dt.DayOfWeek) - 1) % 7;

            while (index < 0)
            {
                index += 7;
            }

            return daysOfWeekRU[index];
        }
        public static string[] Get_DaysOfWeekRU_Names_Short()
        {
            string[] daysOfWeekRU = { "ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС" };
            return daysOfWeekRU;
        }

        /// <summary>
        /// используется для заполнения кнопок в Page_WatchSchedule
        /// </summary>
        /// <param name="dt">время сечас</param>
        /// <returns></returns>
        public static (string[], DateTime[]) Get_DayAndDatetime_List(DateTime dt)
        {
            dt = dt.Date;

            if (dt.DayOfWeek == DayOfWeek.Sunday)
                dt-= new TimeSpan(1,0,0,0);

            string[] daysNames = new string[14];
            DateTime[] daysDates = new DateTime[daysNames.Length];
            for (int i = 0; i < 14; i++)
            {
                DateTime ndt = dt - new TimeSpan(((int)dt.DayOfWeek-1) - i, 0, 0, 0);
                daysDates[i] = ndt;
                daysNames[i] = Get_DayOfWeekRU_Short(ndt);
            }
            return (daysNames, daysDates);
        }
    }
}
