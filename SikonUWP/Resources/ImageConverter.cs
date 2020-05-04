using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SikonUWP.Handlers;

namespace SikonUWP.Resources
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string imageName && ImageHandler.Dictionary.ContainsKey(imageName))
                return ImageHandler.Dictionary[imageName];
            if (value is BitmapImage image)
                return image;
            return "/Assets/SplashScreen.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
