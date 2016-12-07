using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Graph.View
{
    /// <summary>
    /// Interaction logic for AssignWeight.xaml
    /// Вікно задання ваги
    /// </summary>
    public partial class AssignWeight : Window
    {
        public AssignWeight()
        {
            InitializeComponent();
            Weight = 0;
        }

        public int Weight { get; private set; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {            
            if (!string.IsNullOrWhiteSpace(txtWeight.Text))
            {
                Weight = int.Parse(txtWeight.Text);
                windowClose = true;
                this.Close();
            }
        }
        private void txtWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!windowClose)
                e.Cancel = true;
        }

        private bool windowClose = false;

    }
}