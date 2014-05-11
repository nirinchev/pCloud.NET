using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace pCloud.Views
{
    public sealed class Chrome : UserControl
    {
        public Chrome()
        {
            this.Loaded += Chrome_Loaded;
        }

        void Chrome_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.Chrome_Loaded;
            var statusBar = StatusBar.GetForCurrentView();

            var solidBackground = this.Background as SolidColorBrush;
            if (solidBackground != null)
            {
                statusBar.BackgroundColor = solidBackground.Color;
                statusBar.BackgroundOpacity = solidBackground.Opacity;
            }

            var solidForeground = this.Foreground as SolidColorBrush;
            if (solidForeground != null)
            {
                statusBar.ForegroundColor = solidForeground.Color;
            }
        }

        public bool ShowStatusBarProgress
        {
            get { return (bool)GetValue(ShowStatusBarProgressProperty); }
            set { SetValue(ShowStatusBarProgressProperty, value); }
        }

        public static readonly DependencyProperty ShowStatusBarProgressProperty =
            DependencyProperty.Register("ShowStatusBarProgress", typeof(bool), typeof(Chrome), new PropertyMetadata(new PropertyChangedCallback(OnShowStatusBarProgressChanged)));

        private static void OnShowStatusBarProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
            else
            {
                StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
            }
        }
    }
}
