using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace Projeto_GerenciamentoDeUsuarios
{
    /// <summary>
    /// Lógica interna para Window1.xaml
    /// </summary>
    public partial class CardWindow : Window
    {
        public CardWindow(CardVision card)
        {
            InitializeComponent();
            this.Content = card;
        }
    }
}
