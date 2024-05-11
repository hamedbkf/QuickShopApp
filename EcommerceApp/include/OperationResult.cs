using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Mime;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace EcommerceApp {
    public static class OperationResult {
        
        // operation result turns red and changes icon to red x
        public static void error_result(TextBlock resultatop, Image lefttick, Image righttick) {
            resultatop.Foreground = Brushes.Red;
            lefttick.Visibility = Visibility.Visible;
            righttick.Visibility = Visibility.Visible;
            lefttick.Source = new BitmapImage(new Uri("../images/ntick.png", UriKind.Relative));
            righttick.Source = new BitmapImage(new Uri("../images/ntick.png", UriKind.Relative));
        }
        
        // operation result turns red and changes icon to red x
        public static void success_result(TextBlock resultatop, Image lefttick, Image righttick) {
            resultatop.Foreground = Brushes.Green;
            lefttick.Visibility = Visibility.Visible;
            righttick.Visibility = Visibility.Visible;
            lefttick.Source = new BitmapImage(new Uri("../images/tick.png", UriKind.Relative));
            righttick.Source = new BitmapImage(new Uri("../images/tick.png", UriKind.Relative));
        }
        
    }

}