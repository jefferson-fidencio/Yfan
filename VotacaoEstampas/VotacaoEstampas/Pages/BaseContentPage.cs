using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace VotacaoEstampas.Pages
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
