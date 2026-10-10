
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
using System.ComponentModel.DataAnnotations;
using BCryptNet = BCrypt.Net.BCrypt;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualBasic.FileIO;
using Mysqlx.Crud;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

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

        public static int Change_user_data(TextBox email_space, TextBox user_space, PasswordBox password_space, TextBox fullname_space, int imageValue, int AdminLevel, int option, string finish_message)
        {
            string email = email_space.Text.Trim();
            string user = user_space.Text.Trim();
            string password = password_space.Password.Trim();
            string hashpassword = BCryptNet.HashPassword(password);
            string fullname = fullname_space.Text.Trim();
            int image = imageValue;
            int adminLevel = AdminLevel;

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
                    query = "INSERT INTO users (email, user, password, name, imageValue) VALUES (@email, @user, @hashpassword, @fullname, @imageValue)";
                }
                else if (option == 2)
                {
                    query = "INSERT INTO users (email, user, password, IsAdmin, name, imageValue, AdminLevel) VALUES (@email, @user, @hashpassword, @admin, @fullname, @imageValue, @AdminLevel)";
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
                            command.Parameters.AddWithValue("@fullname", fullname);
                            command.Parameters.AddWithValue("@imageValue", image);
                        }
                        else if (option == 2)
                        {
                            command.Parameters.AddWithValue("@email", email);
                            command.Parameters.AddWithValue("@user", user);
                            command.Parameters.AddWithValue("@hashpassword", hashpassword);
                            command.Parameters.AddWithValue("@admin", 1);
                            command.Parameters.AddWithValue("@fullname", fullname);
                            command.Parameters.AddWithValue("@imageValue", image);
                            command.Parameters.AddWithValue("@AdminLevel", adminLevel);
                        }
                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show(finish_message);
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

        // DELETE THE USER USING ITS ID
        public static void DeleteUser(int ID, string finish_message)
        {
            int id = ID;

            Open_database();

            string query = "DELETE FROM users WHERE id = @id";
            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Connection))
                {

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show(finish_message);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Erro nos sistema: Não foi possível deletar o usuário.");
            }
        }


        // UPDATE LAST LOGIN DATE
        public static void UpdateLastLogin(int id)
        {
            Open_database();

            string query = @"UPDATE users SET lastLogin = NOW() WHERE id = @id";

            using MySqlCommand command =
                new MySqlCommand(query, Connection);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        // UPDATE LAST ACTIVITIE DATE
        public static void UpdateLastActive(int id)
        {
            Open_database();

            string query = @"UPDATE users SET lastActive = NOW() WHERE id = @id";

            using MySqlCommand command =
                new MySqlCommand(query, Connection);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }

        // VERIFY EMAIL OR USER EXISTENCE, VERIFY PASSWORD, RETURN ID WITH EMAIL OR USER AND VERIFY ADMIN PASSWORD
        public static int Verify_data_base(TextBox email_or_user_space, PasswordBox password_space, int option, int AdminID = 0)
        {
            string login = email_or_user_space.Text.Trim();
            string password = password_space.Password.Trim();
            Open_database();

            if (option == 1) // 1 RETURN 0 IF THE USER OR EMAIL EXISTS IN DATABASE
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
            else if (option == 2) // 2 RETURN 0 IF THE PASSWORD IS CORRECT
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
                        return 102;
                    }
                    else
                    {
                        return 103;
                    }
                }
                catch (MySqlException ex)
                {
                    return 1;
                }
            }
            else if (option == 3) // 3 RETUNR ID USING EMAIL OR USER
            {
                string getID = "SELECT id FROM users WHERE user = @login OR email = @login";
                using var returnID = new MySqlCommand(getID, Connection);
                returnID.Parameters.AddWithValue("@login", login);

                try
                {
                    using var reader = returnID.ExecuteReader();

                    if (reader.Read())
                    {
                        // RETURN ID
                        return Convert.ToInt32(reader["id"]);
                    }

                    if (!new EmailAddressAttribute().IsValid(login))
                    {
                        return 102; // USER NOT REGISTERED
                    }
                    else
                    {
                        return 103; // EMAIL NOT REGISTERED
                    }
                }
                catch (MySqlException ex)
                {
                    return -1;
                }
            }
            else if (option == 4) // Verify Admin Password
            {
                string sqlBuscarHash = "SELECT password FROM users WHERE id = @id";

                using var verifyHash = new MySqlCommand(sqlBuscarHash, Connection);
                verifyHash.Parameters.AddWithValue("@id", AdminID);

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

                        return 104;
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    return 1;
                }
            }
            return 1;
        }

        // RETURN EMAIL USING ID
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
                    return reader["email"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null; // DataBase Error
            }
            return null;
        }

        // RETURN FULL NAME USING ID
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
                    return reader["name"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null;
            }
            return null;
        }

        // RETURN USER USING ID
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
                    return reader["user"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null;
            }
            return null;
        }

        // RETURN SYSTEM CONFIG USING ID
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
                    return reader["systemConfig"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null;
            }
            return null;
        }

        // RETURN LAST ACTIVITIE USING ID
        public static string ReturnLastActive(int id)
        {
            Open_database();
            string getLastActive = "SELECT lastActive FROM users WHERE id = @id";
            using var returnLastActive = new MySqlCommand(getLastActive, Connection);
            returnLastActive.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnLastActive.ExecuteReader();

                if (reader.Read())
                {
                    return reader["lastActive"].ToString();
                }
            }
            catch (MySqlException ex)
            {
                return null;
            }
            return null;
        }

        // RETURN FALSE OR TRUE ISADMIN
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

        // GET THE ADMIN LEVEL FROM DATABASE
        public static int GetAdminLevel(int id)
        {
            Open_database();
            string getAdminLevel = "SELECT AdminLevel FROM users WHERE id = @id";
            using var returnAdminLevel = new MySqlCommand(getAdminLevel, Connection);
            returnAdminLevel.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnAdminLevel.ExecuteReader();

                if (reader.Read())
                {
                    int AdminLevel = Convert.ToInt32(reader["AdminLevel"]);

                    return AdminLevel;
                }
            }
            catch (MySqlException ex)
            {
                return 0;
            }
            return 0;

        }

        // GET THE STATE THE ACTUAL USER IS AT (LOCKED = CANT LOGIN FOR 5 MINUTES)
        public static bool ReturnIsLocked(int id)
        {
            Open_database();
            string getLock = "SELECT IsLocked FROM users WHERE id = @id";
            using var returnLock = new MySqlCommand(getLock, Connection);
            returnLock.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnLock.ExecuteReader();

                if (reader.Read())
                {
                    int isLocked = Convert.ToInt32(reader["IsLocked"]);

                    if (isLocked == 1)
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

        // RETURN IF THE USER IS BANNED (BANNED = ACCOUNT IS NOT DELETED BUT THE USER CANT LOGIN ANYMORE)
        public static bool ReturnIsBanned(int id)
        {
            Open_database();
            string getBanned = "SELECT IsBanned FROM users WHERE id = @id";
            using var returnBanned = new MySqlCommand(getBanned, Connection);
            returnBanned.Parameters.AddWithValue("@id", id);

            try
            {
                using var reader = returnBanned.ExecuteReader();

                if (reader.Read())
                {
                    int isBanned = Convert.ToInt32(reader["IsBanned"]);

                    if (isBanned == 1)
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

        // GENERATES A RANDOM STRING TO VERIFY IF THE ACTUAL USER IS A ROBOT
        public static string Verify_robot()
        {
            Random random = new Random();
            char[] alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
            char[] letter = new char[6];
            string text = "";

            for (int i = 0; i < 6; i++)
            {
                int repeat = random.Next(0, 26);
                letter[i] = alphabet[repeat];
                text = $"{text}{letter[i]}";
            }
            return text;

        }

        // VERIFY IF AN ADMIN EXIST
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

        // GET THE NUMBER OF REGISTERS IN THE DATABASE
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

        // RETURN THE IMAGE ID TO GET IT INTO THE APPLICATION
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

        // CHANGES THE SYSTEM CONFIG STRING 
        public static int changeConfig(bool darkmode, bool savelogin, int id)
        {
            Open_database();
            string query = "UPDATE users SET systemConfig = @config WHERE ID=@id";
            string configString = $"DarkMode:{darkmode.ToString().ToLower()} SaveLogin:{savelogin.ToString().ToLower()}";


            try
            {
                using (MySqlCommand command = new MySqlCommand(query, Connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@config", configString);
                    command.ExecuteNonQuery();
                }
                return 0;
            }
            catch (System.Exception ex)
            {
                return 1;
            }

        }

        // GET IF THE USER IS ONLINE
        public static bool ReturnIsOnline(int id)
        {
            Open_database();

            string query = @"SELECT CASE WHEN lastActive >= NOW() - INTERVAL 30 SECOND THEN 1 ELSE 0 END FROM users WHERE id = @id";

            using MySqlCommand command = new MySqlCommand(query, Connection);

            command.Parameters.AddWithValue("@id", id);

            object result = command.ExecuteScalar();

            return Convert.ToBoolean(result);
        }

        // GET THE IMAGE PATH USING ITS ID
        public static string getImage(int imageNumber)
        {
            string imageText = imageNumber.ToString();
            string archivePath = $"/ProfileImage/{imageText}.png";
            return archivePath;
        }

        // GET THE NUMBER OF IMAGES IN THE PROFILEIMAGE FOLDER
        public static int GetImageCount()
        {
            int count = 0;

            while (true)
            {
                try
                {
                    string path = getImage(count);

                    var resource = Application.GetResourceStream(
                        new Uri(path, UriKind.Relative)
                    );

                    if (resource == null)
                        break;

                    count++;
                }
                catch
                {
                    break;
                }
            }

            return count;
        }
        public static int UpdateUserProfile(int id, string fullname, string email, string username, bool isAdmin)
        {
            fullname = fullname.Trim();
            email = email.Trim();
            username = username.Trim();

            try
            {
                using var connection =
                    new MySqlConnection(connectionString);

                connection.Open();

                // Verifica e-mail duplicado, ignorando o próprio usuário
                string checkEmail = @"SELECT COUNT(*) FROM users WHERE email = @email AND id <> @id";

                using (var cmd = new MySqlCommand(checkEmail, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@id", id);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return 100;
                }

                // Verifica nome de usuário duplicado
                string checkUser = @"SELECT COUNT(*) FROM users WHERE user = @user AND id <> @id";

                using (var cmd = new MySqlCommand(checkUser, connection))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@id", id);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return 101;
                }

                // Atualiza os dados
                string query = @"UPDATE users SET name = @fullname, email = @email, user = @user, IsAdmin = @isAdmin WHERE id = @id";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@fullname", fullname);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@user", username);
                command.Parameters.AddWithValue("@isAdmin", isAdmin ? 1 : 0);
                command.Parameters.AddWithValue("@id", id);

                int affectedRows = command.ExecuteNonQuery();

                return affectedRows > 0 ? 0 : 102;
            }
            catch (MySqlException)
            {
                return 1;
            }
        }

        public static int UpdateManagedUser(
            int adminId,
            int targetId,
            string fullname,
            string email,
            string username,
            bool makeAdmin)
        {
            fullname = fullname.Trim();
            email = email.Trim();
            username = username.Trim();

            if (string.IsNullOrWhiteSpace(fullname) ||
                fullname.Length > 150 ||
                !new EmailAddressAttribute().IsValid(email) ||
                email.Length > 254 ||
                username.Length < 3 ||
                username.Length > 30 ||
                username.Any(char.IsWhiteSpace))
                return 1;

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                bool actorAdmin;
                bool actorBanned;
                int actorLevel;

                // Verifica quem está realizando a edição
                using (var cmd = new MySqlCommand(@"SELECT IsAdmin, IsBanned, AdminLevel FROM users WHERE id = @id FOR UPDATE", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", adminId);

                    using var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                        return 105;

                    actorAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                    actorBanned = Convert.ToBoolean(reader["IsBanned"]);
                    actorLevel = reader["AdminLevel"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AdminLevel"]);
                }

                if (!actorAdmin || actorBanned)
                    return 105;

                bool targetAdmin;
                int targetLevel;

                using (var cmd = new MySqlCommand(@"SELECT IsAdmin, AdminLevel FROM users WHERE id = @id FOR UPDATE", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", targetId);

                    using var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                        return 102;

                    targetAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                    targetLevel = reader["AdminLevel"] == DBNull.Value
                        ? 0 : Convert.ToInt32(reader["AdminLevel"]);
                }

                if (adminId == targetId && !makeAdmin)
                    return 106;

                if (adminId != targetId && targetAdmin && actorLevel >= targetLevel)
                    return 105;


                using (var cmd = new MySqlCommand(@"SELECT COUNT(*) FROM users WHERE email = @email AND id <> @id",
                    connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@id", targetId);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return 100;
                }

                using (var cmd = new MySqlCommand(@"SELECT COUNT(*) FROM users WHERE user = @username AND id <> @id", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@id", targetId);

                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        return 101;
                }

                int newLevel = makeAdmin ? (targetAdmin ? targetLevel : actorLevel + 1) : 0;

                using (var cmd = new MySqlCommand(@"UPDATE users SET name = @name, email = @email, user = @username, IsAdmin = @admin, AdminLevel = @level WHERE id = @id", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@name", fullname);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@admin", makeAdmin ? 1 : 0);
                    cmd.Parameters.AddWithValue("@level", newLevel);
                    cmd.Parameters.AddWithValue("@id", targetId);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                return 0;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062)
                {
                    if (ex.Message.Contains("email",
                        StringComparison.OrdinalIgnoreCase))
                        return 100;

                    if (ex.Message.Contains("user",
                        StringComparison.OrdinalIgnoreCase))
                        return 101;
                }

                return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public static int SetUserBanStatus(
            int adminId,
            int targetId,
            string password,
            bool ban)
        {
            if (adminId == targetId)
                return 105;

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                string hash;
                bool admin;
                bool adminBanned;
                int adminLevel;

                using (var cmd = new MySqlCommand(@"SELECT password, IsAdmin, IsBanned, AdminLevel FROM users WHERE id = @id FOR UPDATE", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", adminId);

                    using var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                    {
                        return 105;
                    }
                    hash = reader["password"].ToString() ?? "";
                    admin = Convert.ToBoolean(reader["IsAdmin"]);
                    adminBanned = Convert.ToBoolean(reader["IsBanned"]);
                    adminLevel = reader["AdminLevel"] == DBNull.Value
                        ? 0 : Convert.ToInt32(reader["AdminLevel"]);
                }

                if (!admin || adminBanned)
                {
                    return 105;
                }
                if (!BCryptNet.Verify(password, hash))
                {
                    return 104;
                }
                bool targetAdmin;
                bool currentBan;
                int targetLevel;

                using (var cmd = new MySqlCommand(@"SELECT IsAdmin, IsBanned, AdminLevel FROM users WHERE id = @id FOR UPDATE", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", targetId);

                    using var reader = cmd.ExecuteReader();

                    if (!reader.Read())
                        return 102;

                    targetAdmin = Convert.ToBoolean(reader["IsAdmin"]);
                    currentBan = Convert.ToBoolean(reader["IsBanned"]);
                    targetLevel = reader["AdminLevel"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AdminLevel"]);
                }

                if (targetAdmin && adminLevel >= targetLevel)
                    return 105;

                if (currentBan == ban)
                    return 107;

                using (var cmd = new MySqlCommand(
                    @"UPDATE users SET IsBanned = @banned WHERE id = @id", connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@banned", ban ? 1 : 0);
                    cmd.Parameters.AddWithValue("@id", targetId);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        // =========================================================================================================================================================== //

        // LOG TABLE FUNCTIONS
        public static void sendLog()
        {

        }

    }
}