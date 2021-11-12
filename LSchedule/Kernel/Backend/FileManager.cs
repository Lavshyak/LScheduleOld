using Xamarin.Essentials;
using System.IO;
using System;

namespace LSchedule.Kernel.Backend
{
    class FileManager
    {
        static public string GetPath_Schedule(ScheduleParams scheduleParams, DateTime dt)
        {
            string dir = $"{FileSystem.CacheDirectory}/Schedules/{scheduleParams.Institution}/{scheduleParams.Age}/{scheduleParams.Department}/{scheduleParams.Direction}/{TimeUtils.IsChet(dt)}";
            CreateDirectory(dir);
            return dir + $"/{TimeUtils.GetNumberOfWeek(dt)}.schedule";
        }


        static public string GetPath_InstitutionXml(ScheduleParams scheduleParams)
        {
            string dir = $"{FileSystem.AppDataDirectory}/InstitutionXmls";
            CreateDirectory(dir);
            return dir+$"/{scheduleParams}.xml";
        }
        static public string GetPath_ScheduleParamsFavoriteXml()
        {
            string dir = $"{FileSystem.AppDataDirectory}";
            CreateDirectory(dir);
            return dir + "/ScheduleParamsFavorites.xml";
        }
        static private void CreateDirectory(string dir)
        {
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
