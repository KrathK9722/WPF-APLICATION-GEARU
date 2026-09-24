using MySql.Data.MySqlClient;
using Mysqlx.Expr;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class StartScreen : Window
    {

        public static int ID;
        public string email;
        public string user;
        public static bool IsAdmin;

        public static string connectionString = "Server=127.0.0.1:3306;Database=arfe;Uid=root;Pwd=;";

        public static MySqlConnection Connection { get; set; }

        public int page = 0;
        public int maxPage = 0;
        public int registerNumber = GlobalFunctions.verNumeroRegistros();

        public StartScreen()
        {
            InitializeComponent();

            //Salvar Sessão
            string archivePath = @"C:\Users\arthur_kochan\Desktop\Tokens\session.txt";

            string content = ID.ToString();
            File.SetAttributes(archivePath, FileAttributes.Normal);
            File.WriteAllText(archivePath, content);
            File.SetAttributes(archivePath, FileAttributes.Hidden);

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
            // No novo design Windows 11 o menu fica sempre aberto.
            // Esse método continua existindo porque os botões
            // "Menu" do XAML utilizam esse evento.

            ShowScreen(landing_page);
            viewCard();
        }


        // ==========================================================
        // MOSTRAR CARDS DE CASA
        // ==========================================================

        public void viewCard()

        {
            registerNumber = GlobalFunctions.verNumeroRegistros();
            if (registerNumber > 3)
            {
                maxPage = (int)Math.Ceiling(registerNumber / 3.0) - 1;
            }
            ContainerDeCards.Children.Clear();
            int inicio = page * 3;
            int fim = inicio + 3;

            number_page_card.Text = $"Página: {page + 1}/{maxPage + 1}";
            for (int i = inicio; i < fim; i++)
            {
                if (i < registerNumber)
                {
                    CardVision novoCard = new CardVision();

                    novoCard.CardTitle = $"Preço: valor#{i}";
                    novoCard.CardDescription = $"Localização: Local#{i}";
                    novoCard.CardImageSource = "C:\\Users\\arthur_kochan\\Documents\\Desafio_CRUD\\Desafio_CRUD\\help-removebg-preview.png";
                    novoCard.CardArea = $"Area: #{i}m²";
                    novoCard.Width = 140;
                    novoCard.Margin = new Thickness(13.5);

                    ContainerDeCards.Children.Add(novoCard);
                }
            }
        }


        // ==========================================================
        // SAIR
        // ==========================================================

        private void exit_click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Deseja realmente sair?",
                "Sair",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ID = 0;
                string archivePath = @"C:\Users\arthur_kochan\Desktop\Tokens\session.txt";

                string content = ID.ToString();
                File.SetAttributes(archivePath, FileAttributes.Normal);
                File.WriteAllText(archivePath, content);
                File.SetAttributes(archivePath, FileAttributes.Hidden);
                Login_Screen login_screen = new Login_Screen();
                login_screen.Show();
                Close();
            }
        }


        // ==========================================================
        // INÍCIO
        // ==========================================================

        private void start_click(object sender, RoutedEventArgs e)
        {
            page = 0;
            viewCard();
            ShowScreen(landing_page);
        }

        private void pass_page_Click(object sender, RoutedEventArgs e)
        {
            if (registerNumber > 3)
            {
                maxPage = (int)Math.Ceiling(registerNumber / 3.0) - 1;
            }
            if (page < maxPage){ 
                page += 1;
            }
            viewCard();
        }

        private void return_page_Click(object sender, RoutedEventArgs e)
        {
            if (page > 0)
            {
                page -= 1;
            }
            viewCard();
        }

        // ==========================================================
        // REGISTRAR CASA
        // ==========================================================

        private void register_click(object sender, RoutedEventArgs e)
        {
            ShowScreen(register_screen);

            // Valores iniciais
            area_slide.Value = 10;
            price_slide.Value = 100000;

            register_location.Clear();

            register_bathroom.SelectedIndex = -1;
            register_bedroom.SelectedIndex = -1;
            register_floor.SelectedIndex = -1;

            has_furniture.IsChecked = false;
        }


        private void TextBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            // Evento mantido para compatibilidade com o XAML.
        }


        private void ComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            // Evento mantido para compatibilidade com o XAML.
        }


        private void CheckBox_Checked(
            object sender,
            RoutedEventArgs e)
        {
            // Evento mantido para compatibilidade com o XAML.
        }


        // ==========================================================
        // SLIDER DE PREÇO
        // ==========================================================

        private void Slider_PriceChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (price_register != null)
            {
                double value = e.NewValue;

                price_register.Text =
                    $"Preço: R$ {value:N0}";
            }
        }


        // ==========================================================
        // SLIDER DE ÁREA
        // ==========================================================

        private void Slider_AreaChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e)
        {
            if (area_register != null)
            {
                double value = e.NewValue;

                area_register.Text =
                    $"Área: {value:N0} m²";
            }
        }


        // ==========================================================
        // CRIAR REGISTRO
        // ==========================================================

        private void confirm_Click(
            object sender,
            RoutedEventArgs e)
        {
            // ------------------------------------------
            // VALIDAÇÃO
            // ------------------------------------------

            if (string.IsNullOrWhiteSpace(register_location.Text))
            {
                MessageBox.Show(
                    "Digite a localização da casa.",
                    "Campo obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                register_location.Focus();
                return;
            }


            if (register_floor.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Selecione a quantidade de andares.",
                    "Campo obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (register_bedroom.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Selecione a quantidade de quartos.",
                    "Campo obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (register_bathroom.SelectedIndex < 0)
            {
                MessageBox.Show(
                    "Selecione a quantidade de banheiros.",
                    "Campo obrigatório",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // ------------------------------------------
            // VALORES
            // ------------------------------------------

            string location = register_location.Text.Trim();

            int area =
                Convert.ToInt32(area_slide.Value);

            double price =
                price_slide.Value;

            bool hasFurniture =
                has_furniture.IsChecked == true;


            // IMPORTANTE:
            // SelectedIndex começa em 0.
            // Por isso adicionamos 1 para obter o valor real.

            int bedrooms =
                register_bedroom.SelectedIndex + 1;

            int bathrooms =
                register_bathroom.SelectedIndex + 1;

            int floors =
                register_floor.SelectedIndex + 1;


            // ------------------------------------------
            // SALVAR
            // ------------------------------------------

            try
            {
                GlobalFunctions.SaveHouse(
                    location,
                    area,
                    price,
                    hasFurniture,
                    bedrooms,
                    bathrooms,
                    floors);


                MessageBox.Show(
                    "Casa cadastrada com sucesso!",
                    "Cadastro realizado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                // --------------------------------------
                // LIMPAR FORMULÁRIO
                // --------------------------------------

                area_slide.Value = 10;
                price_slide.Value = 100000;

                register_location.Clear();

                register_bathroom.SelectedIndex = -1;
                register_bedroom.SelectedIndex = -1;
                register_floor.SelectedIndex = -1;

                has_furniture.IsChecked = false;


                // Atualiza a tabela
                viewCard();


                // Volta para a tela inicial
                ShowScreen(landing_page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Não foi possível cadastrar a casa.\n\n" +
                    ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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


        // ==========================================================
        // REMOVER CASA ESPECÍFICA
        // ==========================================================

        private void remove_specific_button_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowScreen(remove_specific_screen);
        }


        // ==========================================================
        // REMOVER TODAS
        // ==========================================================

        private void remove_all_button_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Tem certeza que deseja remover TODAS as casas?\n\n" +
                "Essa ação não poderá ser desfeita.",
                "Confirmar remoção",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);


            if (result != MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                GlobalFunctions.RemoveHouse();

                MessageBox.Show(
                    "Todas as casas foram removidas.",
                    "Remoção concluída",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                viewCard();

                ShowScreen(landing_page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao remover as casas.\n\n" +
                    ex.Message,
                    "Erro",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void switch_click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void landing_page_data_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}