using System.Windows;
using System.Windows.Controls;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class CardVision : UserControl
    {
        // 1. Registro da propriedade do Título
        public static readonly DependencyProperty CardTitleProperty =
            DependencyProperty.Register(
                nameof(CardTitle),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Título Padrão"));

        public string CardTitle
        {
            get => (string)GetValue(CardTitleProperty);
            set => SetValue(CardTitleProperty, value);
        }

        // 2. Registro da propriedade da Descrição
        public static readonly DependencyProperty CardDescriptionProperty =
            DependencyProperty.Register(
                nameof(CardDescription),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Descrição padrão do card."));

        public string CardDescription
        {
            get => (string)GetValue(CardDescriptionProperty);
            set => SetValue(CardDescriptionProperty, value);
        }

        // 3. Registro da propriedade da Imagem
        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register(
                nameof(CardImageSource),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("https://via.placeholder.com/150"));

        public string CardImageSource
        {
            get => (string)GetValue(CardImageSourceProperty);
            set => SetValue(CardImageSourceProperty, value);
        }

        public CardVision()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CardAreaProperty =
            DependencyProperty.Register(
                nameof(CardArea),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Area padrão do card."));

        public string CardArea
        {
            get => (string)GetValue(CardAreaProperty);
            set => SetValue(CardAreaProperty, value);
        }
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Card '{CardTitle}' foi clicado!");
            e.Handled = true;
        }
    }
}