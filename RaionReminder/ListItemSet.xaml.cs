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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для UserControl.xaml
    /// </summary>
    public partial class ListItemSet : UserControl
    {
        public ListItemSet()
        {
            InitializeComponent();
        }

        private void ListItemSetControl_Loaded(object sender, RoutedEventArgs e)
        {
            App MyApplication = ((App)Application.Current);
            AppSettings mySettings = MyApplication.mySettings;
            if (mySettings.ShowDaysAfterConsideration) DaysCountLabel.ToolTip = "Прошло с момента рассмотрения";
            else DaysCountLabel.ToolTip = "Осталось дней для публикации";
        }
    }
}
