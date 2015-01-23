using System;
using Caliburn.Micro;
using System.Windows;
using MahApps.Metro.Controls;

namespace GetLyrics
{
    public sealed class MetroWindowManager : WindowManager
    {
        static readonly ResourceDictionary[] Resources;
        static MetroWindowManager()
        {
            Resources = new[] 
            {
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/Blue.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute) },
                new ResourceDictionary
                { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.AnimatedTabControl.xaml", UriKind.RelativeOrAbsolute) }
            };
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = view as MetroWindow;
            if (window == null)
            {
                window = new MetroWindow
                {
                    Content = view,
                    Title = "Cool Appz!",
                    AllowDrop = true,
                    Height = 650,
                    Width = 800,
                    ShowIconOnTitleBar = true,
                };
                foreach (var resourceDictionary in Resources)
                {
                    window.Resources.MergedDictionaries.Add(resourceDictionary);
                }
                window.SetValue(View.IsGeneratedProperty, true);
                var owner = InferOwnerOf(window);
                if (owner != null)
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    window.Owner = owner;
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
            {
                var owner2 = InferOwnerOf(window);
                if (owner2 != null && isDialog)
                {
                    window.Owner = owner2;
                }
            }

            return window;
        }
    }
}
