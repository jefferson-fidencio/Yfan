using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotacaoEstampas.Model;
using VotacaoEstampas.Pages;
using Xamarin.Forms;

namespace VotacaoEstampas
{
    public class App : Application
    {
        public static string VERSION_NAME = "0.1.0";
        public static Colecao UltimaColecao;
        internal static Votacao VotacaoAtual;
        public static IEnumerable<byte[]> ImagensEstampasColecaoAtual;

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new ConfigurarAplicacaoPage());

            //ja vai carregando a ultima colecao
            LoadColecaoAsync();
            VerifYfanReportsExe();

        }

        private async void VerifYfanReportsExe()
        {
            await DependencyService.Get<IPersistenceService>().VerifYfanReportsExe();
        }

        private async void LoadColecaoAsync()
        {
            UltimaColecao = await DependencyService.Get<IPersistenceService>().LoadColecaoAsync();

            //se ja tem colecao salva, carrega as imagens
            if (UltimaColecao != null && UltimaColecao.Estampas != null && UltimaColecao.Estampas.Count > 0)
            {
                List<string> nomesArquivosEstampas = new List<string>();
                foreach (Estampa estampa in UltimaColecao.Estampas)
                {
                    nomesArquivosEstampas.Add(estampa.Id.ToString());
                }
                LoadEstampasImagesAsync(nomesArquivosEstampas);
            }
        }

        private async void LoadEstampasImagesAsync(List<string> nomesArquivosEstampas)
        {
            ImagensEstampasColecaoAtual = await DependencyService.Get<IPersistenceService>().LoadImagesBytesOrderedAsync(nomesArquivosEstampas);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
