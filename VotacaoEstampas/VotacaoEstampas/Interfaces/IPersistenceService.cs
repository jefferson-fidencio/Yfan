using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotacaoEstampas.Model;
using Xamarin.Forms;

namespace VotacaoEstampas
{
    public interface IPersistenceService
    {
        void SaveImageAsync(string filename, byte[] image);
        void SaveEstampaAsync(string filename, byte[] image);
        Stream LoadImage(string filename);
        void SaveConfigAsync(string config);
        string LoadConfig();
        IEnumerable<byte[]> LoadImagesBytes(string foldername);
        Task<IEnumerable<byte[]>> LoadImagesBytesOrderedAsync(List<string> filesnames);
        void ClearImagesAsync(string foldername);
        Task VerifYfanReportsExe();
        Task<Colecao> LoadColecaoAsync();
        Task SaveColecaoAsync(Colecao colecao);
        string GetImagesLocalPath();
        void CopyLocalFolderToClipboard();
        Stream GetXlsxFile();
        void CopyReportToDownloads();
    }
}
