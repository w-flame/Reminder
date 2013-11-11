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
    /// Логика взаимодействия для DenyReason.xaml
    /// </summary>
    public partial class DenyReason : Window
    {
        public DenyReason()
        {
            InitializeComponent();
        }

        public string reasonText;
        public PublishListItem item;
        public DateTime caseDate;

        private void DenyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Reason.SelectedIndex == 0)
            {
                Reason.Style = null;
                Reason.Style = this.FindResource("WrongTextField") as Style;
                return;
            }
            
            reasonText = ((ComboBoxItem)Reason.SelectedItem).Content.ToString();
            
            if (Reason.SelectedIndex == 8 && JudgeDecisionComment.Text != "") reasonText += ": "+JudgeDecisionComment.Text;
            
            DialogResult = true;
            Close();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DescriptionText.Text = DescriptionText.Text.Replace("#CASENUM#", item.CaseNumber);
            DescriptionText.Text = DescriptionText.Text.Replace("#JUDGE#", item.JudgeName);
            DescriptionText.Text = DescriptionText.Text.Replace("#DATE#", caseDate.ToShortDateString());
            Reason.Focus();
        }
    }
}
