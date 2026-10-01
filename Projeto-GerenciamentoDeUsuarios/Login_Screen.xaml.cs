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

        public void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            login_grid.IsEnabled = false;
            var cap = new CAPTCHA_WINDOW(this, "login"); // Abre captcha como dialogo e seta Main como owner
            bool? result = cap.ShowDialog();

            login_grid.IsEnabled = true;
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
        private void btn_login_account_Click(object sender, RoutedEventArgs e) // CRIAR CONTA
        {
            int error = 0;
            error_data_not_found.Visibility = Visibility.Collapsed;
            error_fill_everything.Visibility = Visibility.Collapsed;
            error_system.Visibility = Visibility.Collapsed;
            error_verify.Visibility = Visibility.Collapsed;
            error_wrong_user_or_password.Visibility = Visibility.Collapsed;

            // ERROR MESSAGE FILL EVERYTHING
            if (string.IsNullOrWhiteSpace(txt_enter_email_or_user.Text) || string.IsNullOrWhiteSpace(txt_enter_password.Password))
            {
                error_fill_everything.Visibility = Visibility.Visible;
                error += 1;
                reset_capcha();
                return;
            }

            // ERROR ROBOT VERIFY
            if (checado == false)
            {
                error_verify.Visibility = Visibility.Visible;
                error += 1;
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

                    // Erro de email não cadastrado
                    if (system_return == 103)
                    {
                        error_wrong_user_or_password.Visibility = Visibility.Visible;
                    }

                    // Erro de usuário não cadastrado
                    if (system_return == 102)
                    {
                        error_wrong_user_or_password.Visibility = Visibility.Visible;
                    }

                    // Erro de dados não encontrados
                    if (system_return == 105)
                    {
                        error_data_not_found.Visibility = Visibility.Visible;
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

                        // Erro de senha errada
                        if (system_return == 104)
                        {
                            error_wrong_user_or_password.Visibility = Visibility.Visible;
                        }
                        // Erro de dados não encontrados
                        if (system_return == 105)
                        {
                            error += 1;
                            error_data_not_found.Visibility = Visibility.Visible;
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
        private void close_screen_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            return;

        }
    }
}