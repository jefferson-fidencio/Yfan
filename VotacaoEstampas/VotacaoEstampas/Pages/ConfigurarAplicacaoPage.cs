using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using VotacaoEstampas.CustomControls;
using VotacaoEstampas.Pages;

using Xamarin.Forms;

namespace VotacaoEstampas
{
    public class ConfigurarAplicacaoPage : BaseContentPage
    {
        //definir em style, capitulo 12 do xamarin book
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Small, typeof(Label));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly FontAttributes ATRIBUTOS_TEXTO_LABELS = FontAttributes.Bold;
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(80, 80, 80);
        private readonly string TEXT_LBL_SELECT_LOGO = "Logo da página inicial:";
        private readonly string TEXT_LBL_AGRADECIMENTO = "Imagem de agradecimento final:";
        private readonly string TEXT_LBL_LOGO_SELECTED = "Nenhuma logo selecionada.";
        private readonly string TEXT_LBL_HEADER_SELECTED = "Nenhum cabeçalho selecionado.";
        private readonly string TEXT_LBL_AGRADECIMENTO_SELECTED = "Nenhuma imagem de agradecimento selecionada.";
        private readonly string TEXT_LBL_LOGO_SELECTED_OK = "Logo já selecionada.";
        private readonly string TEXT_LBL_AGRADECIMENTO_OK = "Imagem de agradecimento já selecionada.";
        private readonly string TEXT_LBL_HEADER_OK = "Imagem de cabeçalho já selecionada.";
        private readonly string TEXT_LBL_COLOR_SELECTED = "Tema selecionado: ";

        // elementos visuais
        Frame SelectorCorButton;
        StackLayout SelectorCorSpinner;
        Label imgSelectorText;
        Label selectorAgradecimentoText;
        Image imgLogoSelected;
        Label selectorHeaderText;

        // variaveis
        bool pagCarregada = false;
        private Color _selectedColor = Color.FromRgb(150, 150, 150);

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!pagCarregada)
            {
                pagCarregada = true;
                CriarPagina();
                OnAppearing(); //força recarregar pagina - alguns android estao chamando OnAppearing antes de OnSizeAllocated
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (pagCarregada)
            {
                SetVisibilidadeScroll(false);

                var logoSalva = DependencyService.Get<IPersistenceService>().LoadImage("logo.jpg");
                if (logoSalva != null)
                {
                    imgSelectorText.Text = TEXT_LBL_LOGO_SELECTED_OK;
                    imgLogoSelected.Source = ImageSource.FromStream(() =>
                    {
                        return logoSalva;
                    });
                }
                else
                {
                    if (imgLogoSelected != null)
                        imgLogoSelected.Source = ImageSource.FromResource("VotacaoEstampas.Images.logo_white_512.png");
                }

                var corSalva = DependencyService.Get<IPersistenceService>().LoadConfig();
                if (corSalva != null)
                {
                    var corSalvaTxt = BuscarCorPorNome(corSalva);
                    SelectorCorButton.BackgroundColor = Color.FromRgb(corSalvaTxt.R, corSalvaTxt.G, corSalvaTxt.B);
                }
                else
                {
                    if (SelectorCorButton != null)
                        SelectorCorButton.BackgroundColor = _selectedColor;
                }

                var imgAgradecimentoSalva = DependencyService.Get<IPersistenceService>().LoadImage("agradecimento.jpg");
                if (imgAgradecimentoSalva != null)
                {
                    selectorAgradecimentoText.Text = TEXT_LBL_AGRADECIMENTO_OK;
                }
                else
                {
                    if (selectorAgradecimentoText != null)
                        selectorAgradecimentoText.Text = TEXT_LBL_AGRADECIMENTO_SELECTED;
                }

                var imgHeaderSalva = DependencyService.Get<IPersistenceService>().LoadImage("header.jpg");
                if (imgHeaderSalva != null)
                {
                    selectorHeaderText.Text = TEXT_LBL_HEADER_OK;
                }
                else
                {
                    if (selectorHeaderText != null)
                        selectorHeaderText.Text = TEXT_LBL_HEADER_SELECTED;
                }
            }
        }

        private void CriarPagina()
        {
            var alturaTela = Height;
            var paddingTela = alturaTela * .02f;
            var alturaLogo = alturaTela * .5f;
            var alturaFilePickerContainer = alturaTela * .05f;
            var alturaCorSelectorContainer = alturaTela * .05f;

            Padding = new Thickness(paddingTela);
            BackgroundColor = COR_BACKGROUND_TELA;

            var lblSelecaoLogo = new Label
            {
                Text = TEXT_LBL_SELECT_LOGO,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };

            var lblVersaoApp = new Label
            {
                Text = "Versão " + App.VERSION_NAME,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.End,
                HorizontalOptions = LayoutOptions.End
            };

            var containerHeader = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children = { lblSelecaoLogo, lblVersaoApp },
                Orientation = StackOrientation.Horizontal
            };

            // instancia de objetos da pagina
            imgLogoSelected = new Image
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = alturaLogo,
                WidthRequest = alturaLogo,
            };

            imgSelectorText = new Label
            {
                Text = TEXT_LBL_LOGO_SELECTED,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            var imgSelectorButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.file_open_icon_128.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            imgSelectorButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                var arquivo = await CrossFilePicker.Current.PickFile();
                if (arquivo != null)
                {
                    imgSelectorText.Text = arquivo.FileName;
                    Stream stream = new MemoryStream(arquivo.DataArray);
                    imgLogoSelected.Source = ImageSource.FromStream(() => { return stream; });
                    DependencyService.Get<IPersistenceService>().SaveImageAsync("logo.jpg", arquivo.DataArray);
                }
            }));

            var imgSelectorContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = alturaFilePickerContainer,
                WidthRequest = alturaLogo,
                HorizontalOptions = LayoutOptions.Fill,
                Children = {
                    imgSelectorText, imgSelectorButton
                },
            };

            #region selecao imagem agradecimento

            selectorAgradecimentoText = new Label
            {
                Text = TEXT_LBL_AGRADECIMENTO_SELECTED,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            var imgSelectorAgradecimentoButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.file_open_icon_128.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            imgSelectorAgradecimentoButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                var arquivo = await CrossFilePicker.Current.PickFile();
                if (arquivo != null)
                {
                    //selectorAgradecimentoText.Text = arquivo.FileName;
                    selectorAgradecimentoText.Text = TEXT_LBL_AGRADECIMENTO_OK;
                    Stream stream = new MemoryStream(arquivo.DataArray);
                    //imgLogoSelected.Source = ImageSource.FromStream(() => { return stream; });
                    DependencyService.Get<IPersistenceService>().SaveImageAsync("agradecimento.jpg", arquivo.DataArray);
                }
            }));
            var imgAgradecimentoSelectorContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = alturaFilePickerContainer,
                WidthRequest = alturaLogo,
                HorizontalOptions = LayoutOptions.Fill,
                Children = {
                    selectorAgradecimentoText, imgSelectorAgradecimentoButton
                },
            };

            #endregion

            #region selecao imagem header

            selectorHeaderText = new Label
            {
                Text = TEXT_LBL_HEADER_SELECTED,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.TailTruncation
            };
            var imgSelectorHeaderButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.file_open_icon_128.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            imgSelectorHeaderButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                var arquivo = await CrossFilePicker.Current.PickFile();
                if (arquivo != null)
                {
                    selectorHeaderText.Text = TEXT_LBL_HEADER_OK;
                    Stream stream = new MemoryStream(arquivo.DataArray);
                    DependencyService.Get<IPersistenceService>().SaveImageAsync("header.jpg", arquivo.DataArray);
                }
            }));
            var imgHeaderSelectorContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = alturaFilePickerContainer,
                WidthRequest = alturaLogo,
                HorizontalOptions = LayoutOptions.Fill,
                Children = {
                    selectorHeaderText, imgSelectorHeaderButton
                },
            };

            #endregion

            var SelectorCorText = new Label
            {
                Text = TEXT_LBL_COLOR_SELECTED,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            SelectorCorSpinner = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 10,
            };
            CriarRetangulosCores(SelectorCorSpinner);

            var SelectorCorScroll = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HeightRequest = alturaFilePickerContainer,
                WidthRequest = alturaLogo,
                Padding = new Thickness(0, 0, 0, 20),
                Content = SelectorCorSpinner
            };

            SelectorCorButton = new Frame
            {
                BackgroundColor = _selectedColor,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                //OutlineColor = Color.White,
                //CornerRadius = 0
            };
            SelectorCorButton.GestureRecognizers.Add(new TapGestureRecognizer(sender =>
            {
                SetVisibilidadeScroll(!SelectorCorSpinner.IsVisible);
            }));

            var SelectorCorContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = {
                    SelectorCorText, SelectorCorButton,
                },
            };

            var btnIniciarAplicacao = new CustomButton
            {
                Text = "Iniciar Aplicação",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,

                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30

            };
            btnIniciarAplicacao.Clicked += async (sender, eventArgs) =>
            {

                await Navigation.PushAsync(new ConfigurarEstampasPage());
                Navigation.RemovePage(this);

            };
            var containerBtn = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(40, 10),
                Children = {
                    btnIniciarAplicacao
                }
            };

            // set conteudo da página
            Content = new StackLayout
            {
                //Padding = new Thickness(paddingTela),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                   containerHeader, imgLogoSelected, imgSelectorContainer, imgAgradecimentoSelectorContainer, imgHeaderSelectorContainer, SelectorCorContainer, SelectorCorScroll, btnIniciarAplicacao
                },
            };
        }

        private void SetVisibilidadeScroll(bool visible)
        {
            if (SelectorCorSpinner != null)
            {
                SelectorCorSpinner.HeightRequest = visible ? -1d : 0;
                SelectorCorSpinner.IsVisible = visible;
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

        private void CriarRetangulosCores(StackLayout selectorCorSpinner)
        {
            // Loop through the Color structure fields. 
            foreach (FieldInfo info in typeof(Color).GetRuntimeFields())
            { // Skip the obsolete (i.e. misspelled) colors. 
                if (info.GetCustomAttribute<ObsoleteAttribute>() != null) continue; if (info.IsPublic && info.IsStatic && info.FieldType == typeof(Color))
                {
                    selectorCorSpinner.Children.Add(CreateColorRectangle((Color)info.GetValue(null), info.Name));
                }
            }

            // Loop through the Color structure properties.
            foreach (PropertyInfo info in typeof(Color).GetRuntimeProperties())
            {
                MethodInfo methodInfo = info.GetMethod;
                if (methodInfo.IsPublic && methodInfo.IsStatic && methodInfo.ReturnType == typeof(Color))
                {
                    selectorCorSpinner.Children.Add(CreateColorRectangle((Color)info.GetValue(null), info.Name));
                }
            }
        }

        private Frame CreateColorRectangle(Color color, string nomeCor)
        {
            var retangulo = new Frame { BackgroundColor = color };
            retangulo.GestureRecognizers.Add(new TapGestureRecognizer(sender =>
            {
                _selectedColor = color;
                SelectorCorButton.BackgroundColor = _selectedColor;
                DependencyService.Get<IPersistenceService>().SaveConfigAsync(nomeCor);
            }));
            //retangulo.OutlineColor = Color.White;
            //retangulo.CornerRadius = 0;
            return retangulo;
        }

    }
}
