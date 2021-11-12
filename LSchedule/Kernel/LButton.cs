using System;
using System.Collections.Generic;
using System.Text;

namespace LSchedule.Kernel
{
    class LButton : Xamarin.Forms.Button
    {
        private bool _isToday = false;
        public bool IsToday
        {
            get { return _isToday; }
        }
        private DateTime _dt;
        public DateTime Date
        {
            get { return _dt; }
            set
            {
                _dt = value;
                if (value.Date == DateTime.Now.Date)
                    _isToday = true;
            }
        }

        public int myId;

    }
}
