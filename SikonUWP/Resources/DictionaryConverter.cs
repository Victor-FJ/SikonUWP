using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using ModelLibrary.Model;

namespace SikonUWP.Resources
{
    public class DictionaryConverter : IValueConverter
    {
        private readonly Dictionary<object, string> _colorDictionary = new Dictionary<object, string>()
        {
            {Event.EventType.Plenum, "#FF4D637A" },
            {Event.EventType.Tema, "#FFBBCBDA" },
            {Event.EventType.Workshop, "#FFF9C164" },
            {Event.EventType.WakeUp, "#FFDDD1C1" },
            {Event.EventType.Marked, "#FFF68C1F" },
            {Event.EventType.Konkurrence, "#FF7B8A61"},
            {Event.EventType.Forplejning, "#FFCACACA"}
        };

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (_colorDictionary.ContainsKey(value))
                return _colorDictionary[value];
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
