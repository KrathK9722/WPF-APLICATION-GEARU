
using System.Windows;


namespace Projeto_GerenciamentoDeUsuarios
{
    public partial class CardWindow : Window
    {
        public CardWindow(CardVision card)
        {
            InitializeComponent();

            card.HorizontalAlignment = HorizontalAlignment.Stretch;
            card.VerticalAlignment = VerticalAlignment.Stretch;
            card.Width = double.NaN;
            card.Height = double.NaN;
            card.MinWidth = 390;
            card.MinHeight = 640;

            CardHost.Children.Clear();
            CardHost.Children.Add(card);
        }
    }
}