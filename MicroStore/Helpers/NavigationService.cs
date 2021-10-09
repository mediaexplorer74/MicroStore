﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MicroStore.Services
{
    public class NavigationService : INavigationService
    {
        public Frame CurrentFrame { get; set; }
        public Frame AppFrame { get; set; }


        public void Navigate(Type page)
        {
            CurrentFrame.Navigate(page);
        }

        public void Navigate(Type page, object parameter)
        {
            CurrentFrame.Navigate(page, parameter);
        }

        public void Navigate(string page)
        {
            Type type = Type.GetType("MicroStore.Views." + page);
            Navigate(type);
        }

        public void Navigate(string page, object parameter)
        {
            Type type = Type.GetType("MicroStore.Views." + page);
            Navigate(type, parameter);
        }


        public void AppNavigate(Type page)
        {
            AppFrame.Navigate(page);
        }

        public void AppNavigate(Type page, object parameter)
        {
            AppFrame.Navigate(page, parameter);
        }

        public void AppNavigate(string page)
        {
            Type type = Type.GetType("MicroStore.Views." + page);
            AppNavigate(type);
        }

        public void AppNavigate(string page, object parameter)
        {
            Type type = Type.GetType("MicroStore.Views." + page);
            AppNavigate(type, parameter);
        }


        public async Task<bool> OpenInBrowser(string url)
        {
            // Wrap in a try-catch block in order to prevent the
            // app from crashing from invalid links.
            // (specifically from project badges)
            try
            {
                return await OpenInBrowser(new Uri(url));
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> OpenInBrowser(Uri uri)
        {
            return await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
