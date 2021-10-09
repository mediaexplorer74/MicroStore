using MicroStore.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MicroStore.Helpers
{
    public static class NavigationHelper
    {
        public static Frame PageFrame { get; set; }

        public static void NavigateToHome()
        {
            Navigate(typeof(HomeView));
        }

        public static void NavigateToMyApps()
        {
            Navigate(typeof(MyAppsView));
        }

        public static void NavigateToSettings()
        {
            Navigate(typeof(SettingsView));
        }
        public static void NavigateToSettings(SettingsPages page)
        {
            Navigate(typeof(SettingsView), page);
        }

        public async static Task<bool> OpenInBrowser(Uri uri)
        {
            return await Launcher.LaunchUriAsync(uri);
        }
        public async static Task<bool> OpenInBrowser(string url)
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

        public static async Task<bool> OpenDiscordInvite(string inviteCode)
        {
            var quarrelLaunchUri = new Uri("quarrel://invite/" + inviteCode);
            var launchUri = new Uri("https://discord.gg/" + inviteCode);
            switch (await Launcher.QueryUriSupportAsync(quarrelLaunchUri, LaunchQuerySupportType.Uri))
            {
                case LaunchQuerySupportStatus.Available:
                    return await Launcher.LaunchUriAsync(quarrelLaunchUri);

                default:
                    return await OpenInBrowser(launchUri);
            }
        }

        public static void Navigate(Type destinationPage)
        {
            PageFrame.Navigate(destinationPage);
        }
        public static void Navigate(Type destinationPage, object parameter)
        {
            PageFrame.Navigate(destinationPage, parameter);
        }

        public static void RemovePreviousFromBackStack()
        {
            PageFrame.BackStack.RemoveAt(PageFrame.BackStack.Count - 1);
        }

        public static Tuple<Type, object> ParseProtocol(Uri ptcl)
        {
            Type destination = typeof(HomeView);

            if (ptcl == null)
                return new Tuple<Type, object>(destination, null);

            string path;
            switch (ptcl.Scheme)
            {
                case "http":
                    path = ptcl.ToString().Remove(0, 23);
                    break;

                case "https":
                    path = ptcl.ToString().Remove(0, 24);
                    break;

                case "MicroStore":
                    path = ptcl.ToString().Remove(0, ptcl.Scheme.Length + 3);
                    break;

                default:
                    // Unrecognized protocol
                    return new Tuple<Type, object>(destination, null);
            }
            if (path.StartsWith("/"))
                path = path.Remove(0, 1);

            // System.Web.HttpUtility.ParseQueryString
            var queryParams = "params";//ParseQueryString1(ptcl.Query.Replace("\r", String.Empty).Replace("\n", String.Empty));

            PageInfo pageInfo = Pages.Find(p => p.Path == path.Split('/', (char)StringSplitOptions.RemoveEmptyEntries)[0]);
            destination = pageInfo != null ? pageInfo.PageType : typeof(HomeView);

            return new Tuple<Type, object>(destination, queryParams);
        }

        /*
        public static IImageResizeSettings ParseQueryString1(string queryString)
        {
            var qs = HttpUtility.ParseQueryString(queryString);
            var settings = new ImageResizeSettings();

            settings.Width = IntParser.ParseOrDefault(qs[QS_WIDTH]);
            settings.Height = IntParser.ParseOrDefault(qs[QS_HEIGHT]);
            settings.Anchor = EnumParser.ParseOrDefault<ImageAnchorLocation>(qs[QS_ANCHOR], settings.Anchor);
            settings.Mode = EnumParser.ParseOrDefault<ImageFitMode>(qs[QS_MODE], settings.Mode);
            settings.Scale = EnumParser.ParseOrDefault<ImageScaleMode>(qs[QS_SCALE], settings.Scale);
            settings.BackgroundColor = StringHelper.EmptyAsNull(qs[QS_BACKGROUND_COLOR]);

            return settings;
        }
        */

        public static Tuple<Type, object> ParseProtocol(string url)
        {
            return ParseProtocol(String.IsNullOrWhiteSpace(url) ? null : new Uri(url));
        }

        public static List<PageInfo> Pages = new List<PageInfo>
        {
            new PageInfo()
            {
                PageType = typeof(HomeView),
                Icon = new SymbolIcon(Symbol.Home),
                Title = "Home",
                Path = "home"
            },

            new PageInfo()
            {
                PageType = typeof(MyAppsView),
                Icon = new SymbolIcon(Symbol.AllApps),
                Title = "My Apps",
                Path = "myapps"
            },
        };
    }

    public class PageInfo
    {
        public PageInfo() { }

        public PageInfo(string title, string subhead, IconElement icon)
        {
            Title = title;
            Subhead = subhead;
            Icon = icon;
        }

        //NavigationViewItem
        
        public PageInfo(PivotItem navItem)
        {
            Title = (navItem.Content == null) ? "" : navItem.Content.ToString();
            //Icon = (navItem.Icon == null) ? new SymbolIcon(Symbol.Document) : navItem.Icon;
            Visibility = navItem.Visibility;

            var tooltip = ToolTipService.GetToolTip(navItem);
            Tooltip = (tooltip == null) ? "" : tooltip.ToString();
        }
        

        public string Title { get; set; }
        public string Subhead { get; set; }
        public IconElement Icon { get; set; }
        public Type PageType { get; set; }
        public string Path { get; set; }
        public string Tooltip { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Visible;

        // Derived properties
        public ListViewItem NavViewItem
        {
            get
            {
                var item = new ListViewItem()
                {
                    //Icon = Icon,
                    Content = Title,
                    Visibility = Visibility
                };
                ToolTipService.SetToolTip(item, new ToolTip() { Content = Tooltip });

                return item;
            }
        }
        public string Protocol
        {
            get
            {
                return "MicroStore://" + Path;
            }
        }
        public Uri IconAsset
        {
            get
            {
                return new Uri("ms-appx:///Assets/Icons/" + Path + ".png");
            }
        }
    }

    public enum SettingsPages
    {
        General,
        AppMessages,
        About,
        Debug
    }
}
