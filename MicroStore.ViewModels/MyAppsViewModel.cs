﻿using MicroStore.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using MicrosoftStore;
using MicrosoftStore.Models;
using StoreLib.Models;
using StoreLib.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;

namespace MicroStore.ViewModels
{
    public class MyAppsViewModel : ObservableRecipient
    {
        public MyAppsViewModel()
        {
            ViewAppCommand = new AsyncRelayCommand(ViewAppAsync);
        }

        private readonly INavigationService NavService = Ioc.Default.GetRequiredService<INavigationService>();
        private readonly IStorefrontApi StorefrontApi = Ioc.Default.GetRequiredService<IStorefrontApi>();

        private ObservableCollection<AppViewModelBase> _Apps;
        public ObservableCollection<AppViewModelBase> Apps
        {
            get => _Apps;
            set => SetProperty(ref _Apps, value);
        }

        private AppViewModelBase _SelectedApp;
        public AppViewModelBase SelectedApp
        {
            get => _SelectedApp;
            set => SetProperty(ref _SelectedApp, value);
        }

        private IAsyncRelayCommand _ViewAppCommand;
        public IAsyncRelayCommand ViewAppCommand
        {
            get => _ViewAppCommand;
            set => SetProperty(ref _ViewAppCommand, value);
        }

        private bool _IsLoadingMyApps;
        public bool IsLoadingMyApps
        {
            get => _IsLoadingMyApps;
            set => SetProperty(ref _IsLoadingMyApps, value);
        }

        public async Task ViewAppAsync()
        {
            // TODO: This really shouldn't have to make two separate API calls.
            // Is there a better way to get the product ID using the package family name?

            var culture = CultureInfo.CurrentUICulture;
            var region = new RegionInfo("ru-RU");//(culture.LCID);

            // Get the full product details
            var dcat = DisplayCatalogHandler.ProductionConfig();
            await dcat.QueryDCATAsync(SelectedApp.PackageFamilyName, IdentiferType.PackageFamilyName);
            if (dcat.ProductListing != null && dcat.ProductListing.Products.Count > 0)
            {
                var dcatProd = dcat.ProductListing.Products[0];
                var item = await StorefrontApi.GetProduct(dcatProd.ProductId, region.TwoLetterISORegionName, culture.Name);
                var product = item.Convert<ProductDetails>().Payload;
                if (product?.PackageFamilyNames != null && product?.ProductId != null)
                {
                    NavService.Navigate("ProductDetailsView", product);
                }
            }
        }
    }

    public class AppViewModelBase : ObservableObject
    {
        public AppViewModelBase()
        {
            LaunchCommand = new AsyncRelayCommand(LaunchAsync);
        }

        private string _DisplayName;
        public string DisplayName
        {
            get => _DisplayName;
            set => SetProperty(ref _DisplayName, value);
        }

        private string _PackageFamilyName;
        public string PackageFamilyName
        {
            get => _PackageFamilyName;
            set => SetProperty(ref _PackageFamilyName, value);
        }

        private string _Description;
        public string Description
        {
            get => _Description;
            set => SetProperty(ref _Description, value);
        }

        private System.IO.Stream _Icon;
        public System.IO.Stream Icon
        {
            get => _Icon;
            set => SetProperty(ref _Icon, value);
        }

        private IAsyncRelayCommand _LaunchCommand;
        public IAsyncRelayCommand LaunchCommand
        {
            get => _LaunchCommand;
            set => SetProperty(ref _LaunchCommand, value);
        }

        public async Task<bool> LaunchAsync() => false;
    }
}
