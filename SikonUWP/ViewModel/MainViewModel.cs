using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using ModelLibrary.Model;
using SikonUWP.Annotations;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;
using SikonUWP.Persistency;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class MainViewModel: INotifyPropertyChanged
    {
        private readonly Frame _frame;
        private readonly NavigationView _navigationView;

        public static MainViewModel Instance { get; private set; }

        private User _loggedUser;

        public User LoggedUser
        {
            get { return _loggedUser; }
            set { _loggedUser = value; OnPropertyChanged(); }
        }


        public MainViewModel()
        {
            
        }
        public MainViewModel(Frame mainPageFrame, NavigationView navigationView)
        {
            _frame = mainPageFrame;
            _navigationView = navigationView;
            Instance = this;
            Load();
        }

        #region Navigation

        /// <summary>
        /// Navigates to a page
        /// </summary>
        /// <param name="pageType">The type of the page</param>
        public void NavigateToPage(Type pageType)
        {
            _frame.Navigate(pageType);
            UptNaviCursor(pageType);
            if (_frame.CanGoBack)
                _navigationView.IsBackEnabled = true;
        }

        public void UptNaviCursor(Type pageType)
        {
            foreach (object menuItem in _navigationView.MenuItems)
                if (menuItem is NavigationViewItem navigationViewItem
                    && navigationViewItem.Tag is Type naviPageType
                    && naviPageType == pageType)
                    _navigationView.SelectedItem = navigationViewItem;
        }

        #endregion

        private async void Load()
        {
            try
            {
                bool ok = await PersistencyManager.TryOpenConn();
                if (ok)
                    await ImageSingleton.Instance.ImageCatalog.SyncImages();
                else
                    await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName,
                        "ConnectionString er forkert");
            }
            catch (HttpRequestException)
            {
                await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, PersistencyManager.Message);
            }

            NavigateToPage(typeof(EventHomePage));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
