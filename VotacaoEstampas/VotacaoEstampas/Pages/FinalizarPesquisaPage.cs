using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using VotacaoEstampas.CustomControls;
using Xamarin.Forms;

namespace VotacaoEstampas.Pages
{
    public class FinalizarPesquisaPage : BaseContentPage
    {
        // constantes
        private readonly Color COR_PAGE_HEADER_FOOTER = Color.Black;
        private readonly string TXT_PAGE_FOOTER = "www.yfan.com.br";
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        private readonly double FONTE_TEXTO_BUTTONS = Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
        private readonly Color COR_TEXTO_LABELS = Color.Black;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly FontAttributes ATRIBUTOS_TEXTO_LABELS = FontAttributes.None;
        private readonly string TXT_AGRADECIMENTO = "Agradecemos por colaborar conosco, sua opinião é muito importante para nós.\n\n" +
                                    "O nosso objetivo é entender o perfil dos nossos clientes para proporcionar a sua satisfação.\n\n" +
                                    "Você já  está concorrendo a prêmios a partir de agora.\n\n" +
                                    "Para mais informações, consulte  nossa equipe ou acesse o nosso site.\n\n";

        // elementos visuais
        Frame header;
        Image imageAgradecimento;
        Image logoHeader;
        StackLayout containerAgradecimento;
        StackLayout btnsContainer;

        // variaveis
        bool pagCarregada = false;
        VotarEstampaPage _votarEstampaPage;

        public FinalizarPesquisaPage(VotarEstampaPage votarEstampaPage)
        {
            _votarEstampaPage = votarEstampaPage;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var corSalva = DependencyService.Get<IPersistenceService>().LoadConfig();
            if (corSalva != null)
            {
                var corSalvaTxt = BuscarCorPorNome(corSalva);
                BackgroundColor = Color.FromRgb(corSalvaTxt.R, corSalvaTxt.G, corSalvaTxt.B);
            }

            var imgAgradecimentoSalva = DependencyService.Get<IPersistenceService>().LoadImage("agradecimento.jpg");
            if (imgAgradecimentoSalva != null)
            {
                imageAgradecimento.Source = ImageSource.FromStream(() =>
                {
                    return imgAgradecimentoSalva;
                });
            }
            else
            {
                imageAgradecimento.Source = ImageSource.FromResource("VotacaoEstampas.Images.agradecimento.jpg");
            }

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

#if !__ANDROID__
            RedimensionarAgradecimento();
#endif
        }

        private async void RedimensionarAgradecimento()
        {
            var contHeig = containerAgradecimento.Height;
            containerAgradecimento.Children.Add(imageAgradecimento);

            while (imageAgradecimento.Height == -1 || imageAgradecimento.Height == 0)
                await Task.Delay(100);

            double ratio = contHeig / imageAgradecimento.Height;

            imageAgradecimento.HeightRequest = imageAgradecimento.Height * ratio;
            imageAgradecimento.WidthRequest = imageAgradecimento.Width * ratio;

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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!pagCarregada)
            {
                pagCarregada = true;

#if __ANDROID__
                CriarPaginaAndroid();
#else
                CriarPagina();
#endif
            }
        }

        private void CriarPaginaAndroid()
        {
            var alturaTela = Height;
            var alturaHeader = alturaTela * .20f;
            var alturaFooter = alturaTela * .08f;
            var spacingContent = alturaTela * .0f;

            logoHeader = new Image
            {
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
                Padding = new Thickness(-5),
            };
            header.Content = logoHeader;

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

            var btnVoltarPesquisa = new CustomButton
            {
                Text = "Voltar à votação",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                //WidthRequest = larguraBody / 2,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnVoltarPesquisa.Clicked += async (sender, eventArgs) => await Navigation.PopAsync();
            var btnConfirmarPesquisa = new CustomButton
            {
                Text = "Confirmar",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                //WidthRequest = larguraBody / 2,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnConfirmarPesquisa.Clicked += async (sender, eventArgs) =>
            {
                await Navigation.PushAsync(new AgradecimentoPage());
                Navigation.RemovePage(_votarEstampaPage);
                Navigation.RemovePage(this);
            };

            btnsContainer = new StackLayout
            {
                Padding = new Thickness(20, 0),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 20f,
                Children = {
                    btnVoltarPesquisa, btnConfirmarPesquisa
                }
            };

            imageAgradecimento = new Image
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFill
            };

            var containerEstampa = new StackLayout
            {
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { imageAgradecimento }
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
                        btnsContainer,
                    },
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 10, 0, 0),
                HeightRequest = alturaFooter
            };



            // set conteudo da página
            Content = new StackLayout
            {
                Padding = new Thickness(-4),
                Orientation = StackOrientation.Vertical,
                Spacing = spacingContent,
                Children = {
                       header, containerEstampa, footer
                    }
            };
        }

        private void CriarPagina()
        {
            var alturaTela = Height;
            var larguraTela = Width;
            var alturaHeader = alturaTela * .20f;
            var alturaBody = alturaTela * .70f;
            //var alturaFooter = alturaTela * .08f;
            //var alturaImageAgradecimento = alturaTela * .5f;
            var spacingContent = alturaTela * .03f;
            var larguraBody = larguraTela * .8f;

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
                WidthRequest = alturaHeader
            };
            header.Content = logoHeader;

            BackgroundColor = Color.Gray;

            var btnVoltarPesquisa = new CustomButton
            {
                Text = "Voltar à votação",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
#if __ANDROID__
                WidthRequest = larguraBody / 2,
#else
                WidthRequest = larguraBody / 4,
#endif
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnVoltarPesquisa.Clicked += async (sender, eventArgs) => await Navigation.PopAsync();
            var btnConfirmarPesquisa = new CustomButton
            {
                Text = "Confirmar",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
#if __ANDROID__
                WidthRequest = larguraBody / 2,
#else
                WidthRequest = larguraBody / 4,
#endif
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnConfirmarPesquisa.Clicked += async (sender, eventArgs) =>
            {
                await Navigation.PushAsync(new AgradecimentoPage());
                Navigation.RemovePage(_votarEstampaPage);
                Navigation.RemovePage(this);
            };

            btnsContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.End,
                Spacing = 10,
                Children = {
                    btnVoltarPesquisa, btnConfirmarPesquisa
                }
            };
            /*var txtbtnsContainer = new StackLayout {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Children = { txtContainer, btnsContainer}
            };*/

            imageAgradecimento = new Image
            {
                HorizontalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit
            };
            containerAgradecimento = new StackLayout
            {
                Padding = new Thickness(0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                // Children = { imageAgradecimento }
            };

            var body = new StackLayout
            {
                Padding = new Thickness(0),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = larguraBody,
                HeightRequest = alturaBody,
                //Spacing = spacingContent,
                Children = {
                    containerAgradecimento, btnsContainer
                }
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
                //Spacing = spacingContent,
                Children = {
                       header, body, //footer
                    }
            };
        }
    }
}
