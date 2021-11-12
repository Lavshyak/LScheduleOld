using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace LSchedule.Kernel
{
    /// <summary>
    /// хранит относительные еденицы размеров
    /// </summary>
    class InterfaceUtils
    {
        /// <summary>
        /// ширина экрана/2
        /// </summary>
        public static readonly double Dhalf_W = DeviceDisplay.MainDisplayInfo.Width * 0.5;
        /// <summary>
        /// высота экрана/2
        /// </summary>
        public static readonly double Dhalf_H = DeviceDisplay.MainDisplayInfo.Height * 0.5;
    }
}
