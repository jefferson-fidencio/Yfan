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
using VotacaoEstampas.Model;
using VotacaoEstampas.Pages;

using Xamarin.Forms;

namespace VotacaoEstampas
{
    public class ConfigurarEstampasPage : BaseContentPage
    {
        //definir em style, capitulo 12 do xamarin book
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly FontAttributes ATRIBUTOS_TEXTO_LABELS = FontAttributes.Bold;
        private readonly string TEXT_LBL_COLOR_SELECTED = "Estampas selecionadas para pesquisa: ";
        private readonly string TEXT_RELATORIO = "Relatório (votações desta coleção)";
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(80, 80, 80);

        // elementos visuais
        StackLayout SelectorEstampasSpinner;
        Image imgRemoveButton;
        FrameEstampa selectedImage;

        // variaveis
        bool pagCarregada = false;
        bool _estampasCarregadas = false;
        private Color _selectedColor = Color.FromRgb(150, 150, 150);
        private List<KeyValuePair<int, byte[]>> imagensAdicionadas = new List<KeyValuePair<int, byte[]>>();
        private bool _estampasAlteradas;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (!pagCarregada)
            {
                pagCarregada = true;
                CriarPagina();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_estampasCarregadas)
            {
                _estampasCarregadas = true;
                if (App.ImagensEstampasColecaoAtual != null)
                {
                    foreach (var estampa in App.ImagensEstampasColecaoAtual)
                    {
                        var dadosArquivo = estampa;
                        var idArquivo = GetNextImageIndex();

                        Stream stream = new MemoryStream(dadosArquivo);
                        CriarImagem(ImageSource.FromStream(() => { return stream; }), idArquivo);
                        imagensAdicionadas.Add(new KeyValuePair<int, byte[]>(idArquivo, dadosArquivo));
                    }
                }
                else
                {
                    CriarImagem(ImageSource.FromResource("VotacaoEstampas.Images.1484814357_image.png"), 1);
                    CriarImagem(ImageSource.FromResource("VotacaoEstampas.Images.1484814357_image.png"), 3);
                    CriarImagem(ImageSource.FromResource("VotacaoEstampas.Images.1484814357_image.png"), 2);
                }
            }
        }

        private void CriarPagina()
        {
            var alturaTela = Height;
            var larguraTela = Width;
            var paddingTela = alturaTela * .02f;
            var alturaEstampasPickerContainer = alturaTela * .05f;
            var alturaEstampasSelectorContainer = alturaTela * .5f;
            var larguraEstampasSelectorContainer = alturaTela * .9f;

            var alturaHeader = alturaTela * .20f;
            var alturaFooter = alturaTela * .08f;
            var alturaRecomentadaEstampa = alturaTela - alturaHeader - alturaFooter;

            Padding = new Thickness(paddingTela);
            BackgroundColor = COR_BACKGROUND_TELA;

            var SelectorRelatoriosText = new Label
            {
                Text = TEXT_RELATORIO,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center                
            };
            var imgRelatorio = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.1484886998_6.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
            };
            imgRelatorio.GestureRecognizers.Add(new TapGestureRecognizer(async sender => {
                if (App.UltimaColecao.Votacoes != null && App.UltimaColecao.Votacoes.Count != 0)
                    await Navigation.PushAsync(new RelatorioPage());
                else
                    DisplayAlert("Não disponível", "Não há relatórios salvos.", "OK");
            }));
            var SelectorRelatorioContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 30,
                HeightRequest = alturaEstampasPickerContainer,
                Children = {
                    imgRelatorio,SelectorRelatoriosText 
                },
            };

            var imgAddButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.icon_add_128.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
            };
            imgAddButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                var arquivo = await CrossFilePicker.Current.PickFile();
                if (arquivo != null)
                {
                    if (imagensAdicionadas.Count == 0)
                    {
                        SelectorEstampasSpinner.Children.Clear();
                    }

                    var dadosArquivo = arquivo.DataArray;
                    var idArquivo = GetNextImageIndex();

                    Stream stream = new MemoryStream(dadosArquivo);
                    CriarImagem(ImageSource.FromStream(() => { return stream; }), idArquivo);
                    imagensAdicionadas.Add(new KeyValuePair<int, byte[]> (idArquivo, dadosArquivo));
                    _estampasAlteradas = true;
                }
            }));
            imgRemoveButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.icon_remove_128.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                IsVisible = false
            };
            imgRemoveButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                if (selectedImage != null)
                {
                    if (SelectorEstampasSpinner.Children.Any((x) => x == selectedImage))
                    {
                        var answer = await DisplayAlert("Excluir?", "Deseja realmente excluir esta espampa?", "Sim", "Não");
                        if (answer)
                        {
                            SelectorEstampasSpinner.Children.Remove(selectedImage);
                            try { imagensAdicionadas.Remove(imagensAdicionadas.FirstOrDefault(x => x.Key == selectedImage.idEstampa)); }
                            catch { }
                            selectedImage = null;
                            _estampasAlteradas = true;
                        }
                    }
                }
            }));
            var imgSelectorContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                Children = {
                    imgRemoveButton, imgAddButton
                },
            };
            var SelectorEstampasText = new Label
            {
                Text = TEXT_LBL_COLOR_SELECTED,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            var SelectorEstampasContainer = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HeightRequest = alturaEstampasPickerContainer,
                Children = {
                    SelectorEstampasText, imgSelectorContainer
                },
            };
            var SelectorEstampasSizeHint = new Label
            {
                Text = string.Format("Tamanho recomendado para estampas é de {0}x{1} pixels.",Convert.ToInt32(larguraTela), Convert.ToInt32(alturaRecomentadaEstampa)),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = COR_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            SelectorEstampasSpinner = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
            };

            var SelectorEstampasScroll = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                HeightRequest = alturaEstampasSelectorContainer,
                WidthRequest = larguraEstampasSelectorContainer,
                Padding = new Thickness(0, 0, 0, 20),
                Content = SelectorEstampasSpinner
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
            btnIniciarAplicacao.Clicked += BtnIniciarAplicacao_Clicked;//async (sender, eventArgs) => 
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
                   SelectorRelatorioContainer, SelectorEstampasContainer,SelectorEstampasSizeHint, SelectorEstampasScroll, btnIniciarAplicacao
                },
            };
        }

        private int GetNextImageIndex()
        {
            return (imagensAdicionadas != null && imagensAdicionadas.Count > 0) ? imagensAdicionadas.Count : 0;
        }

        private async void BtnIniciarAplicacao_Clicked(object sender, EventArgs e)
        {
            if (imagensAdicionadas.Count < 3)
            {
                await DisplayAlert("Selecione Estampas", "Você deve selecionar pelo menos 3 estampas.", "OK");
                return;
            }
            
            await Navigation.PushAsync(new IniciarPesquisaPage());
            Navigation.RemovePage(this);

            // verificamos se a colecao mudou (imagens alteradas)
            if (App.UltimaColecao.Estampas == null || _estampasAlteradas)
            {
                //se a colecao mudou, salva nova colecao
                DisplayAlert("Nova coleção", "A coleção de estampas foi alterada. Os dados da última coleção foram perdidos.", "OK");

                //persiste estampas (imagens)
                var estampas = new List<Estampa>();
                var caminhoEstapas = DependencyService.Get<IPersistenceService>().GetImagesLocalPath();
                DependencyService.Get<IPersistenceService>().ClearImagesAsync("Estampas");
                foreach (var estampa in imagensAdicionadas)
                {
                    var estampaPersist = new Estampa
                    {
                        Id = estampa.Key,
                        Path = caminhoEstapas + "/" + estampa.Key
                    };
                    estampas.Add(estampaPersist);
                    DependencyService.Get<IPersistenceService>().SaveEstampaAsync(estampa.Key + ".jpg", estampa.Value);
                }

                // persiste colecao
                App.UltimaColecao.Votacoes = new List<Votacao>();
                App.UltimaColecao.Estampas = estampas;
                App.UltimaColecao.Data = DateTime.Now;
                PersistirColecaoeCarregarImagensAsync();

                //atualiza colecao atual
                App.ImagensEstampasColecaoAtual = imagensAdicionadas.Select(x => x.Value);
            }
        }

        private async void PersistirColecaoeCarregarImagensAsync()
        {
            //salva
            await DependencyService.Get<IPersistenceService>().SaveColecaoAsync(App.UltimaColecao);

            /*List<string> nomesArquivosEstampas = new List<string>();
            foreach (Estampa estampa in App.UltimaColecao.Estampas)
            {
                nomesArquivosEstampas.Add(estampa.Id.ToString());
            }

            //carrega na ordem correta
            App.ImagensEstampasColecaoAtual = await DependencyService.Get<IPersistenceService>().LoadImagesBytesOrderedAsync(nomesArquivosEstampas);*/

        }

        private void CriarImagem(ImageSource imageSource, int idArquivo)
        {
            var img = new Image {
                Source = imageSource
            };

            img.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                //deseleciona todas
                foreach (var item in SelectorEstampasSpinner.Children)
                {
                    var child = item as FrameEstampa;
                    if (child != null) child.OutlineColor = Color.Transparent;
                }

                //seleciona a atual
                var parent = sender.Parent as FrameEstampa;
                if (parent != null)
                    parent.OutlineColor = Color.Aqua;

                //habilita botao de remocao
                imgRemoveButton.IsVisible = true;
                selectedImage = parent;

            }));
            var retanguloSelecao = new FrameEstampa {
                idEstampa = idArquivo,
                Content = img,
                OutlineColor = Color.Transparent
            };
            SelectorEstampasSpinner.Children.Add(retanguloSelecao);

        }      
          
    }
}
