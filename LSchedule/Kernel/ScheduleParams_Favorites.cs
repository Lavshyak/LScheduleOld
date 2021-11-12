using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace LSchedule.Kernel
{
    static class ScheduleParams_Favorites
    {
        private static List<ScheduleParams> _favorites_List = new List<ScheduleParams>();

        public static ScheduleParams[] Get_Favorites_Array => _favorites_List.ToArray();

        public static bool Add(ScheduleParams scheduleParams)
        {
            if(!_favorites_List.Contains(scheduleParams))
            {
                _favorites_List.Add(scheduleParams);
                Updated_Notify?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool Delete(ScheduleParams scheduleParams)
        {
            if(_favorites_List.Contains(scheduleParams))
            {
                _favorites_List.Remove(scheduleParams);
                Updated_Notify?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool Delete(int index)
        {
            if (_favorites_List.Count>index && index>=0)
            {
                _favorites_List.RemoveAt(index);
                Updated_Notify?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Поменять местами
        /// </summary>
        /// <param name="a">На b</param>
        /// <param name="b">На a</param>
        /// <returns></returns>
        public static bool Swap(int a, int b)
        {
            if (a < 0 && b < 0) return false;
            if (a >= _favorites_List.Count && b >= _favorites_List.Count) return false;
            if (a == b) return false;

            ScheduleParams temp;
            temp = _favorites_List[a];
            if (a == _favorites_List.Count - 1 && b == _favorites_List.Count)
                b = 0;
            else if (a == 0 && b == -1)
                b = _favorites_List.Count - 1;
            _favorites_List[a] = _favorites_List[b];
            _favorites_List[b] = temp;
            Updated_Notify?.Invoke();
            return true;
        }

        private static void SaveAll()
        {
            XElement favorites = new XElement("favorites");

            foreach(ScheduleParams scheduleParams in _favorites_List)
            {
                XElement favorite = new XElement("favorite");

                favorite.Add(new XAttribute("Institution", scheduleParams.Institution));
                favorite.Add(new XAttribute("Age", scheduleParams.Age));
                favorite.Add(new XAttribute("Department", scheduleParams.Department));
                favorite.Add(new XAttribute("Direction", scheduleParams.Direction));

                favorites.Add(favorite);
            }

            XDocument xDoc = new XDocument();
            xDoc.Add(favorites);

            xDoc.Save(Backend.FileManager.GetPath_ScheduleParamsFavoriteXml());
        }
        public static void LoadAll()
        {
            _favorites_List.Clear();

            string path = Backend.FileManager.GetPath_ScheduleParamsFavoriteXml();
            if (!File.Exists(path))
                return;

            XDocument xDoc = XDocument.Load(path);
            XElement favorites = xDoc.Root;
            foreach(XElement favorite in favorites.Elements())
            {
                ScheduleParams scheduleParams = new ScheduleParams();

                scheduleParams.Institution = favorite.Attribute("Institution").Value;
                scheduleParams.Age = favorite.Attribute("Age").Value;
                scheduleParams.Department = favorite.Attribute("Department").Value;
                scheduleParams.Direction = favorite.Attribute("Direction").Value;

                _favorites_List.Add(scheduleParams);
            }

            Updated_Notify?.Invoke();
        }

        public delegate void Updated_Handler();
        public static event Updated_Handler Updated_Notify = SaveAll;
    }
}
