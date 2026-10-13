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
using Mysqlx.Crud;
using Org.BouncyCastle.Bcpg.OpenPgp;

// FAZER A VERIFICAÇÃO FUNCIONAR DOS DADOS, TA SALVANDO E MANDANDO MSG MESMO COM A VERIFICAÇÃO ERRADA POR CONTA DO GLOBAL
namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RemoveUser : Window
    {

        // Variáveis
        bool checado = false;
        public static int AdminID;
        public static bool IsAdmin;

        // Instâncias
        public string connectionString = GlobalFunctions.connectionString;
        public bool adminExist = GlobalFunctions.AdminExist();


        // Inicialização/Função Primária
        public RemoveUser()
        {
            InitializeComponent();
        }

        private void comeback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StartScreen.remove_user_opened = false;
        }
                
        private void btn_delete_account_Click(object sender, RoutedEventArgs e) // DELETAR UMA CONTA
        {
            int error = 0;
            error_fill_everything.Visibility = Visibility.Collapsed;
            error_system.Visibility = Visibility.Collapsed;
            error_user_or_email_not_exist.Visibility = Visibility.Collapsed;

            // ERROR MESSAGE FILL EVERYTHING
            if (string.IsNullOrWhiteSpace(txt_enter_email_or_user.Text) || string.IsNullOrWhiteSpace(txt_enter_password.Password))
            {
                error_fill_everything.Visibility = Visibility.Visible;
                error += 1;
                return;
            }
            {
                
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
                        error_user_or_email_not_exist.Visibility = Visibility.Visible;
                    }

                    // Erro de usuário não cadastrado
                    if (system_return == 102)
                    {
                        error_user_or_email_not_exist.Visibility = Visibility.Visible;
                    }
                    return;
                }
                //Verificação de senha
                else
                {
                    int delete_user_token = GlobalFunctions.GetUserIdByLogin(txt_enter_email_or_user.Text);
                    if (AdminID == delete_user_token)
                    {
                        MessageBox.Show("`Você não pode excluir sua própria conta.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        sendEventLog(delete_user_token, "Tentativa de Exclusão", "Falha");
                        return;
                    }
                    if (1 == delete_user_token)
                    {
                        MessageBox.Show("`Você não pode excluir o Administrador Principal.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        sendEventLog(delete_user_token, "Tentativa de Exclusão", "Falha");
                        return;
                    }
                    if (GlobalFunctions.ReturnIsAdmin(AdminID))
                    {
                        MessageBox.Show("`Um usuário comum não pode realizar esta operação.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                        sendEventLog(delete_user_token, "Tentativa de Exclusão", "Falha");
                        return;
                    }
                    system_return = GlobalFunctions.Verify_data_base(txt_enter_email_or_user, txt_enter_password, 4, AdminID);
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
                            error_incorrect_password.Visibility = Visibility.Visible; 
                        }
                        error = 0;
                        sendEventLog(AdminID, "Tentativa de Exclusão de Conta", "Falha");
                        GlobalFunctions.SendLog("RemoveUser", AdminID, GlobalFunctions.ReturnUser(AdminID), delete_user_token, GlobalFunctions.ReturnUser(delete_user_token),
                            "Tentativa falha de exclusão da conta");
                        return;
                    }
                    else
                    {
                        GlobalFunctions.SendLog("RemoveUser", AdminID, GlobalFunctions.ReturnUser(AdminID), delete_user_token, GlobalFunctions.ReturnUser(delete_user_token),
                            "Usuário deletado do sistema.");
                        GlobalFunctions.DeleteUser(delete_user_token, "Usuário deletado com sucesso!");
                        error = 0;
                        txt_enter_email_or_user.Clear();
                        txt_enter_password.Clear();
                        this.Close();
                        return;
                    }
                }
            }
            else
            {
                error = 0;
                sendEventLog(AdminID, "Tentativa de Exclusão de Conta", "Falha");
                return;
            }
        }

        public void sendEventLog(int id, string events, string result)
        {
            GlobalFunctions.SendEventLog("Login_Screen", id, GlobalFunctions.ReturnUser(id), events, result);
        }
    }
}