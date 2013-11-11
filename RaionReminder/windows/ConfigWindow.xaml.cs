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
using System.ComponentModel;
using props = RaionReminder.Properties.Settings;
using System.Security;
using System.Collections.ObjectModel;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для Config.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private ObservableCollection<JudgeGroup> judgegroups;

        BackgroundWorker refresh_worker;

        AppSettings mySettings;

        public ConfigWindow()
        {

            mySettings = ((App)Application.Current).mySettings;

            if (mySettings.JudgeGroups == null) mySettings.JudgeGroups = new ObservableCollection<JudgeGroup>();
            
            InitializeComponent();

        }

        private void RefreshGroupsButton_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<JudgeGroup> noGroupsList = new ObservableCollection<JudgeGroup>();
            noGroupsList.Add(new JudgeGroup() { id = 0, name = "Загрузка..." });
            GroupsList.ItemsSource = noGroupsList;
            GroupsList.SelectedIndex = 0;
            GroupsList.IsEnabled = false;
            RefreshGroupsButton.IsEnabled = false;
            refresh_worker.RunWorkerAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mySettings.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);


            this.DataContext = mySettings;
            string pass = mySettings.exclPass;
            if (pass.Length > 0)
            {
                ExclPass.IsEnabled = false;
            }

            pass = mySettings.bsrPass;
            if (pass.Length > 0)
            {
                BsrPass.IsEnabled = false;
            }

            if (mySettings.UseLocalSettings) useLocalSettings.IsChecked = true;
            else useDBSettings.IsChecked = true;

            scanDays.Value = mySettings.scan_days;
            pubDays.Value = mySettings.pub_days;
            scanDays.ValueChanged += ParameterChanged;
            pubDays.ValueChanged += ParameterChanged;

            refresh_worker = new BackgroundWorker();
            refresh_worker.DoWork += new DoWorkEventHandler(refresh_worker_DoWork);
            refresh_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refresh_worker_RunWorkerCompleted);

            RefreshGroupsButton_Click(null, null);
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveButton.IsEnabled = true;
        }



        void refresh_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            judgegroups = (ObservableCollection<JudgeGroup>)e.Result;

            RefillGroups();

            if (judgegroups.Count > 0)
            {
                GroupsList.SelectedIndex = 0;
                GroupsList.IsEnabled = true;
            }
            else
            {
                ObservableCollection<JudgeGroup> noGroupsList = new ObservableCollection<JudgeGroup>();
                noGroupsList.Add(new JudgeGroup() { id = 0, name = "Невозможно загрузить список групп" });
                GroupsList.ItemsSource = noGroupsList;
                GroupsList.SelectedIndex = 0;

                if (ExpertSettingsPanel.Visibility == System.Windows.Visibility.Collapsed)
                {
                    if (mySettings.AdminMode)
                    {
                        ShowExpertPanel_Click(null, null);
                        MessageBox.Show("Невозможно загрузить список групп судей. \nПожалуйста проверьте настройки подключения к базам данных.");
                    }
                    else
                    {
                        MessageBox.Show("Невозможно загрузить список групп судей. Позовите системного администратора, чтобы настроить программу");
                    }
                }
            }
            RefreshGroupsButton.IsEnabled = true;

        }

        private void RefillGroups()
        {
            ObservableCollection<JudgeGroup> judges = new ObservableCollection<JudgeGroup>();
            
            foreach (JudgeGroup jg in judgegroups)
            {
                int current_group = jg.id;

                bool in_usergroups = false;

                foreach (JudgeGroup ug in mySettings.JudgeGroups)
	            {
		            if (current_group == ug.id) {
                        in_usergroups = true;
                        break;
                    }
	            }

                if (!in_usergroups)
                {
                    judges.Add(jg);
                }
            }

            GroupsList.ItemsSource = judges;

            if (GroupsList.HasItems) GroupsList.SelectedIndex = 0;
        }

        void refresh_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = DataModel.GetJudgeGroups(mySettings.SDPLinkName);
            }
            catch (Exception ex)
            {
                Logging.Log("judge groups refresh", ex.Message + "\r\n" + ex.StackTrace);
                e.Result = new ObservableCollection<JudgeGroup>();
            }
        }

        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {

            JudgeGroup jg = (JudgeGroup)GroupsList.SelectedItem;

            mySettings.JudgeGroups.Add(jg);

            ((ObservableCollection<JudgeGroup>)GroupsList.ItemsSource).Remove(jg);

            if (GroupsList.Items.Count > 0) GroupsList.SelectedIndex = 0;
            Default_PropertyChanged(null, null);
        }
       
        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            JudgeGroup remove_item = (JudgeGroup)MyGroupsList.SelectedItem;

            foreach (JudgeGroup item in mySettings.JudgeGroups)
            {
                if (item.id == remove_item.id)
                {
                    mySettings.JudgeGroups.Remove(item);
                    break;
                }
            }

            RefillGroups();
            if (MyGroupsList.HasItems)
                MyGroupsList.SelectedIndex = 0;
            MyGroupsList.Focus();
            Default_PropertyChanged(null, null);
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            if (ExpertSettingsPanel.Visibility == System.Windows.Visibility.Collapsed)
            {
                mySettings.SaveLocal();
                SaveButton.IsEnabled = false;
            }
            else
            {
                if (ExclPass.IsEnabled) mySettings.exclPass = ExclPass.Password;
                if (BsrPass.IsEnabled) mySettings.bsrPass = BsrPass.Password;
                mySettings.scan_days = scanDays.Value;
                mySettings.pub_days = pubDays.Value;

                try {
	                mySettings.Save();
	               
	                SaveButton.IsEnabled = false;
					              
	                string pass = mySettings.exclPass;
	                if (pass.Length > 0)
	                {
	                    ExclPass.Password = "";
	                    ExclPass.IsEnabled = false;
	                }
	
	
	                pass = mySettings.bsrPass;
	                if (pass.Length > 0)
	                {
	                    BsrPass.Password = "";
	                    BsrPass.IsEnabled = false;
	                }
                }
                catch (Exception ex) {
                	if (ex.Message.IndexOf("SETTINGS") != -1) {
                		MessageBox.Show("Сохранение в БД завершилось с ошибкой. Скорее всего Вы не обновили базу данных.");
                	}
                }
                
            }
        }

        

        private void DeletePassword_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string param = (string)e.Parameter;
            switch (param)
            {
                case "My":
                    ExclPass.IsEnabled = true;
                break;
                case "BSR":
                    BsrPass.IsEnabled = true;
            break;
            }
        }

        private void CheckConnection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            switch ((string)e.Parameter)
            {
                case "My":
                    try
                    {
                        string pass;
                        if (ExclPass.IsEnabled) pass = ExclPass.Password;
                        else pass = mySettings.exclPass;
                        if (DataModel.CheckFireBirdConnection(ExclBase.Text, ExclName.Text, pass)) MessageBox.Show("Соединение успешно.");
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Подключение неудачно: "+ex.Message);
                    }
                    break;
                case "BSR":
                    try
                    {
                        string pass;
                        if (BsrPass.IsEnabled) pass = BsrPass.Password;
                        else pass = mySettings.bsrPass;
                        if (DataModel.CheckOracleConnection(BsrBase.Text, BsrName.Text, pass)) MessageBox.Show("Соединение успешно.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Подключение неудачно: " + ex.Message);
                    }
                    break;
            }
        }

        private void CheckConnection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowExpertPanel_Click(object sender, RoutedEventArgs e)
        {
            if (ExpertSettingsPanel.Visibility == System.Windows.Visibility.Collapsed)
            {
                ExpertSettingsPanel.Visibility = System.Windows.Visibility.Visible;
                ShowExpertPanel.Content = "Обычный режим";
                if (mySettings.UseLocalSettings) SaveButton.Content = "Применить";
                else SaveButton.Content = "Сохранить в БД";
            }
            else
            {
                ExpertSettingsPanel.Visibility = System.Windows.Visibility.Collapsed;
                ShowExpertPanel.Content = "Режим эксперта";
                SaveButton.Content = "Применить";
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mySettings.Reload();
        }

        private void ParameterChanged(object sender, RoutedEventArgs e)
        {
            Default_PropertyChanged(null, null);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (SaveButton.IsEnabled)
            {
                if (MessageBox.Show("В конфигурацию были внесены изменения. Сохранить?", "Конфигурация изменена", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SaveButton_Click(null,null);
                }
                else {
                	mySettings.Reload();
                }
            }
        }

        private void ResetSettings_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Все настройки будут сброшены до значений по умолчанию. Эту операцию нельзя отменить. Сбрасываем?", "Сброс настроек", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                mySettings.Reset();
            }
        }

        private void useDBSettings_Checked(object sender, RoutedEventArgs e)
        {
            if (ExpertSettingsPanel.Visibility == Visibility.Visible)
            {
                SaveButton.Content = "Сохранить в БД";
                mySettings.UseLocalSettings = false;
            }
        }

        private void useLocalSettings_Checked(object sender, RoutedEventArgs e)
        {
            if (ExpertSettingsPanel.Visibility == Visibility.Visible)
            {
                SaveButton.Content = "Применить";
                mySettings.UseLocalSettings = true;
            }
        }

        private void LoadDBSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ExclPass.IsEnabled) mySettings.exclPass = ExclPass.Password;
                mySettings.GetAllSettingsFromDB();
                string pass = mySettings.exclPass;
                if (pass.Length > 0)
                {
                    ExclPass.Password = "";
                    ExclPass.IsEnabled = false;
                }

                pass = mySettings.bsrPass;
                if (pass.Length > 0)
                {
                    BsrPass.Password = "";
                    BsrPass.IsEnabled = false;
                }
                MessageBox.Show("Настройки успешно загружены");
            }
            catch (ApplicationException)
            {
                MessageBox.Show("БД содержит таблицу настроек, но она пуста. Возможно вы пока не сохраняли никаких настроек.");
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Unable to complete network request") >= 0) MessageBox.Show("Невозможно загрузить настройки. Проверьте правильность адреса сервера БД");
                else if (ex.Message.IndexOf("Error while trying to open file") > 0) MessageBox.Show("Невозможно загрузить настройки. Проверь правильность пути БД");
                else if (ex.Message.IndexOf("Your user name and password are not defined") >= 0) MessageBox.Show("Невозможно загрузить настройки. Проверьте правильность имени пользователя и пароля");
                else if (ex.Message.IndexOf("Table unknown") >= 0) MessageBox.Show("Невозможно загрузить настройки. Похоже вы не обновили базу данных.");
            }
        }

    }
}
