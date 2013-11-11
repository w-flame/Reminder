using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
         private BackgroundWorker unpublishedrefresh_worker = new BackgroundWorker();
         private BackgroundWorker exclude_worker = new BackgroundWorker();
         private BackgroundWorker caseinfo_worker = new BackgroundWorker();
         private BackgroundWorker excel_worker = new BackgroundWorker();
         private DispatcherTimer RefreshTimer;
         private ObservableCollection<PublishListItem> listItems = new ObservableCollection<PublishListItem>();
         private string bsrPath;

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();

            unpublishedrefresh_worker.DoWork += new DoWorkEventHandler(unpublishedrefresh_worker_DoWork);
            unpublishedrefresh_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(unpublishedrefresh_worker_RunWorkerCompleted);

            exclude_worker.DoWork += new DoWorkEventHandler(exclude_worker_DoWork);
            exclude_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(exclude_worker_RunWorkerCompleted);

            caseinfo_worker.DoWork += new DoWorkEventHandler(caseinfo_worker_DoWork);
            caseinfo_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(caseinfo_worker_RunWorkerCompleted);

            excel_worker.DoWork += new DoWorkEventHandler(excel_worker_DoWork);
            
            RefreshTimer = new DispatcherTimer(DispatcherPriority.ContextIdle);
            //!!
            RefreshTimer.Interval = new TimeSpan(0, 30, 0);
            RefreshTimer.Tick += new EventHandler(RefreshTimer_Tick);
            RefreshTimer.Start();

            UnpublishedListBox.ItemsSource = listItems;
           

        }

        void excel_worker_DoWork(object sender, DoWorkEventArgs e)
        {
        	ObservableCollection<PublishListItem> items = (ObservableCollection<PublishListItem>)e.Argument;
        	ExcelExport.ExportMainWindow(items,mySettings.ShowDaysAfterConsideration,mySettings.pub_days);
        }

        void caseinfo_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NotPublishedListItem item = (NotPublishedListItem)e.Result;
            if (item == null)
            {
                noCaseInfo.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                noCaseInfo.Visibility = System.Windows.Visibility.Hidden;
                caseDetails.DataContext = item;
            }
            CaseInfoThrobber.Visibility = System.Windows.Visibility.Hidden;
        }

        void caseinfo_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] param = (int[])e.Argument;
            try
            {
            	e.Result = DataModel.GetCaseInfo(param[0], param[1], param[2], Convert.ToBoolean(param[3]),mySettings.SDPLinkName);

            }
            catch (Exception ex)
            {

                Logging.Log("case info", ex.Message + "\n" + ex.StackTrace);
                e.Result = null;
            }
        }


        int[] caseInfoParams = new int[4];
        DispatcherTimer caseInfoTimer;

        private void UnpublishedListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnpublishedListBox.SelectedItem != null)
            {
                if (caseInfoTimer == null)
                {
                    caseInfoTimer = new DispatcherTimer();
                    caseInfoTimer.Interval = TimeSpan.FromMilliseconds(200.0);
                    caseInfoTimer.Tick += new EventHandler(caseInfoTimer_Tick);
                }

                if (caseInfoTimer.IsEnabled)
                {
                    caseInfoTimer.Stop();
                }

                PublishListItem item = (PublishListItem)UnpublishedListBox.SelectedItem;
                caseInfoParams[0] = item.id;
                caseInfoParams[1] = item.vidpr;
                caseInfoParams[2] = item.stage;
                caseInfoParams[3] = Convert.ToInt32(item.canceled);

                caseInfoTimer.Start();
            }
            else {
            	caseDetails.Visibility = Visibility.Collapsed;
            }
            
            e.Handled = true;
        }

        void caseInfoTimer_Tick(object sender, EventArgs e)
        {
            if (caseinfo_worker.IsBusy) return;
			
            int[] localParams = new int[4];
            localParams[0] = caseInfoParams[0];
            localParams[1] = caseInfoParams[1];
            localParams[2] = caseInfoParams[2];
            localParams[3] = caseInfoParams[3];

            CaseInfoThrobber.Visibility = System.Windows.Visibility.Visible;
            caseinfo_worker.RunWorkerAsync(localParams);
            caseInfoTimer.Stop();
            caseDetails.Visibility = Visibility.Visible;
        }

        void exclude_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                MessageBox.Show("Публикация успешно запрещена");
                ReloadUnpublished();
            }
            else
            {
                UnpublishedThrobber.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show("В процессе операции произошла ошибка. Пожалуйста сообщите кому следует.");
            }
        }

        void exclude_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = DataModel.AddToExcluded(mySettings.SDPLinkName,(ExcludeInfo)e.Argument,mySettings.FirstGrAfterConsideration);
            }
            catch (Exception ex)
            {
                Logging.Log("add to excluded", ex.Message + "\n" + ex.StackTrace);
                e.Result = false;
            }
        }

        void unpublishedrefresh_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                NoUnpublishedInfo.Text = "Не могу получить информацию.\nПопробуйте обновить информацию позже.\nЕсли не поможет, то позовите системного администратора.";
                NoUnpublishedInfo.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                List<PublishListItem> Items = (List<PublishListItem>)e.Result;

                if (Items.Count == 0)
                {
                    UnpublishedListBox.ItemsSource = null;
                    NoUnpublishedInfo.Text = "Поздравляю! Все дела опубликованы";
                    NoUnpublishedInfo.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    ObservableCollection<PublishListItem> items_collection = new ObservableCollection<PublishListItem>();
                    string uploaded_cases = "";
                    foreach (PublishListItem item in Items)
                    {

                        //Ищем дела, которые только что загрузили в БСР
                        foreach (PublishListItem last_item in listItems)
                        {
                            if (item.id == last_item.id && last_item.inBSR == false && item.inBSR == true)
                            {
                                uploaded_cases += item.CaseNumber + ", ";
                                break;
                            }
                        }

                        items_collection.Add(item);
                    }

                    listItems = items_collection;
                    UnpublishedListBox.ItemsSource = listItems;
                    if (uploaded_cases != "")
                    {
                        uploaded_cases = uploaded_cases.Substring(0, uploaded_cases.Length - 2);
                        tb.ShowBalloonTip("Новые дела загружены в БСР", "В БСР загружены следующие дела:\n" + uploaded_cases, BalloonIcon.Info);
                    }

                    NoUnpublishedInfo.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            UnpublishedThrobber.Visibility = System.Windows.Visibility.Collapsed;
            CommandManager.InvalidateRequerySuggested();
        }

        void unpublishedrefresh_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (mySettings.JudgeGroups == null || mySettings.JudgeGroups.Count == 0)
            {
            	e.Result = null;
                return;
            }

            try
            {
                e.Result = DataModel.GetUnpublishedCases(mySettings.JudgeGroups,mySettings.FirstGrAfterConsideration, mySettings.FirstAdmAfterConsideration, mySettings.ShowCanceledCivil,mySettings.AutoExcludeCanceledCivil,mySettings.ShowDaysAfterConsideration,mySettings.pub_days,mySettings.SDPLinkName);
            }
            catch (Exception ex)
            {
                Logging.Log("get unpublished info", ex.Message + "\r\n" + ex.StackTrace);
                e.Result = null;
            }
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            ReloadUnpublished();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ReloadUnpublished();
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        public class MARGINS
        {
            public int cxLeftWidth, cxRightWidth,
                cyTopHeight, cyBottomHeight;
        }

        [DllImport("user32.dll")]
        private extern static Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);
        [DllImport("user32.dll")]
        private extern static Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);

        private TaskbarIcon tb;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Ищем в папке с программой файл nobsrweb и если он там есть, то прячем кнопку Экспортировать решение
            bool bsrweb = true; ;
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (System.IO.File.Exists(path + @"\nobsrweb"))
            {
                OpenSite.Visibility = System.Windows.Visibility.Collapsed;
                bsrweb = false;
            }

            #region Commands
            CommandBinding cb = new CommandBinding(Commands.Commands.OpenWindow, openWindowCommandHandler, openWindowCanExecuteHandler);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.RefreshMain,RefreshMain_Executed,RefreshMain_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.DenyPublication, DenyPublication_Executed, DenyPublication_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.OpenClient, OpenClient_Executed, OpenClient_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.OpenSite, OpenSite_Executed, OpenSite_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.ShowHelp, ShowHelp_Executed, ShowHelp_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.ShowAbout, ShowAbout_Executed, ShowAbout_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.ShowList, ShowList_Executed, ShowList_CanExecute);
            this.CommandBindings.Add(cb);
            cb = new CommandBinding(Commands.Commands.ExportExcel, ExportExcel_Executed, ExportExcel_CanExecute);
            this.CommandBindings.Add(cb);
            #endregion

            #region TeskbarIcon
            tb = new TaskbarIcon();
            tb.Icon = Properties.Resources.icon16;

            ContextMenu tbMenu = new ContextMenu();
            MenuItem miShow = new MenuItem();
            miShow.Header = "Показать окно";
            miShow.FontWeight = System.Windows.FontWeights.Bold;
            miShow.Command = Commands.Commands.OpenWindow;
            miShow.CommandTarget = this;
            tbMenu.Items.Add(miShow);

            Image im;

            if (bsrweb)
            {

                MenuItem miOpenSite = new MenuItem();
                miOpenSite.Header = "Запуск веб-клиента БСР";
                miOpenSite.Command = Commands.Commands.OpenSite;
                miOpenSite.CommandTarget = this;
                im = new Image();
                im.Source = new BitmapImage(new Uri("pack://application:,,,/Reminder;component/images/upload.png"));
                miOpenSite.Icon = im;
                tbMenu.Items.Add(miOpenSite);

            }

            MenuItem miOpenClient = new MenuItem();
            miOpenClient.Header = "Запустить клиент БСР";
            miOpenClient.Command = Commands.Commands.OpenClient;
            miOpenClient.CommandTarget = this;
            im = new Image();
            im.Source = new BitmapImage(new Uri("pack://application:,,,/Reminder;component/images/bsr16.png"));
            miOpenClient.Icon = im;
            tbMenu.Items.Add(miOpenClient);

            MenuItem miShowAbout = new MenuItem();
            miShowAbout.Header = "О программе";
            miShowAbout.Command = Commands.Commands.ShowAbout;
            miShowAbout.CommandTarget = this;
            tbMenu.Items.Add(miShowAbout);

            tbMenu.Items.Add(new Separator());
            MenuItem miExit = new MenuItem();
            miExit.Header = "Выход";
            miExit.Click += new RoutedEventHandler(miExit_Click);
            tbMenu.Items.Add(miExit);
            tb.ContextMenu = tbMenu;

            tb.DoubleClickCommand = Commands.Commands.OpenWindow;
            tb.DoubleClickCommandTarget = this;
            #endregion  

            #region NoMaximizeButton

            Int32 GWL_STYLE = -16;
            Int32 WS_MAXIMIZEBOX = 0x10000;
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            Int32 windowLong = GetWindowLong(hWnd, GWL_STYLE);
            windowLong = windowLong & ~WS_MAXIMIZEBOX;
            SetWindowLong(hWnd, GWL_STYLE, windowLong);
            #endregion

            #region Aero
            if (Environment.OSVersion.Version.Major >= 6 && DwmIsCompositionEnabled())
            {
                
                // Get the current window handle
                IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                this.Background = Brushes.Transparent;

                // Set the proper margins for the extended glass part
                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                int result = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                
            }
            #endregion


            


            if (mySettings.LaunchMinimized)
            {
                WindowState = System.Windows.WindowState.Minimized;
            }

         
            ReloadUnpublished();
        }

        private void RefreshMain_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        	ReloadUnpublished();
        }

        private void RefreshMain_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        	if (unpublishedrefresh_worker.IsBusy || excel_worker.IsBusy) e.CanExecute = false;
			else e.CanExecute = true;
        }
        
        private void ShowList_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ListWindow lw = new ListWindow();
            lw.Show();
        }

        private void ShowList_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        
        

        private void ExportExcel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        	ObservableCollection<PublishListItem> items = (ObservableCollection<PublishListItem>)UnpublishedListBox.ItemsSource;
        	excel_worker.RunWorkerAsync(items);
        }

        private void ExportExcel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        	if (unpublishedrefresh_worker.IsBusy || excel_worker.IsBusy) e.CanExecute = false;
        	else if (UnpublishedListBox.ItemsSource == null) e.CanExecute = false;
			else e.CanExecute = ExcelExport.Check();
        }

        public bool realExit = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (realExit)
            {
                e.Cancel = false;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Minimized;
                this.ShowInTaskbar = false;
                e.Cancel = true;
            }
        }    
        void miExit_Click(object sender, RoutedEventArgs e)
        {
            realExit = true;
            Close();
        }

        private void MyMainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Minimized) this.ShowInTaskbar = false;
        }

        void openWindowCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.ShowInTaskbar = true;
            }
            this.Activate();
        }
        void openWindowCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        public AppSettings mySettings;
        private void LoadSettings()
        {
            bool DefaultWindowPosition = false;
            bool AdminMode = false;
            bool dbsettings = false;
            int new_db_arg = 0;
            string db_base = "";
            string db_user = "";
            string db_pass = "";
            string sdplink = "IBASEDATA";
            foreach (string arg in Environment.GetCommandLineArgs())
            {
                if (arg == "/defaultWindow") DefaultWindowPosition = true;
                else if (arg == "/admin") AdminMode = true;
                else if (arg.IndexOf("/sdplink:") == 0 && arg.Length > 9) sdplink = arg.Substring(9);
                else if (arg == "/db")
                {
                    new_db_arg = 1;
                    dbsettings = true;
                }
                else if (new_db_arg > 0)
                {
                    switch (new_db_arg)
                    {
                        case 1: db_base = arg; break;
                        case 2: db_user = arg; break;
                        case 3: db_pass = arg; break;
                    }
                    new_db_arg++;
                }
            }


            App MyApplication = ((App)Application.Current);
            try
            {
                if (dbsettings && new_db_arg == 4)
                    MyApplication.mySettings = new AppSettings(AdminMode,db_base,db_user,db_pass);
                else
                    MyApplication.mySettings = new AppSettings(AdminMode);
                MyApplication.mySettings.SDPLinkName = sdplink;
            }
            catch (Exception ex)
            {
                Logging.Log("load settings",ex.Message + " " + ex.StackTrace);
                if (AdminMode) MessageBox.Show("При загрузке настроек произошла ошибка: " + ex.Message);
                else
                {
                    MessageBox.Show("При загрузке настроек произошла ошибка. Позовите системного администратора");
                    
                }
                Close();
                return;
            }
            
            mySettings = MyApplication.mySettings;


            if (!DefaultWindowPosition)
            {
                if (mySettings.MainWindowWidth > 0) this.Width = mySettings.MainWindowWidth;
                if (mySettings.MainWindowHeight > 0) this.Height = mySettings.MainWindowHeight;


                if (mySettings.MainWindowX >= 0)
                    if (mySettings.MainWindowX >= SystemParameters.VirtualScreenWidth)
                        this.Left = SystemParameters.VirtualScreenWidth - this.Width;
                    else
                        this.Left = mySettings.MainWindowX;
                if (mySettings.MainWindowY >= 0)
                    if (mySettings.MainWindowY >= SystemParameters.VirtualScreenHeight)
                        this.Top = SystemParameters.VirtualScreenHeight - this.Height;
                    else
                        this.Top = mySettings.MainWindowY;
            }

            RegistryKey reg = Registry.LocalMachine.OpenSubKey("Software\\BSR\\Client");
            bsrPath = "";
            if (reg != null)
            {
                bsrPath = (string)reg.GetValue("PathInstall", "") + "\\Client\\frmLogin.exe";
            }

            if (mySettings.JudgeGroups != null && mySettings.JudgeGroups.Count >0)
            {
                RefreshButton.IsEnabled = true;
            }
            else
            {
                RefreshButton.IsEnabled = false;
            }

            if (mySettings.ShowDaysAfterConsideration)
            {
                UnpublishedListBox.Items.SortDescriptions.Clear();
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("DaysCount", ListSortDirection.Descending));
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("CaseNumber", ListSortDirection.Ascending));
            }
            else
            {
                UnpublishedListBox.Items.SortDescriptions.Clear();
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("DaysCount", ListSortDirection.Ascending));
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("CaseNumber", ListSortDirection.Ascending));
            }

        }

        private void SaveSettings()
        {
            mySettings.MainWindowX = this.Left;
            mySettings.MainWindowY = this.Top;
            mySettings.MainWindowWidth = this.ActualWidth;
            mySettings.MainWindowHeight = this.ActualHeight;
            mySettings.SaveLocal();
        }

        public void ReloadUnpublished()
        {
            UnpublishedListBox.ItemsSource = null;
            NoUnpublishedInfo.Visibility = System.Windows.Visibility.Collapsed;

            RefreshButton.IsEnabled = true;
            if (mySettings.JudgeGroups != null)
            {
                if (mySettings.JudgeGroups.Count == 0)
                {
                    RefreshButton.IsEnabled = false;
                	NoJudgeGroupsBorder.Visibility = System.Windows.Visibility.Visible;
                    addgroups.Style = (Style)FindResource("FlashingTextButton");
                    return;
                }
                else
                {
                    addgroups.Style = (Style)FindResource("TextButton");
                    NoJudgeGroupsBorder.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                NoJudgeGroupsBorder.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            if (unpublishedrefresh_worker.IsBusy) return;

            if (mySettings.JudgeGroups != null)
            {
                if (mySettings.JudgeGroups.Count != 0)
                {
                    NoUnpublishedInfo.Visibility = System.Windows.Visibility.Collapsed;
                    UnpublishedThrobber.Tooltip = "Загрузка";
                    UnpublishedThrobber.Visibility = System.Windows.Visibility.Visible;
                    unpublishedrefresh_worker.RunWorkerAsync();
                }
            }
        }

        private void MyMainWindow_Closed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void ConfigButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshTimer.Stop();
            ConfigWindow cw = new ConfigWindow();
            cw.Owner = this;
            cw.ShowDialog();
            if (mySettings.ShowDaysAfterConsideration)
            {
                UnpublishedListBox.Items.SortDescriptions.Clear();
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("DaysCount", ListSortDirection.Descending));
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("CaseNumber", ListSortDirection.Ascending));
            }
            else
            {
                UnpublishedListBox.Items.SortDescriptions.Clear();
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("DaysCount", ListSortDirection.Ascending));
                UnpublishedListBox.Items.SortDescriptions.Add(new SortDescription("CaseNumber", ListSortDirection.Ascending));
            }
            ReloadUnpublished();
            RefreshTimer.Start();
        }

        private void ShowAbout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            About af = new About();
            af.Owner = this;
            af.ShowDialog();
        }

        private void ShowAbout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\help.chm";
            System.Diagnostics.Process.Start(path);
        }

        private void ShowHelp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)+"\\help.chm";
            if (System.IO.File.Exists(path)) e.CanExecute = true;
            else e.CanExecute = false;
        }

        private void OpenClient_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        	ProcessStartInfo psi = new ProcessStartInfo(bsrPath);
        	psi.WorkingDirectory = System.IO.Path.GetDirectoryName(bsrPath);
        	System.Diagnostics.Process.Start(psi);
        }

        private void OpenClient_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (bsrPath != "") e.CanExecute = true;
            else e.CanExecute = false;
        }

        private void OpenSite_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(mySettings.BSRURL);
        }

        private void OpenSite_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (mySettings.BSRURL != "") e.CanExecute = true;
            else e.CanExecute = false;
        }

        private void DenyPublication_Executed(object sender, ExecutedRoutedEventArgs e)
        {     
            DenyReason dr = new DenyReason();
            dr.Owner = this;
            dr.item = (PublishListItem)UnpublishedListBox.SelectedItem;
            
            
            DateTime dt = DateTime.Now;
            
            if (mySettings.ShowDaysAfterConsideration) {
            	dt = dt.AddDays(-dr.item.DaysCount);
            }
            else {
            	dt = dt.AddDays(Convert.ToDouble(dr.item.DaysCount-mySettings.pub_days));
            }
            dr.caseDate = dt;
            
            
            if (dr.ShowDialog().Value)
            {
                UnpublishedThrobber.Tooltip = "Исключение";
                UnpublishedThrobber.Visibility = System.Windows.Visibility.Visible;
                ExcludeInfo ei = new ExcludeInfo();
                ei.item = (PublishListItem)UnpublishedListBox.SelectedItem;
                ei.message = dr.reasonText;
                exclude_worker.RunWorkerAsync(ei);
            }
        }

        private void DenyPublication_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (exclude_worker.IsBusy) e.CanExecute = false;
            else if (this.UnpublishedListBox.SelectedIndex != -1)
            {
                if (mySettings.DenyOnlyInBSR)
                {
                    if (((PublishListItem)this.UnpublishedListBox.SelectedItem).inBSR) e.CanExecute = true;
                    else e.CanExecute = false;
                }
                else
                {
                    e.CanExecute = true;
                }
            }
        }

        

       


    
   }

   
}
