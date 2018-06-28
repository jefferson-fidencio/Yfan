using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VotacaoEstampas.Pages
{
    public class VotarEstampaPage : BaseContentPage
    {
        // constantes
        private readonly Color COR_PAGE_HEADER_FOOTER = Color.Black;
        private readonly string TXT_PAGE_FOOTER = "www.yfan.com.br";
        private readonly string TXT_PAGE_USER = "Bem vindo, ";
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Small, typeof(Entry));
        private readonly double FONTE_TEXTO_BUTTONS = Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly FontAttributes ATRIBUTOS_TEXTO_LABELS = FontAttributes.Bold;
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(100, 100, 100);

        // elementos visuais
        Frame header;
        Image imageEstampa;
        Image btnAnterior;
        Image btnProximo;
        Image logoHeader;
        StackLayout containerEstampa;

        // variaveis
        bool pagCarregada = false;
        private List<KeyValuePair<Guid, byte[]>> _estampas = new List<KeyValuePair<Guid, byte[]>>();
        private List<bool> _estampasVotadas = new List<bool>();
        int _indiceEstampaAtual = -1;
        double _maxHeightImage;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_estampas.Count == 0) //ainda nao carregamos as estampas
            {
                var corSalva = DependencyService.Get<IPersistenceService>().LoadConfig();
                if (corSalva != null)
                {
                    var corSalvaTxt = BuscarCorPorNome(corSalva);
                    BackgroundColor = Color.FromRgb(corSalvaTxt.R, corSalvaTxt.G, corSalvaTxt.B);
                }

                CarregarEstampas();

                //seleciona primeira estampa
                ExibirProximaEstampa();

                var logoSalva = DependencyService.Get<IPersistenceService>().LoadImage("header.jpg");
                if (logoSalva != null)
                {
                    header.Padding = new Thickness(0);
                    logoHeader.VerticalOptions = LayoutOptions.FillAndExpand;
                    logoHeader.Aspect = Aspect.Fill;
                    logoHeader.Source = ImageSource.FromStream(() =>
                    {
                        return logoSalva;
                    });
                }
                else
                {
                    logoHeader.Source = ImageSource.FromResource("VotacaoEstampas.Images.logo_white_512.png");
                }
            }

            //RedimensionarEstampa();
        }

        private async void RedimensionarEstampa()
        {
            while (imageEstampa.Height == -1 || imageEstampa.Height == 0)
                await Task.Delay(100);

            if (imageEstampa.Height > _maxHeightImage)
            {
                imageEstampa.HeightRequest = _maxHeightImage;
                imageEstampa.WidthRequest = _maxHeightImage;
                containerEstampa.VerticalOptions = LayoutOptions.CenterAndExpand;
                containerEstampa.HorizontalOptions = LayoutOptions.CenterAndExpand;
                containerEstampa.HeightRequest = _maxHeightImage;
            }
            else
            {
                double ratio = containerEstampa.Height / imageEstampa.Height;

                imageEstampa.HeightRequest = imageEstampa.Height * ratio;
                imageEstampa.WidthRequest = imageEstampa.Width * ratio;
            }
        }

        private Color BuscarCorPorNome(string nome)
        {
            // Loop through the Color structure fields. 
            foreach (FieldInfo info in typeof(Color).GetRuntimeFields())
            { // Skip the obsolete (i.e. misspelled) colors. 
                if (info.GetCustomAttribute<ObsoleteAttribute>() != null) continue; if (info.IsPublic && info.IsStatic && info.FieldType == typeof(Color))
                {
                    if (info.Name == nome)
                        return (Color)info.GetValue(null);
                }
            }

            // Loop through the Color structure properties.
            foreach (PropertyInfo info in typeof(Color).GetRuntimeProperties())
            {
                MethodInfo methodInfo = info.GetMethod;
                if (methodInfo.IsPublic && methodInfo.IsStatic && methodInfo.ReturnType == typeof(Color))
                {
                    if (info.Name == nome)
                        return (Color)info.GetValue(null);
                }
            }

            return Color.Default;
        }
        private void ExibirProximaEstampa()
        {
            _indiceEstampaAtual++;
            if (_estampas.Count > _indiceEstampaAtual)
            {
                var estampa = _estampas[_indiceEstampaAtual].Value;
                Stream stream = new MemoryStream(estampa);
                imageEstampa.Source = ImageSource.FromStream(() => { return stream; });
#if __ANDROID__
                if (_estampas.Count == _indiceEstampaAtual + 1)
                    btnProximo.Opacity = 0;
                else
                    btnProximo.Opacity = 100;

                if (_indiceEstampaAtual == 0)
                    btnAnterior.Opacity = 0;
                else
                    btnAnterior.Opacity = 100;
#else
                if (_estampas.Count == _indiceEstampaAtual + 1)
                    btnProximo.IsVisible = false;
                else
                    btnProximo.IsVisible = true;

                if (_indiceEstampaAtual == 0)
                    btnAnterior.IsVisible = false;
                else
                    btnAnterior.IsVisible = true;
#endif
            }
        }

        private void ExibirEstampaAnterior()
        {
            _indiceEstampaAtual--;
            if (_estampas.Count > _indiceEstampaAtual)
            {
                var estampa = _estampas[_indiceEstampaAtual].Value;
                Stream stream = new MemoryStream(estampa);
                imageEstampa.Source = ImageSource.FromStream(() => { return stream; });

#if __ANDROID__
                if (_estampas.Count == _indiceEstampaAtual + 1)
                    btnProximo.Opacity = 0;
                else
                    btnProximo.Opacity = 100;

                if (_indiceEstampaAtual == 0)
                    btnAnterior.Opacity = 0;
                else
                    btnAnterior.Opacity = 100;
#else
                if (_estampas.Count == _indiceEstampaAtual + 1)
                    btnProximo.IsVisible = false;
                else
                    btnProximo.IsVisible = true;

                if (_indiceEstampaAtual == 0)
                    btnAnterior.IsVisible = false;
                else
                    btnAnterior.IsVisible = true;
#endif
            }
        }

        private void CarregarEstampas()
        {
            var estampas = App.ImagensEstampasColecaoAtual;
            if (estampas != null)
            {
                foreach (var estampa in estampas)
                {
                    var dadosArquivo = estampa;
                    var idArquivo = Guid.NewGuid();
                    _estampas.Add(new KeyValuePair<Guid, byte[]>(idArquivo, dadosArquivo));
                    _estampasVotadas.Add(false); //ainda nao votada
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!pagCarregada)
            {
                pagCarregada = true;
                CriarPagina();
            }
        }

        private void CriarPagina()
        {
            var alturaTela = Height;
            var alturaHeader = alturaTela * .20f;
            var alturaFooter = alturaTela * .08f;
            var alturaEstampa = _maxHeightImage = alturaTela * .65f;
            var spacingContent = alturaTela * .0f;

            logoHeader = new Image
            {
                //Source = ImageSource.FromResource("VotacaoEstampas.Images.logo_white_512.png"),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFill
            };
            header = new Frame
            {
                OutlineColor = COR_PAGE_HEADER_FOOTER,
                BackgroundColor = COR_PAGE_HEADER_FOOTER,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = alturaHeader,
                WidthRequest = alturaHeader,
#if __ANDROID__
                Padding = new Thickness(-5),
#else
                Padding = new Thickness(0),
#endif
            };
            header.Content = logoHeader;

            BackgroundColor = COR_BACKGROUND_TELA;

            // instancia de objetos da pagina que nao precisam de size request relativo ao tamanho da pagina
            var txtFooter = new Label
            {
                Text = TXT_PAGE_FOOTER,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var btnNAO = new CustomControls.CustomButton
            {
                Text = "NÃO",
                FontSize = FONTE_TEXTO_BUTTONS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                WidthRequest = 90,
                BorderRadius = 45,
                HorizontalOptions = LayoutOptions.Center,

                CorBackgroundCustomRed = 200,
                CorBackgroundCustomGreen = 00,
                CorBackgroundCustomBlue = 00
            };
            btnNAO.Clicked += BtnNAO_Clicked;
            var btnSIM = new CustomControls.CustomButton
            {
                Text = "SIM",
                FontSize = FONTE_TEXTO_BUTTONS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                WidthRequest = 90,
                BorderRadius = 45,
                HorizontalOptions = LayoutOptions.Center,

                CorBackgroundCustomRed = 00,
                CorBackgroundCustomGreen = 140,
                CorBackgroundCustomBlue = 00
            };
            btnSIM.Clicked += BtnSIM_Clicked;  

            imageEstampa = new Image
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFill
            };

            containerEstampa = new StackLayout
            {
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { imageEstampa }
            };

            btnAnterior = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.ArrowLeft.png"),
#if __ANDROID__
                HorizontalOptions = LayoutOptions.Start,
#endif
            };
            btnAnterior.GestureRecognizers.Add(new TapGestureRecognizer(sender => {
                ExibirEstampaAnterior();
            }));

#if !__ANDROID__
            var btnAnteriorContainer = new Frame {
                Padding = 0,
                Content = btnAnterior,
                HorizontalOptions = LayoutOptions.Start,
            };
#endif
            btnProximo = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.ArrowRight.png"),
#if __ANDROID__
                HorizontalOptions = LayoutOptions.End,
#endif
            };
            btnProximo.GestureRecognizers.Add(new TapGestureRecognizer(sender => {
                ExibirProximaEstampa();
            }));
#if !__ANDROID__
            var btnProximoContainer = new Frame
            {
                Padding = 0,
                Content = btnProximo,
                HorizontalOptions = LayoutOptions.End,
            };
#endif
            var buttonsContainer = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Spacing = 20f,
                Children = {
#if __ANDROID__
                    btnAnterior,
#else
                    btnAnteriorContainer,
#endif
                    new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 50,
                        Children =
                        {
                            btnNAO, btnSIM                            
                        }
                    },
#if __ANDROID__
                    btnProximo
#else
                    btnProximoContainer
#endif
                }
            };

            var txtContainerFooter = new Frame
            {
                OutlineColor = COR_PAGE_HEADER_FOOTER,
                BackgroundColor = COR_PAGE_HEADER_FOOTER,

                VerticalOptions = LayoutOptions.EndAndExpand,
                Content = txtFooter
            };

            var footer = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = {
                        buttonsContainer, //txtContainerFooter
                    },
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0,10,0,0),
                HeightRequest = alturaFooter
            };
            
          

            // set conteudo da página
            Content = new StackLayout
            {
                //Padding = -5, isso resolve o bug de padding no android
#if __ANDROID__
                Padding = new Thickness(-4),
#else
                Padding = new Thickness(0),
#endif
                Orientation = StackOrientation.Vertical,
                Spacing = spacingContent,
                Children = {
                       header, containerEstampa, footer
                    }
            };
        }

        private void BtnNAO_Clicked(object sender, EventArgs e)
        {
            AnimarBotao(sender);

            //altera estado da votacao para a estampa atual
            for (int indexSearched = 0; indexSearched < App.VotacaoAtual.Votos.Count; indexSearched++)
            {
                if (indexSearched == _indiceEstampaAtual)
                {
                    App.VotacaoAtual.Votos[indexSearched] = false;
                    _estampasVotadas[indexSearched] = true; //esta estampa ja foi votada
                    break;
                }
            }

#if __ANDROID__
            if (btnProximo.Opacity == 0)
#else
            if (!btnProximo.IsVisible)
#endif
                AbrirPaginaFimPesquisa();
            else
                ExibirProximaEstampa();
        }

        private async void AnimarBotao(object sender)
        {
            var button = sender as Button;
            button.Rotation = 0; button.RotateTo(360, 500);
            await button.ScaleTo(3, 250);
            await button.ScaleTo(1, 250);
        }

        private void BtnSIM_Clicked(object sender, EventArgs e)
        {
            AnimarBotao(sender);

            //altera estado da votacao para a estampa atual
            for (int indexSearched = 0; indexSearched < App.VotacaoAtual.Votos.Count; indexSearched++ )
            {
                if (indexSearched == _indiceEstampaAtual)
                {
                    App.VotacaoAtual.Votos[indexSearched] = true;
                    _estampasVotadas[indexSearched] = true; //esta estampa ja foi votada
                    break;
                }
            }

#if __ANDROID__
            if (btnProximo.Opacity == 0)
#else
            if (!btnProximo.IsVisible)
#endif
                AbrirPaginaFimPesquisa();
            else
                ExibirProximaEstampa();          
        }

        private async void AbrirPaginaFimPesquisa()
        {
            if (_estampasVotadas.Contains(false)) //alguma estampa nao foi votada ainda
            {
                DisplayAlert("Por favor vote as estampas", "Olá! Existem estampas que ainda nao foram votadas. Por favor retorne e vote se SIM ou NÃO.", "Certo");
                return;
            }

            await Navigation.PushAsync(new FinalizarPesquisaPage(this));
        }


    }
}
