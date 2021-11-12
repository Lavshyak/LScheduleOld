using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;

namespace LSchedule.Kernel.Backend
{
    static class InstitutionsXmlManager
    {
        /// <summary>
        /// Словарь уч заведение:ссылка на xml
        /// </summary>
        public static readonly Dictionary<string, string> InstitutionsDictionary = new Dictionary<string, string>()
            {
                {"USURT","https://raw.githubusercontent.com/Lavshyak/LSchedule/main/InstitutionsData/USURT.xml"}
            };

        /// <summary>
        /// Получить список коротких названий поддерживаемых уч заведений
        /// </summary>
        /// <returns>array of names</returns>
        public static string[] GetInstitutionsNamesArray()
        {
            return InstitutionsDictionary.Keys.ToArray();
        }

        //получить список коротких названий того, че есть в уч заведении
        public static string[] Get_Ages_NamesShort(ScheduleParams scheduleParams)
        {
            return GetShortNamesOfElements(GetAges(scheduleParams));
        }
        public static string[] Get_Departments_NamesShort(ScheduleParams scheduleParams)
        {
            return GetShortNamesOfElements(GetDepartments(scheduleParams));
        }
        public static string[] Get_Directions_NamesShort(ScheduleParams scheduleParams)
        {
            if(scheduleParams.Institution == "USURT")
            {
                for(int i=-7; i<=7; i+=7)
                {
                    string path = FileManager.GetPath_Schedule(scheduleParams, DateTime.Now + new TimeSpan(i, 0, 0, 0));
                    if (ScheduleManager_USURT.DownloadSchedule(scheduleParams, DateTime.Now + new TimeSpan(i, 0, 0, 0)) == -1) continue;
                    return ScheduleManager_USURT.GetNamesOfDirections(path);
                }
            }
            return null;
        }

        /// <summary>
        /// Получить ссылку на расписание
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <param name="chet">1-нечетная, 2-четная</param>
        /// <returns>Ссылка в виде строки</returns>
        public static string Get_ScheduleUrl(ScheduleParams scheduleParams, int chet)
        {
            XElement[] departments = GetDepartments(scheduleParams);
            foreach(XElement xE in departments)
            {
                if(xE.Attribute("NameShort").Value==scheduleParams.Department)
                {
                    string url;

                    if (chet == 1)
                        url = xE.Element("NChet").Value;
                    else if (chet == 2)
                        url = xE.Element("Chet").Value;
                    else
                        throw new Exception($"{chet} - ни четное, ни нечетное");

                    return url;
                }
            }
            return null;
        }



        //приватное

        /// <summary>
        /// xml уч заведения если не актуален, сделать актуальным.
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <returns>1 - актуален, 0 - старый, -1 - нет файла</returns>
        private static int InstitutionXmlToActual(ScheduleParams scheduleParams)
        {
            string path = FileManager.GetPath_InstitutionXml(scheduleParams);
            if((DateTime.Now-File.GetLastWriteTime(path)).TotalDays>=7 || !File.Exists(FileManager.GetPath_InstitutionXml(scheduleParams)))
            {
                bool tgv = InstitutionsDictionary.TryGetValue(scheduleParams.Institution, out string url);
                if (tgv == false)
                    throw new Exception($"\"{scheduleParams.Institution}\" - не удалось найти такое название вуза");

                return Weber.Download(path, url);
            }
            return 1;
        }

        /// <summary>
        /// Получить корневой элемент xml уч заведения
        /// </summary>
        /// <param name="scheduleParams"></param>
        /// <returns></returns>
        private static XElement GetInstitutionXDocRoot(ScheduleParams scheduleParams)
        {
            int wtf = InstitutionXmlToActual(scheduleParams);
            if (wtf == -1)
                throw new Exception("Не удалось скачать xml уч заведения");
            XDocument doc = XDocument.Load(FileManager.GetPath_InstitutionXml(scheduleParams));
            return doc.Root;
        }

        // Получить нужные элементы
        private static XElement[] GetAges(ScheduleParams scheduleParams)
        {
            XElement root = GetInstitutionXDocRoot(scheduleParams);
            return root.Element("Ages").Elements().ToArray();
        }
        private static XElement[] GetDepartments(ScheduleParams scheduleParams)
        {
            XElement[] ages = GetAges(scheduleParams);
            foreach(XElement xE in ages)
            {
                if(xE.Attribute("NameShort").Value==scheduleParams.Age)
                {
                    return xE.Element("Departments").Elements().ToArray();
                }
            }
            return null;
        }


        /// <summary>
        /// Получить короткие названия всех переданных элементов
        /// </summary>
        /// <param name="xElements"></param>
        /// <returns></returns>
        private static string[] GetShortNamesOfElements(XElement[] xElements)
        {
            List<string> ret = new List<string>();
            foreach(XElement xE in xElements)
            {
                ret.Add(xE.Attribute("NameShort").Value);
            }
            return ret.ToArray();
        }

    }
}
