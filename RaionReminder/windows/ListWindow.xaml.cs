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
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Office.Interop.Excel;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для ListWindow.xaml
    /// </summary>
    public partial class ListWindow : System.Windows.Window
    {
        AppSettings mySettings;

        public ListWindow()
        {
            InitializeComponent();
            mySettings = ((App)System.Windows.Application.Current).mySettings;
        }

        private class stage_vid
        {
            public string header { get; set; }
            public int value;
        }

        Hashtable stages_types_link = new Hashtable(3);
        GetCasesListResult info = new GetCasesListResult();
        BackgroundWorker refresh_worker;
        BackgroundWorker export_worker;
        BackgroundWorker deny_worker;
        BackgroundWorker summary_worker;
        DateTime sDate;
        DateTime eDate;
        refreshBWParam lastParam;
        DateTime query_time = new DateTime();

        private struct exportBWParam
        {
            public GetCasesListResult res;
            public string vidpr;
            public string stage;
        }

        private struct refreshBWParam
        {
            public int vidpr;
            public int stage;
            public DateTime startDate;
            public DateTime endDate;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ObservableCollection<stage_vid> types = new ObservableCollection<stage_vid>();
            types.Add(new stage_vid() { header = "Уголовные", value = 1 });
            types.Add(new stage_vid() { header = "Гражданские", value = 2 });
            types.Add(new stage_vid() { header = "Административные", value = 5 });
            types.Add(new stage_vid() { header = "Производство по материалам", value = 4 });

            vidpr.ItemsSource = types;
            vidpr.SelectedIndex = 0;

            dateTo.SelectedDate = DateTime.Now;
            dateFrom.SelectedDate = DateTime.Now.AddMonths(-1);

            refresh_worker = new BackgroundWorker();
            refresh_worker.DoWork += new DoWorkEventHandler(refresh_worker_DoWork);
            refresh_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refresh_worker_RunWorkerCompleted);

            export_worker = new BackgroundWorker();
            export_worker.WorkerReportsProgress = true;
            export_worker.DoWork += new DoWorkEventHandler(export_worker_DoWork);
            export_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(export_worker_RunWorkerCompleted);
            export_worker.ProgressChanged += new ProgressChangedEventHandler(export_worker_ProgressChanged);

            summary_worker = new BackgroundWorker();
            summary_worker.WorkerReportsProgress = true;
            summary_worker.DoWork += new DoWorkEventHandler(summary_worker_DoWork);
            summary_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(summary_worker_RunWorkerCompleted);
            summary_worker.ProgressChanged += new ProgressChangedEventHandler(summary_worker_ProgressChanged);

            if (mySettings.AdminMode)
            {
                ContextMenu includeMenu = new ContextMenu();
                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem();
                item.Header = "Убрать выбранные из исключенных";
                item.Command = Commands.Commands.RemoveExcluded;
                includeMenu.Items.Add(item);

                ExcludedList.ContextMenu = includeMenu;

                this.CommandBindings.Add(new CommandBinding(Commands.Commands.RemoveExcluded, RemoveExcluded_Executed, RemoveExcluded_CanExecute));
            }
            
            #region Exclude menu
            
            ContextMenu excludeMenu = new ContextMenu();
            System.Windows.Controls.MenuItem item2 = new System.Windows.Controls.MenuItem();
            item2.Header = "Исключить из публикациии";
            Image im = new Image();
            im.Source = new BitmapImage(new Uri("pack://application:,,,/Reminder;component/images/page.png"));
            item2.Icon = im;
            
            
            System.Windows.Controls.MenuItem subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Затрагивающие безопасность государства";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Затрагивающие безопасность государства";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Возникающие из семейно-правовых отношений, в том числе по делам об усыновлении (удочерении) ребенка,\nдругим делам, затрагивающим права и законные интересы несовершеннолетних";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Возникающие из семейно-правовых отношений, в том числе по делам об усыновлении (удочерении) ребенка, другим делам, затрагивающим права и законные интересы несовершеннолетних";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "О преступлениях против половой неприкосновенности и половой свободы личности";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "О преступлениях против половой неприкосновенности и половой свободы личности";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Об ограничении дееспособности гражданина или о признании его недееспособным";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Об ограничении дееспособности гражданина или о признании его недееспособным";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "О принудительной госпитализации гражданина в психиатрический стационар и\nпринудительном психиатрическом освидетельствовании";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "О принудительной госпитализации гражданина в психиатрический стационар и принудительном психиатрическом освидетельствовании";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "О внесении исправлений или изменений в запись актов гражданского состояния";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "О внесении исправлений или изменений в запись актов гражданского состояния";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Об установлении фактов, имеющих юридическое значение";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Об установлении фактов, имеющих юридическое значение";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "По решению судьи";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "По решению судьи";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Закрытое судебное заседание";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Закрытое судебное заседание";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Дело не рассматривалось по существу";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Дело не рассматривалось по существу";
            item2.Items.Add(subItem);
            
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Объединенное дело";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Объединенное дело";
            item2.Items.Add(subItem);
            
            //Всегда должен быть последним!!!
            subItem = new System.Windows.Controls.MenuItem();
            subItem.Header = "Отменено вышестоящей инстанцией";
            subItem.Command = Commands.Commands.DenyPublication;
            subItem.CommandParameter = "Отменено вышестоящей инстанцией";
            item2.Items.Add(subItem);
            
            excludeMenu.Items.Add(item2);
            
            notPublishedList.ContextMenu = excludeMenu;
            
            this.CommandBindings.Add(new CommandBinding(Commands.Commands.DenyPublication,DenyPublication_Executed,DenyPublication_CanExecute));
            deny_worker = new BackgroundWorker();
            deny_worker.DoWork += new DoWorkEventHandler(deny_worker_DoWork);
            deny_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(deny_worker_RunWorkerCompleted);
            #endregion
        }

        void summary_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                loadingThrobber.Tooltip = "Выгрузка";
            }
        }

        void summary_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingThrobber.Visibility = System.Windows.Visibility.Collapsed;
        }

        void summary_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] param = (object[])e.Argument;
            
            bool UseMaterials = (bool)param[2];
            bool first_mode = (bool)param[3];
            bool canceled_civil = (bool)param[4];
            bool show_late = (bool)param[5];
            
            SummaryInfo info;
            try {
            	info = DataModel.GetSummaryInfo((DateTime)param[0], (DateTime)param[1], UseMaterials, canceled_civil, first_mode, mySettings.pub_days,mySettings.SDPLinkName);
            } catch (Exception ex) {
            	Logging.Log("get summary info",ex.Message+"\n"+ex.StackTrace);
            	return;
            }
            
            

            summary_worker.ReportProgress(1);
			try {
            	e.Result = ExcelExport.ExportSummary(first_mode, canceled_civil, show_late,  sDate,eDate,info, UseMaterials, mySettings.pub_days);
            } catch (Exception ex) {
            	Logging.Log("summary info excel export",ex.Message+"\n"+ex.StackTrace);
            	return;
            }
            return;
        }

        

        void deny_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	loadingThrobber.Visibility = Visibility.Hidden;
        	if ((bool)e.Result) {	
        		refreshButton_Click(null,null);
        	}
        	else {
        		MessageBox.Show("Операция завершилась с ошибкой. Подробности в reminder.log в папке Мои Документы");
        	}
        }

        void deny_worker_DoWork(object sender, DoWorkEventArgs e)
        {
        	try {
        		List<ExcludeInfo> items = (List<ExcludeInfo>)e.Argument;
	        	foreach (ExcludeInfo item in items) {
	        		DataModel.AddToExcluded(mySettings.SDPLinkName,item,mySettings.FirstGrAfterConsideration);
	        	}
        		e.Result = true;
        	} catch (Exception ex) {
        		Logging.Log("add to excluded (list)",ex.Message+"\n"+ex.StackTrace);
        		e.Result = false;
        	}
        }
        
        
        private bool ShowReasonForm = true;
        private void DenyPublication_Executed(object sender, ExecutedRoutedEventArgs e) {
        	List<ExcludeInfo> items  = new List<ExcludeInfo>();
        	 	
        	string reason = (string)e.Parameter;
        	if (reason == "По решению судьи" && ShowReasonForm) {
        		JudgeCommentForListWindow reasonWin = new JudgeCommentForListWindow();
        		reasonWin.Owner = this;
        		if (!reasonWin.ShowDialog().Value) {
        			return;
        		}
        		
        		ShowReasonForm = !reasonWin.hideReasonWindow;
        		if (reasonWin.comment != "") {
        			reason += ": "+reasonWin.comment;
        		}
        		
        	}
        	
        	foreach (NotPublishedListItem item in notPublishedList.SelectedItems) {
        		ExcludeInfo ex_item = new ExcludeInfo();
        		ex_item.item = new PublishListItem(item.id,0,item.number,item.Judge,item.vidpr,item.stage,item.inBSR,item.ReadyToPublish);			
				ex_item.message = reason;
				items.Add(ex_item);
        	}
        	loadingThrobber.Tooltip = "Запрещаем\nпубликацию";
        	loadingThrobber.Visibility = Visibility.Visible;
        	
        	deny_worker.RunWorkerAsync(items);       
        }
        
        private void DenyPublication_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            if (notPublishedList.SelectedItems.Count != 0)
            {
                if (mySettings.DenyOnlyInBSR)
                {
                    e.CanExecute = true;
                    foreach (NotPublishedListItem item in notPublishedList.SelectedItems)
                    {
                    	if (!item.inBSR)
                        {
                            e.CanExecute = false;
                            break;
                        }
                    }
                }
                else
                {
                    e.CanExecute = true;
                }
            }
            else e.CanExecute = false;
        	
            //Первая уголовная инстанция
            bool all_not_valid = false;
            if (((stage_vid)vidpr.SelectedItem).value == 1 && ((stage_vid)stage.SelectedItem).value == 113) {
            	all_not_valid = true;
            	foreach (NotPublishedListItem item in notPublishedList.SelectedItems)
                {
            		if (item.validityDate != new DateTime() && item.validityDate <= DateTime.Now)
                    {
                        all_not_valid = false;
                        break;
                    }
                }
            }
            	
        	System.Windows.Controls.MenuItem parent_item = ((System.Windows.Controls.MenuItem)notPublishedList.ContextMenu.Items[0]);
        	System.Windows.Controls.MenuItem cancel_item = (System.Windows.Controls.MenuItem)parent_item.Items[parent_item.Items.Count-1];
        	if (all_not_valid && cancel_item != null) {
        		cancel_item.Visibility = Visibility.Visible;
        	}
        	else {
        		cancel_item.Visibility = Visibility.Collapsed;
        	}
            
            
        }

        private void RemoveExcluded_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить дело(-а) из списка исключений?\r\nПримечание: автоматически исключаемые дела всё равно вернутся в этот список", "Разрешить публикацию", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int[] ids = new int[ExcludedList.SelectedItems.Count];
                int i = 0;
                foreach (ExcludedListItem item in ExcludedList.SelectedItems)
                {
                    ids[i] = item.excludeId;
                    i++;
                }

                try
                {
                    DataModel.RemoveFromExcluded(ids);

                    while (ExcludedList.SelectedItems.Count>0)
                    {
                        ((ObservableCollection<ExcludedListItem>)ExcludedList.ItemsSource).Remove((ExcludedListItem)ExcludedList.SelectedItems[0]);
                    }

                }
                catch (Exception ex)
                {
                    Logging.Log("remove from excluded", ex.Message + "\r\n" + ex.StackTrace);
                    MessageBox.Show("Во время операции произошла ошибка:" + ex.Message);
                }

            }
        }

        private void RemoveExcluded_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ExcludedList.SelectedItems.Count != 0) e.CanExecute = true;
            else e.CanExecute = false;
        }

        void export_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            exportProgress.Value  = e.ProgressPercentage;
        }

        void export_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            exportProgressPanel.Visibility = System.Windows.Visibility.Collapsed;
            if (!(bool)e.Result) MessageBox.Show("В процессе выгрузки произошла ошибка. Подробности файле reminder.log, находящемся в папке Мои документы");
        }

        void export_worker_DoWork(object sender, DoWorkEventArgs e)
        {
        	exportBWParam param = (exportBWParam)e.Argument;
        	e.Result = ExcelExport.ExportPeriod(query_time,sDate,eDate, param.res,param.vidpr,param.stage,export_worker,PublishedList);
            return;
        }

        void refresh_worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                MessageBox.Show("В процессе выполнения запроса произошла ошибка. Подробности записаны в reminder.log, находящийся в папке Мои документы");

            }
            else
            {

                info = (GetCasesListResult)e.Result;
                notPublishedList.ItemsSource = info.NotPublished;
                PublishedList.ItemsSource = info.Published;
                ExcludedList.ItemsSource = info.Excluded;
                info.Judges.Insert(0,"Все судьи");
                JudgeFilter.ItemsSource = info.Judges;
                JudgeFilter.SelectedIndex = 0;

                query_time = DateTime.Now;

                if (info.NotPublished.Count + info.Published.Count + info.Excluded.Count > 0) toExcelButton.IsEnabled = true;
                else toExcelButton.IsEnabled = false;

                statusText.Content = string.Format("Рассмотрено: {0}; Опубликовано {1}; Исключено {2}; Ожидает публикации: {3}. Запрос выполнялся {4} сек.", new object[] { info.NotPublished.Count + info.Published.Count + info.Excluded.Count, info.Published.Count, info.Excluded.Count, info.NotPublished.Count, info.query_time.TotalSeconds });
            }
            loadingThrobber.Visibility = System.Windows.Visibility.Collapsed;
        }

        void refresh_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                refreshBWParam param = (refreshBWParam)e.Argument;
                e.Result = DataModel.GetCasesList(param.vidpr, param.stage, param.startDate, param.endDate,mySettings.ShowCanceledCivil,mySettings.AutoExcludeCanceledCivil,mySettings.SDPLinkName);
            }
            catch (Exception ex)
            {
                Logging.Log("generate list", ex.Message + "\n" + ex.StackTrace);
                e.Result = null;
            }
            
        }

        private void vidpr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            stage_vid vp = vidpr.SelectedItem as stage_vid;

            ObservableCollection<stage_vid> stages = new ObservableCollection<stage_vid>();

            if (vp == null) return;

            switch (vp.value)
            {
                case 1:
                    stages.Add(new stage_vid() { header = "Первая инстанция", value = 113 });
                    stages.Add(new stage_vid() { header = "Апелляция", value = 114 });
                    stages.Add(new stage_vid() { header = "Кассация", value = 115 });
                    break;
                case 2:
                    stages.Add(new stage_vid() { header = "Первая инстанция", value = 113 });
                    stages.Add(new stage_vid() { header = "Апелляция", value = 114 });
                    stages.Add(new stage_vid() { header = "Кассация", value = 115 });
                    break;
                case 4:
                    stages.Add(new stage_vid() { header = "Первая инстанция", value = 113 });
                    break;
                case 5:
                    stages.Add(new stage_vid() { header = "Первая инстанция", value = 113 });
                    stages.Add(new stage_vid() { header = "Первый пересмотр", value = 110 });
                    stages.Add(new stage_vid() { header = "Второй пересмотр", value = 111 });
                    break;
            }
            stage.ItemsSource = stages;
            stage.SelectedIndex = 0;
        }    

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            sDate = (DateTime)dateFrom.SelectedDate;
            eDate = (DateTime)dateTo.SelectedDate;

            if (sDate > eDate)
            {
                MessageBox.Show("Начальная дата должна быть меньше или равна конечной");
                return;
            }

            refreshBWParam param = new refreshBWParam()
            {
                vidpr = ((stage_vid)vidpr.SelectedItem).value,
                stage = ((stage_vid)stage.SelectedItem).value,
                startDate = sDate,
                endDate = eDate
            };

            loadingThrobber.Visibility = System.Windows.Visibility.Visible;
            loadingThrobber.Tooltip = "Загрузка";
            lastParam = param;
            refresh_worker.RunWorkerAsync(param);
        }

        private void toExcelButton_Click(object sender, RoutedEventArgs e)
        {

            exportBWParam param = new exportBWParam();
            GetCasesListResult res = new GetCasesListResult();
            res.Published = (ObservableCollection<PublishedListItem>)PublishedList.ItemsSource;
            res.NotPublished = (ObservableCollection<NotPublishedListItem>)notPublishedList.ItemsSource;
            res.Excluded = (ObservableCollection<ExcludedListItem>)ExcludedList.ItemsSource;
            param.res = res;
            param.vidpr = ((stage_vid)vidpr.SelectedItem).header;
            param.stage = ((stage_vid)stage.SelectedItem).header;
            exportProgressPanel.Visibility = System.Windows.Visibility.Visible;
            export_worker.RunWorkerAsync(param);
        }

		
		void JudgeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (JudgeFilter.SelectedIndex == 0)  {
				notPublishedList.ItemsSource = info.NotPublished;
                PublishedList.ItemsSource = info.Published;
                ExcludedList.ItemsSource = info.Excluded;
			}
			else {
				string SelectedJudge = (string)JudgeFilter.SelectedItem;
				
				ObservableCollection<NotPublishedListItem> notpub = new ObservableCollection<NotPublishedListItem>();
				foreach (NotPublishedListItem item in info.NotPublished) {
					if (item.Judge == SelectedJudge) notpub.Add(item);
					
				}
				
				ObservableCollection<PublishedListItem> pub = new ObservableCollection<PublishedListItem>();
				foreach (PublishedListItem item in info.Published) {
					if (item.Judge == SelectedJudge) pub.Add(item);
					
				}
				
				ObservableCollection<ExcludedListItem> excl = new ObservableCollection<ExcludedListItem>();
				foreach (ExcludedListItem item in info.Excluded) {
					if (item.Judge == SelectedJudge) excl.Add(item);
					
				}
				
				notPublishedList.ItemsSource = notpub;
				PublishedList.ItemsSource = pub;
				ExcludedList.ItemsSource = excl;


                statusText.Content = string.Format("Рассмотрено: {0}; Опубликовано {1}; Исключено {2}; Ожидает публикации: {3}", new object[] { notpub.Count + pub.Count + excl.Count, pub.Count, excl.Count, notpub.Count });
			}
		}

        private void ShowSummary_Click(object sender, RoutedEventArgs e)
        {
            sDate = (DateTime)dateFrom.SelectedDate;
            eDate = (DateTime)dateTo.SelectedDate;

            if (sDate > eDate)
            {
                MessageBox.Show("Начальная дата должна быть меньше или равна конечной");
                return;
            }

            SummaryConfig sc = new SummaryConfig();
            sc.Owner = this;
            sc.first_mode = mySettings.FirstGrAfterConsideration;
            sc.show_canceled = mySettings.ShowCanceledCivil;

            if (sc.ShowDialog().Value)
            {
                summary_worker.RunWorkerAsync(new object[] { sDate, eDate, sc.use_materials, sc.first_mode, sc.show_canceled, sc.show_late  });

                loadingThrobber.Visibility = System.Windows.Visibility.Visible;
                loadingThrobber.Tooltip = "Сбор данных";
            }

            
        }
    }
}
