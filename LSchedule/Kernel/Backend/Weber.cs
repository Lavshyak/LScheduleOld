using System;
using System.Net;
using System.IO;

namespace LSchedule.Kernel.Backend
{
    class Weber
    {
        /// <summary>
        /// Скачать файл
        /// </summary>
        /// <param name="path"></param>
        /// <param name="url"></param>
        /// <returns>
        /// 1 - скачалось, файл есть;
        /// 0 - не скачалось, файл есть;
        /// -1 - не скачалось и файла нет;
        /// </returns>
        public static int Download(string path, string url)
        {
            int ret=1;

            if (IsUrlAvaible(url))
            {
                using (WebClient wc = new WebClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    wc.DownloadFile(url, path);
                }
            }
            else
            {
                ret = 0;
            }

            if (!File.Exists(path))
                ret = -1;

            return ret;
        }

        /// <summary>
        /// Проверяет ссылку на доступность
        /// </summary>
        /// <param name="url"></param>
        /// <returns>
        /// true - доступна; false - не доступно
        /// </returns>
        private static bool IsUrlAvaible(string url)
        {
            if (url == null)
                return false;
            Uri uri = new Uri(url);
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
