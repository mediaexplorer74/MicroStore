using MicroStore.Helpers;
using MicrosoftStore.Models;
using StoreLib.Models;
using StoreLib.Services;
using System;
using System.Globalization;
using System.Linq;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MicroStore.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProductDetailsView : Page
    {
        public ProductDetailsView()
        {
            this.InitializeComponent();
        }

        private Visibility ShowPlatform(MicrosoftStore.Enums.PlatWindows plat)
        {
            return ViewModel.SupportsPlatform(plat) ? Visibility.Visible : Visibility.Collapsed;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is ProductDetails details)
            {
                ViewModel.Product = details;
            }
            else if (e.Parameter is ViewModels.ProductDetailsViewModel vm)
            {
                ViewModel = vm;
            }

            if (ViewModel?.Product != null)
            {
                string packageFamily = ViewModel.Product.PackageFamilyNames[0];

                if (await PackageHelper.IsAppInstalledAsync(packageFamily))
                {
                    UpdateInstallButtonToLaunch();
                }
            }
        }


        // MoreButton_Click
        private async void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = new TextBlock()
                {
                    Text = "ViewModel.Product.Title", //!
                    FontSize = 24,
                    FontWeight = FontWeights.Bold
                },
                Content = new ScrollViewer()
                {
                    Content = new TextBlock()
                    {
                        Text = "ViewModel.Product.Description", //!
                        TextWrapping = TextWrapping.Wrap
                    }
                },
                PrimaryButtonText = "Close",
                IsSecondaryButtonEnabled = false
            };
            await dialog.ShowAsync();
            return;
        }

        private void AddToCollection_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }


        // Download_Click
        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;

           
            await PackageHelper.DownloadPackage(ViewModel.Product,
                gettingPackagesCallback: product =>
                {
                    /*ProgressDialog gettingPackagesDialog = new ProgressDialog()
                    {
                        Title = ViewModel.Product.Title,
                        Body = "Fetching packages..."
                    };
                    gettingPackagesDialog.ShowAsync();*/

                    Debug.WriteLine("[i] Starting to fetch package(s) " + ViewModel.Product.Title + "...");
                },
                noPackagesCallback: async product =>
                {
                    var noPackagesDialog = new ContentDialog()
                    {
                        Title = product.Title,
                        Content = "No available packages for this product.",
                        PrimaryButtonText = "Ok"
                    };
                    await noPackagesDialog.ShowAsync();
                },
                packagesLoadedCallback: product =>
                {
                    //gettingPackagesDialog.Hide();
                    Debug.WriteLine("[i] Packages loaded...");
                },
                packageDownloadedCallback: async (product, details, file, toast) =>
                {
                    try
                    {
                        Windows.Storage.Pickers.FileSavePicker savePicker
                        = new Windows.Storage.Pickers.FileSavePicker();
                        savePicker.SuggestedStartLocation =
                            Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;//.Downloads;
                    
                        if (file.FileType.EndsWith("bundle"))
                            savePicker.FileTypeChoices.Add("Windows App Bundle", 
                                new string[] { file.FileType });
                        else
                            savePicker.FileTypeChoices.Add("Windows App Package", 
                                new string[] { file.FileType });
                    
                    
                        savePicker.SuggestedFileName = file.DisplayName;
                        savePicker.SuggestedSaveFile = file;

                    
                        var userFile = await savePicker.PickSaveFileAsync();
                        if (userFile != null)
                        {
                            await file.MoveAndReplaceAsync(userFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[ex] savePicker error: " + ex.Message);
                    }
                });


            UpdateInstallButtonToLaunch();

            InstallButton.IsEnabled = true;
            return;

            //Experimental part
            var culture = CultureInfo.CurrentUICulture;
            string productId = ViewModel.Product.ProductId;


            DisplayCatalogHandler dcathandler = new DisplayCatalogHandler(
                DCatEndpoint.Production, new Locale(culture, true));

            await dcathandler.QueryDCATAsync(productId);
            
            var packs = await dcathandler.GetMainPackagesForProductAsync();
            
            string packageFamilyName = dcathandler.ProductListing.Product.Properties.PackageFamilyName;

            //gettingPackagesDialog.Hide();
            
            if (packs != null)// && packs.Count > 0)
            {
                PackageInstance package = PackageHelper.GetLatestDesktopPackage(
                    packs.ToList(), 
                    packageFamilyName, 
                    ViewModel.Product);

                if (package == null)
                {
                   return;
                }
                else
                {
                    Windows.Storage.StorageFile file = (
                        await PackageHelper.DownloadPackage(package, ViewModel.Product)).Item1;

                    Windows.UI.Notifications.ToastNotification toast 
                        = PackageHelper.GenerateDownloadSuccessToast(package, ViewModel.Product, file);

                    Windows.UI.Notifications.ToastNotificationManager.GetDefault()
                        .CreateToastNotifier().Show(toast);
   
                }
            }

            InstallButton.IsEnabled = true;
        }//


        // InstallUsingAppInstaller
        private async void InstallUsingAppInstaller_Click(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;

            var gettingPackagesDialog = new ProgressDialog()
            {
                Title = ViewModel.Product.Title,
                Body = "Fetching packages..."
            };
            await PackageHelper.InstallPackage(ViewModel.Product, true,
                gettingPackagesCallback: product =>
                {
                    gettingPackagesDialog.ShowAsync();
                },
                noPackagesCallback: async product =>
                {
                    ContentDialog noPackagesDialog = new ContentDialog()
                    {
                        Title = ViewModel.Product.Title,
                        Content = "No available packages for this product.",
                        PrimaryButtonText = "Ok"
                    };
                    await noPackagesDialog.ShowAsync();
                },
                packagesLoadedCallback: product =>
                {
                    gettingPackagesDialog.Hide();
                },
                packageInstalledCallback: (product, package) =>
                {
                    UpdateInstallButtonToLaunch();
                });

            InstallButton.IsEnabled = true;
        }

        private void HeroImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            UpdateHeroImageSpacer();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateHeroImageSpacer();
        }

        private void UpdateHeroImageSpacer()
        {
            if (HeroImage.Source is Windows.UI.Xaml.Media.Imaging.BitmapImage bitmap 
                && bitmap.UriSource.Host == "via.placeholder.com")
            {
                HeroImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Height of the card including padding and spacing
                double cardHeight = InfoCard.ActualHeight + InfoCard.Margin.Top + InfoCard.Margin.Bottom;
                    //+ ((StackPanel)ContentScroller.Content).Spacing * 2;

                // Required amount of additional spacing to place the card at the bottom of the hero image,
                // or at the bottom of the page (whichever places the card higher up)
                double offset = Math.Min(HeroImage.ActualHeight - cardHeight, ActualHeight - cardHeight);
                HeroImageSpacer.Height = Math.Max(offset, 0);
            }
        }

        private async void UpdateInstallButtonToLaunch()
        {
            string packageFamily = ViewModel.Product.PackageFamilyNames[0];
            InstallButtonText.Text = "Launch";

            //InstallUsingAppInstallerMenuItem.IsEnabled = false;

            //InstallButton.Click -= InstallButton_Click;

            
            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //{
            //Windows.ApplicationModel.Core.AppListEntry app 
            //  = await PackageHelper.GetAppByPackageFamilyNameAsync(packageFamily);
            // await app.LaunchAsync();
            //};

            //InstallButton.Click -= InstallButton_Click;
            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //{ 
            //    await AppLaunch_Click; 
           // }

        }

        //
        private async void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;

            if(InstallButtonText.Text == "Launch")
            {
                AppLaunch();
            }
            else
            {
                // Show Progress Dialog 
                //ProgressDialog gettingPackagesDialog = new ProgressDialog()
                //{
                //    Title = ViewModel.Product.Title,
                //    Body = "Fetching packages..."
                //};

              // install package
              await PackageHelper.InstallPackage(ViewModel.Product, false,
                gettingPackagesCallback: product =>
                {
                    //gettingPackagesDialog.ShowAsync();
                    Debug.WriteLine("[i] Start Getting Package(s):" + ViewModel.Product);
                },
                noPackagesCallback: async product =>
                {
                    // Show Content Dialog 
                    ContentDialog noPackagesDialog = new ContentDialog()
                    {
                        Title = ViewModel.Product.Title,
                        Content = "No available packages for this product.",
                        PrimaryButtonText = "Ok"
                    };
                    await noPackagesDialog.ShowAsync();
                },
                packagesLoadedCallback: product =>
                {
                    //gettingPackagesDialog.Hide();
                    Debug.WriteLine("[i] Loading Package(s):" + ViewModel.Product);
                },
                packageInstalledCallback: (product, package) =>
                {
                    Debug.WriteLine("[i] Package " + ViewModel.Product + 
                        " installed. Changing Install label to Launch");

                    // Experimental 
                    InstallButton.IsEnabled = true;

                    UpdateInstallButtonToLaunch();                       
                });

            }

            InstallButton.IsEnabled = true;

        }//InstallButton_Click end

        private async void UpdateInstallButtonToInstall()
        {
            string packageFamily = ViewModel.Product.PackageFamilyNames[0];
            InstallButtonText.Text = "Install";
            
            //InstallUsingAppInstallerMenuItem.IsEnabled = false;

            //InstallButton.Click -= InstallButton_Click;

            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //{
            //    Windows.ApplicationModel.Core.AppListEntry app 
            //      = await PackageHelper.GetAppByPackageFamilyNameAsync(packageFamily);
            //     await app.LaunchAsync();
            //};
        }

        private async void AppLaunch()
        {
            string packageFamily = ViewModel.Product.PackageFamilyNames[0];
            //InstallButtonText.Text = "Launch";
            //InstallUsingAppInstallerMenuItem.IsEnabled = false;

            //InstallButton.Click -= InstallButton_Click;

            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //InstallButton.Click += async (Button sender, RoutedEventArgs e) =>
            //{
            Windows.ApplicationModel.Core.AppListEntry app
              = await PackageHelper.GetAppByPackageFamilyNameAsync(packageFamily);
            await app.LaunchAsync();
            //};
        }



    }
}
