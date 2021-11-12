using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static LSchedule.Kernel.InterfaceUtils;

namespace LSchedule.Pages
{
   /// <summary>
   /// избранные параметры и управление ими
   /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page_Favorites : ContentPage
    {

        public Page_Favorites()
        {
            InitializeComponent();

            Kernel.ScheduleParams_Favorites.Updated_Notify += OnFavoritesUpdated;
            Kernel.ScheduleParams_Favorites.LoadAll();

            SizeInterface();
        }

        private void Show_Popup_AddFavorite(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new Additional.Popup_AddFavorite());
        }

        private void OnFavoritesUpdated()
        {
            Favorites_StackLayout.Children.Clear();
            Kernel.ScheduleParams[] favorites = Kernel.ScheduleParams_Favorites.Get_Favorites_Array;
            string nowParams;
            StackLayout nowStackLayout;
            Button nowButton;
            if(favorites == null)
            {
                return;
            }
            int i = 0;
            double tempSize = Dhalf_W * 0.1;
            foreach (Kernel.ScheduleParams favorite in favorites)
            {
                nowParams = $"{favorite.Institution} {favorite.Age} {favorite.Department} {favorite.Direction}";
                nowStackLayout = new StackLayout() { Orientation = StackOrientation.Horizontal};

                nowStackLayout.Children.Add(new Label() { Text = $"{(i+1).ToString()}: ", FontSize = 20, VerticalTextAlignment = TextAlignment.Center });
                nowStackLayout.Children.Add(new Label() { Text = nowParams, FontSize = 20, VerticalTextAlignment = TextAlignment.Center });

                nowButton = new Button() { Text = "▲", AutomationId = "up", HorizontalOptions = LayoutOptions.EndAndExpand, WidthRequest = tempSize, HeightRequest = tempSize };
                nowButton.Clicked += Favorites_OnButtonClick_InChildrens;
                nowStackLayout.Children.Add(nowButton);

                nowButton = new Button() { Text = "▼", AutomationId = "down", HorizontalOptions = LayoutOptions.End, WidthRequest = tempSize, HeightRequest = tempSize };
                nowButton.Clicked += Favorites_OnButtonClick_InChildrens;
                nowStackLayout.Children.Add(nowButton);

                nowButton = new Button() { Text = "×", AutomationId = "del", HorizontalOptions = LayoutOptions.End, WidthRequest = tempSize, HeightRequest = tempSize, Margin = new Thickness(0, 0, tempSize*0.1, 0) };
                nowButton.Clicked += Favorites_OnButtonClick_InChildrens;
                nowStackLayout.Children.Add(nowButton);

                nowStackLayout.AutomationId = i.ToString();
                Favorites_StackLayout.Children.Add(nowStackLayout);
                i++;
            }
        }

        private async void Favorites_OnButtonClick_InChildrens(object _sender, EventArgs e)
        {
            Button sender = _sender as Button;
            int index = int.Parse((sender.Parent as StackLayout).AutomationId);
            switch (sender.AutomationId)
            {
                case "up":
                    Kernel.ScheduleParams_Favorites.Swap(index, index - 1);
                    break;
                case "down":
                    Kernel.ScheduleParams_Favorites.Swap(index, index + 1);
                    break;
                case "del":
                    bool answer = await DisplayAlert("Уверены?", $"Удалить {((sender.Parent as StackLayout).Children[1] as Label).Text} ?", "Да", "Нет");
                    if (!answer)
                        return;
                    Kernel.ScheduleParams_Favorites.Delete(index);
                    break;
            }
        }

        /// <summary>
        /// устанавливает размеры и интерфейса
        /// </summary>
        private void SizeInterface()
        {
            AbsoluteLayout.SetLayoutBounds(button_Add, new Rectangle(0,Dhalf_H*0.55,Dhalf_W*0.8,Dhalf_H*0.05));
            AbsoluteLayout.SetLayoutBounds(Favorites_ScrollView, new Rectangle(0, 0, Dhalf_W * 0.8, Dhalf_H * 0.54));
        }
    }
}