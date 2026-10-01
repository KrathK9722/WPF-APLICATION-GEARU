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

// FAZER A VERIFICAÇÃO FUNCIONAR DOS DADOS, TA SALVANDO E MANDANDO MSG MESMO COM A VERIFICAÇÃO ERRADA POR CONTA DO GLOBAL
namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // Variáveis
        bool checado = false;

        // Instâncias
        public string connectionString = GlobalFunctions.connectionString;
        public bool adminExist = GlobalFunctions.AdminExist();


        // Inicialização/Função Primária
        public MainWindow()
        {
            InitializeComponent();
            setTitle();
        }
        public void setTitle()
        {
            string archivePath = @"C:\Documentos\GEARU\Tokens\session.txt";
            if (File.Exists(archivePath)) {
                string savedIdText = File.ReadAllText(archivePath);
                if (int.TryParse(savedIdText, out int savedId)) { }
                if (adminExist && Convert.ToInt32(savedId) == 0)
                {
                    Login_Screen loginScreen = new Login_Screen();
                    loginScreen.Show();
                    this.Close();
                }
                else if (Convert.ToInt32(savedId) != 0)
                {
                    StartScreen.ID = Convert.ToInt32(savedId);
                    StartScreen Start_screen = new StartScreen();
                    Start_screen.Show();
                    this.Close();
                    return;
                }
            }
            else
            {
                if (!Directory.Exists(@"C:\Documentos\GEARU\Tokens"))
                {
                    Directory.CreateDirectory(@"C:\Documentos\GEARU\Tokens");
                }
                if (!File.Exists(archivePath))
                {
                    File.WriteAllText(archivePath, "0");
                    File.SetAttributes(archivePath, FileAttributes.Hidden);
                }
                if (File.Exists(archivePath))
                {
                    string savedIdText = File.ReadAllText(archivePath);
                    if (int.TryParse(savedIdText, out int savedId)) { }
                    if (adminExist && Convert.ToInt32(savedId) == 0)
                    {
                        title_create_account.Text = "Criar Conta de Usuário";
                    }
                    else if (Convert.ToInt32(savedId) == 0)
                    {
                        title_create_account.Text = "Criar Conta de Administrador";
                    }
                    else
                    {
                        StartScreen.ID = Convert.ToInt32(savedId);
                        StartScreen Start_screen = new StartScreen();
                        Start_screen.Show();
                        this.Close();
                        return;
                    }
                }
                else
                {
                    this.Close();
                    MessageBox.Show("Erro ao criar o arquivo.");
                }
            }
        }

        public void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            register_grid.IsEnabled = false;
            var cap = new CAPTCHA_WINDOW(this); // Abre captcha como dialogo e seta Main como owner
            bool? result = cap.ShowDialog();

            register_grid.IsEnabled = true;
            if (result == true)
            {
                // Captcha confirmado e válido
                checado = true;
                checkbox_not_robot.IsChecked = true;

                grid_not_robot.Visibility = Visibility.Collapsed;
                checked_image_grid.Visibility = Visibility.Visible;

                error_verify.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Captcha não confirmado ou cancelado
                reset_capcha();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            grid_not_robot.Visibility = Visibility.Visible;
            checado = false;
        }

        public void reset_capcha()
        {
            checado = false;
            checkbox_not_robot.IsChecked = false;
            grid_not_robot.Visibility = Visibility.Visible;
            checked_image_grid.Visibility = Visibility.Collapsed;
        }
        public void btn_create_account_Click(object sender, RoutedEventArgs e) // CRIAR CONTA
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
            error_verify.Visibility = Visibility.Collapsed;
            // ERROR MESSAGE FILL EVERYTHING
            if (string.IsNullOrWhiteSpace(txt_create_email.Text) || string.IsNullOrWhiteSpace(txt_create_password.Password))
            {
                error_fill_everything.Visibility = Visibility.Visible;
                error += 1;
                reset_capcha();
                return;
            }
            else
            {
                error_fill_everything.Visibility = Visibility.Collapsed;
            }

            // ERROR MESSAGE CREATE USER
            if (txt_create_user.Text.Length < 3)
            {
                error_create_user.Visibility = Visibility.Visible;
                error += 1;
            }
            else
            {
                error_create_user.Visibility = Visibility.Collapsed;
            }

            // ERROR MESSAGE CREATE EMAIL
            if (!new EmailAddressAttribute().IsValid(txt_create_email.Text))
            {
                error_create_email.Visibility = Visibility.Visible;
                error += 1;
            }
            else
            {
                error_create_email.Visibility = Visibility.Collapsed;
            }

            // ERROR MESSAGE CREATE PASSWORD
            if (txt_create_password.Password.Length < 8)
            {
                error_create_password.Visibility = Visibility.Visible;
                error += 1;
            }
            else
            {
                error_create_password.Visibility = Visibility.Collapsed;
            }

            // ERROR MESSAGE PASSWORD REPEAT
            if (txt_create_password.Password != txt_register_password.Password)
            {
                error_repeat_password.Visibility = Visibility.Visible;
                error += 1;
            }
            else
            {
                error_repeat_password.Visibility = Visibility.Collapsed;
            }

            // ERROR ROBOT VERIFY
            if (checado == false)
            {
                error_verify.Visibility = Visibility.Visible;
                error += 1;
            }
            else
            {
                error_verify.Visibility = Visibility.Collapsed;
            }

            

            // PRINT IF REGISTER IS CORRECT
            if (error == 0)
            {
                int system_return;
                if (adminExist)
                {
                    system_return = GlobalFunctions.Change_user_data(txt_create_email, txt_create_user, txt_create_password, 1, "Success!! Account Created.");
                }
                else
                {
                    system_return = GlobalFunctions.Change_user_data(txt_create_email, txt_create_user, txt_create_password, 2, "Success!! Admin Account Created.");
                }
                if (system_return != 0)
                {
                    // Erro geral do sistema
                    if (system_return == 1)
                    {
                        error_system.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        error_system.Visibility = Visibility.Collapsed;
                    }
                    
                    // Erro de email duplicado
                    if (system_return == 100)
                    {
                        if (new EmailAddressAttribute().IsValid(txt_create_email.Text))
                        {
                            error_duplicated_email.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        error_duplicated_email.Visibility = Visibility.Collapsed;
                    }

                    // Erro de usuário duplicado
                    if (system_return == 101)
                    {
                        if (txt_create_user.Text.Length > 3)
                        {
                            error_duplicated_user.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        error_duplicated_user.Visibility = Visibility.Collapsed;
                    }

                }
                else
                {
                    Login_Screen login_Screen = new Login_Screen();
                    login_Screen.Show();
                    this.Close();
                    txt_register_password.Clear();
                    reset_capcha();
                    return;

                }
                
            }
            else
            {
                error = 0;
                reset_capcha();
                return;
            }
        }
        private void close_screen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}