using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class CardImage : UserControl
    {
        private static CardImage selectedCard = null;

        public CardImage()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register(
                nameof(CardImageSource),
                typeof(string),
                typeof(CardImage),
                new PropertyMetadata("")
            );

        public string CardImageSource
        {
            get => (string)GetValue(CardImageSourceProperty);
            set => SetValue(CardImageSourceProperty, value);
        }

        public int ImageValue { get; set; }

        private void Card_Click(object sender, MouseButtonEventArgs e)
        {

            if (selectedCard != null)
            {
                selectedCard.Resources["CardBorder"] = new SolidColorBrush(Color.FromRgb(227, 228, 247));
            }

            selectedCard = this;

            this.Resources["CardBorder"] =
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067C0"));

            if (GlobalFunctions.AdminExist())
            {
                RegisterNewUser.ImageValue = ImageValue;
            }
            else
            {
                MainWindow.ImageValue = ImageValue;
            }
        }
    }
}