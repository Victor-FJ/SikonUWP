using System;
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

        private void Menu_OnClick(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void IconListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxItemShare.IsSelected)
            {
                ResultTextBlock.Text = "Share";
            }
            else if (ListBoxItemFavorits.IsSelected)
            {
                ResultTextBlock.Text = "Favorits";
            }
        }
    }
}
