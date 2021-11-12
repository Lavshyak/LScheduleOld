using System;
using System.Collections.Generic;
using System.Text;

namespace LSchedule.Kernel
{
    /// <summary>
    /// Institution - учебное заведение,
    /// Age - курс,
    /// Department - факультет,
    /// Direction - направление
    /// </summary>
    class ScheduleParams : IEquatable<ScheduleParams>
    {
        private string _institution, _age,_department, _direction;

        public string Institution
        {
            get { return _institution; }
            set 
            {
                _institution = value; 
            }
        }

        public string Age
        {
            get { return _age; }
            set 
            { 
                _age = value; 
            }
        }

        public string Department
        {
            get { return _department;  }
            set 
            { 
                _department = value; 
            }
        }

        public string Direction
        {
            get{ return _direction; }
            set
            {
                _direction = value;
            }
        }

        public bool Equals(ScheduleParams other) 
        {
            if (this.Institution != other.Institution)
                return false;
            else if (this.Age != other.Age)
                return false;
            else if (this.Department != other.Department)
                return false;
            else if (this.Direction != other.Direction)
                return false;
            else
                return true;
        }

    }
}
