using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using VotacaoEstampas.CustomControls;
using VotacaoEstampas.Pages;
using Xamarin.Forms;

namespace VotacaoEstampas
{
    public class IniciarPesquisaPage : BaseContentPage
    {
        //definir em style, capitulo 12 do xamarin book
        private readonly double FONTE_TEXTO_LABELS = Device.GetNamedSize(NamedSize.Medium, typeof(Entry));
        private readonly Color COR_TEXTO_LABELS = Color.White;
        private readonly string FAMILIA_TEXTO_LABELS = "Times New Roman";
        private readonly FontAttributes ATRIBUTOS_TEXTO_LABELS = FontAttributes.Bold;
        private readonly Color COR_BACKGROUND_TELA = Color.FromRgb(100, 100, 100);
        private readonly Color COR_BACKGROUND_BUTTON = Color.FromRgb(30, 30, 30);

        // variaveis
        bool pagCarregada = false;

        // componentes
        Image logo;
        CustomEntry txtNome;
        CustomEntry txtEmail;
        CustomEntry txtFone;
        
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var logoSalva = DependencyService.Get<IPersistenceService>().LoadImage("logo.jpg");
            if (logoSalva != null)
            {
                logo.Source = ImageSource.FromStream(() =>
                {
                    return logoSalva;
                });
            }
            else
            {
                logo.Source = ImageSource.FromResource("VotacaoEstampas.Images.logo_white_512.png");
            }

            var corSalva = DependencyService.Get<IPersistenceService>().LoadConfig();
            if (corSalva != null)
            {
                var corSalvaTxt = BuscarCorPorNome(corSalva);
                BackgroundColor = Color.FromRgb(corSalvaTxt.R, corSalvaTxt.G, corSalvaTxt.B);
            }

            //reseta campos de input
            if (txtNome != null)
                txtNome.Text = "";
                //txtNome.Text = "Jeff";
            if (txtEmail != null)
                txtEmail.Text = "";
                //txtEmail.Text = "jdfid@asdsda.com";
            if (txtFone != null)
                txtFone.Text = "";
                //txtFone.Text = "(44)4444444444";
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
            var alturaTela = Height;
            var paddingTela = alturaTela * .05f;
            var alturaLogo = alturaTela * .65f;
            var alturaMarginTopTextos = alturaTela * .1f;
            var alguraTextos = alturaTela * .3f;

            Padding = new Thickness(0,0,0,paddingTela);
            BackgroundColor = COR_BACKGROUND_TELA;

            // instancia de objetos da pagina
            logo = new Image
            {
                //Source = ImageSource.FromResource("VotacaoEstampas.Images.logo_white_512.png"),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = alturaLogo,
                WidthRequest = alturaLogo
            };

            txtNome = new CustomEntry
            {
                Placeholder = "Nome:",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                PlaceholderColor = COR_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS, 
                HeightRequest = FONTE_TEXTO_LABELS + 30,
                Keyboard = Keyboard.Text
            };
            txtNome.Completed += (object sender, EventArgs e) => { txtEmail.Focus(); };

            txtEmail = new CustomEntry
            {
                Placeholder = "Email:",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                PlaceholderColor = COR_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                Keyboard = Keyboard.Email
            };
            txtEmail.Completed += (object sender, EventArgs e) => { txtFone.Focus(); };

            txtFone = new CustomEntry
            {
                Placeholder = "Fone:",
                FontSize = FONTE_TEXTO_LABELS,
                FontFamily = FAMILIA_TEXTO_LABELS,
                FontAttributes = ATRIBUTOS_TEXTO_LABELS,
                PlaceholderColor = COR_TEXTO_LABELS,
                TextColor = COR_TEXTO_LABELS,
                Keyboard = Keyboard.Numeric,
            };
            txtFone.TextChanged += TxtFone_TextChanged;

            var btnIniciarPesquisa = new CustomButton
            {
                Text = "Iniciar Pesquisa",
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
            txtFone.Completed += (object sender, EventArgs e) => { btnIniciarPesquisa.Focus(); };
            btnIniciarPesquisa.Clicked += VerificarUser;
                
            var containerBtn = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Padding = new Thickness(40,0),
                Children = {
                    btnIniciarPesquisa
                }
            };

            // set conteudo da página
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 20,
                Children = {
                   logo, txtNome, txtEmail, txtFone, containerBtn
                }
            };
        }

        private void VerificarUser(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNome.Text) || string.IsNullOrEmpty(txtEmail.Text) || string.IsNullOrEmpty(txtFone.Text))
            {
                DisplayAlert("Por favor preencha os campos", "Olá! Precisamos de sua identificação para iniciar a pesquisa.", "Certo");
                return;
            }

            var patternEmail = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            var regexEmail = new Regex(patternEmail);
            if (!regexEmail.IsMatch(txtEmail.Text))
            {
                DisplayAlert("Por favor preencha o e-mail", "Olá! Precisamos do seu e-mail informado corretamente.", "Certo");
                return;
            }

            var patternFone = @"^[(]\d{2}[)]\d{8,}$";
            var regexFone = new Regex(patternFone);
            if (!regexFone.IsMatch(txtFone.Text))
            {
                DisplayAlert("Por favor preencha o telefone", "Olá! Precisamos do seu telefone com DDD.", "Certo");
                return;
            }

            //cria nova votacao
            List<bool> votos = new List<bool>();
            foreach (var estampa in App.UltimaColecao.Estampas)
            {
                //inicia votacao com tudo nao votado
                votos.Add(false);
            }
            App.VotacaoAtual = new Model.Votacao() {
                Data = DateTime.Now,
                Cliente = new Model.Cliente {
                    Nome = txtNome.Text,
                    Email = txtEmail.Text,
                    Telefone = txtFone.Text
                },
                Votos = votos
            };

            Navigation.PushAsync(new VotarEstampaPage());
        }

        private void TxtFone_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = sender as Entry;

            //se ta apagando, nao faz nada
            if (e.OldTextValue != null)
                if (e.NewTextValue.Length < e.OldTextValue.Length)
                    return;

            //coloca parenteses
            if (e.NewTextValue.Length == 1)
                text.Text = "(" + e.NewTextValue;

            //coloca parenteses
            if (e.OldTextValue != null)
                if (e.OldTextValue.Length == 3)
                    text.Text = e.OldTextValue + ")" + e.NewTextValue.Replace(e.OldTextValue, "");
        }
    }
}
