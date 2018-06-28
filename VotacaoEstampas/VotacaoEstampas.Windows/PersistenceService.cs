using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VotacaoEstampas.Model;
using VotacaoEstampas.Windows;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Xamarin.Forms;

[assembly: Dependency(typeof(PersistenceService))]
namespace VotacaoEstampas.Windows
{
    public class PersistenceService : IPersistenceService
    {
        public Stream LoadImage(string filename)
        {
            try
            {
                var task = LoadImageAsync(filename);
                task.Wait(); // HACK: to keep Interface return types simple (sorry!)
                return task.Result;
            }
            catch { return null; }
        }

        private async Task<Stream> LoadImageAsync(string filename)
        {
            MemoryStream data = new MemoryStream();
            StorageFolder local = ApplicationData.Current.LocalFolder;
            var file = await local.GetFileAsync(filename).AsTask().ConfigureAwait(false);
            //var file = await local.GetFileAsync(filename));
            return await file.OpenStreamForReadAsync();
        }

        private async Task<Stream> LoadImageAsyncFromFolder(string foldername, string filename)
        {
            MemoryStream data = new MemoryStream();
            StorageFolder local = await ApplicationData.Current.LocalFolder.GetFolderAsync(foldername).AsTask().ConfigureAwait(false);
            var file = await local.GetFileAsync(filename);
            return await file.OpenStreamForReadAsync();
        }

        public async void SaveImageAsync(string filename, byte[] image)
        {
            StorageFolder local = ApplicationData.Current.LocalFolder;
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
            StorageFolder local = ApplicationData.Current.LocalFolder;
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
                StorageFolder local = ApplicationData.Current.LocalFolder;
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

        public static byte[] StreamToBytes(Stream input)
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
                return StreamToBytes(task.Result);
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
                filesList.Add(StreamToBytes(stream));
            }
            return filesList;
        }

        private Colecao _ultimaColecaoCarregada;
        public async Task<Colecao> LoadColecaoAsync()
        {
            var res = await LoadTextAsync("colecao.xml");

            /* DESERIALIZAR OBJETO */
            var colecao = DeserializeObject(res);
            _ultimaColecaoCarregada = colecao;
            return colecao;
        }

        private Colecao DeserializeObject(string res)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Colecao));
                using (TextReader reader = new StringReader(res))
                {
                    return ((Colecao)xmlSerializer.Deserialize(reader));
                }
            }
            catch (Exception)
            {
                return new Colecao();
            }
        }

        public async Task SaveColecaoAsync(Colecao colecao)
        {
            var local = ApplicationData.Current.LocalFolder;
            var file = await local.CreateFileAsync("colecao.xml", CreationCollisionOption.ReplaceExisting);

            // Serializa objeto 
            var colecaoStringXml = SerializeObject(colecao);

            using (StreamWriter writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                writer.Write(colecaoStringXml);
            }
        }

        public string SerializeObject(Colecao toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Retorna o diretorio atualmente utilizado para salvar imagens
        /// </summary>
        /// <returns> string contendo caminho </returns>
        public string GetImagesLocalPath()
        {
            return ApplicationData.Current.LocalFolder.Path + "/Estampas";
        }

        public async Task<IEnumerable<byte[]>> LoadImagesBytesOrderedAsync(List<string> filesnames)
        {
            List<byte[]> filesList = new List<byte[]>();
            var folder = "Estampas";
            foreach (string filename in filesnames)
            {
                var imageStream = await LoadImageAsyncFromFolder(folder, filename + ".jpg");
                filesList.Add(StreamToBytes(imageStream));
            }
            return filesList;
        }

        public async Task VerifYfanReportsExe()
        {
            try
            {
                StorageFolder local = ApplicationData.Current.LocalFolder;
                if (local != null)
                {
                    StorageFile yfanReportsExecutable = null;

                    try
                    {
                        yfanReportsExecutable = await local.GetFileAsync("YfanReports.exe").AsTask().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        //nao encontrou o arquivo
                    }

                    if (yfanReportsExecutable == null) //se arquivo nao foi copiado para diretorio local
                    {
                        var folderExtras = await Package.Current.InstalledLocation.GetFolderAsync("Extras");
                        var relatorioYfanExe = await folderExtras.GetFileAsync("YfanReports.exe");
                        var eppPlusDll = await folderExtras.GetFileAsync("EPPlus.dll");
                        var relatorioYfanCopiado = await relatorioYfanExe.CopyAsync(ApplicationData.Current.LocalFolder);
                        var eppPlusDllCopiada = await eppPlusDll.CopyAsync(ApplicationData.Current.LocalFolder);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Erro ao verificar gerador de relatórios: {0}", ex.Message));
            }
        }

        public void CopyLocalFolderToClipboard()
        {
            var dt = new DataPackage();
            dt.SetText(ApplicationData.Current.LocalFolder.Path.ToString());
            Clipboard.SetContent(dt);
        }

        public Stream GetXlsFile()
        {
            //TODO nada
            return null;
        }

        public Stream GetXlsxFile()
        {
            throw new NotImplementedException();
        }

        public void CopyReportToDownloads()
        {
            throw new NotImplementedException();
        }
    }
}
