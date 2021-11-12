using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LSchedule.Kernel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LSchedule.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page_WatchSchedule : ContentPage
    {
        private LButton[] _buttons_DaysOfWeek_Array = new LButton[14];
        int _selectedButtonId = -1;
        bool _firstLoad_ForDaysButtons = true;

        ScheduleParams[] _scheduleParams;

        public Page_WatchSchedule()
        {
            InitializeComponent();

            Fill_Days_Buttons();

            Kernel.ScheduleParams_Favorites.Updated_Notify += Favorites_Changed;

            Fill_chetn();
        }

        private void Fill_Days_Buttons()
        {
            string[] dNames;
            DateTime[] dDates;
            (dNames, dDates) = Kernel.Backend.TimeUtils.Get_DayAndDatetime_List(DateTime.Now);
            for (int i = 0; i < _buttons_DaysOfWeek_Array.Length; i++)
            {
                LButton nowLB = new LButton() { myId = i, Text = dNames[i] + Environment.NewLine + dDates[i].Day, Date = dDates[i], WidthRequest = 50, BorderColor = Color.Blue, BorderWidth = 0 };
                if (nowLB.Date == DateTime.Now.Date)
                {
                    nowLB.TextColor = Color.Red;
                    if (i == 6)
                    {
                        _selectedButtonId = i + 1;
                    }
                    else
                    {
                        _selectedButtonId = i;
                    }
                }
                if (i == 6 || i==13)
                {
                    nowLB.IsEnabled = false;
                }
                nowLB.Clicked += DayButton_Clicked;
                _buttons_DaysOfWeek_Array[i] = nowLB;
                Buttons_DaysOfWeek.Children.Add(_buttons_DaysOfWeek_Array[i]);
            }
            DayButton_Clicked(_buttons_DaysOfWeek_Array[_selectedButtonId], new EventArgs());
            _firstLoad_ForDaysButtons = false;
        }

        private void DayButton_Clicked(object sender, EventArgs e)
        {
            LButton btn = sender as LButton;
            int id = btn.myId;
            if (_selectedButtonId == id && _firstLoad_ForDaysButtons == false)
                return;

            if (_selectedButtonId != -1)
            {
                _buttons_DaysOfWeek_Array[_selectedButtonId].BorderWidth = 0;
            }

            _selectedButtonId = id;
            _buttons_DaysOfWeek_Array[_selectedButtonId].BorderWidth = 1;

            Fill_chetn();
            Display_Schedule();
        }

        private void FillPicker()
        {
            _scheduleParams = ScheduleParams_Favorites.Get_Favorites_Array;
            picker_ScheduleParams.Items.Clear();
            if (_scheduleParams.Length < 1)
                return;
            foreach (ScheduleParams scheduleParams in _scheduleParams)
            {
                picker_ScheduleParams.Items.Add($"{scheduleParams.Institution} {scheduleParams.Age} {scheduleParams.Department} {scheduleParams.Direction}");
            }
            picker_ScheduleParams.SelectedIndex = 0;
        }

        private void Favorites_Changed()
        {
            FillPicker();
        }

        private void Display_Schedule()
        {
            if (picker_ScheduleParams.SelectedIndex < 0 || _selectedButtonId < 0)
                return;
            CurrentSchedule_StackLayout.Children.Clear();
            CurrentSchedule_StackLayout.Children.Add(Kernel.ScheduleGetter.GetSchedule(_scheduleParams[picker_ScheduleParams.SelectedIndex], _buttons_DaysOfWeek_Array[_selectedButtonId].Date));
        }

        private void picker_ScheduleParams_SelectedIndexChanged(object sender, EventArgs e)
        {

            Display_Schedule();
        }

        private void Fill_chetn()
        {
            if (_selectedButtonId < 0)
                return;
            int c = Kernel.Backend.TimeUtils.IsChet(_buttons_DaysOfWeek_Array[_selectedButtonId].Date);
            if (c == 1)
                chetnNow.Text = "выбранный день: нечетная";
            if (c == 2)
                chetnNow.Text = "выбранный день: четная";
        }
    }
}