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
using ModelLibrary.Annotations;
using ModelLibrary.Model;
using SikonUWP.Common;
using SikonUWP.Handlers;
using SikonUWP.Model;
using SikonUWP.Persistency;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Frame _frame;
        private readonly NavigationView _navigationView;

        public static MainViewModel Instance { get; private set; }

        

        public bool LoadRing { get; 
            set; }
        private string _loadText;
        public string LoadText
        {
            get => _loadText;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _loadText = "Loaded successfuldt";
                    LoadRing = false;
                }
                else
                {
                    _loadText = value;
                    LoadRing = true;
                }
                OnPropertyChanged(nameof(LoadRing));
                OnPropertyChanged();
            }
        }

        public ICommand ReloadCommand { get; set; }

        public MainViewModel(Frame mainPageFrame, NavigationView navigationView)
        {
            _frame = mainPageFrame;
            _navigationView = navigationView;
            Instance = this;
            Load();

            ReloadCommand = new RelayCommand(async () => { bool ok = await Reload(); if (ok) NavigateToPage(mainPageFrame.CurrentSourcePageType);});
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
            await Reload();
            NavigateToPage(typeof(EventHomePage));
        }

        public async Task<bool> Reload()
        {
            try
            {
                LoadText = "1/6 - Opretter forbindelse";
                bool ok = await PersistencyManager.TryOpenConn();
                if (ok)
                {
                    LoadText = "2/6 - Henter billeder";
                    await ImageSingleton.Instance.ImageCatalog.SyncImages();
                    LoadText = "3/6 - Henter lokaler";
                    await RoomCatalogSingleton.Instance.LoadRooms();
                    LoadText = "4/6 - Henter oplægsholdere";
                    await SpeakerCatalogSingleton.Instance.LoadSpeakers();
                    LoadText = "5/6 - Henter deltagere";
                    LoadText = "6/6 - Henter begivenheder";
                    await EventSingleton.Instance.EventCatalog.Load();
                    LoadText = null;
                }
                else
                {
                    LoadText = "Fejl";
                    await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, "ConnectionString er forkert");
                }
                return ok;
            }
            catch (HttpRequestException)
            {
                LoadText = "Fejl";
                await MessageDialogUtil.MessageDialogAsync(PersistencyManager.FileName, PersistencyManager.Message);
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
