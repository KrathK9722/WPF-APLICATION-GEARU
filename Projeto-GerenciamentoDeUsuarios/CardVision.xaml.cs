using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class CardVision : UserControl
    {
        private bool editing = false;

        public bool IsEditing => editing;

        public event Action? UserUpdated;

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
                UserId = this.UserId,

                CardUser = this.CardUser,
                CardEmail = this.CardEmail,
                CardImageSource = this.CardImageSource,
                CardUserType = this.CardUserType,
                CardAccountStatus = this.CardAccountStatus,
                CardAccountBanned = this.CardAccountBanned,
                CardOnlineColor = this.CardOnlineColor,
                CardLastTimeActive = this.CardLastTimeActive,
                CardFullName = this.CardFullName,

                // Esconde visualizar e mostra fechar
                CardOpenClickVisibility = Visibility.Collapsed,
                CardCloseClickVisibility = Visibility.Visible,

                // Mostra editar apenas para administradores
                CardEditVisibility =
                    GlobalFunctions.ReturnIsAdmin(StartScreen.ID)
                    ? Visibility.Visible
                    : Visibility.Collapsed,

                Margin = new Thickness(0)
            };

            CardWindow popup = new CardWindow(cardCopia);
            cardCopia.UserUpdated += () =>
            {
                UserUpdated?.Invoke();
            };
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

        // EDIT USER IN CARD
        public int UserId { get; set; }

        // VISIBILIDADE DO BOTÃO EDITAR
        public static readonly DependencyProperty CardEditVisibilityProperty =
            DependencyProperty.Register(
                nameof(CardEditVisibility),
                typeof(Visibility),
                typeof(CardVision),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility CardEditVisibility
        {
            get => (Visibility)GetValue(CardEditVisibilityProperty);
            set => SetValue(CardEditVisibilityProperty, value);
        }

        // CLIQUE NO BOTÃO EDITAR

        private void CardEditButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (!GlobalFunctions.ReturnIsAdmin(StartScreen.ID) ||
                GlobalFunctions.ReturnIsBanned(StartScreen.ID))
            {
                MessageBox.Show("Você não possui permissão para editar usuários.");
                return;
            }

            string? username = GlobalFunctions.ReturnUser(UserId);

            if (username == null)
            {
                MessageBox.Show("Usuário não encontrado.");
                return;
            }

            // Carrega os valores diretamente do banco
            CardEditUser.Text = username;
            CardEditFullName.Text =
                GlobalFunctions.ReturnFullName(UserId) ?? "";

            CardEditEmail.Text =
                GlobalFunctions.ReturnEmail(UserId) ?? "";

            CardEditType.SelectedIndex =
                GlobalFunctions.ReturnIsAdmin(UserId) ? 1 : 0;

            // Impede alteração da própria permissão
            CardEditType.IsEnabled = UserId != StartScreen.ID;

            editing = true;

            // Substitui textos pelos campos editáveis
            CardUserText.Visibility = Visibility.Collapsed;
            CardFullNameText.Visibility = Visibility.Collapsed;
            CardEmailText.Visibility = Visibility.Collapsed;
            CardTypeText.Visibility = Visibility.Collapsed;

            CardEditUser.Visibility = Visibility.Visible;
            CardEditFullName.Visibility = Visibility.Visible;
            CardEditEmail.Visibility = Visibility.Visible;
            CardEditType.Visibility = Visibility.Visible;

            // Troca os botões
            CardButton.Visibility = Visibility.Collapsed;
            CardEditButton.Visibility = Visibility.Collapsed;
            CardCloseButton.Visibility = Visibility.Collapsed;

            CardCancelEditButton.Visibility = Visibility.Visible;
            CardSaveEditButton.Visibility = Visibility.Visible;

            // Mostra o botão de banimento
            CardBanButton.Visibility = Visibility.Visible;
            UpdateBanButton();

            // Aumenta o card para acomodar os campos
            MinHeight = 500;
            Height = 640;
        }

        private void CardCancelEditButton_Click(
            object sender, RoutedEventArgs e)
        {
            CancelCardEdition();
        }

        private void CancelCardEdition()
        {
            editing = false;

            CardUserText.Visibility = Visibility.Visible;
            CardFullNameText.Visibility = Visibility.Visible;
            CardEmailText.Visibility = Visibility.Visible;
            CardTypeText.Visibility = Visibility.Visible;

            CardEditUser.Visibility = Visibility.Collapsed;
            CardEditFullName.Visibility = Visibility.Collapsed;
            CardEditEmail.Visibility = Visibility.Collapsed;
            CardEditType.Visibility = Visibility.Collapsed;

            CardBanButton.Visibility = Visibility.Collapsed;

            CardCancelEditButton.Visibility = Visibility.Collapsed;
            CardSaveEditButton.Visibility = Visibility.Collapsed;

            CardButton.Visibility = CardOpenClickVisibility;
            CardEditButton.Visibility = CardEditVisibility;

            CardCloseButton.Visibility = Visibility.Visible;

            MinHeight = 420;
            Height = double.NaN;
        }

        private void CardSaveEditButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (!editing)
                return;

            string fullname = CardEditFullName.Text.Trim();
            string email = CardEditEmail.Text.Trim();
            string username = CardEditUser.Text.Trim();

            bool makeAdmin = CardEditType.SelectedIndex == 1;

            if (string.IsNullOrWhiteSpace(fullname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            if (fullname.Length < 2 || fullname.Length > 150)
            {
                MessageBox.Show("Nome completo inválido.");
                return;
            }

            if (!new EmailAddressAttribute().IsValid(email) ||
                email.Length > 254)
            {
                MessageBox.Show("Digite um e-mail válido.");
                return;
            }

            if (username.Length < 3 ||
                username.Length > 30 ||
                username.Any(char.IsWhiteSpace))
            {
                MessageBox.Show(
                    "O usuário deve ter entre 3 e 30 caracteres, sem espaços.");
                return;
            }

            int result = GlobalFunctions.UpdateManagedUser(
                StartScreen.ID,
                UserId,
                fullname,
                email,
                username,
                makeAdmin
            );

            switch (result)
            {
                case 0:
                    MessageBox.Show("Usuário atualizado com sucesso!");

                    CancelCardEdition();

                    // Atualiza a StartScreen
                    UserUpdated?.Invoke();
                    break;

                case 100:
                    MessageBox.Show("E-mail já cadastrado.");
                    break;

                case 101:
                    MessageBox.Show("Usuário já cadastrado.");
                    break;

                case 102:
                    MessageBox.Show("Usuário não encontrado.");
                    break;

                case 105:
                    MessageBox.Show("Você não tem permissão para esta alteração.");
                    break;

                case 106:
                    MessageBox.Show(
                        "Você não pode remover sua própria permissão administrativa.");
                    break;

                default:
                    MessageBox.Show("Erro ao atualizar usuário.");
                    break;
            }
        }

        private void UpdateBanButton()
        {
            bool banned = GlobalFunctions.ReturnIsBanned(UserId);

            if (banned)
            {
                BanButtonText.Text = "Desbanir usuário";
                BanButtonIcon.Text = "✓";
                CardBanButton.Background = Brushes.ForestGreen;
            }
            else
            {
                BanButtonText.Text = "Banir usuário";
                BanButtonIcon.Text = "🚩";
                CardBanButton.Background = Brushes.Firebrick;
            }

            CardBanButton.IsEnabled = UserId != StartScreen.ID;
        }

        private string? RequestAdminPassword(bool ban)
        {
            Window dialog = new Window
            {
                Title = "Confirmação administrativa",
                Width = 380,
                Height = 230,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Window.GetWindow(this),
                Background = Brushes.White
            };

            StackPanel panel = new StackPanel
            {
                Margin = new Thickness(25)
            };

            TextBlock title = new TextBlock
            {
                Text = ban ? "Confirmar banimento" : "Confirmar desbanimento",
                FontWeight = FontWeights.SemiBold,
                FontSize = 18,
                Margin = new Thickness(0, 0, 0, 12)
            };

            TextBlock description = new TextBlock
            {
                Text = "Digite sua senha de administrador:",
                Margin = new Thickness(0, 0, 0, 8)

            };

            PasswordBox passwordBox = new PasswordBox
            {
                Height = 34,
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 15)
            };

            StackPanel buttons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            Button cancel = new Button
            {
                Content = "Cancelar",
                Width = 100,
                Height = 32,
                Margin = new Thickness(0, 0, 8, 0)
            };

            Button confirm = new Button
            {
                Content = "Confirmar",
                Width = 100,
                Height = 32,
                Background = Brushes.Firebrick,
                Foreground = Brushes.White
            };

            string? password = null;

            cancel.Click += (s, e) =>
            {
                dialog.DialogResult = false;
            };

            confirm.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    MessageBox.Show("Digite sua senha.");
                    return;
                }

                password = passwordBox.Password;
                dialog.DialogResult = true;
            };

            buttons.Children.Add(cancel);
            buttons.Children.Add(confirm);

            panel.Children.Add(title);
            panel.Children.Add(description);
            panel.Children.Add(passwordBox);
            panel.Children.Add(buttons);

            dialog.Content = panel;

            dialog.Loaded += (s, e) => passwordBox.Focus();

            return dialog.ShowDialog() == true ? password : null;
        }

        private void CardBanButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (!GlobalFunctions.ReturnIsAdmin(StartScreen.ID) ||
                GlobalFunctions.ReturnIsBanned(StartScreen.ID))
            {
                MessageBox.Show("Acesso negado.");
                return;
            }

            if (UserId == StartScreen.ID)
            {
                MessageBox.Show("Você não pode banir sua própria conta.");
                return;
            }

            bool currentlyBanned = GlobalFunctions.ReturnIsBanned(UserId);
            bool ban = !currentlyBanned;

            string? password = RequestAdminPassword(ban);

            if (password == null)
                return;

            int result = GlobalFunctions.SetUserBanStatus(
                StartScreen.ID,
                UserId,
                password,
                ban
            );

            switch (result)
            {
                case 0:
                    MessageBox.Show(
                        ban ? "Usuário banido!" : "Usuário desbanido!");

                    CancelCardEdition();
                    UserUpdated?.Invoke();
                    break;

                case 104:
                    MessageBox.Show("Senha de administrador incorreta.");
                    break;

                case 105:
                    MessageBox.Show("Permissão insuficiente.");
                    break;

                case 102:
                    MessageBox.Show("Usuário não encontrado.");
                    break;

                case 107:
                    MessageBox.Show(
                        "O status da conta mudou. Tente novamente.");
                    UpdateBanButton();
                    break;

                default:
                    MessageBox.Show("Erro ao alterar o status da conta.");
                    break;
            }
        }





    }
}