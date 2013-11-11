using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Shell;

namespace RaionReminder
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application,ISingleInstanceApp
    {
        public AppSettings mySettings = null;

    	private const string Unique = "PublishReminderRaionAlreadyHere";
	    [STAThread]
	    public static void Main()
	    {
	        if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
	        {
	            var application = new App();
	            application.InitializeComponent();
	            application.Run();
	            // Allow single instance code to perform cleanup operations
	            SingleInstance<App>.Cleanup();
	        }
	    }
	    #region ISingleInstanceApp Members
	    public bool SignalExternalCommandLineArgs(IList<string> args)
	    {
	        bool AdminMode = false;
            bool dbsettings = false;
            int new_db_arg = 0;
            string db_base = "";
            string db_user = "";
            string db_pass = "";
            string sdplink = "IBASEDATA";
            foreach (string arg in args)
            {
               
                if (arg == "/admin") AdminMode = true;
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
            AppSettings settings = null;
            try
            {
            	if (dbsettings && new_db_arg == 4) {
                    settings = new AppSettings(AdminMode,db_base,db_user,db_pass);
                    settings.SDPLinkName = sdplink;
            	}
            }
            catch (Exception ex)
            {
                Logging.Log("double app load settings",ex.Message + " " + ex.StackTrace);
            }
            if (settings != null) {
            	this.mySettings = settings;
            	((MainWindow)this.MainWindow).mySettings = settings;
            	((MainWindow)this.MainWindow).ReloadUnpublished();
            	
            }
	    	
	    	this.MainWindow.WindowState = WindowState.Normal;
	        this.MainWindow.ShowInTaskbar = true;
	        this.MainWindow.Activate();
	        return true;
	    }
	    #endregion
    }


}
