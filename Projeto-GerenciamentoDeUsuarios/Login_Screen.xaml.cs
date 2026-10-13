
using System.Windows;


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
            error_fill_everything.Visibility = Visibility.Collapsed;
            error_system.Visibility = Visibility.Collapsed;
            error_verify.Visibility = Visibility.Collapsed;
            error_wrong_user_or_password.Visibility = Visibility.Collapsed;
            error_account_locked.Visibility = Visibility.Collapsed;
            error_account_locked_time.Visibility = Visibility.Collapsed;

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
                    reset_capcha();
                    return;
                }
                //Verificação de senha
                else
                {
                    system_return = GlobalFunctions.Verify_data_base(txt_enter_email_or_user, txt_enter_password, 2);
                    int user_token = GlobalFunctions.GetUserIdByLogin(txt_enter_email_or_user.Text);
                    GlobalFunctions.UpdateLastTryLogin(user_token);
                    if (GlobalFunctions.ReturnIsLocked(user_token))
                    {
                        if (!GlobalFunctions.HasLockExpired(user_token))
                        {
                            error_account_locked.Visibility = Visibility.Visible;
                            error_account_locked_time.Visibility = Visibility.Visible;
                            error_account_locked_time.Content = $"Aguarde {GlobalFunctions.GetRemainingLockTime(user_token)} para realizar a próxima tentativa.";
                            reset_capcha();
                            GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Tentativa de Login", "Falha");
                            return;
                        }
                        else
                        {
                            GlobalFunctions.ResetPasswordTry(user_token);
                            GlobalFunctions.SetIsLocked(user_token, 0);
                            GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Bloqueio temporário de conta", "Bloqueio desfeito");
                        }
                    }
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
                            if (GlobalFunctions.GetPasswordTry(user_token) >= 2)
                            {
                                if (!GlobalFunctions.ReturnIsLocked(user_token))
                                {
                                    GlobalFunctions.SendLockedTime(user_token);
                                    GlobalFunctions.SetIsLocked(user_token, 1);
                                    GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Bloqueio temporário de conta", "Bloqueio efetuado");
                                }
                            }
                            else if (GlobalFunctions.GetPasswordTry(user_token) == -1)
                            {
                                MessageBox.Show("`Erro ao Verificar conta, tente novamente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                GlobalFunctions.AddPasswordTry(user_token);
                                if (GlobalFunctions.GetPasswordTry(user_token) >= 2)
                                {
                                    GlobalFunctions.SendLockedTime(user_token);
                                    GlobalFunctions.SetIsLocked(user_token, 1);
                                    GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Bloqueio temporário de conta", "Bloqueio efetuado");
                                }
                            }
                        }
                        error = 0;
                        reset_capcha();
                        GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Tentativa de Login", "Falha");
                        return;
                    }
                    else if (GlobalFunctions.ReturnIsBanned(user_token))
                    {
                        MessageBox.Show("`Conta Banida do Sistema", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Tentativa de Login", "Falha");
                        return;
                    }
                    else
                    {
                        GlobalFunctions.SendEventLog("Login_Screen", user_token, GlobalFunctions.ReturnUser(user_token), "Tentativa de Login", "Sucesso");
                        StartScreen.ID = user_token;
                        StartScreen Start_screen = new StartScreen();
                        Start_screen.Show();
                        error = 0;
                        GlobalFunctions.UpdateLastLogin(user_token);
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
                GlobalFunctions.SendEventLog("Login_Screen", 0, "Sem registro", "Tentativa de Login", "Falha");
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