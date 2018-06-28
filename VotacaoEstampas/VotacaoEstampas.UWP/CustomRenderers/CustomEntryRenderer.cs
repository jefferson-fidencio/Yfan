
using VotacaoEstampas.CustomControls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(VotacaoEstampas.UWP.CustomRenderers.CustomEntryRenderer))]
namespace VotacaoEstampas.UWP.CustomRenderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                var nativePhoneTextBox = (FormsCustomizableTextBox)(Control.Content as Windows.UI.Xaml.Controls.Grid).Children[1];
                nativePhoneTextBox.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 30, 30, 30));
                nativePhoneTextBox.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 30, 30, 30));
                //nativePhoneTextBox.Height = 100;
                nativePhoneTextBox.Margin = new Windows.UI.Xaml.Thickness(5);
                //nativePhoneTextBox.BorderThickness = new Windows.UI.Xaml.Thickness(0);

                var border = new Border();
                border.CornerRadius = new CornerRadius(15);
                /*border.BorderThickness = new Windows.UI.Xaml.Thickness(1);
                border.BorderBrush = new SolidColorBrush(Colors.Brown);*/
                border.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255,30,30,30));
                //border.Margin = new Windows.UI.Xaml.Thickness(10);

                var parent = Control.Content as Windows.UI.Xaml.Controls.Grid;
                if (parent != null)
                {
                    parent.Children.Remove(nativePhoneTextBox);
                    parent.Children.Add(border);
                    border.Child = nativePhoneTextBox;
                }
            }
        }
    }
}
