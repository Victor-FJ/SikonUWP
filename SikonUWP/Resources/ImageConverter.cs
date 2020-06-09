using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using SikonUWP.Handlers;
using SikonUWP.Model;

namespace SikonUWP.Resources
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string imageName && ImageSingleton.Instance.ImageCatalog.Dictionary.ContainsKey(imageName))
                return ImageSingleton.Instance.ImageCatalog.Dictionary[imageName];
            if (value is BitmapImage || value is string)
                return value;
            return "../Assets/BlankImage.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
