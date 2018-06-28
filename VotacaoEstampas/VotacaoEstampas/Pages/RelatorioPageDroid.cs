using DevExpress.Mobile.DataGrid;
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
using VotacaoEstampas.Repository;
using Xamarin.Forms;

namespace VotacaoEstampas
{
    public class RelatorioPageDroid : BaseContentPage
    {
        // componentes
        GridControl grid;

        // constantes
        private const string TXT_TITULO_RELATORIO = "Relatório de votações";

        // variaveis
        bool pagCarregada = false;
        VotacaoRepository dataGridSource;

        // styles
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        private readonly double FONTE_TEXTO_LABELS_TITULO = Device.GetNamedSize(NamedSize.Large, typeof(Label));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(80, 80, 80);

        public RelatorioPageDroid()
        {
            dataGridSource = new VotacaoRepository();
            BindingContext = dataGridSource;
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        
        private void CriarPagina()
        {
            var alturaTela = Height;
            var larguraTela = Width;
            var alturaHeader = alturaTela * .12f;
            var alturaBody = alturaTela * .9f;
            var padding_pagina = alturaTela * .05;

#region Header

            var imgBackButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.1485125102_back.png"),
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
            };
            imgBackButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                Navigation.PopAsync();
            }));

            var txt_titulo_relatorio = new Label
            {
                Text = TXT_TITULO_RELATORIO,
                FontSize = FONTE_TEXTO_LABELS_TITULO,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };

            var btnRelatorioClientes = new CustomButton
            {
                Text = "Exportar relatório para Excell",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.Black,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnRelatorioClientes.Clicked += async (sender, eventArgs) =>
            {
                ExportarRelatorio();
            };
            var containerMenu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnRelatorioClientes }
            };
            var header = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HeightRequest = alturaHeader,
                Children = {
                    new StackLayout {
                       Orientation = StackOrientation.Horizontal,
                       HorizontalOptions = LayoutOptions.FillAndExpand,
                       Children = {
                            imgBackButton,
                           txt_titulo_relatorio,
                       }
                   }, containerMenu,
                },
            };

            #endregion

            #region GridControl

            grid = new GridControl {
                BackgroundColor = Color.White,
                AutoGenerateColumnsMode = AutoGenerateColumnsMode.None,
                ItemsSource = dataGridSource.Votacoes,
                Columns = {
                    new TextColumn {
                        FieldName = "Cliente.Nome",
                        Caption = "Nome"
                    },
                     new TextColumn {
                        FieldName = "Cliente.Email",
                        Caption = "Email"
                    },
                      new TextColumn {
                        FieldName = "Cliente.Telefone",
                        Caption = "Telefone"
                    },
                       new TextColumn {
                        FieldName = "Data",
                        Caption = "Data Votação"
                    },
                }
            };
            if (App.ImagensEstampasColecaoAtual != null)
            {
                int i = 0;
                foreach (var estampa in App.ImagensEstampasColecaoAtual)
                {
                    var dadosArquivo = estampa;
                    Stream stream = new MemoryStream(dadosArquivo);
                    var imageHeaderColumnTemplate = new DataTemplate(() =>
                    {
                        return new Image() { Source = ImageSource.FromStream(() => { return stream; }) };
                    });
                    var coluna = new TextColumn() { FieldName = "VotoString" + i, HeaderTemplate = imageHeaderColumnTemplate };

                    grid.Columns.Add(coluna);
                    i++;
                }
            }

            #endregion

            BackgroundColor = COR_BACKGROUND_TELA;
            Padding = new Thickness(padding_pagina);
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.StartAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    header, grid
                },
            };
        }

        private async void ExportarRelatorio()
        {
            grid.ExportToExcel(DependencyService.Get<IPersistenceService>().GetXlsxFile(), DevExpress.Export.ExportTarget.Xlsx);
            await DisplayAlert("Exportar relatório", "O relatório foi exportado no formato xlsx (Excell) para a pasta \"Downloads\".", "Certo!");
        }
    }
}
