using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DroverXamarin
{
    public partial class App : Application
    {
		static public int ScreenWidth;
		static public int ScreenHeight;

        public App()
        {
            InitializeComponent();
			UIInstance.app = this;
			MainPage = new DroverXamarin.InitPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
			if (Settings.AppState.Equals("DEFAULT"))
			{
 				DroverXamarin.MainPage.instance.resetGUI();
			}
			else if (Settings.AppState.Equals("SEARCHING"))
			{
				DroverXamarin.MainPage.instance.hideAll();
				DroverXamarin.MainPage.instance.showSearch(true);
			}
        }
    }
}
