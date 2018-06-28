using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VotacaoEstampas.CustomControls
{
    public class CustomButton : Button
    {
        public int CorBackgroundCustomRed { get; internal set; }
        public int CorBackgroundCustomGreen { get; internal set; }
        public int CorBackgroundCustomBlue { get; internal set; }
    }
}
