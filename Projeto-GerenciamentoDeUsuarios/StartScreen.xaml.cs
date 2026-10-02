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

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class StartScreen : Window
    {

        // ARMAZENAR VALORES
        public static int ID;
        public string email;
        public string user;
        public static bool IsAdmin;
        private static bool SaveLogin = false;
        private bool _isDarkMode = false;

        // VERIFICAR TELAS ABERTAS
        public static bool create_user_opened = false;
        public static bool edit_user_opened = false;
        public static bool remove_user_opened = false;
            
        // CONEXÃO
        public static string connectionString = GlobalFunctions.connectionString;
        public static MySqlConnection Connection { get; set; }

        // NÚMEROS DE USUÁRIOS REGISTRADOS
        public int registerNumber = GlobalFunctions.verNumeroRegistros();

        // CÓDIGO
        public StartScreen()
        {
            InitializeComponent();
            if (ID == 0)
            {
                string archivePath = @"C:\Documentos\GEARU\Tokens\session.txt";
                string savedIdText = File.ReadAllText(archivePath);
                ID = Convert.ToInt32(savedIdText);
            }

            email = GlobalFunctions.ReturnEmail(ID);
            user = GlobalFunctions.ReturnUser(ID);
            IsAdmin = GlobalFunctions.ReturnIsAdmin(ID);

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
            remove_specific_screen.Visibility = Visibility.Collapsed;

            // Mostra somente a tela escolhida
            screen.Visibility = Visibility.Visible;
        }


        // ==========================================================
        // MENU LATERAL
        // ==========================================================

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
            registerNumber = GlobalFunctions.verNumeroRegistros();
            ContainerDeCards.Children.Clear();

            while (cardsCriados < registerNumber)
            {
                idBanco += 1;
                if (GlobalFunctions.ReturnUser(idBanco) == null)
                {
                    continue;
                }
                if (cardsCriados < registerNumber)
                {
                    string userType = "Comum";
                    int imageNumber = GlobalFunctions.ReturnImage(idBanco);
                    if (GlobalFunctions.ReturnIsAdmin(idBanco) == true)
                    {
                        userType = "Admin";
                        imageNumber = 10;
                    }
                    string archivePath = GlobalFunctions.getImage(imageNumber);
                    CardVision novoCard = new CardVision();
                    novoCard.CardUser = $"Usuário: {GlobalFunctions.ReturnUser(idBanco)}";
                    novoCard.CardEmail = $"Email: {GlobalFunctions.ReturnEmail(idBanco)}";
                    novoCard.CardImageSource = $"{archivePath}";
                    novoCard.CardUserType = $"Tipo de Usuário: {userType}";
                    novoCard.CardFullName = $"{GlobalFunctions.ReturnFullName(idBanco)}";
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
            viewCard();
            ShowScreen(landing_page);
        }



        // ==========================================================
        // REGISTRAR USUÁRIO
        // ==========================================================

        private void register_click(object sender, RoutedEventArgs e)
        {
            ShowScreen(register_screen);
        }
        private void create_user_click(object sender, RoutedEventArgs e)
        {
            if (create_user_opened == true)
            {
                return;
            }
            RegisterNewUser registrarUser = new RegisterNewUser();
            registrarUser.Show();
            RegisterNewUser.ID = ID;
            RegisterNewUser.email = email;
            RegisterNewUser.IsAdmin = IsAdmin;
            RegisterNewUser.user = user;
            create_user_opened = true;
        }

        // ==========================================================
        // EDITAR
        // ==========================================================

        private void edit_click(object sender, RoutedEventArgs e)
        {
            ShowScreen(edit_screen);
        }


        // ==========================================================
        // REMOVER
        // ==========================================================

        private void remove_click(object sender, RoutedEventArgs e)
        {
            ShowScreen(remove_screen);
        }
        private void remove_specific_button_Click(object sender, RoutedEventArgs e)
        {
            ShowScreen(remove_specific_screen);
        }

        // ==========================================================
        // CONFIGURAÇÕES DO SISTEMA
        // ==========================================================
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
            string archivePath = @"C:\Documentos\GEARU\Tokens\session.txt";
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
            string archivePath = @"C:\Documentos\GEARU\Tokens\session.txt";

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
                ID = 0;
                Login_Screen login_screen = new Login_Screen();
                if (BotaoSaveLogin.IsChecked == false)
                {
                    if (create_user_opened)
                    {
                        RegisterNewUser registerUser = new RegisterNewUser();
                        registerUser.Close();
                    }
                    login_screen.Show();
                }
                else
                {
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