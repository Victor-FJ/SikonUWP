using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SikonUWP.Model
{
    public class ImageSingleton
    {
        /// <summary>
        /// Instance of singleton class
        /// </summary>
        public static readonly ImageSingleton Instance = new ImageSingleton();

        public ImageCatalog ImageCatalog { get; set; }

        private ImageSingleton()
        {
            ImageCatalog = new ImageCatalog();
        }
    }
}
