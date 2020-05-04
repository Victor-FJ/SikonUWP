using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using SikonUWP.Persistency;

namespace SikonUWP.Handlers
{
    public static class ImageHandler
    {
        private static StorageFolder _imageFolder;
        public static Dictionary<string, BitmapImage> Dictionary { get; set; }

        /// <summary>
        /// Downloads and deletes images so the locally saved images are synchronized with the database
        /// </summary>
        /// <returns></returns>
        public static async Task SyncImages()
        {
            //Gets image names from database
            List<string> imageNames = await ImagePersistence.GetNames();
            //Gets images from local storage and put them in a dictionary
            if (_imageFolder == null)
                _imageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFile> files = await _imageFolder.GetFilesAsync();
            Dictionary<string, StorageFile> localImageDictionary = files.ToDictionary(x => x.Name, x => x);

            //Synchronising local storage with database
            Dictionary = new Dictionary<string, BitmapImage>();
            foreach (string imageName in imageNames)
            {
                //If an image from the database is already saved locally then we do nothing
                if (localImageDictionary.ContainsKey(imageName))
                {
                    Dictionary.Add(imageName, await AsBitmapImage(localImageDictionary[imageName]));
                    localImageDictionary.Remove(imageName);
                }
                else
                {
                    //If its not then the image is requsted and saved locally
                    StorageFile file = await _imageFolder.CreateFileAsync(imageName, CreationCollisionOption.FailIfExists);
                    byte[] pixelBytes = await ImagePersistence.Get(imageName);
                    await FileIO.WriteBytesAsync(file, pixelBytes);
                    Dictionary.Add(imageName, await AsBitmapImage(file));
                }
            }

            //Remaining local files not found in the database must have been deleted by another client,
            //and should therefore be deleted here as well
            foreach (StorageFile file in localImageDictionary.Values)
                await file.DeleteAsync();
        }

        /// <summary>
        /// Opens a filepicker so the user can find and pick an image for the program
        /// </summary>
        /// <returns>The image as an StorageFile</returns>
        public static async Task<StorageFile> PickSingleImage()
        {
            //Great filepicker for picking an image
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            // Open a stream for the selected file 
            StorageFile file = await openPicker.PickSingleFileAsync();
            return file;
        }

        /// <summary>
        /// Tries to add an image to the database as well as saving it locally
        /// </summary>
        /// <param name="file">The StorageFile containing the image</param>
        /// <returns>Boolean value that idecates whether the image was saved successfully</returns>
        public static async Task<bool> AddImage(StorageFile file)
        {
            byte[] pixelBytes = await AsByteArray(file);
            bool ok = await ImagePersistence.Post(file.Name, pixelBytes);
            if (ok)
            {
                StorageFile copiedFile = await file.CopyAsync(_imageFolder);
                Dictionary.Add(file.Name, await AsBitmapImage(copiedFile));
            }

            return ok;
        }

        /// <summary>
        /// Tries to remove an image from the database as well as locally
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <returns></returns>
        public static async Task<bool> RemoveImage(string fileName)
        {
            bool ok = await ImagePersistence.Delete(fileName);
            if (ok)
            {
                StorageFile file = await _imageFolder.GetFileAsync(fileName);
                await file.DeleteAsync();
                Dictionary.Remove(fileName);
            }

            return ok;
        }

        /// <summary>
        /// Creates a bitmapImage of the storageFile
        /// </summary>
        /// <param name="file">The storageFile that contains the image</param>
        /// <returns>The bitmapImage</returns>
        public static async Task<BitmapImage> AsBitmapImage(StorageFile file)
        {
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                    return bitmapImage;
                }
            }
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Converts the data of the storageFile into a byte array
        /// </summary>
        /// <param name="file">The StorageFile containing the image</param>
        /// <returns>The byte array with the image data</returns>
        public static async Task<byte[]> AsByteArray(StorageFile file)
        {
            using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var reader = new DataReader(fileStream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)fileStream.Size);

                byte[] pixelBytes = new byte[fileStream.Size];
                reader.ReadBytes(pixelBytes);

                return pixelBytes;
            }
        }
    }
}
