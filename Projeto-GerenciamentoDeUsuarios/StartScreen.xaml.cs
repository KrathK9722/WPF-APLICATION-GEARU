using MySql.Data.MySqlClient;
using Mysqlx.Expr;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Tls;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class StartScreen : Window
    {

        // ARMAZENAR VALORES
        public static int ID;
        public string email;
        public string user;
        public static bool IsAdmin;

        private string archivePath = MainWindow.archivePath;

        // VERIFICAR TELAS ABERTAS
        public static bool create_user_opened = false;
        public static bool edit_user_opened = false;
        public static bool remove_user_opened = false;

        public static bool exit = false;
            
        // CONEXÃO
        public static string connectionString = GlobalFunctions.connectionString;
        public static MySqlConnection Connection { get; set; }
        private DispatcherTimer onlineTimer;

        // NÚMEROS DE USUÁRIOS REGISTRADOS
        public int registerNumber = GlobalFunctions.verNumeroRegistros();

        // CÓDIGO
        public StartScreen()
        {
            InitializeComponent();
            exit = false;
            if (ID == 0)
            {
                string savedIdText = File.ReadAllText(archivePath);
                ID = Convert.ToInt32(savedIdText);
            }

            email = GlobalFunctions.ReturnEmail(ID);
            user = GlobalFunctions.ReturnUser(ID);
            IsAdmin = GlobalFunctions.ReturnIsAdmin(ID);

            StartOnlineTimer();
            SetScreen();
        }


        // ==========================================================
        // CONTROLE DAS TELAS
        // ==========================================================

        private void SetScreen()
        {
            string config = GlobalFunctions.ReturnConfig(ID);
            if (config == "DarkMode:false SaveLogin:true")
            {
                BotaoDarkMode.IsChecked = false;
                BotaoSaveLogin.IsChecked = true;
            }
            else if (config == "DarkMode:true SaveLogin:true")
            {
                BotaoDarkMode.IsChecked = true;
                BotaoSaveLogin.IsChecked = true;
            }
            else if (config == "DarkMode:true SaveLogin:false")
            {
                BotaoDarkMode.IsChecked = true;
                BotaoSaveLogin.IsChecked = false;
            }
            else
            {
                BotaoDarkMode.IsChecked = false;
                BotaoSaveLogin.IsChecked = false;
            }
            GlobalFunctions.UpdateLastActive(ID);
            StartOnlineTimer();
            title_landing_page.Text = $"Bem vindo ao Sistema GEARU, {user}";
            ShowScreen(landing_page);
            viewCard();
        }


        private void ShowScreen(Grid screen)
        {
            // Todas as telas ficam escondidas
            register_screen.Visibility = Visibility.Collapsed;
            edit_screen.Visibility = Visibility.Collapsed;
            remove_screen.Visibility = Visibility.Collapsed;
            landing_page.Visibility = Visibility.Collapsed;

            // Mostra somente a tela escolhida
            screen.Visibility = Visibility.Visible;

            int imageNumber = GlobalFunctions.ReturnImage(ID);

            profile_image.ImageSource = new BitmapImage(
                new Uri(
                    $"pack://application:,,,/ProfileImage/{imageNumber}.png",
                    UriKind.Absolute
                )
            );

            profile_text.Text = $"Perfil de {user}";
        }

        private void menu_button_click(object sender, RoutedEventArgs e)
        {
            ShowScreen(landing_page);
            viewCard();
        }


        // ==========================================================
        // MOSTRAR CARDS DE CASA
        // ==========================================================

        public void viewCard()

        {
            int idBanco = 0;
            int cardsCriados = 0;
            ContainerDeCards.Children.Clear();

            while (cardsCriados < GlobalFunctions.verNumeroRegistros())
            {
                idBanco += 1;
                if (GlobalFunctions.ReturnUser(idBanco) == null)
                {
                    continue;
                }
                if (cardsCriados < GlobalFunctions.verNumeroRegistros())
                {
                    string userType = "Usuário";
                    int imageNumber = GlobalFunctions.ReturnImage(idBanco);
                    if (GlobalFunctions.ReturnIsAdmin(idBanco) == true)
                    {
                        userType = "Admin";
                    }
                    string archivePath = GlobalFunctions.getImage(imageNumber);
                    CardVision novoCard = new CardVision();
                    novoCard.CardUser = $"Usuário: {GlobalFunctions.ReturnUser(idBanco)}";
                    novoCard.CardEmail = $"Email: {GlobalFunctions.ReturnEmail(idBanco)}";
                    novoCard.CardImageSource = $"{archivePath}";
                    novoCard.CardUserType = $"{userType}";
                    novoCard.CardFullName = $"{GlobalFunctions.ReturnFullName(idBanco)}";
                    novoCard.CardAccountBanned = GlobalFunctions.ReturnIsBanned(idBanco) ? "Conta Banida" : "Conta Ativa";
                    novoCard.CardAccountStatus = GlobalFunctions.ReturnIsOnline(idBanco) ? "Online" : "Offline";
                    novoCard.CardOnlineColor = GlobalFunctions.ReturnIsOnline(idBanco) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22C55E")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CA3AF"));
                    if (GlobalFunctions.ReturnLastActive(idBanco) == "")
                    {
                        novoCard.CardLastTimeActive = "Ainda sem atividade";
                    }
                    else
                    {
                        novoCard.CardLastTimeActive = GlobalFunctions.ReturnIsOnline(idBanco) ? "Online Agora" : $"Última vez ativo: {GlobalFunctions.ReturnLastActive(idBanco)}";
                    }
                    
                    novoCard.Width = 280;
                    novoCard.Margin = new Thickness(13.5);

                    ContainerDeCards.Children.Add(novoCard);
                    cardsCriados += 1;
                }
            }
        }

        // ==========================================================
        // INÍCIO
        // ==========================================================

        private void start_click(object sender, RoutedEventArgs e)
        {
            if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
            {
                exit = true;
                MessageBox.Show("Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }
            viewCard();
            ShowScreen(landing_page);
        }



        // ==========================================================
        // REGISTRAR USUÁRIO
        // ==========================================================

        private void register_click(object sender, RoutedEventArgs e)
        {
            if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
            {
                exit = true;
                MessageBox.Show("`Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }
            ShowScreen(register_screen);
        }
        private void create_user_click(object sender, RoutedEventArgs e)
        {
            if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
            {
                exit = true;
                MessageBox.Show("`Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }
            if (create_user_opened == true)
            {
                return;
            }
            RegisterNewUser registrarUser = new RegisterNewUser();
            registrarUser.Show();
            RegisterNewUser.AdminID = ID;
            create_user_opened = true;
        }

        // ==========================================================
        // EDITAR
        // ==========================================================

        private void edit_click(object sender, RoutedEventArgs e)
        {
            if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
            {
                exit = true;
                MessageBox.Show("`Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }
            ShowScreen(edit_screen);
        }


        // ==========================================================
        // REMOVER
        // ==========================================================

        private void remove_click(object sender, RoutedEventArgs e)
        {
            if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
            {
                exit = true;
                MessageBox.Show("`Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
                return;
            }
            ShowScreen(remove_screen);
        }
        private void remove_user_button_Click(object sender, RoutedEventArgs e)
        {
            if (remove_user_opened == true)
            {
                return;
            }
            RemoveUser removerUser = new RemoveUser();
            removerUser.Show();
            RemoveUser.AdminID = ID;
            RemoveUser.IsAdmin = IsAdmin;
            remove_user_opened = true;
        }

        // ==========================================================
        // CONFIGURAÇÕES DO SISTEMA
        // ==========================================================

        private void StartOnlineTimer()
        {
            onlineTimer = new DispatcherTimer();

            onlineTimer.Interval = TimeSpan.FromSeconds(30);

            onlineTimer.Tick += (sender, e) => // FUNÇÃO LAMBDA = FUNÇÃO CURTA, PODIA FAZER COM private void OnlineTimer(sender, e){} MAS ASSIM FICA MAIS FACIL E LIMPO
            {
                if (GlobalFunctions.ReturnUser(ID) == null && exit == false)
                {
                    exit = true;
                    MessageBox.Show("`Saida repentina do sistema. Causa: Conta Excluida ou Desativada", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ID = 0;
                    Environment.Exit(0);
                    return;
                }
                GlobalFunctions.UpdateLastActive(ID);
            };

            onlineTimer.Start();
        }
        private void BotaoDarkMode_Checked(object sender, RoutedEventArgs e)
        {
            this.Resources["WindowBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#202020"));
            this.Resources["SidebarBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
            this.Resources["SidebarHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D2D"));
            this.Resources["SidebarSelected"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#26384F"));
            this.Resources["PrimaryBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("DarkSlateBlue"));
            this.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2B2B2B"));
            this.Resources["TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3F3F3"));
            this.Resources["TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#AAAAAA"));
            this.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF404040"));
            this.Resources["ButtonText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            this.Resources["ButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Gray"));
            Option_Changed();
        }

        private void BotaoDarkMode_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Resources["WindowBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F7F7F7"));
            this.Resources["SidebarBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F2F2F2"));
            this.Resources["SidebarHover"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8E8E8"));
            this.Resources["SidebarSelected"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCEBFF"));
            this.Resources["PrimaryBlue"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0067C0"));
            this.Resources["BorderColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1E1E1"));
            this.Resources["TextPrimary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
            this.Resources["TextSecondary"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#616161"));
            this.Resources["ButtonBackground"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            this.Resources["ButtonText"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Black"));
            this.Resources["ButtonBorder"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("DarkGray"));
            Option_Changed();
        }
        private void BotaoSaveLogin_Checked(object sender, RoutedEventArgs e)
        {
            //Salvar Sessão
            string content = ID.ToString();
            File.SetAttributes(archivePath, FileAttributes.Normal);
            File.WriteAllText(archivePath, content);
            File.SetAttributes(archivePath, FileAttributes.Hidden);
            Option_Changed();
        }
        private void BotaoSaveLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            //Salvar Sessão
            int value = 0;

            string content = value.ToString();
            File.SetAttributes(archivePath, FileAttributes.Normal);
            File.WriteAllText(archivePath, content);
            File.SetAttributes(archivePath, FileAttributes.Hidden);
            Option_Changed();
        }

        private void Option_Changed()
        {
            bool darkmode = false;
            bool savelogin = false;
            if (BotaoDarkMode.IsChecked == true)
            {
                darkmode = true;
            }
            if (BotaoSaveLogin.IsChecked == true)
            {
                savelogin = true;
            }

            GlobalFunctions.changeConfig(darkmode, savelogin, ID);
        }

        // ==========================================================
        // SAIR
        // ==========================================================
        private void exit_click(object sender, RoutedEventArgs e)
        {
                Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Deseja realmente sair?",
                "Sair",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Login_Screen login_screen = new Login_Screen();
                if (BotaoSaveLogin.IsChecked == false)
                {
                    if (create_user_opened)
                    {
                        RegisterNewUser registerUser = new RegisterNewUser();
                        registerUser.Close();
                    }
                    if (onlineTimer != null)
                    {
                        onlineTimer.Stop();
                    }
                    ID = 0;
                    exit = true;
                    login_screen.Show();
                }
                else
                {
                    if (onlineTimer != null)
                    {
                        onlineTimer.Stop();
                    }
                    ID = 0;
                    Environment.Exit(0);
                }
            }
            else
            {
                e.Cancel = true;
            }

        }
    }
}