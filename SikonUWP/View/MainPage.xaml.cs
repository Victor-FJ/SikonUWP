using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SikonUWP.Persistency;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SikonUWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Open();
        }

        private async void Open()
        {
            bool ok = await PersistencyManager.Tester();
        }

       
        
        // Denne del af koden navigere til en side i Viewet 
        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            string tag = (string) args.InvokedItemContainer.Tag;
            Navigate(tag);
        }

        private void Navigate(string pageName)
        {
            Type pageType = Assembly.GetExecutingAssembly().GetType($"SikonUWP.View.{pageName}");

            if (pageType == null)
                return;

            ContentFrame.Navigate(pageType);
        }
    }
}
