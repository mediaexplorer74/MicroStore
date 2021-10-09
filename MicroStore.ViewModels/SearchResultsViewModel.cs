﻿using MicroStore.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using MicrosoftStore;
using MicrosoftStore.Models;
using StoreLib.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace MicroStore.ViewModels
{
    public class SearchResultsViewModel : ObservableRecipient
    {
        public SearchResultsViewModel()
        {
            //PopulateProductDetailsCommand = new AsyncRelayCommand(PopulateProductDetailsAsync);
            GetResultsCommand = new AsyncRelayCommand(GetResultsAsync);
            ViewProductCommand = new RelayCommand(ViewProduct);

            ProductDetails.CollectionChanged += Products_CollectionChanged;
        }

        private async void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var culture = CultureInfo.CurrentUICulture;
            var region = new RegionInfo("ru-ru");//(culture.LCID);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems.OfType<ProductDetailsViewModel>();
                    for (int i = 0; i < newItems.Count(); i++)
                    {
                        var pvm = newItems.ElementAt(i);
                        var pd = await GetProductDetailsAsync(pvm.Product, culture, region);
                        if (pd != null)
                        {
                            ProductDetails[e.NewStartingIndex + i].Product = pd.Product;
                        }
                    }
                    break;
            }
        }

        public SearchResultsViewModel(string query)
        {
            //PopulateProductDetailsCommand = new AsyncRelayCommand(PopulateProductDetailsAsync);
            GetResultsCommand = new AsyncRelayCommand(GetResultsAsync);
            ViewProductCommand = new RelayCommand(ViewProduct);
            Query = query;
        }

        private readonly IStorefrontApi StorefrontApi = Ioc.Default.GetRequiredService<IStorefrontApi>();
        private readonly IMSStoreApi MSStoreApi = Ioc.Default.GetRequiredService<IMSStoreApi>();
        private readonly INavigationService NavService = Ioc.Default.GetRequiredService<INavigationService>();

        private string _Query;
        public string Query
        {
            get => _Query;
            set
            {
                SetProperty(ref _Query, value);
                GetResultsCommand.Execute(null);
            }
        }

        private ObservableCollection<ProductDetails> _Products = new ObservableCollection<ProductDetails>();
        public ObservableCollection<ProductDetails> Products
        {
            get => _Products;
            set => SetProperty(ref _Products, value);
        }

        private ObservableCollection<ProductDetailsViewModel> _ProductDetails = new ObservableCollection<ProductDetailsViewModel>();
        public ObservableCollection<ProductDetailsViewModel> ProductDetails
        {
            get => _ProductDetails;
            set => SetProperty(ref _ProductDetails, value);
        }

        private ProductDetailsViewModel _SelectedProductDetails;
        public ProductDetailsViewModel SelectedProductDetails
        {
            get => _SelectedProductDetails;
            set => SetProperty(ref _SelectedProductDetails, value);
        }

        private IAsyncRelayCommand _PopulateProductDetailsCommand;
        public IAsyncRelayCommand PopulateProductDetailsCommand
        {
            get => _PopulateProductDetailsCommand;
            set => SetProperty(ref _PopulateProductDetailsCommand, value);
        }

        private IAsyncRelayCommand _GetSuggestionsCommand;
        public IAsyncRelayCommand GetResultsCommand
        {
            get => _GetSuggestionsCommand;
            set => SetProperty(ref _GetSuggestionsCommand, value);
        }

        private IRelayCommand _ViewProductCommand;
        public IRelayCommand ViewProductCommand
        {
            get => _ViewProductCommand;
            set => SetProperty(ref _ViewProductCommand, value);
        }

        public async Task GetResultsAsync()
        {
            var culture = CultureInfo.CurrentUICulture;
            var region = new RegionInfo("ru-RU");//(culture.LCID);
            ProductDetails.Clear();

            int pageSize = 25;

            MicrosoftStore.Responses.ResponseItem<MicrosoftStore.Responses.ProductCardListPayload> firstPage;

            try
            {
                
                firstPage = await StorefrontApi.Search(Query, region.TwoLetterISORegionName, culture.Name, "apps", "all", "Windows.Desktop", pageSize, 0);
            }
            catch
            {
                return;
            }
                
            foreach (var product in firstPage.Payload.Cards)
            {
                ProductDetails.Add(new ProductDetailsViewModel(product));
            }

            double requestCount = System.Math.Ceiling((double)firstPage.Payload.TotalItems / pageSize);
            for (int i = 1; i < requestCount; i++)
            {
                var search = await StorefrontApi.Search(
                    Query, region.TwoLetterISORegionName, culture.Name,
                    "apps", "all", "Windows.Desktop",
                    pageSize, i * pageSize);
                foreach (var product in search.Payload.Cards)
                {
                    ProductDetails.Add(new ProductDetailsViewModel(product));
                }
            }
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsAsync(ProductDetails productDetails, CultureInfo culture, RegionInfo region)
        {
            try
            {
                var item = await StorefrontApi.GetProduct(productDetails.ProductId, "RU", "ru-RU");// "CA", "en-CA");
                var candidate = item.Convert<ProductDetails>().Payload;
                if (candidate?.PackageFamilyNames != null && candidate?.ProductId != null)
                {
                    return new ProductDetailsViewModel(candidate);
                }
                else return null;
            }
            catch (System.Exception ex)
            {
                // MP! note: Report likely JSON parsing issue so we can resolve type mapping mistakes.  Fail silentely on release build.
                cLog.Out(ex.Message);
                return null;
            }
        }

        public void ViewProduct()
        {
            GetResultsCommand.Cancel();
            NavService.Navigate("ProductDetailsView", SelectedProductDetails);
        }
    }
}
