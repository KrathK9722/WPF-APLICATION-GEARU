using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.IO;
using System.Reflection;

namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RegisterNewUser : Window
    {

        // Variáveis
        bool checado = false;
        public static int AdminID;
        private bool NewUserIsAdmin = false;
        public static int ImageValue = 0;

        // Instâncias
        private string connectionString = GlobalFunctions.connectionString;
        private bool adminExist = GlobalFunctions.AdminExist();


        // Inicialização/Função Primária
        public RegisterNewUser()
        {
            InitializeComponent();
            viewAvatar();
        }

        private void viewAvatar()
        {
            int quantidadeImagens = GlobalFunctions.GetImageCount();

            ContainerDeCards.Children.Clear();

            for (int i = 0; i < quantidadeImagens; i++)
            {
                CardImage novoCard = new CardImage();

                novoCard.CardImageSource = GlobalFunctions.getImage(i);
                novoCard.ImageValue = i;

                novoCard.Width = 100;
                novoCard.Height = 100;
                novoCard.Margin = new Thickness(8);

                ContainerDeCards.Children.Add(novoCard);
            }
        }

        private void button_adminlevel_Checked(object sender, RoutedEventArgs e)
        {
            NewUserIsAdmin = true;
            button_adminlevel.Content = "Usuário de nivel Administrador";
        }
        private void button_adminlevel_Unchecked(object sender, RoutedEventArgs e)
        {
            NewUserIsAdmin = false;
            button_adminlevel.Content = "Usuário de nivel Comum";
        }

        private void btn_create_account_Click(object sender, RoutedEventArgs e) // CRIAR CONTA
        {
            int error = 0;
            error_create_email.Visibility = Visibility.Collapsed;
            error_create_password.Visibility = Visibility.Collapsed;
            error_create_user.Visibility = Visibility.Collapsed;
            error_fill_everything.Visibility = Visibility.Collapsed;
            error_duplicated_email.Visibility = Visibility.Collapsed;
            error_duplicated_user.Visibility = Visibility.Collapsed;
            error_repeat_password.Visibility = Visibility.Collapsed;
            error_system.Visibility = Visibility.Collapsed;
            error_enter_full_name.Visibility = Visibility.Collapsed;
            // ERROR MESSAGE FILL EVERYTHING
            if (string.IsNullOrWhiteSpace(txt_create_email.Text) || string.IsNullOrWhiteSpace(txt_create_password.Password))
            {
                error_fill_everything.Visibility = Visibility.Visible;
                error += 1;
                return;
            }

            // ERROR MESSAGE CREATE USER
            if (txt_create_user.Text.Length < 3)
            {
                error_create_user.Visibility = Visibility.Visible;
                error += 1;
            }
            // ERROR MESSAGE ENTER FULL NAME
            string fullname = txt_enter_full_name.Text.Trim();

            string[] names = fullname.Split(' ',StringSplitOptions.RemoveEmptyEntries);

            if (names.Length < 2 || names.Any(name => name.Length < 2) || names.Any(name => !name.All(c =>
                    char.IsLetter(c) || c == '\'' || c == '-')))
            {
                error_enter_full_name.Visibility = Visibility.Visible;
                error += 1;
            }

            // ERROR MESSAGE CREATE EMAIL
            if (!new EmailAddressAttribute().IsValid(txt_create_email.Text))
            {
                error_create_email.Visibility = Visibility.Visible;
                error += 1;
            }

            // ERROR MESSAGE CREATE PASSWORD
            if (txt_create_password.Password.Length < 8)
            {
                error_create_password.Visibility = Visibility.Visible;
                error += 1;
            }

            // ERROR MESSAGE PASSWORD REPEAT
            if (txt_create_password.Password != txt_register_password.Password)
            {
                error_repeat_password.Visibility = Visibility.Visible;
                error += 1;
            }

            // PRINT IF REGISTER IS CORRECT
            if (error == 0)
            {
                int system_return;
                if (NewUserIsAdmin) 
                { 
                    system_return = GlobalFunctions.Change_user_data(txt_create_email, txt_create_user, txt_create_password, txt_enter_full_name, ImageValue, GlobalFunctions.GetAdminLevel(AdminID)+1, 2, "Success!! New Admin Account Created."); 
                }
                else 
                {
                    system_return = GlobalFunctions.Change_user_data(txt_create_email, txt_create_user, txt_create_password, txt_enter_full_name, ImageValue, 0, 1, "Success!! User Account Created.");
                }
                if (system_return != 0)
                {
                    // Erro geral do sistema
                    if (system_return == 1)
                    {
                        error_system.Visibility = Visibility.Visible;
                    }

                    // Erro de email duplicado
                    if (system_return == 100)
                    {
                        if (new EmailAddressAttribute().IsValid(txt_create_email.Text))
                        {
                            error_duplicated_email.Visibility = Visibility.Visible;
                        }
                    }

                    // Erro de usuário duplicado
                    if (system_return == 101)
                    {
                        if (txt_create_user.Text.Length > 3)
                        {
                            error_duplicated_user.Visibility = Visibility.Visible;
                        }
                    }

                }
                else
                {
                    Login_Screen login_Screen = new Login_Screen();
                    txt_register_password.Clear();
                    return;

                }

            }
            else
            {
                error = 0;
                return;
            }
        }
        private void comeback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StartScreen.create_user_opened = false;
        }
    }
}