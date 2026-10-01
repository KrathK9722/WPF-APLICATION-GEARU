
/* 
  
TIPOS DE RETORNO:

3 = Retorno negativo
2 = Retorno positivo
1 or -1 = erro de tentativa
0 = tentativa concluida
100 = Email ja cadastrado no sistema
101 = Usuário ja cadastrado no sistema
102 = Usuário não cadastrado
103 = Email não cadastrado
104 = Senha incorreta

*/

using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Projeto_GerenciamentoDeUsuarios
{
    public static class GlobalFunctions
    {
        public static string connectionString = "Server=localhost;Database=GEARU;Uid=root;Pwd=;";

        // Conexão fica guardada aberta na memória do app
        public static MySqlConnection Connection { get; set; }

        public static void Open_database()
        {
            try
            {
                // CORREÇÃO: Abre a conexão sem o 'using' para que ela permaneça viva no app
                if (Connection == null || Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection = new MySqlConnection(connectionString);
                    Connection.Open();
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.MessageBox.Show("Erro ao conectar: " + ex.Message);
            }
        }

        public static int Change_user_data(TextBox email_space, TextBox user_space, PasswordBox password_space, TextBox fullname_space, int option, string finish_message)
        {
            string email = email_space.Text.Trim();
            string user = user_space.Text.Trim();
            string password = password_space.Password.Trim();
            string hashpassword = BCryptNet.HashPassword(password);
            string fullname = fullname_space.Text.Trim();
            int admin = 1;

            // Garantir abertura do banco de dados
            Open_database();

            string query = "";
            string sqlVerifyuser = "SELECT user FROM users WHERE user = @user";
            string sqlVerifyemail = "SELECT email FROM users WHERE email = @email";

            using MySqlCommand verify_user = new MySqlCommand(sqlVerifyuser, Connection);
            using MySqlCommand verify_email = new MySqlCommand(sqlVerifyemail, Connection);
            verify_email.Parameters.AddWithValue("@email", email);
            verify_user.Parameters.AddWithValue("@user", user);

            object email_exist = verify_email.ExecuteScalar();
            object user_exist = verify_user.ExecuteScalar();

            if (email_exist == null && user_exist == null)
            {
                if (option == 1)
                {
                    query = "INSERT INTO users (email, user, password, name) VALUES (@email, @user, @hashpassword, @fullname)";
                }
                else if (option == 2)
                {
                    query = "INSERT INTO users (email, user, password, IsAdmin, name) VALUES (@email, @user, @hashpassword, @admin, @fullname)";
                }
                else if (option == 3)
                {
                }
                else if (option == 4)
                {
                }

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, Connection))
                    {
                        if (option == 1)
                        {
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@user", user);
                            command.Parameters.AddWithValue("@hashpassword", hashpassword);
                        }
                        else if (option == 2)
                        {
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@user", user);
                            command.Parameters.AddWithValue("@hashpassword", hashpassword);
                            command.Parameters.AddWithValue("@admin", admin);
                            command.Parameters.AddWithValue("@fullname", fullname);
                        }
                        else if (option == 3)
                        {
                        }
                        else if (option == 4)
                        {
                        }

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show(finish_message);
                    Limpar_campos(email_space, user_space, password_space);
                    return 0;
                }
                catch (System.Exception ex)
                {
                    return 1;
                }
            }
            else if (email_exist != null)
            {
                return 100;
            }
            else if (user_exist != null)
            {
                return 101;
            }
            return 1;
        }

        

        public static int Verify_data_base(TextBox email_or_user_space, PasswordBox password_space, int option)
        {   
            string login = email_or_user_space.Text.Trim();
            string password = password_space.Password.Trim();
            Open_database();

            if (option == 1)
            {
                string sqlVerifyLogin = "SELECT user, email FROM users WHERE user = @login OR email = @login";

                using MySqlCommand verify_login = new MySqlCommand(sqlVerifyLogin, Connection);
                verify_login.Parameters.AddWithValue("@login", login);

                using MySqlDataReader reader = verify_login.ExecuteReader();
                if (reader.Read())
                {
                    return 0;             
                }
                if (!new EmailAddressAttribute().IsValid(login))
                {
                    return 102;

                }
                else
                {
                    return 103;
                }
            }
            else if(option == 2)
            {
                string sqlBuscarHash = "SELECT password FROM users WHERE user = @login OR email = @login";
                using var verifyHash = new MySqlCommand(sqlBuscarHash, Connection);

                verifyHash.Parameters.AddWithValue("@login", login);

                try
                {
                    using var reader = verifyHash.ExecuteReader();

                    if (reader.Read())
                    {
                        
                        string hashBanco = reader["password"].ToString();

                        bool correctPassword = BCryptNet.Verify(password, hashBanco);

                        if (correctPassword)
                        {
                            return 0;
                        }
                        else
                        {
                            return 104;
                        }
                    }
                    if (!new EmailAddressAttribute().IsValid(login))
                    {
                        return 102; // Usuário não cadastrado
                    }
                    else
                    {
                        return 103; // Email não cadastrado
                    }
                }
                catch (MySqlException ex)
                {
                    return 1;
                }
            }
            else if (option == 3)
            {
                string getID = "SELECT id FROM users WHERE user = @login OR email = @login";
                using var returnID = new MySqlCommand(getID, Connection);
                returnID.Parameters.AddWithValue("@login", login);

                try
                {
                    using var reader = returnID.ExecuteReader();

                    if (reader.Read())
                    {
                        // Retorna ID
                        return Convert.ToInt32(reader["id"]);
                    }

                    if (!new EmailAddressAttribute().IsValid(login))
                    {
                        return 102; // Usuário não cadastrado
                    }
                    else
                    {
                        return 103; // Email não cadastrado
                    }
                }
                catch (MySqlException ex)
                {
                    return -1; // Erro de banco
                }
            }
            return 1;
        }
        public static string ReturnEmail(int id)
        {
            Open_database();
            string getEmail = "SELECT email FROM users WHERE id = @id";
            using var returnEmail = new MySqlCommand(getEmail, Connection);
            returnEmail.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnEmail.ExecuteReader();

                if (reader.Read())
                {
                    // Retorna email
                    return reader["email"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null; // Erro de banco
            }
            return null;
        }
        public static string ReturnFullName(int id)
        {
            Open_database();
            string getName = "SELECT name FROM users WHERE id = @id";
            using var returnName = new MySqlCommand(getName, Connection);
            returnName.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnName.ExecuteReader();

                if (reader.Read())
                {
                    // Retorna email
                    return reader["name"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null; // Erro de banco
            }
            return null;
        }
        public static string ReturnUser(int id)
        {
            Open_database();
            string getUser = "SELECT user FROM users WHERE id = @id";
            using var returnUser = new MySqlCommand(getUser, Connection);
            returnUser.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnUser.ExecuteReader();

                if (reader.Read())
                {
                    // Retorna usuário
                    return reader["user"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null; // Erro de banco
            }
            return null;
        }
        public static string ReturnConfig(int id)
        {
            Open_database();
            string getConfig = "SELECT systemConfig FROM users WHERE id = @id";
            using var returnConfig = new MySqlCommand(getConfig, Connection);
            returnConfig.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnConfig.ExecuteReader();

                if (reader.Read())
                {
                    // Retorna usuário
                    return reader["systemConfig"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null; // Erro de banco
            }
            return null;
        }
        public static bool ReturnIsAdmin(int id)
        {
            Open_database();
            string getAdmin = "SELECT IsAdmin FROM users WHERE id = @id";
            using var returnAdmin = new MySqlCommand(getAdmin, Connection);
            returnAdmin.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnAdmin.ExecuteReader();

                if (reader.Read())
                {
                    int isAdmin = Convert.ToInt32(reader["IsAdmin"]);

                    if (isAdmin == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (MySqlException ex)
            {
                return false;
            }
            return false;

        }
        public static void Limpar_campos(TextBox email_space, TextBox user_space, PasswordBox password_space)
        {
            email_space.Clear();
            user_space.Clear();
            password_space.Clear();
        }
        public static string Verify_robot()
        {
            Random random = new Random();
            char[] alphabet = ['A', 'B', 'C', 'D','E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
            char[] letter = new char[6];
            string text="";

            for (int i = 0; i < 6; i++)
            {
                int repeat = random.Next(0, 26);
                letter[i] = alphabet[repeat];
                text = $"{text}{letter[i]}";
            }
            return text;

        }
        public static bool AdminExist()
        {
            Open_database();

            string query = "SELECT EXISTS(SELECT 1 FROM users WHERE IsAdmin = 1)";
            using var command = new MySqlCommand(query, Connection);

            try
            {
                object result = command.ExecuteScalar();
                return Convert.ToBoolean(result);
            }
            catch (MySqlException)
            {
                throw;
            }
        }
        public static int verNumeroRegistros()
        {
            Open_database();
            string query = "SELECT COUNT(*) id FROM users";
            using var countRegister = new MySqlCommand(query, Connection);

            if (Convert.ToInt32(countRegister.ExecuteScalar()) > 0)
            {
                return Convert.ToInt32(countRegister.ExecuteScalar());
            }
            else
            {
                return 0;
            }
        }
        public static int ReturnImage(int id)
        {
            Open_database();
            string getImage = "SELECT imageValue FROM users WHERE id = @id";
            using var returnImage = new MySqlCommand(getImage, Connection);
            returnImage.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnImage.ExecuteReader();

                if (reader.Read())
                {
                    int imageValue = Convert.ToInt32(reader["imageValue"]);

                    return imageValue;
                }
            }
            catch (MySqlException ex)
            {
                return 0;
            }
            return 0;

        }
        public static string getImage(int imageNumber)
        {
            string imageText = imageNumber.ToString();
            string user = Environment.UserName;
            string archivePath = $"/ProfileImage/{imageText}.png";
            return archivePath;
        }
    }
}