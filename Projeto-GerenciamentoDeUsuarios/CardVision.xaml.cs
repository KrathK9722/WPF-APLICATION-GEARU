using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                new PropertyMetadata("Erro"));

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
                new PropertyMetadata("Sem Email Cadastrado"));

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
                new PropertyMetadata("Erro de visualização"));

        public string CardUserType
        {
            get => (string)GetValue(CardUserTypeProperty);
            set => SetValue(CardUserTypeProperty, value);
        }

        public static readonly DependencyProperty CardLastTimeActiveProperty =
            DependencyProperty.Register(
                nameof(CardLastTimeActive),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Sem Atividade"));

        public string CardLastTimeActive
        {
            get => (string)GetValue(CardLastTimeActiveProperty);
            set => SetValue(CardLastTimeActiveProperty, value);
        }

        public static readonly DependencyProperty CardAccountStatusProperty =
            DependencyProperty.Register(
                nameof(CardAccountStatus),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Erro de Status"));

        public string CardAccountStatus
        {
            get => (string)GetValue(CardAccountStatusProperty);
            set => SetValue(CardAccountStatusProperty, value);
        }

        public static readonly DependencyProperty CardAccountBannedProperty =
            DependencyProperty.Register(
                nameof(CardAccountBanned),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Erro de Status"));

        public string CardAccountBanned
        {
            get => (string)GetValue(CardAccountBannedProperty);
            set => SetValue(CardAccountBannedProperty, value);
        }

        public static readonly DependencyProperty CardFullNameProperty =
            DependencyProperty.Register(
                nameof(CardFullName),
                typeof(string),
                typeof(CardVision),
                new PropertyMetadata("Sem Nome cadastrado"));

        public string CardFullName
        {
            get => (string)GetValue(CardFullNameProperty);
            set => SetValue(CardFullNameProperty, value);
        }

        public static readonly DependencyProperty CardOpenClickVisibilityProperty =
        DependencyProperty.Register(
        nameof(CardOpenClickVisibility),
        typeof(Visibility),
        typeof(CardVision),
        new PropertyMetadata(Visibility.Visible));

        public Visibility CardOpenClickVisibility
        {
            get => (Visibility)GetValue(CardOpenClickVisibilityProperty);
            set => SetValue(CardOpenClickVisibilityProperty, value);
        }

        public static readonly DependencyProperty CardOnlineColorProperty =
            DependencyProperty.Register(
            nameof(CardOnlineColor),
            typeof(Brush),
            typeof(CardVision),
            new PropertyMetadata(Brushes.Gray));

        public Brush CardOnlineColor
        {
            get => (Brush)GetValue(CardOnlineColorProperty);
            set => SetValue(CardOnlineColorProperty, value);
        }


        public static readonly DependencyProperty CardCloseClickVisibilityProperty =
        DependencyProperty.Register(
        nameof(CardCloseClickVisibility),
        typeof(Visibility),
        typeof(CardVision),
        new PropertyMetadata(Visibility.Collapsed));

        public Visibility CardCloseClickVisibility
        {
            get => (Visibility)GetValue(CardCloseClickVisibilityProperty);
            set => SetValue(CardCloseClickVisibilityProperty, value);
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {

            e.Handled = true;
            CardVision cardCopia = new CardVision
            {
                CardUser = this.CardUser,
                CardEmail = this.CardEmail,
                CardImageSource = this.CardImageSource,
                CardUserType = this.CardUserType,
                CardAccountStatus = this.CardAccountStatus,
                CardFullName = this.CardFullName,
                CardOpenClickVisibility = Visibility.Collapsed,
                CardCloseClickVisibility = Visibility.Visible,

                Margin = new Thickness(0)
            };

            CardWindow popup = new CardWindow(cardCopia);

            popup.ShowDialog();
        }
        private void Card_Close_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);

            // Se ela existir, fecha ela imediatamente
            if (parent != null)
            {
                parent.Close();
            }
        }

    }
}