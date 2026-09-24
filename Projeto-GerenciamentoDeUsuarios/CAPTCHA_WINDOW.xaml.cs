using System.Windows;
using System.Windows.Controls;


namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Lógica interna para CAPTCHA_WINDOW.xaml
    /// </summary>
    public partial class CAPTCHA_WINDOW : Window
    {
        private string captcha;
        public bool Verified { get; private set; }
        public string Origem { get; private set; }
       
        // Para janela MainWindow
        public CAPTCHA_WINDOW(MainWindow owner)
        {
            InitializeComponent();
            Owner = owner;
            Origem = "registro";
            InicializarCaptcha();
        }

        // Construtor para quando for chamado por OUTRA janela
        public CAPTCHA_WINDOW(Window owner, string origem)
        {
            InitializeComponent();
            Owner = owner;
            Origem = origem;
            InicializarCaptcha();
        }
        private void InicializarCaptcha()
        {
            captcha = GlobalFunctions.Verify_robot();
            random_value.Content = captcha;
        }
        private void captcha_box_TextChanged(object sender, TextChangedEventArgs e)
        {
            Verified = (captcha_box.Text == captcha);
        }

        private void confirm_captcha_click(object sender, RoutedEventArgs e)
        {
            DialogResult = Verified;
            Close();
        }
    }
}
