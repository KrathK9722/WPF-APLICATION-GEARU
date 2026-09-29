using System.Windows;
using System.Windows.Controls;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class CardVision : UserControl
    {
        // 1. Registro da propriedade do Título
        public static readonly DependencyProperty CardUserProperty =
            DependencyProperty.Register(
                nameof(CardUser),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Usuário: Erro"));

        public string CardUser
        {
            get => (string)GetValue(CardUserProperty);
            set => SetValue(CardUserProperty, value);
        }

        // 2. Registro da propriedade da Descrição
        public static readonly DependencyProperty CardEmailProperty =
            DependencyProperty.Register(
                nameof(CardEmail),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Email: Sem Email Cadastrado"));

        public string CardEmail
        {
            get => (string)GetValue(CardEmailProperty);
            set => SetValue(CardEmailProperty, value);
        }

        // 3. Registro da propriedade da Imagem
        public static readonly DependencyProperty CardImageSourceProperty =
            DependencyProperty.Register(
                nameof(CardImageSource),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata(GlobalFunctions.getImage(0)));

        public string CardImageSource
        {
            get => (string)GetValue(CardImageSourceProperty);
            set => SetValue(CardImageSourceProperty, value);
        }

        public CardVision()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CardUserTypeProperty =
            DependencyProperty.Register(
                nameof(CardUserType),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Usuário: Erro"));

        public string CardUserType
        {
            get => (string)GetValue(CardUserTypeProperty);
            set => SetValue(CardUserTypeProperty, value);
        }
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Card '{CardUser}' foi clicado!");
            e.Handled = true;
        }
    }
}