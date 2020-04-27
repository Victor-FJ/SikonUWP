﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using SikonUWP.Common;
using SikonUWP.View;

namespace SikonUWP.ViewModel
{
    public class MainViewModel
    {
        private readonly Frame _frame;
        private readonly NavigationView _navigationView;

        public static MainViewModel Instance { get; private set; }


        public MainViewModel(Frame mainPageFrame, NavigationView navigationView)
        {
            _frame = mainPageFrame;
            _navigationView = navigationView;
            Instance = this;
        }

        #region Navigation

        /// <summary>
        /// Navigates to a page
        /// </summary>
        /// <param name="pageType">The type of the page</param>
        public void NavigateToPage(Type pageType)
        {
            foreach (object menuItem in _navigationView.MenuItems)
                if (menuItem is NavigationViewItem navigationViewItem
                    && navigationViewItem.Tag is Type naviPageType
                    && naviPageType == pageType)
                    _navigationView.SelectedItem = navigationViewItem;

            _frame.Navigate(pageType);
        }

        #endregion
    }
}
