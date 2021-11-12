using System;
using Xamarin.Forms;
using System.IO;

namespace LSchedule.Kernel
{
    /// <summary>
    /// Готовые расписания для вставляния в интерфейс
    /// </summary>
    static class ScheduleGetter
    {
        /// <summary>
        /// Получить нужное расписание для нужного вуза 
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static StackLayout GetSchedule(ScheduleParams scheduleParams, DateTime dt)
        {
            StackLayout ret = new StackLayout() { Orientation=StackOrientation.Vertical, VerticalOptions=LayoutOptions.EndAndExpand };

            //USURT
            if (scheduleParams.Institution=="USURT")
            {
                return Backend.ScheduleManager_USURT.GetSchedule(scheduleParams, dt, ret);
            }
            ret.Children.Add(new Label() { Text="неверное название уч. заведения" });
            return ret;
        }

        public static StackLayout IfFileNotFound()
        {
            StackLayout ret = new StackLayout() { Orientation = StackOrientation.Vertical };
            ret.Children.Add(new Label() { TextColor = Color.Red, FontSize = 25, Text="не удалось скачать файл с расписанием"});
            return ret;
        }
    }
}
