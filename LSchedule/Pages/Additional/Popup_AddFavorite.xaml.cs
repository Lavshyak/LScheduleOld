using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Diagnostics;
using static LSchedule.Kernel.InterfaceUtils;

namespace LSchedule.Pages.Additional
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup_AddFavorite : Rg.Plugins.Popup.Pages.PopupPage
    {
        //пикеры параметров расписания
        private Picker[] _pickers = new Picker[4];
        //кнопка для добавления
        private Button _addToFavorites_Button;
        //ScheduleParams локальный
        private Kernel.ScheduleParams _scheduleParams = new Kernel.ScheduleParams();

        //конструктор
        public Popup_AddFavorite()
        {
            InitializeComponent();

            Debug.WriteLine($"H: {DeviceDisplay.MainDisplayInfo.Height}");
            Debug.WriteLine($"W: {DeviceDisplay.MainDisplayInfo.Width}");
            Debug.WriteLine($"Density: {DeviceDisplay.MainDisplayInfo.Density}");

            Fill_root_StackLayout();
            foreach (string institute in Kernel.Backend.InstitutionsXmlManager.GetInstitutionsNamesArray())
            {
                _pickers[0].Items.Add(institute);
            }
        }


        /// <summary>
        /// устанавливает размеры интерфейса. определяет _addToFavorites_Button и _pickers
        /// </summary>
        private void Fill_root_StackLayout()
        {
            _root_StackLayout.Margin = new Thickness(Dhalf_W * 0.1, Dhalf_H * 0.23, Dhalf_W * 0.1, Dhalf_H * 0.143);
            _root_StackLayout.Padding = new Thickness(Dhalf_W * 0.01, Dhalf_H * 0.01, Dhalf_W * 0.01, 0);

            string[] texts = { "Уч. заведение", "Курс", "Факультет", "Направление" };

            //добавить пикеры
            for (int i = 0; i < 4; i++)
            {
                _pickers[i] = new Picker()
                {
                    Title = texts[i].ToUpper(),
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    //WidthRequest = _dhalf_W * 0.5,
                    WidthRequest = Dhalf_W * 0.5,
                    HeightRequest = Dhalf_H * 0.06,
                    Margin = new Thickness(0, 0, 0, 0),
                    TitleColor = Color.Green,
                    HorizontalTextAlignment = TextAlignment.Start,
                    AutomationId = $"{i}"
                };
                _pickers[i].SelectedIndexChanged += _pickers_SelectedIndexChanged;
                _root_StackLayout.Children.Add(_pickers[i]);
            }

            //выключить ненужное
            for (int i = 1; i < 4; i++)
            {
                _pickers[i].IsEnabled = false;
            }

            //добавить кнопку
            _addToFavorites_Button = new Button()
            {
                Text = "Добавить",
                WidthRequest = Dhalf_W * 0.2,
                MinimumWidthRequest = Dhalf_W * 0.2,
                HeightRequest = Dhalf_H * 0.05,
                MinimumHeightRequest = Dhalf_H * 0.05,
                BackgroundColor = new Color(0.188, 0.428, 0.916),
                BorderColor = Color.Black,
                BorderWidth = 1,
                TextColor = Color.BlanchedAlmond,
                TextTransform = TextTransform.Uppercase,
                //Margin = new Thickness(0, 0, 0, 0),
                VerticalOptions = LayoutOptions.End,
                IsEnabled = false
            };
            _addToFavorites_Button.Clicked += _addToFavorites_Button_Clicked;
            _root_StackLayout.Children.Add(_addToFavorites_Button);
        }

        private void _pickers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;

            if (picker.SelectedItem == null)
                return;

            int autoId = int.Parse(picker.AutomationId);

            string[] names;

            switch (autoId)
            {
                case 0:
                    _scheduleParams.Institution = picker.SelectedItem.ToString();
                    _pickers_DoOther(autoId);
                    names = Kernel.Backend.InstitutionsXmlManager.Get_Ages_NamesShort(_scheduleParams);

                    foreach (string name in names)
                    {
                        _pickers[1].Items.Add(name);
                    }
                    break;

                case 1:
                    _scheduleParams.Age = picker.SelectedItem.ToString();
                    _pickers_DoOther(autoId);
                    names = Kernel.Backend.InstitutionsXmlManager.Get_Departments_NamesShort(_scheduleParams);

                    foreach (string name in names)
                    {
                        _pickers[2].Items.Add(name);
                    }
                    break;

                case 2:
                    _scheduleParams.Department = picker.SelectedItem.ToString();
                    _pickers_DoOther(autoId);
                    names = Kernel.Backend.InstitutionsXmlManager.Get_Directions_NamesShort(_scheduleParams);

                    if (names == null)
                    {
                        DisplayAlert("Ошибка", "Не удалось получить список направлений. Возможны проблемы с сайтом уч. заведения", "OK");
                        return;
                    }

                    foreach (string name in names)
                    {
                        _pickers[3].Items.Add(name);
                    }
                    break;

                case 3:
                    _scheduleParams.Direction = picker.SelectedItem.ToString();
                    _addToFavorites_Button.IsEnabled = true;
                    break;

            }
        }
        /// <summary>
        /// Удостовериться в дефолтности пикеров, которые ниже по списку
        /// </summary>
        /// <param name="autoId">Тот, который был изменен</param>
        private void _pickers_DoOther(int autoId)
        {
            for (int i = autoId + 1; i < 4; i++)
            {
                _pickers[i].Items.Clear();
            }
            for (int i = autoId + 2; i < 4; i++)
            {
                _pickers[i].IsEnabled = false;
            }
            if (autoId < 3)
                _pickers[autoId + 1].IsEnabled = true;

            _addToFavorites_Button.IsEnabled = false;
        }

        private void _addToFavorites_Button_Clicked(object sender, EventArgs e)
        {
            Kernel.ScheduleParams_Favorites.Add(_scheduleParams);
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}