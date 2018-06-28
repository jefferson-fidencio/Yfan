using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotacaoEstampas.Model;
using VotacaoEstampas.UWP;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace VotacaoEstampas.UWP
{
    public class SaveAndLoad : IPersistenceService
    {
        public Stream LoadImage(string filename)
        {
            try
            {
                var task = LoadImageAsync(filename);
                task.Wait(); // HACK: to keep Interface return types simple (sorry!)
                return task.Result;
            }
            catch { return null;}
        }

        private async Task<Stream> LoadImageAsync(string filename)
        {
            MemoryStream data = new MemoryStream();
            StorageFolder local = ApplicationData.Current.LocalFolder;
            var file = await local.GetFileAsync(filename).AsTask().ConfigureAwait(false);
            //var file = await local.GetFileAsync(filename));
            return await file.OpenStreamForReadAsync();
        }

        public async void SaveImageAsync(string filename, byte[] image)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(image, 0, image.Length);
                stream.Flush();
            }
        }

        public async void SaveEstampaAsync(string filename, byte[] image)
        {
            StorageFolder local = ApplicationData.Current.LocalFolder;
            var newlocal = await local.CreateFolderAsync("Estampas", CreationCollisionOption.OpenIfExists);
            var file = await newlocal.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                stream.Write(image, 0, image.Length);
                stream.Flush();
            }
        }

        public async void SaveConfigAsync(string config)
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("configs.txt", CreationCollisionOption.ReplaceExisting);
            using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Write(config);
            }
        }

        public string LoadConfig()
        {
            var task = LoadTextAsync("configs.txt");
            task.Wait(); // HACK: to keep Interface return types simple (sorry!)
            return task.Result;
        }

        private async Task<string> LoadTextAsync(string filename)
        {
            try
            {
                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                if (local != null)
                {
                    var file = await local.GetFileAsync(filename).AsTask().ConfigureAwait(false);
                    using (StreamReader streamReader = new StreamReader(await file.OpenStreamForReadAsync()))
                    {
                        var text = streamReader.ReadToEnd();
                        return text;
                    }
                }
                return "";
            }
            catch (Exception)
            {
                return "";
            }
        }

        public IEnumerable<Stream> LoadImages(string foldername)
        {
            try
            {
                var task = LoadImagesAsync(foldername);
                task.Wait(); // HACK: to keep Interface return types simple (sorry!)
                return task.Result;
            }
            catch { return null; }
        }

        public async void ClearImagesAsync(string foldername)
        {
            try
            {
                StorageFolder local = await ApplicationData.Current.LocalFolder.GetFolderAsync(foldername).AsTask().ConfigureAwait(false);
                await local.DeleteAsync().AsTask().ConfigureAwait(false);
            }
            catch { }
        }


        private async Task<IEnumerable<Stream>> LoadImagesAsync(string foldername)
        {
            var filesList = new List<Stream>();
            StorageFolder local = await ApplicationData.Current.LocalFolder.GetFolderAsync(foldername).AsTask().ConfigureAwait(false);
            var files = await local.GetFilesAsync().AsTask().ConfigureAwait(false);
            foreach (var file in files)
            {
                var stream = await file.OpenStreamForReadAsync();
                filesList.Add(stream);
            }

            return filesList;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public byte[] LoadImageBytes(string filename)
        {
            try
            {
                var task = LoadImageAsync(filename);
                task.Wait(); // HACK: to keep Interface return types simple (sorry!)
                return ReadFully(task.Result);
            }
            catch { return null; }
        }

        public IEnumerable<byte[]> LoadImagesBytes(string foldername)
        {
            try
            {
                var task = LoadImagesAsyncBytes(foldername);
                task.Wait(); // HACK: to keep Interface return types simple (sorry!)
                return task.Result;
            }
            catch { return null; }
        }

        private async Task<IEnumerable<byte[]>> LoadImagesAsyncBytes(string foldername)
        {
            var filesList = new List<byte[]>();
            StorageFolder local = await ApplicationData.Current.LocalFolder.GetFolderAsync(foldername).AsTask().ConfigureAwait(false);
            var files = await local.GetFilesAsync().AsTask().ConfigureAwait(false);
            foreach (var file in files)
            {
                var stream = await file.OpenStreamForReadAsync();
                filesList.Add(ReadFully(stream));
            }

            return filesList;
        }

        public Task<Colecao> LoadColecaoAsync()
        {
            throw new NotImplementedException();
        }

        public void SaveColecaoAsync(Colecao colecao)
        {
            throw new NotImplementedException();
        }
    }
}
