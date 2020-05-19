using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SikonUWP.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SikonUWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserLogin_CreatePage : Page
    {
        public UserLogin_CreatePage()
        {
            this.InitializeComponent();
        }

        //private async void LogIn(object sender, RoutedEventArgs e)
        //{
        //    bool ok = await UserLogin_CreateViewModel.vmInstance.LogIn();
        //    if (ok == true)
        //    {
        //        Frame.Navigate(typeof(MainPage));
        //    }
        //}
    }
}
