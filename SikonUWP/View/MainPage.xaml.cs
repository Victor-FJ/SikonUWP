using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SikonUWP.Persistency;
using SikonUWP.ViewModel;

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
            //Pass the to controls to the viewmodel
            MainViewModel viewModel = new MainViewModel(ContentFrame, NavigationView);
            this.DataContext = viewModel;
            //Changes the language to be danish
            ApplicationLanguages.PrimaryLanguageOverride = "da-DK";
            SetNavigationViewTags();
        }

        //Navigere til en side i viewet når brugeren vælger et item i navigationviewet
        private void NavigationView_OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer.Tag is Type pageType)
                ContentFrame.Navigate(pageType);
            if (ContentFrame.CanGoBack)
                NavigationView.IsBackEnabled = true;
        }

        //Navigere tilbage en side i viewet hvis det er mugligt når bruger vælger tilbage knappen i navigationviewet
        private void NavigationView_OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
                ContentFrame.GoBack();
            ((MainViewModel)this.DataContext).UptNaviCursor(ContentFrame.CurrentSourcePageType);
            if (!ContentFrame.CanGoBack)
                NavigationView.IsBackEnabled = false;
        }

        //Justere farven på title baren
        private void AdjustTitlebarColor()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            Color backGroundColor = Color.FromArgb(255, 173, 216, 230);
            titleBar.BackgroundColor = backGroundColor;
            titleBar.ButtonBackgroundColor = backGroundColor;
        }

        //Omdanner teksten i navViewItem.Tag til en Type af den side som teksten beskrev
        private void SetNavigationViewTags()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach (object menuItem in NavigationView.MenuItems)
                if (menuItem is NavigationViewItem navigationViewItem
                    && navigationViewItem.Tag is string naviPageType)
                    navigationViewItem.Tag = assembly.GetType($"SikonUWP.View.{naviPageType}");
        }
    }
}