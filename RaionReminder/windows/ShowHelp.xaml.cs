using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для ShowHelp.xaml
    /// </summary>
    public partial class ShowHelp : Window
    {
        public ShowHelp()
        {
            InitializeComponent();
        }

        private void yesbutton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
