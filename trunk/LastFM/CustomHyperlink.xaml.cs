using System.Windows;
using System.Windows.Data;

namespace LastFM
{
    public partial class CustomHyperlink
    {
        public CustomHyperlink()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        static readonly DependencyProperty AddressProperty = DependencyProperty.Register("Address", typeof(string), typeof(CustomHyperlink));
        public string Address
        {
            get
            {
                return (string)base.GetValue(AddressProperty);
            }
            set
            {
                base.SetValue(AddressProperty, value);
            }
        }

        static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomHyperlink));
        public string Text
        {
            get
            {
                return (string)base.GetValue(TextProperty);
            }
            set
            {
                base.SetValue(TextProperty, value);
            }
        }

      
        private void TextBlock_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //string address = System.Web.HttpUtility.UrlEncode(Address, System.Text.Encoding.UTF8);
            System.Diagnostics.Process.Start(Address);
        }
    }
    public sealed class ArtistToAddressConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string artist = (string)value;
            return string.Format("http://last.fm/music/{0}", System.Web.HttpUtility.UrlEncode(artist, System.Text.Encoding.UTF8));
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

}