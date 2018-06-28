using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using VotacaoEstampas.Model;
using Xamarin.Forms;
using VotacaoEstampas.Extensions;
using VotacaoEstampas.Droid;

[assembly: Dependency(typeof(PersistenceService))]
namespace VotacaoEstampas.Droid
{
    public class PersistenceService : IPersistenceService
    {
        private Colecao _ultimaColecaoCarregada;
        private const Environment.SpecialFolder _diretorioLocalPath = Environment.SpecialFolder.Personal;

        #region Images Files

        public Stream LoadImage(string filename)
        {
            try
            {
                var location = Environment.GetFolderPath(_diretorioLocalPath);
                var filePath = Path.Combine(location, filename);
                return File.OpenRead(filePath);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public async void SaveImageAsync(string filename, byte[] image)
        {
            await Task.Run(() =>
            {
                var localStorage = Environment.GetFolderPath(_diretorioLocalPath);
                var filePath = Path.Combine(localStorage, filename);
                File.WriteAllBytes(filePath, image);
            });
        }

        public async void SaveEstampaAsync(string filename, byte[] image)
        {
            await Task.Run(() =>
            {
                var localStorage = Environment.GetFolderPath(_diretorioLocalPath);
                var estampasDir = Path.Combine(localStorage, "Estampas");
                Directory.CreateDirectory(estampasDir);
                var filePath = Path.Combine(estampasDir, filename);
                File.WriteAllBytes(filePath, image);
            });
        }

        public async void ClearImagesAsync(string foldername)
        {
            await Task.Run(() =>
            {
                try
                {
                    var filesList = new List<byte[]>();
                    var location = Environment.GetFolderPath(_diretorioLocalPath);
                    var folderPath = Path.Combine(location, foldername);
                    var directory = Directory.GetFiles(folderPath);
                    foreach (var file in directory)
                    {
                        File.Delete(file);
                    }
                }
                catch (Exception)
                {
                }
            });
        }

        public IEnumerable<byte[]> LoadImagesBytes(string foldername)
        {
            try
            {
                var filesList = new List<byte[]>();
                var location = Environment.GetFolderPath(_diretorioLocalPath);
                var folderPath = Path.Combine(location, foldername);
                var directory = Directory.GetFiles(folderPath);
                foreach (var file in directory)
                {
                    filesList.Add(File.ReadAllBytes(file));
                }
                return filesList;
            }
            catch { return null; }
        }

        public async Task<IEnumerable<byte[]>> LoadImagesBytesOrderedAsync(List<string> filesnames)
        {
            List<byte[]> filesList = new List<byte[]>();
            var folder = "Estampas";
            foreach (string filename in filesnames)
            {
                var imageStream = await LoadImageAsyncFromFolder(folder, filename + ".jpg");
                filesList.Add(imageStream.StreamToBytes());
            }
            return filesList;
        }

        public string GetImagesLocalPath()
        {
            return _diretorioLocalPath + "/Estampas";
        }

        #endregion

        #region Text Files

        public async Task<Colecao> LoadColecaoAsync()
        {
            var res = await LoadTextAsync("colecao.xml");
            var colecao = res.DeserializarColecao();
            _ultimaColecaoCarregada = colecao;
            return colecao;
        }

        public async Task SaveColecaoAsync(Colecao colecao)
        {
            await Task.Run(() =>
            {
                var localStorage = Environment.GetFolderPath(_diretorioLocalPath);
                var filePath = Path.Combine(localStorage, "colecao.xml");
                var colecaoStringXml = colecao.SerializarColecao();
                File.WriteAllText(filePath, colecaoStringXml);
            });
        }

        public async void SaveConfigAsync(string config)
        {
            await Task.Run(() =>
            {
                try
                {
                    var localStorage = Environment.GetFolderPath(_diretorioLocalPath);
                    var filePath = Path.Combine(localStorage, "configs.txt");
                    File.WriteAllText(filePath, config);
                }
                catch (Exception ex)
                {
                }
            });
        }

        public string LoadConfig()
        {
            return LoadText("configs.txt");
        }

        #endregion

        #region Reports

        public Stream GetXlsxFile()
        {
            try
            {
                /* var localStorage = System.Environment.GetFolderPath(_diretorioLocalPath);
                 var filePath = Path.Combine(localStorage, "grid.xlsx");*/
                var fileDestPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "RelatorioYfan.xlsx");
                return File.OpenWrite(fileDestPath);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void CopyReportToDownloads() {
            try
            {
                var localStorage = System.Environment.GetFolderPath(_diretorioLocalPath);
                var filePath = Path.Combine(localStorage, "grid.xlsx");
                var fileDestPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).Path, "RelatorioYfan.xlsx");
                File.Copy(filePath, fileDestPath);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task VerifYfanReportsExe()
        {
            try
            {
                /* StorageFolder local = ApplicationData.Current.LocalFolder;
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
                 }*/

            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Erro ao verificar gerador de relatórios: {0}", ex.Message));
            }
        }

        public void CopyLocalFolderToClipboard()
        {
            /* var dt = new DataPackage();
             dt.SetText(ApplicationData.Current.LocalFolder.Path.ToString());
             Clipboard.SetContent(dt);*/
        }
        #endregion

        #region Private Methods

        private async Task<Stream> LoadImageAsyncFromFolder(string foldername, string filename)
        {
            return await Task.Run(() =>
            {
                var location = Environment.GetFolderPath(_diretorioLocalPath);
                var folderPath = Path.Combine(location, foldername);
                var filePath = Path.Combine(folderPath, filename);
                try
                {
                    return File.OpenRead(filePath);
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            });
        }

        private async Task<string> LoadTextAsync(string filename)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var location = Environment.GetFolderPath(_diretorioLocalPath);
                    var filePath = Path.Combine(location, filename);
                    return File.ReadAllText(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return "";
                }
            });
        }

        private string LoadText(string filename)
        {
            try
            {
                var location = Environment.GetFolderPath(_diretorioLocalPath);
                var filePath = Path.Combine(location, filename);
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion
    }
}
