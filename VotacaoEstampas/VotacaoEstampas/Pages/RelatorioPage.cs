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
    public class RelatorioPage : BaseContentPage
    {
        // constantes
#if __ANDROID__
        private const string TXT_TITULO_RELATORIO = "Relatório de votações";
#else
        private const string TXT_TITULO_RELATORIO = "Relatório de votações da última coleção";
#endif
        private const string TXT_TOTAL_VOTOS = "Total de votacões realizadas: ";
        private const string TXT_VOTOS_ESTAMPAS = "Total de votacões positivas e negativas por estampas: ";
#if __ANDROID__
        private const string TXT_DATA_COLECAO = "Período: ";
#else
        private const string TXT_DATA_COLECAO = "Data da coleção: ";
#endif
        private const string TXT_DATA_COLECAO_DE = "De ";
        private const string TXT_DATA_COLECAO_PARA = "A ";
        private const string TXT_TITULO_CLIENTES_VOTOS = "Clientes e Votações:";
        private const string TXT_NOME_CLIENTE = "Nome: ";
        private const string TXT_DATA_VOTACAO_CLIENTE = "Data: ";
        private const string TXT_EMAIL_CLIENTE = "Email: ";
        private const string TXT_TELEFONE_CLIENTE = "Telefone: ";
        private const string TXT_VOTO_POSITIVO = "SIM";
        private const string TXT_VOTO_NEGATIVO = "NÃO";
#if __ANDROID__
        private const int NUM_COLUNAS_GRID_VOTOS = 3;
#else
        private const int NUM_COLUNAS_GRID_VOTOS = 5;
#endif

        // elementos visuais
        StackLayout clientes_votos_container;
        StackLayout estampas_votos_container;
        ScrollView bodyClientes;
        ScrollView bodyEstampas;

        // variaveis
        bool pagCarregada = false;

        // styles
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Medium, typeof(Label));
        private readonly double FONTE_TEXTO_LABELS_TITULO = Device.GetNamedSize(NamedSize.Large, typeof(Label));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(80, 80, 80);

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

            foreach (Votacao votacao in App.UltimaColecao.Votacoes)
            {
                CarregarClientesVotacoes(votacao);
            }

            var totalVotacoes = 0;
            var votosPositivosEstampa = new List<int>();
            var votosNegativosEstampa = new List<int>();

            foreach (Votacao votacao in App.UltimaColecao.Votacoes)
            {
                //inicializa com 0 em todas as estampas
                if (votosPositivosEstampa.Count == 0)
                {
                    foreach (bool voto in votacao.Votos)
                    {
                        votosPositivosEstampa.Add(0);
                        votosNegativosEstampa.Add(0);
                    }
                }

                //contabiliza votos
                int index_voto_atual = 0;
                foreach (bool voto in votacao.Votos)
                {
                    //se foi votado, eh +1
                    if (voto == true)
                        votosPositivosEstampa[index_voto_atual] = votosPositivosEstampa[index_voto_atual] + 1;
                    else
                        votosNegativosEstampa[index_voto_atual] = votosNegativosEstampa[index_voto_atual] + 1;


                    index_voto_atual++;
                }

                totalVotacoes++;
            }

            CarregarEstampasVotacoes(totalVotacoes, votosPositivosEstampa, votosNegativosEstampa);
        }

        private void CarregarEstampasVotacoes(int totalVotacoes, List<int> votosPositivosEstampa, List<int> votosNegativosEstampa)
        {
            var txt_total_votos = new Label
            {
                Text = TXT_TOTAL_VOTOS + totalVotacoes,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var txt_votos_estampas = new Label
            {
                Text = TXT_VOTOS_ESTAMPAS,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var votos_container = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                ColumnSpacing = 10,
                RowSpacing = 10
            };

            // Crio o numero de linhas suficiente para conter o numero de colunas certo por votos
            for (int i = 0; i < votosPositivosEstampa.Count / NUM_COLUNAS_GRID_VOTOS; i++)
            {
                votos_container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < NUM_COLUNAS_GRID_VOTOS; i++)
            {
                votos_container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var estampas = DependencyService.Get<IPersistenceService>().LoadImagesBytes("Estampas");
            int coluna = 0;
            int linha = 0;
            int index_voto_atual = 0;
            for (int i = 0; i < votosPositivosEstampa.Count; i++)
            {
                var estampa_voto = CriarEstampaVotosTotal(estampas.ElementAt(index_voto_atual), votosPositivosEstampa.ElementAt(index_voto_atual), votosNegativosEstampa.ElementAt(index_voto_atual));
                index_voto_atual++;
                votos_container.Children.Add(estampa_voto, coluna, linha);

                if (coluna == (NUM_COLUNAS_GRID_VOTOS - 1))
                {
                    coluna = 0;
                    linha++;
                }
                else
                {
                    coluna++;
                }
            }

            //cria cliente
            var client_votos_container = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = {
                    txt_total_votos,
                    txt_votos_estampas,
                    votos_container
                }
            };

            //adiciona para container
            estampas_votos_container.Children.Add(
                new Frame
                {
                    Padding = new Thickness(30, 10),
                    OutlineColor = Color.White,
                    Content = client_votos_container
#if __ANDROID__
                ,
                    BackgroundColor = Color.Transparent
#endif
                });
        }

        private StackLayout CriarEstampaVotosTotal(byte[] estampa, int totalVotosPositivos, int totalVotosNegativos)
        {
            var imagem_estampa = CriarImagem(ImageSource.FromStream(() =>
            {
                return new MemoryStream(estampa);
            }));
            var img_voto_pos = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.1484898084_Tick_Mark_Dark.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            var img_voto_neg = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.deny.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            var txt_voto_pos = new Label
            {
                Text = totalVotosPositivos.ToString(),
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_voto_neg = new Label
            {
                Text = totalVotosNegativos.ToString(),
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var container_voto = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 20,
                Children = {
                     img_voto_pos,
                     txt_voto_pos,
                     img_voto_neg,
                     txt_voto_neg
                 }
            };

            return new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 10,
                Children = {
                    imagem_estampa,
                    container_voto
                }
            };
        }

        private void CarregarClientesVotacoes(Votacao votacao)
        {
            var txt_nome_cliente = new Label
            {
                Text = TXT_NOME_CLIENTE + votacao.Cliente.Nome,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_data_votacao_cliente = new Label
            {
                Text = TXT_DATA_VOTACAO_CLIENTE + votacao.Data.ToString(@"dd\/MM\/yyyy HH:mm"),
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_email_cliente = new Label
            {
                Text = TXT_EMAIL_CLIENTE + votacao.Cliente.Email,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_telefone_cliente = new Label
            {
                Text = TXT_TELEFONE_CLIENTE + votacao.Cliente.Telefone,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var votos_container = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                ColumnSpacing = 10,
                RowSpacing = 10
            };

            // Crio o numero de linhas suficiente para conter o numero de colunas certo por votos
            for (int i = 0; i < votacao.Votos.Count / NUM_COLUNAS_GRID_VOTOS; i++)
            {
                votos_container.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }
            for (int i = 0; i < NUM_COLUNAS_GRID_VOTOS; i++)
            {
                votos_container.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var estampas = DependencyService.Get<IPersistenceService>().LoadImagesBytes("Estampas");
            int coluna = 0;
            int linha = 0;
            int index_voto_atual = 0;
            for (int i = 0; i < votacao.Votos.Count; i++)
            {
                var estampa_voto = CriarEstampaVoto(estampas.ElementAt(index_voto_atual), votacao.Votos[index_voto_atual]);
                index_voto_atual++;
                votos_container.Children.Add(estampa_voto, coluna, linha);

                if (coluna == (NUM_COLUNAS_GRID_VOTOS - 1))
                {
                    coluna = 0;
                    linha++;
                }
                else
                {
                    coluna++;
                }
            }

            //cria cliente
            var client_votos_container = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = {
                    txt_nome_cliente,
                    txt_data_votacao_cliente,
                    txt_email_cliente,
                    txt_telefone_cliente,
                    votos_container
                    //container_estampa_voto
                }
            };

            //adiciona para container
            clientes_votos_container.Children.Add(
                new Frame
                {
                    Padding = new Thickness(30, 10),
                    OutlineColor = Color.White,
                    Content = client_votos_container
#if __ANDROID__
                ,
                    BackgroundColor = Color.Transparent
#endif
                });
        }

        private StackLayout CriarEstampaVoto(byte[] estampa, bool voto)
        {
            var imagem_estampa = CriarImagem(ImageSource.FromStream(() =>
            {
                return new MemoryStream(estampa);
            }));
            var img_voto = new Image
            {
                Source = voto ? ImageSource.FromResource("VotacaoEstampas.Images.1484898084_Tick_Mark_Dark.png") : ImageSource.FromResource("VotacaoEstampas.Images.deny.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            };
            var txt_voto = new Label
            {
                Text = voto ? TXT_VOTO_POSITIVO : TXT_VOTO_NEGATIVO,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            var container_voto = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Spacing = 20,
                Children = {
                     img_voto,
                     txt_voto
                 }
            };

            return new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = 10,
                Children = {
                    imagem_estampa,
                    container_voto
                }
            };
        }

        private Frame CriarImagem(ImageSource imageSource)
        {
            var img = new Image
            {
                Source = imageSource,
                Aspect = Aspect.Fill
            };
            return new Frame
            {
                Content = img,
                Padding = 5,
#if __ANDROID__
                HeightRequest = Height * .1,
                WidthRequest = HeightRequest,
                BackgroundColor = Color.Transparent,
#else
                HeightRequest = Height * .2,
                WidthRequest = Width * .2,
#endif
                OutlineColor = Color.White
            };
        }

        private void CriarPagina()
        {
            var alturaTela = Height;
            var larguraTela = Width;
            var alturaHeader = alturaTela * .2f;
            var alturaBody = alturaTela * .8f;
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
            var imgExportButton = new Image
            {
                Source = ImageSource.FromResource("VotacaoEstampas.Images.1489554476_export.png"),
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.End,
            };
            imgExportButton.GestureRecognizers.Add(new TapGestureRecognizer(async sender =>
            {
                ExportarRelatorio();
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

            var txt_data_colecao = new Label
            {
                Text = TXT_DATA_COLECAO,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var txt_data_colecao_de = new Label
            {
                Text = TXT_DATA_COLECAO_DE,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_data_colecao_de_input = new Label
            {
                Text = App.UltimaColecao.Data.ToString(@"dd\/MM\/yyyy HH:mm"),
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_data_colecao_para = new Label
            {
                Text = TXT_DATA_COLECAO_PARA,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };
            var txt_data_colecao_ate_input = new Label
            {
                Text = DateTime.Now.ToString(@"dd\/MM\/yyyy HH:mm"),
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var data_container = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 20,
                Children = {
                    txt_data_colecao,txt_data_colecao_de, txt_data_colecao_de_input, txt_data_colecao_para, txt_data_colecao_ate_input
                },
            };
            var txt_titulo_clientes_votos = new Label
            {
                Text = TXT_TITULO_CLIENTES_VOTOS,
                FontSize = FONTE_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center
            };

            var btnRelatorioClientes = new CustomButton
            {
#if __ANDROID__
                Text = "Rel. por Clientes",
#else
                Text = "Relatório por Clientes",
#endif
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                //WidthRequest = larguraBody / 4,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnRelatorioClientes.Clicked += async (sender, eventArgs) =>
            {
                bodyClientes.IsVisible = true;
                bodyEstampas.IsVisible = false;
            };
            var btnRelatorioEstampas = new CustomButton
            {
#if __ANDROID__
                Text = "Rel. por Estampas",
#else
                Text = "Relatório por Estampas",
#endif
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                TextColor = COR_TEXTO_LABELS,
                BorderRadius = 20,
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                //WidthRequest = larguraBody / 4,
                CorBackgroundCustomRed = 30,
                CorBackgroundCustomGreen = 30,
                CorBackgroundCustomBlue = 30
            };
            btnRelatorioEstampas.Clicked += async (sender, eventArgs) =>
            {
                bodyClientes.IsVisible = false;
                bodyEstampas.IsVisible = true;
            };
            var containerMenu = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children = { btnRelatorioEstampas, btnRelatorioClientes }
            };
            var header = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 30,
                HeightRequest = alturaHeader,
                Children = {
                    new StackLayout {
                       Orientation = StackOrientation.Horizontal,
                       HorizontalOptions = LayoutOptions.FillAndExpand,
                       Children = {
                            imgBackButton,
                           txt_titulo_relatorio,
                           imgExportButton
                       }
                   }, data_container, containerMenu, //txt_titulo_clientes_votos
                },
            };

#endregion

#region BodyClientes

            clientes_votos_container = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
            };

            bodyClientes = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
#if __ANDROID__
                Padding = new Thickness(0, 0, 0, 10),
#else
                Padding = new Thickness(30, 0, 0, 30),
#endif
                HeightRequest = alturaBody,
                Content = clientes_votos_container,
                IsVisible = false
            };

#endregion

#region BodyEstampas

            estampas_votos_container = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
            };

            bodyEstampas = new ScrollView
            {
                Orientation = ScrollOrientation.Vertical,
#if __ANDROID__
                Padding = new Thickness(0, 0, 0, 10),
#else
                Padding = new Thickness(30, 0, 0, 30),
#endif
                HeightRequest = alturaBody,
                Content = estampas_votos_container,
                //IsVisible = false
            };

#endregion

            BackgroundColor = COR_BACKGROUND_TELA;
            Padding = new Thickness(padding_pagina);
            Content = new StackLayout
            {
                //Padding = new Thickness(paddingTela),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 30,
                HorizontalOptions = LayoutOptions.Center,
                Children = {
                   /*new StackLayout {
                       Orientation = StackOrientation.Horizontal,
                       HorizontalOptions = LayoutOptions.FillAndExpand,
                       Children = {
                           imgBackButton,
                           imgExportButton
                       }
                   }*/
                    header,
                    bodyClientes,
                    bodyEstampas
                },
            };
        }

        private async void ExportarRelatorio()
        {
#if __ANDROID__
            Navigation.PushAsync(new RelatorioPageDroid());
#else
            bool res = await DisplayAlert("Exportar relatório", 
                "1 - Pressione o botão \"Copiar caminho da pasta\";\n" +
                "2 - Abra o Windows Explorer (Fora da aplicação);\n" +
                "3 - Clique na barra de caminho de pasta (superior) e pressione Ctrl + V. Pressione Enter.\n" +
                "4 - Execute o arquivo YfanReports.exe como administrador da máquina.\n" + 
                "5 - O relatório será gerado na pasta corrente.", "Copiar caminho da pasta", "Cancelar");
            if (res)
            {
                DependencyService.Get<IPersistenceService>().CopyLocalFolderToClipboard();
            }
#endif
        }
    }
}
