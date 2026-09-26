using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Lógica interna para Login_Screen.xaml
    /// </summary>
    public partial class Login_Screen : Window
    {

        // Variáveis
        bool checado = false;

        // Instâncias
        public string connectionString = GlobalFunctions.connectionString;

        // Inicialização/Função Primária
        public Login_Screen()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

            login_grid.IsEnabled = false;
            var cap = new CAPTCHA_WINDOW(this, "login"); // Abre captcha como dialogo e seta Main como owner
            bool? result = cap.ShowDialog();

            if (result == true)
            {
                // Captcha confirmado e válido
                checkbox_not_robot.IsEnabled = false;
                checado = true;
                checkbox_not_robot.IsChecked = true;
                login_grid.IsEnabled = true;
            }
            else
            {
                // Captcha não confirmado ou cancelado
                checado = false;
                checkbox_not_robot.IsChecked = false;
                login_grid.IsEnabled = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            checkbox_not_robot.IsEnabled = true;
            checado = false;
        }

        private void reset_capcha()
        {
            checado = false;
            checkbox_not_robot.IsEnabled = true;
            checkbox_not_robot.IsChecked = false;
        }
        private void btn_login_account_Click(object sender, RoutedEventArgs e) // CRIAR CONTA
        {
            int error = 0;

            // ERROR MESSAGE FILL EVERYTHING
            if (string.IsNullOrWhiteSpace(txt_enter_email_or_user.Text) || string.IsNullOrWhiteSpace(txt_enter_password.Password))
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



            // VERIFICAÇÃO DE EMAIL E USUÁRIO
            if (error == 0)
            {
                int system_return = GlobalFunctions.Verify_data_base(txt_enter_email_or_user, txt_enter_password, 1);
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

                    // Erro de email não cadastrado
                    if (system_return == 103)
                    {
                        error_email_not_registered.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        error_email_not_registered.Visibility = Visibility.Collapsed;
                    }

                    // Erro de usuário não cadastrado
                    if (system_return == 102)
                    {
                        error_user_not_registered.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        error_user_not_registered.Visibility = Visibility.Collapsed;
                    }

                    // Erro de dados não encontrados
                    if (system_return == 105)
                    {
                        error_data_not_found.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        error_data_not_found.Visibility = Visibility.Collapsed;
                    }
                    reset_capcha();
                    return;
                }
                //Verificação de senha
                else
                {
                    system_return = GlobalFunctions.Verify_data_base(txt_enter_email_or_user, txt_enter_password, 2);
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

                        // Erro de senha errada
                        if (system_return == 104)
                        {
                            error_wrong_password.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            error_wrong_password.Visibility = Visibility.Collapsed;
                        }
                        // Erro de dados não encontrados
                        if (system_return == 105)
                        {
                            error += 1;
                            error_data_not_found.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            error_data_not_found.Visibility = Visibility.Collapsed;
                        }
                        error = 0;
                        reset_capcha();
                        return;
                    }
                    else
                    {
                        int user_token = GlobalFunctions.Verify_data_base(txt_enter_email_or_user, txt_enter_password, 3);
                        StartScreen.ID = user_token;
                        StartScreen Start_screen = new StartScreen();
                        Start_screen.Show();
 
                        error = 0;
                        txt_enter_email_or_user.Clear();
                        txt_enter_password.Clear();
                        reset_capcha();
                        this.Close();
                        return;
                    }
                }
            }
            else
            {
                error = 0;
                reset_capcha();
                return;
            }
        }
        private void btn_sign_in_screen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow register_screen = new MainWindow();
            register_screen.Show();
            register_screen.setTitle();
            this.Close();
            return;

        }
    }
}