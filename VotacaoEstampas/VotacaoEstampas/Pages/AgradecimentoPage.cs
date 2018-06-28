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
    public class AgradecimentoPage : BaseContentPage
    {
        // constantes
        private readonly Color COR_PAGE_HEADER_FOOTER = Color.Black;
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Large, typeof(Entry));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";

        // elementos visuais
        Frame header;
        Image logoHeader;
        Color CorFundo;
        Label lblAgradecimento;

        // variaveis
        bool pagCarregada = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var corSalva = DependencyService.Get<IPersistenceService>().LoadConfig();
            if (corSalva != null)
            {
                var corSalvaTxt = BuscarCorPorNome(corSalva);
                BackgroundColor = CorFundo = Color.FromRgb(corSalvaTxt.R, corSalvaTxt.G, corSalvaTxt.B);
            }

            // Standard luminance calculation.
            Color color = Color.Default;
            double luminance = 0.2126 * CorFundo.R + 0.7152 * CorFundo.G + 0.0722 * CorFundo.B;
            color = luminance < 0.5 ? Color.White : Color.Black;
            lblAgradecimento.TextColor = color;

            SalvarFinalizarAsync();

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

        private async void SalvarFinalizarAsync()
        {
            //salva votacao
            App.UltimaColecao.Votacoes.Add(App.VotacaoAtual);
            DependencyService.Get<IPersistenceService>().SaveColecaoAsync(App.UltimaColecao);

            await Task.Delay(3000);
            await Navigation.PopAsync();
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
                CriarPagina();
            }
        }

        private void CriarPagina()
        {
            var alturaTela = this.Height;
            var alturaHeader = alturaTela * .25f;
            var alturaBody = alturaTela * .65f;
            //var alturaFooter = alturaTela * .1f;

            BackgroundColor = Color.Gray;

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

            lblAgradecimento = new Label
            {
                Text = "Obrigado, " + App.VotacaoAtual.Cliente.Nome + ", por participar!",
                FontSize = 50f,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.CharacterWrap,
                WidthRequest = alturaTela * .8f,
            };

            var body = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(20),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = alturaBody,
                Children = {
                   lblAgradecimento
                }
            };

            var btnVoltarPesquisa = new CustomButton
            {
                Text = "Nova votação",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.None,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 20,
                WidthRequest = FONTE_TEXTO_LABELS * 10,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnVoltarPesquisa.Clicked += async (sender, eventArgs) => await Navigation.PopAsync();

            var txtContainerFooter = new Frame
            {
                VerticalOptions = LayoutOptions.End,
                Padding = new Thickness(20, 10),
                Content = btnVoltarPesquisa
            };

            /*var footer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                        txtContainerFooter
                    },
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = alturaFooter
            };*/

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
                Children = {
                       header, body, //footer
                    }
            };
        }
    }
}
