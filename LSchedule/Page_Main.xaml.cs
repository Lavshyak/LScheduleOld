using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LSchedule
{
    public partial class Page_Main : TabbedPage
    {
        public Page_Main()
        {
            InitializeComponent();

            Pages.Page_WatchSchedule page_WatchSchedule = new Pages.Page_WatchSchedule();
            Pages.Page_Favorites page_Favorites = new Pages.Page_Favorites();

            mainTabbedPage.Children.Add(page_WatchSchedule);
            
            mainTabbedPage.Children.Add(page_Favorites);

        }
    }
}
