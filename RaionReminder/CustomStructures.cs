using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

using FirebirdSql.Data.FirebirdClient;
using Microsoft.Office.Interop.Excel;

namespace RaionReminder
{
    public class Logging
    {
        public static void Log(string source, string message)
        {
            String Log_path =
    Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\reminder.log";
            StreamWriter sw;
            if (!File.Exists(Log_path)) sw = File.CreateText(Log_path);
            else sw = File.AppendText(Log_path);
            sw.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": (" + source + ") " + message);
            sw.Close();
        }
    }

    public class AppSettings : INotifyPropertyChanged 
    {
        public AppSettings(bool AdminMode, string db, string user, string pass)
        {

            if (Properties.Settings.Default.FirstLaunch)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.FirstLaunch = false;
            }

            _AdminMode = true;
            

            if (UseLocalSettings)
            {
                GetAllSettingsFromConfig();
                exclBase = db;
                exclUser = user;
                exclPass = pass;
            }
            else
            {
                try
                {
                    exclBase = db;
                    exclUser = user;
                    exclPass = pass;
                    Properties.Settings.Default.Save();
                    GetAllSettingsFromDB();
                    _NoDBAvailable = false;
                }
                catch (Exception ex)
                {
                    _NoDBAvailable = true;
                    GetAllSettingsFromConfig();
                    Logging.Log("Settings constructor", ex.Message + " " + ex.StackTrace);
                }

            }
            _AdminMode = AdminMode;
            _SDPLinkName = "IBASEDATA";

            Properties.Settings.Default.Save();
        }

        public AppSettings(bool AdminMode) {
            if (Properties.Settings.Default.FirstLaunch)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.FirstLaunch = false;
              
            }

            if (UseLocalSettings) GetAllSettingsFromConfig();
            else
            {
                try
                {
                    GetAllSettingsFromDB();
                    _NoDBAvailable = false;
                }
                catch (Exception ex)
                {
                    _NoDBAvailable = true;
                    GetAllSettingsFromConfig();
                    Logging.Log("Settings constructor", ex.Message + " " + ex.StackTrace);
                }
            }

            _AdminMode = AdminMode;
            _SDPLinkName = "IBASEDATA";
            Properties.Settings.Default.Save();
    	}

        private bool _NoDBAvailable;
        public bool NoDBAvailable
        {
            get
            {
                return _NoDBAvailable;
            }
        }
        
        private bool _AdminMode;
        public bool AdminMode
        {
            get
            {
                return _AdminMode;
            }
        }
       
        public bool UseLocalSettings
        {
            get
            {
                return Properties.Settings.Default.useLocalSettings;
            }

            set
            {
                if (!_AdminMode) return;
                if (Properties.Settings.Default.useLocalSettings != value)
                {
                    Properties.Settings.Default.useLocalSettings = value;
                    NotifyPropertyChanged("UseLocalSettings");
                }
                else
                {
                    Properties.Settings.Default.useLocalSettings = value;
                }
            }
        }

        private int version;
        private DateTime lastcheck;

        public void Reload()
        {
            if (UseLocalSettings) GetAllSettingsFromConfig();
            else
            {
                try
                {
                    GetAllSettingsFromDB();
                    _NoDBAvailable = false;
                }
                catch (Exception)
                {
                    _NoDBAvailable = true;
                }
            }
        }

        public void Reset()
        {
            Properties.Settings.Default.Reset();
            GetAllSettingsFromConfig();
        }

        public void GetAllSettingsFromDB()
        {
            Properties.Settings.Default.Reload();
        	FbConnectionStringBuilder connStr = new FbConnectionStringBuilder()
            {
                Database = exclBase,
                UserID = exclUser,
                Password = exclPass
            };
            
            using (FbConnection connection = new FbConnection(connStr.ConnectionString))
            {
                connection.Open();

                try
                {

                    using (FbCommand command = new FbCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = "SELECT * FROM SETTINGS";

                        FbDataReader reader = command.ExecuteReader();
         
                        
                        _FirstAdmAfterConsideration = true;
                        
                        int settings_count = 0;
                        while (reader.Read())
                        {
                            settings_count++;
                            version = reader.GetInt32(0);
                            switch (reader.GetString(1))
                            {
                                case "bsrBase":
                                    _bsrBase = reader.GetString(2);
                                    NotifyPropertyChanged("bsrBase");
                                    break;
                                case "bsrUser":
                                    _bsrUser = reader.GetString(2);
                                    NotifyPropertyChanged("bsrUser");
                                    break;
                                case "bsrPass":
                                    _bsrPass = SecurityFunctions.ToSecureString(reader.GetString(2));
                                    NotifyPropertyChanged("bsrPass");
                                    break;
                                case "bsrUrl":
                                    _BSRURL = reader.GetString(2);
                                    NotifyPropertyChanged("BSRURL");
                                    break;
                                case "scanDays":
                                    _scan_days = int.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("scan_days");
                                    break;
                                case "pubDays":
                                    _pub_days = int.Parse(reader.GetString(2));
                                    if (_pub_days <= 0) _pub_days = 30;
                                    NotifyPropertyChanged("pub_days");
                                    break;
                                case "FirstGrAfterConsideration":
                                    _FirstGrAfterConsideration = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("FirstGrAfterConsideration");
                                    break;
                                case "FirstAdmAfterConsideration":
                                    _FirstAdmAfterConsideration = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("FirstAdmAfterConsideration");
                                    break;
                                case "ShowCanceledCivil":
                                    _ShowCanceledCivil = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("ShowCanceledCivil");
                                    break;
                                case "AutoExcludeCanceledCivil":
                                    _AutoExcludeCanceledCivil = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("AutoExcludeCanceledCivil");
                                    break;
                                case "DenyOnlyInBSR":
                                    _DenyOnlyInBSR = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("DenyOnlyInBSR");
                                    break;
                                case "ShowDaysAfterConsideration":
                                    _ShowDaysAfterConsideration = bool.Parse(reader.GetString(2));
                                    NotifyPropertyChanged("ShowDaysAfterConsideration");
                                    break;
                            }    
                        }
                        if (settings_count == 0) throw new ApplicationException("Таблица настроек пуста");
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }

                
            }

            lastcheck = DateTime.Now;
        }

        private void GetAllSettingsFromConfig()
        {
        	Properties.Settings.Default.Reload();
        	_bsrBase = Properties.Settings.Default.bsrBase;
            NotifyPropertyChanged("bsrBase");
            _bsrUser = Properties.Settings.Default.bsrUser;
            NotifyPropertyChanged("bsrUser");
            _bsrPass = SecurityFunctions.DecryptString(Properties.Settings.Default.bsrPass);
            NotifyPropertyChanged("bsrPass");

            _FirstGrAfterConsideration = Properties.Settings.Default.FirstGrAfterConsideration;
            NotifyPropertyChanged("FirstGrAfterConsideration");
             _FirstAdmAfterConsideration = Properties.Settings.Default.FirstAdmAfterConsideration;
            NotifyPropertyChanged("FirstAdmAfterConsideration");
            _ShowCanceledCivil = Properties.Settings.Default.ShowCanceledCivil;
            NotifyPropertyChanged("ShowCanceledCivil");
            _AutoExcludeCanceledCivil = Properties.Settings.Default.AutoExcludeCanceledCivil;
            NotifyPropertyChanged("AutoExcludeCanceledCivil");
            _DenyOnlyInBSR = Properties.Settings.Default.DenyOnlyInBSR;
            NotifyPropertyChanged("DenyOnlyInBSR");
            _ShowDaysAfterConsideration = Properties.Settings.Default.ShowDaysAfterConsideration;
            NotifyPropertyChanged("ShowDaysAfterConsideration");
            _scan_days = Properties.Settings.Default.scan_days;
            NotifyPropertyChanged("scan_days");
            _pub_days = Properties.Settings.Default.pub_days;
            if (_pub_days <= 0) _pub_days = 30;
            NotifyPropertyChanged("pub_days");
            _BSRURL = Properties.Settings.Default.BSRURL;
            NotifyPropertyChanged("BSRURL");
        }

        public void SaveLocal()
        {
            Properties.Settings.Default.Save();
        }

        public bool Save()
        {
            if (UseLocalSettings)
            {
                Properties.Settings.Default.bsrBase = _bsrBase;
                Properties.Settings.Default.bsrUser = _bsrUser;
                Properties.Settings.Default.bsrPass = SecurityFunctions.EncryptString(_bsrPass);

                Properties.Settings.Default.FirstGrAfterConsideration = _FirstGrAfterConsideration;
                Properties.Settings.Default.FirstAdmAfterConsideration = _FirstAdmAfterConsideration;
                Properties.Settings.Default.ShowCanceledCivil = _ShowCanceledCivil;
                Properties.Settings.Default.AutoExcludeCanceledCivil = _AutoExcludeCanceledCivil;

                Properties.Settings.Default.DenyOnlyInBSR = _DenyOnlyInBSR;
                Properties.Settings.Default.ShowDaysAfterConsideration = _ShowDaysAfterConsideration;

                Properties.Settings.Default.scan_days = _scan_days;
                Properties.Settings.Default.pub_days = _pub_days;
                Properties.Settings.Default.BSRURL = _BSRURL;

                Properties.Settings.Default.Save();
            }
            else
            {

                Properties.Settings.Default.Save();

                FbConnectionStringBuilder connStr = new FbConnectionStringBuilder()
                {
                    Database = exclBase,
                    UserID = exclUser,
                    Password = exclPass
                };
                try
                {
                    string ver = null;
                    using (FbConnection connection = new FbConnection(connStr.ConnectionString))
                    {
                        connection.Open();
                        FbTransaction trans =  connection.BeginTransaction();

                        //Очищаем таблицу настроек
                        using (FbCommand command = new FbCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = trans;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "DELETE FROM SETTINGS";
                            command.ExecuteNonQuery();
                        }

                        //Увеличиваем версию на 1
                        using (FbCommand command = new FbCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = trans;
                            command.CommandType = CommandType.Text;
                            command.CommandText = "SELECT GEN_ID(SETTINGSVERSION_GENERATOR, 1 ) FROM RDB$DATABASE";
                            ver = command.ExecuteScalar().ToString();
                        }

                        //Соханяем настройки
                        using (FbCommand command = new FbCommand())
                        {
                            string commandTemplate = "INSERT INTO SETTINGS VALUES (GEN_ID(SETTINGSVERSION_GENERATOR, 0),'{0}',@val)";
                            
                            command.Connection = connection;
                            command.Transaction = trans;
                            command.CommandType = CommandType.Text;
                            
                            FbParameter p = new FbParameter()
                            {
                                DbType = DbType.String,
                                Direction = ParameterDirection.Input,
                                ParameterName = "@val",
                            };
                            command.Parameters.Add(p);


                            command.CommandText = string.Format(commandTemplate, "bsrBase");
                            p.Value = _bsrBase;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "bsrUser");
                            p.Value = _bsrUser;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "bsrPass");
                            p.Value = SecurityFunctions.ToInsecureString(_bsrPass);
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "bsrUrl");
                            p.Value = _BSRURL;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "scanDays");
                            p.Value = _scan_days;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "pubDays");
                            p.Value = _pub_days;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "FirstGrAfterConsideration");
                            p.Value = _FirstGrAfterConsideration;
                            command.ExecuteNonQuery();
                            
                            command.CommandText = string.Format(commandTemplate, "FirstAdmAfterConsideration");
                            p.Value = _FirstAdmAfterConsideration;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "ShowCanceledCivil");
                            p.Value = _ShowCanceledCivil;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "AutoExcludeCanceledCivil");
                            p.Value = _AutoExcludeCanceledCivil;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "DenyOnlyInBSR");
                            p.Value = _DenyOnlyInBSR;
                            command.ExecuteNonQuery();

                            command.CommandText = string.Format(commandTemplate, "ShowDaysAfterConsideration");
                            p.Value = _ShowDaysAfterConsideration;
                            command.ExecuteNonQuery();
                        }

                        trans.Commit();
                        connection.Close();
                        version = int.Parse(ver);
                    }
                    _NoDBAvailable = false;
                }
                catch (Exception e)
                {
                    _NoDBAvailable = true;
                    Logging.Log("saveSettingsToDB", e.Message + " " + e.StackTrace);
                    throw(e);
                }
                
            }
            
            return true;
        }

        private string GetSettingFromDB(string name)
        {
            string result = null;
            
            FbConnectionStringBuilder connStr = new FbConnectionStringBuilder()
            {
                Database = exclBase,
                UserID = exclUser,
                Password = exclPass
            };
            try
            {
                using (FbConnection connection = new FbConnection(connStr.ConnectionString))
                {
                    connection.Open();

                    using (FbCommand command = new FbCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = CommandType.Text;
                        command.CommandText = string.Format("SELECT VERSION, VALUE FROM SETTINGS WHERE NAME = '{0}'",name);

                        FbDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            result = reader.GetString(0);
                            int ver = reader.GetInt32(1);

                            if (ver != version)
                            {
                                GetAllSettingsFromDB();
                            }
                        }
                    }

                    connection.Close();
                }
                _NoDBAvailable = false;
            }
            catch (Exception e)
            {
                _NoDBAvailable = true;
                Logging.Log("getSettingFromDB "+name,e.Message+" "+e.StackTrace);
            }

            
            lastcheck = DateTime.Now;
            return result;
        }

        public string exclBase
        {
            get
            {
                return Properties.Settings.Default.exclBase;
            }

            set
            {
                if (!_AdminMode) return;
                if (Properties.Settings.Default.exclBase != value)
                {
                    Properties.Settings.Default.exclBase = value;
                    NotifyPropertyChanged("exclBase");
                }
                else {
                    Properties.Settings.Default.exclBase = value;
                }
            }
        }

        public string exclUser
        {
            get
            {
                return Properties.Settings.Default.exclUser;
            }

            set
            {
                if (!_AdminMode) return;
                if (Properties.Settings.Default.exclUser != value) {
                    Properties.Settings.Default.exclUser = value;
                    NotifyPropertyChanged("exclUser");
                }
                else {
                    Properties.Settings.Default.exclUser = value;
                }
            }
        }

        public string exclPass
        {
            get
            {
                return  SecurityFunctions.ToInsecureString(SecurityFunctions.DecryptString(Properties.Settings.Default.exclPass));
            }

            set
            {
                if (!_AdminMode) return;
                if (SecurityFunctions.DecryptString(Properties.Settings.Default.exclPass) != SecurityFunctions.ToSecureString(value))
                {
                    Properties.Settings.Default.exclPass = SecurityFunctions.EncryptString(SecurityFunctions.ToSecureString(value));
                    NotifyPropertyChanged("exclPass");
                }
                else {
                    Properties.Settings.Default.exclPass = SecurityFunctions.EncryptString(SecurityFunctions.ToSecureString(value));
                }
            }
        }

        private string _bsrBase;
        public string bsrBase
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    _bsrBase = GetSettingFromDB("bsrBase");
                }
                return _bsrBase;
            }

            set
            {
                if (!_AdminMode) return;
                if (_bsrBase != value) {
                    _bsrBase = value;
                    NotifyPropertyChanged("bsrBase");
                }
                else {
                    _bsrBase = value;
                }
            }
        }

        private string _bsrUser;
        public string bsrUser
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    _bsrUser = GetSettingFromDB("bsrUser");
                }
                return _bsrUser;
            }

            set
            {
                if (!_AdminMode) return;
                if (_bsrUser != value) {
                    _bsrUser = value;
                    NotifyPropertyChanged("bsrUser");
                }
                else {
                    _bsrUser = value;
                }
            }
        }

        private SecureString _bsrPass;
        public string bsrPass
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    _bsrPass = SecurityFunctions.ToSecureString(GetSettingFromDB("bsrPass"));
                }
                return SecurityFunctions.ToInsecureString(_bsrPass);
            }

            set
            {
                if (!_AdminMode) return;
                if (SecurityFunctions.ToInsecureString(_bsrPass) != value) {
                    _bsrPass = SecurityFunctions.ToSecureString(value);
                    NotifyPropertyChanged("bsrPass");
                }
                else {
                    _bsrPass = SecurityFunctions.ToSecureString(value); 
                }
            }
        }
    	
        private bool _FirstGrAfterConsideration;
        public bool FirstGrAfterConsideration
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("FirstGrAfterConsideration"),out res)) {
                    	if (_FirstGrAfterConsideration != res) {
                    		_FirstGrAfterConsideration = res;
                    		NotifyPropertyChanged("FirstGrAfterConsideration");
                    	}
                    	else {
                    		_FirstGrAfterConsideration = res;
                    	}
                    }
                }
                return _FirstGrAfterConsideration;
            }

            set
            {
                if (!_AdminMode) return;
                if (_FirstGrAfterConsideration != value) {
                    _FirstGrAfterConsideration = value;
                    NotifyPropertyChanged("FirstGrAfterConsideration");
                }
                else {
                    _FirstGrAfterConsideration = value;
                }
            }
        }
        
        private bool _FirstAdmAfterConsideration;
        public bool FirstAdmAfterConsideration
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("FirstAdmAfterConsideration"),out res)) {
                    	if (_FirstAdmAfterConsideration != res) {
                    		_FirstAdmAfterConsideration = res;
                    		NotifyPropertyChanged("FirstAdmAfterConsideration");
                    	}
                    	else {
                    		_FirstAdmAfterConsideration = res;
                    	}
                    }
                }
                return _FirstAdmAfterConsideration;
            }

            set
            {
                if (!_AdminMode) return;
                if (_FirstAdmAfterConsideration != value) {
                    _FirstAdmAfterConsideration = value;
                    NotifyPropertyChanged("FirstAdmAfterConsideration");
                }
                else {
                    _FirstAdmAfterConsideration = value;
                }
            }
        }

        private bool _ShowCanceledCivil;
        public bool ShowCanceledCivil
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("ShowCanceledCivil"),out res)) {
                    	if (_ShowCanceledCivil != res) {
                    		_ShowCanceledCivil = res;
                    		NotifyPropertyChanged("ShowCanceledCivil");
                    	}
                    	else {
                    		_ShowCanceledCivil = res;
                    	}
                    }
                }
                return _ShowCanceledCivil;
            }

            set
            {
                if (!_AdminMode) return;
                if (_ShowCanceledCivil != value) {
                    _ShowCanceledCivil = value;
                    NotifyPropertyChanged("ShowCanceledCivil");
                }
                else {
                    _ShowCanceledCivil = value;
                }
            }
        }

        private bool _AutoExcludeCanceledCivil;
        public bool AutoExcludeCanceledCivil
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("AutoExcludeCanceledCivil"),out res)) {
                    	if (_AutoExcludeCanceledCivil != res) {
                    		_AutoExcludeCanceledCivil = res;
                    		NotifyPropertyChanged("AutoExcludeCanceledCivil");
                    	}
                    	else {
                    		_AutoExcludeCanceledCivil = res;
                    	}
                    }
                }
                return _AutoExcludeCanceledCivil;
            }

            set
            {
                if (!_AdminMode) return;
                if (_AutoExcludeCanceledCivil != value) {
                    _AutoExcludeCanceledCivil = value;
                    NotifyPropertyChanged("AutoExcludeCanceledCivil");
                }
                else {
                    _AutoExcludeCanceledCivil = value;
                }
                
            }
        }

        private bool _DenyOnlyInBSR;
        public bool DenyOnlyInBSR
        {
            get
            {

                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("DenyOnlyInBSR"),out res)) {
                    	if (_DenyOnlyInBSR != res) {
                    		_DenyOnlyInBSR = res;
                    		NotifyPropertyChanged("DenyOnlyInBSR");
                    	}
                    	else {
                    		_DenyOnlyInBSR = res;
                    	}
                    }
                }
                return _DenyOnlyInBSR;
            }

            set
            {
                if (!_AdminMode) return;
                if (_DenyOnlyInBSR != value)
                {
                    _DenyOnlyInBSR = value;
                    NotifyPropertyChanged("DenyOnlyInBSR");
                }
                else {
                    _DenyOnlyInBSR = value;
                }
            }
        }

        private bool _ShowDaysAfterConsideration;
        public bool ShowDaysAfterConsideration
        {
            get
            {
                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    bool res = false;
                    if (bool.TryParse(GetSettingFromDB("ShowDaysAfterConsideration"),out res)) {
                    	if (_ShowDaysAfterConsideration != res) {
                    		_ShowDaysAfterConsideration = res;
                    		NotifyPropertyChanged("ShowDaysAfterConsideration");
                    	}
                    	else {
                    		_ShowDaysAfterConsideration = res;
                    	}
                    }
                }
                return _ShowDaysAfterConsideration;
            }

            set
            {
                if (!_AdminMode) return;
                if (_ShowDaysAfterConsideration != value) {
                    _ShowDaysAfterConsideration = value;
                    NotifyPropertyChanged("ShowDaysAfterConsideration");
                }
                else {
                    _ShowDaysAfterConsideration = value;

                }
            }
        }

        private int _scan_days;
        public int scan_days
        {
            get
            {

                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    
                    int res = 180;
                    if (int.TryParse(GetSettingFromDB("scan_days"),out res)) {
                    	if (_scan_days != res) {
                    		_scan_days = res;
                    		NotifyPropertyChanged("scan_days");
                    	}
                    	else {
                    		_scan_days = res;
                    	}
                    }
                    
                }
                return _scan_days;
            }

            set
            {
                if (!_AdminMode) return;
                if (_scan_days != value)
                {
                    _scan_days = value;
                    NotifyPropertyChanged("scan_days");
                }
                else {
                    _scan_days = value;
                }
            }
        }

        private int _pub_days;
        public int pub_days
        {
            get
            {

                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    int res = 30;
                    if (int.TryParse(GetSettingFromDB("pub_days"),out res)) {
                    	if (_pub_days != res) {
                    		_pub_days = res;
                    		NotifyPropertyChanged("pub_days");
                    	}
                    	else {
                    		_pub_days = res;
                    	}
                    }
                }
                return _pub_days;
            }

            set
            {
                if (!_AdminMode) return;
                if (_pub_days != value)
                {
                    _pub_days = value;
                    NotifyPropertyChanged("pub_days");
                }
                else
                {
                    _pub_days = value;
                }
            }
        }

        private string _BSRURL;
        public string BSRURL
        {
            get
            {

                if (!UseLocalSettings && !_NoDBAvailable && (DateTime.Now - lastcheck).Minutes > 5)
                {
                    _BSRURL = GetSettingFromDB("BSRURL");
                }
                return _BSRURL;
            }

            set
            {
                if (!_AdminMode) return;
                if (_BSRURL != value)
                {
                    _BSRURL = value;
                    NotifyPropertyChanged("BSRURL");
                }
                else
                {
                    _BSRURL = value;
                }
            }
        }

        private string _SDPLinkName;
        public string SDPLinkName {
        	get {
        		return _SDPLinkName;
        	}
        	set {
        		_SDPLinkName = value;
        	}
        }
        
        public double MainWindowX {
            get
            {
                return Properties.Settings.Default.MainWindowX;
            }

            set
            {
                Properties.Settings.Default.MainWindowX = value;
            }
        }

        public double MainWindowY
        {
            get
            {
                return Properties.Settings.Default.MainWindowY;
            }

            set
            {
                Properties.Settings.Default.MainWindowY = value;
            }
        }

        public double MainWindowWidth
        {
            get
            {
                return Properties.Settings.Default.MainWindowWidth;
            }

            set
            {
                Properties.Settings.Default.MainWindowWidth = value;
            }
        }

        public double MainWindowHeight
        {
            get
            {
                return Properties.Settings.Default.MainWindowHeight;
            }

            set
            {
                Properties.Settings.Default.MainWindowHeight = value;
            }
        }

        public bool LaunchMinimized
        {
            get
            {
                return Properties.Settings.Default.LaunchMinimized;
            }

            set
            {
                if (Properties.Settings.Default.LaunchMinimized != value)
                {
                    Properties.Settings.Default.LaunchMinimized = value;
                    NotifyPropertyChanged("LaunchMinimized");
                }
                else
                    Properties.Settings.Default.LaunchMinimized = value;
            }
        }

        public ObservableCollection<JudgeGroup> JudgeGroups
        {
            get
            {
                return Properties.Settings.Default.JudgeGroups;
            }
            set
            {
                Properties.Settings.Default.JudgeGroups = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    
    public sealed class PublishListItem
    {
        private int days;
        public bool inBSR { get; set; }
        public bool canceled {get; set;}
        public int pubTerm;
        public int DaysCount
        {
            get { return this.days; }
            set
            {     	
                if (_invertColors)
                {
                    if (pubTerm < 2)
                    {
                        br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                    }
                    else if (pubTerm == 2)
                    {
                        if (value < 1) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (value == 1) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (value == 2) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black
                    }
                    else if (pubTerm == 3)
                    {

                        if (value < 1) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (value == 1) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (value == 2) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x80, 0x00)); //orange
                        else if (value == 3) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black


                    }
                    else if (pubTerm > 3)
                    {

                        if (pubTerm >= value * 4) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (pubTerm * 2 >= value * 4) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (pubTerm * 3 >= value * 4) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x80, 0x00)); //orange
                        else if (pubTerm >= value) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black

                    }
                }
                else
                {
                    if (pubTerm < 2)
                    {
                        br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00));
                    }
                    else if (pubTerm == 2)
                    {
                        if (value >= 2) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (value == 1) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (value == 0) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black
                    } else if (pubTerm == 3) {
                        
                        if (value >= 3) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (value == 2) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (value == 1) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x80, 0x00)); //orange
                        else if (value == 0) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black
                        
                        
                    } else if (pubTerm > 3) {

                        if (pubTerm*3 <= value*4) br = new SolidColorBrush(Color.FromRgb(0x89, 0xD1, 0x00)); //green
                        else if (pubTerm * 2 <= value * 4) br = new SolidColorBrush(Color.FromRgb(0xf5, 0xD4, 0x00)); //yellow
                        else if (pubTerm <= value * 4) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x80, 0x00)); //orange
                        else if (value > 0) br = new SolidColorBrush(Color.FromRgb(0xf5, 0x49, 0x00)); //red
                        else br = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0x00)); //black

                    }
                }
                br.Freeze();
                days = value;
            }
        }
        public string CaseNumber { get; set; }
        public string JudgeName { get; set; }
        public string info { get; set; }
        public int vidpr { get; set; }
        public int stage { get; set; }
        public int id { get; set; }
        private Brush br;
        public Brush DaysCountColor { get { return br; } }
        public bool ReadyToPublish { get; set; }
        private bool _invertColors;

        public PublishListItem(int id, int Count, string Number, string Judge, int vidpr, int stage, bool inBSR, bool ReadyToPublish, bool InvertColors = false, int pubTerm = 30, bool canceled = false)
        {
            this._invertColors = InvertColors;
            this.pubTerm = pubTerm;
            this.canceled = canceled;
            this.id = id;
            this.DaysCount = Count;
            this.CaseNumber = Number;
            this.JudgeName = Judge;
            this.vidpr = vidpr;
            this.stage = stage;
            this.inBSR = inBSR;
            this.ReadyToPublish = ReadyToPublish;
            
        }
    }

    public sealed class MyJudgeChartItem
    {
        public int rank { get; set; }
        public string name { get; set; }
        private string _score;
        public string score { 
            get
            {
                return _score;
            } 
            set 
            { 
                _score = string.Concat(value, " баллов"); 
            } 
        }

        public MyJudgeChartItem(string name, int rank, int score)
        {
            this.rank = rank;
            this.name = name;
            this._score = string.Concat(score, " баллов");
        }

        public MyJudgeChartItem()
        {
            this.rank = 0;
            this.name = "";
            this.score = "";
        }

    }

    public struct ExcludeInfo
    {
        public PublishListItem item;
        public string message;
    }

    public class JudgeGroup : IComparable<JudgeGroup>
    {
        public int id { get; set; }
        public string name { get; set; }

        public int CompareTo(JudgeGroup other) {
            if (this.id == other.id) return 0;
            return 1;
        }
    }

    public class ExclusionInfo
    {
        public int id {get; set;}
        public int case_id { get; set; }
        public string number { get; set; }
        public string reason { get; set; }
        public DateTime date { get; set; }
        public int vidpr { get; set; }
        public int stage { get; set; }
        public string FIO { get; set; }
    }

    public class InfoListItem 
    {
        public int vidpr { get ; set ;}
        public int stage { get ; set ;}
        public int id;
        public string number {get; set;}
        public DateTime date { get ; set ;}
        public DateTime validityDate { get ; set ;}
        public DateTime returnDate { get; set; }
        public string Judge { get; set; }
        public string info { get; set; }
        public bool inBSR { get; set; }
        public bool ReadyToPublish { get; set; }
        public string docStatus { get; set; }
        public bool hasCopies = false;
        public bool canceled  {get; set; }

        public InfoListItem()
        {

        }

        public InfoListItem(InfoListItem item)
        {
        	if (item == null) return; 
            this.id = item.id;
            this.vidpr = item.vidpr;
            this.stage = item.stage;
            this.number = item.number;
            this.date = item.date;
            this.validityDate = item.validityDate;
            this.Judge = item.Judge;
            this.info = item.info;
            this.inBSR = item.inBSR;
            this.docStatus = item.docStatus;
            this.returnDate = item.returnDate;
            this.canceled = item.canceled;
        }      
    }

    public class NotPublishedListItem : InfoListItem
    { 
       public NotPublishedListItem (InfoListItem item) : base(item) {
            this.status = "";
        }

       public string status { get; set; }
    }

    public class PublishedListItem:InfoListItem
    {
        public PublishedListItem(InfoListItem item)
            : base(item)
        {
        	
        }
        
        public DateTime SignificantDate {
        	get {
        		if  (this.vidpr == 1 && this.stage == 113) {
        			return validityDate;
        		}
        		return date;
        	}
        
        }
        
        public DateTime pubDate { get ; set ;}
        public int PublicationLength { get { 
        		if  (this.vidpr == 1 && this.stage == 113) {
        			if (validityDate != new DateTime())
        				return (int)((pubDate - validityDate).TotalDays);
        			else
        				return -10000;
        		}
        		
        		return (int)((pubDate - date).Days);
        	} }
        public string comment { get; set; }
    }

    public class ExcludedListItem:InfoListItem
    {

        public ExcludedListItem(InfoListItem item)
            : base(item)
        {

        }
        public int excludeId;
        public string reason { get; set; }
        public string whoExcluded { get; set; }
    }

    public class GetCasesListResult
    {
        public ObservableCollection<NotPublishedListItem> NotPublished = new ObservableCollection<NotPublishedListItem>();
        public ObservableCollection<PublishedListItem> Published = new ObservableCollection<PublishedListItem>();
        public ObservableCollection<ExcludedListItem> Excluded = new ObservableCollection<ExcludedListItem>();
        public StringCollection Judges = new StringCollection();
        public TimeSpan query_time;
    }

    public class SummaryInfoItem
    {
        public int completed; //рассмотрено всего
        public int imported; //загружено в БСР
        public int published; //опубликовано всего
        public int published_invalid; //опубликовано не вступивших в законную силу
        public int published_too_late_global; //опубликовано с нарушением срока в 30 дней
        public int published_too_late_local; //опубликовано с нарушением локально установленного срока
        public int not_published; //Не опубликовано всего
        public int not_published_invalid; //Не опубликовано так как не вступило в законную силу
        public int not_published_and_late_global; //не опубликовано и срок больше 30 дней
        public int not_published_and_late_local; //не опубликовано и срок больше локально установленного
        public int not_published_ready; //не опубликованы, но полностью готовы к публикации
        public int not_published_denied; //запрещено к публикации
        public Dictionary<string,int> denied = new Dictionary<string,int>(); //запрещённые к публикации по группам
    }

    public class SummaryInfoLates {
    	public string case_number;
    	public int days_count_local;
    	public int days_count_global;
    	public DateTime control_date_local;
    	public DateTime control_date_global;
    	public DateTime pub_date = new DateTime();
    	public bool late_local = false;
    	public bool late_global = false;
    }
    
    public class SummaryInfo
    {
        public void SaveInfo(int vidpr,int stage, SummaryInfoItem info) {
            if (vidpr > 9 || vidpr < 1) throw new ArgumentException("Значение vidpr должно быть от 0 до 10");
            if (stage > 999 || stage < 0) throw new ArgumentException("Значение stage должно быть меньше 999");

            int key = vidpr * 1000 + stage;
            

            if (!stages.ContainsKey(key))  {stages.Add(key,info);}
            else { stages[key] = info;  }

            SummaryInfoItem all_info = new SummaryInfoItem();
            summary = new Dictionary<int, SummaryInfoItem>(4);
            summary.Add(0, new SummaryInfoItem());
            summary.Add(1, new SummaryInfoItem());
            summary.Add(2, new SummaryInfoItem());
            summary.Add(3, new SummaryInfoItem());
            summary.Add(4, new SummaryInfoItem());
            summary.Add(5, new SummaryInfoItem());

            foreach (int i in stages.Keys)
            {
                SummaryInfoItem item = stages[i];

                int type = i / 1000;

                summary[0].completed += item.completed;
                summary[0].imported += item.imported;
                summary[0].not_published += item.not_published;
                summary[0].not_published_and_late_global += item.not_published_and_late_global;
                summary[0].not_published_and_late_local += item.not_published_and_late_local;
                summary[0].not_published_invalid += item.not_published_invalid;
                summary[0].not_published_ready += item.not_published_ready;
                summary[0].published += item.published;
                summary[0].published_too_late_global += item.published_too_late_global;
                summary[0].published_too_late_local += item.published_too_late_local;
                summary[0].published_invalid += item.published_invalid;
                summary[0].not_published_denied += item.not_published_denied;

                summary[type].completed += item.completed;
                summary[type].imported += item.imported;
                summary[type].not_published += item.not_published;
                summary[type].not_published_and_late_global += item.not_published_and_late_global;
                summary[type].not_published_and_late_local += item.not_published_and_late_local;
                summary[type].not_published_invalid += item.not_published_invalid;
                summary[type].not_published_ready += item.not_published_ready;
                summary[type].published += item.published;
                summary[type].published_too_late_global += item.published_too_late_global;
                summary[type].published_too_late_local += item.published_too_late_local;
                summary[type].published_invalid += item.published_invalid;
                summary[type].not_published_denied += item.not_published_denied;

                foreach (string denyreason in item.denied.Keys)
                {
                    if (summary[0].denied.ContainsKey(denyreason))
                    {
                        summary[0].denied[denyreason] += item.denied[denyreason];
                    }
                    else
                    {
                        summary[0].denied.Add(denyreason, item.denied[denyreason]);
                    }

                    if (summary[type].denied.ContainsKey(denyreason))
                    {
                        summary[type].denied[denyreason] += item.denied[denyreason];
                    }
                    else
                    {
                        summary[type].denied.Add(denyreason, item.denied[denyreason]);
                    }
                }

            }
        }
        public SummaryInfoItem GetInfo(int vidpr, int stage)
        {
            if (vidpr >= 10 || vidpr < 0) throw new ArgumentException("Значение vidpr должно быть от 0 до 9");
            if (stage > 999 || stage < 0) throw new ArgumentException("Значение stage должно быть меньше 999");

            if (stage == 0)
            {
                if (!summary.ContainsKey(vidpr)) return null;
                return summary[vidpr];
            }

            int key = vidpr * 1000 + stage;
            if (!stages.ContainsKey(key)) return null;
            return stages[key];
        }
        
        public void SaveLate(int vidpr, int stage, SummaryInfoLates info) {
        	if (vidpr >= 10 || vidpr < 0) throw new ArgumentException("Значение vidpr должно быть от 0 до 9");
            if (stage > 999 || stage < 0) throw new ArgumentException("Значение stage должно быть меньше 999");

            
            int key = vidpr * 1000 + stage;
            

            if (!late.ContainsKey(key))  {
            	List<SummaryInfoLates> l = new List<SummaryInfoLates>();
            	l.Add(info);
            	late.Add(key,l);
            	
            }
            else { late[key].Add(info);  }
        }
        
        public List<SummaryInfoLates> GetLates(int vidpr, int stage) {
        	if (vidpr >= 10 || vidpr <= 0) throw new ArgumentException("Значение vidpr должно быть от 0 до 9");
            if (stage > 999 || stage < 0) throw new ArgumentException("Значение stage должно быть меньше 999");
            
            int key = vidpr * 1000 + stage;
            
            if (late.ContainsKey(key)) {
            	return late[key];
            }
            else {
            	return null;
            }
        }

        private Dictionary<int, SummaryInfoItem> summary = new Dictionary<int, SummaryInfoItem>();
        private Dictionary<int, SummaryInfoItem> stages = new Dictionary<int,SummaryInfoItem>();
        private Dictionary<int, List<SummaryInfoLates>> late = new Dictionary<int, List<SummaryInfoLates>>();
    }

    public class DataModel
    {

        public static bool CheckFireBirdConnection(string Base, string User, string Pass)
        {
            FbConnectionStringBuilder conStr = new FbConnectionStringBuilder()
            {
                Charset = "WIN1251",
                UserID = User,
                Password = Pass,
                Database = Base,
                ServerType = FbServerType.Default
            };

            using (FbConnection connection = new FbConnection(conStr.ToString()))
            {
                    connection.Open();
                    connection.Close();
                    return true;
            }
            
        }

        public static bool CheckOracleConnection(string Base, string User, string Pass)
        {
            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = "Data Source="+Base+";Password="+Pass+";User ID="+User;
                connection.Open();
                connection.Close();
                return true;
            }
        }

        public static GetCasesListResult GetCasesList(int vidpr, int stage, DateTime startDate, DateTime endDate, bool ShowCanceledCivil, bool AutoExcludeCanceledCivil, string SDPLinkName)
        {
            DateTime start_time = DateTime.Now;
            GetCasesListResult result = new GetCasesListResult();

            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;

            #region connectionStrings

            FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
            {
                Charset = "WIN1251",
                UserID = mySettings.exclUser,
                Password = mySettings.exclPass,
                Database = mySettings.exclBase,
                ServerType = FbServerType.Default
            };


            string bsr_pass = mySettings.bsrPass;
            string bsr_db = mySettings.bsrBase;
            string bsr_login = mySettings.bsrUser;

            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;

            #endregion

            #region Выбираем из нашей базы исключения
            
            Hashtable exclusions = new Hashtable();
            using (FbConnection connection = new FbConnection(exclConStr.ToString()))
            {
                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS";

                    /*FbParameter par = new FbParameter()
                    {
                        Direction = ParameterDirection.Input,
                        DbType = DbType.Int32,
                        Value = int.Parse(startDate.ToString("yyyyMMdd")),
                        ParameterName = "@strt"
                    };

                    command.Parameters.Add(par);*/

                    FbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string dumbDate = reader.GetString(4);
                        ExclusionInfo ei = new ExclusionInfo(){
                            id = reader.GetInt32(0),
                            case_id = reader.GetInt32(1),
                            number = reader.GetString(2),
                            reason = reader.GetString(3),
                            date = new DateTime(int.Parse(dumbDate.Substring(0,4)),int.Parse(dumbDate.Substring(4,2)),int.Parse(dumbDate.Substring(6,2))),
                            vidpr = reader.GetInt32(5),
                            stage = reader.GetInt32(6),
                            FIO = reader.GetString(7)
                        };
                        exclusions.Add(ei.case_id,ei);
                    }
                }

                connection.Close();
            }

            #endregion

            

            using (OracleConnection connection = new OracleConnection(bsrConnStr))
            {
                connection.Open();

                OracleTransaction ot = connection.BeginTransaction();

                #region Подготавливаем дополнительные команды и параметры

                OracleCommand oraCommand = new OracleCommand()
                {
                    Connection = connection,
                    Transaction = ot,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ID_DOCUM, PRPUB, DATEPUBLIC, P_ANNOT FROM bsr.BSRP WHERE id_agora=:id"
                };

                OracleParameter oraIdAgora = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType  = DbType.Int32,
                    ParameterName = ":id",
                };

                oraCommand.Parameters.Add(oraIdAgora);

                OracleCommand oraCheckBSRExclusions = new OracleCommand()
                {
                    Connection = connection,
                    Transaction = ot,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT de.prich FROM  BSR.BSRP b left join bsr.rub_docum r on b.id_docum = r.id_docum inner join rub_docum_exception de on r.rubrikat = de.vncode WHERE id_agora = :id"
                };

                OracleParameter oraIdAgora2 = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType = DbType.Int32,
                    ParameterName = ":id",
                };

                oraCheckBSRExclusions.Parameters.Add(oraIdAgora2);

                OracleCommand oraDocumentStatusCommand = new OracleCommand()
                {
                    Connection = connection,
                    Transaction = ot,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT PRPUB FROM BSR.TEXT_DOCUM WHERE ID_DOCUM = :id"
                };

                OracleParameter oraId = new OracleParameter() {
                    Direction = ParameterDirection.Input,
                    OracleType = System.Data.OracleClient.OracleType.Number,
                    ParameterName = ":id"
                };
                oraDocumentStatusCommand.Parameters.Add(oraId);

                #endregion


                #region Выбираем нужный запрос

                string stmt = "";
                switch (vidpr)
                {
                    case 01: //уголовные
                        switch (stage)
                        {
                            
                            case 113: //Первая инстанция
                                stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_DATE, g.USERNAME, d.NAME || ' ' ||  d.LAW_ARTICLE, c.VALIDITY_DATE FROM U1_CASE@{0} c JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID INNER JOIN U1_DEFENDANT@{0} d ON d.CASE_ID = c.ID WHERE (c.VERDICT_DATE >= :sd) AND (c.VERDICT_DATE <= :ed) ORDER BY c.VERDICT_DATE desc, c.ID";

                                break;
                            case 114: //Кассация
                                stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_II_DATE, g.USERNAME, d.NAME || ' ' ||  d.LOW_ARTICLE_I, c.VERDICT_II_DATE FROM U2_CASE@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.SPEAKER = g.GROUPCONTENTID JOIN  U2_DEFENDANT@{0} d ON d.CASE_ID = c.ID WHERE (c.id is not null) AND (c.VERDICT_II_DATE >= :sd) AND (c.VERDICT_II_DATE <= :ed) AND (c.VERDICT_II_ID = 50190000) ORDER BY c.VERDICT_II_DATE desc, c.ID";
                                break;
                            case 115: //Единый Надзор
                                stmt = "SELECT c.ID, c.proceeding_full_number, c.protest_verdict_date, g.USERNAME, d.name || ' ' || d.law_article, c.protest_verdict_date FROM u33_proceeding@{0} c inner JOIN GROUPCONTENT@{0} g ON c.presiding_judge = g.GROUPCONTENTID JOIN  u33_parts@{0} d  ON d.proc_id = c.ID WHERE (c.protest_VERDICT_DATE >= :sd) AND (c.protest_VERDICT_DATE <= :ed) AND (c.verdict_by_protest = 11050001) ORDER BY c.protest_VERDICT_DATE desc, c.ID";
                                break;
                        }
                        break;
                    case 02: //гражданские
                        switch (stage)
                        {
                            case 113: //Первая инстанция
                                stmt = "SELECT c.ID, c.FULL_NUMBER, c.RESULT_DATE, g.USERNAME, c.REQ_ESSENCE, c.VALIDITY_DATE FROM G1_CASE@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE (c.RESULT_DATE >= :sd) AND (c.RESULT_DATE <= :ed) ORDER BY c.DECISION_DATE desc";
                                break;
                            case 114: //Аппеляция
                                stmt = "SELECT c.ID, c.Z_FULLNUMBER, c.DRESALT, g.USERNAME, c.ESSENCE_STATEMENT, c.DRESALT FROM GR2_DELO@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.IDDOC = g.GROUPCONTENTID WHERE (DRESALT >= :sd) AND (DRESALT <= :ed) AND (IDRESH NOT IN (911000001,911000002,911000003,911000004,911000005,911000006,50440013)) ORDER BY c.DRESALT desc";
                                break;
                            case 115: //Кассация
                                stmt = "SELECT c.ID, c.proceeding_full_number, c.protest_verdict_date, g.USERNAME, co.essence_statement, c.protest_verdict_date FROM g33_proceeding@{0} c inner JOIN GROUPCONTENT@{0} g ON c.presiding_judge = g.GROUPCONTENTID inner join g33_complaint@{0} co on c.id = co.proc_id WHERE (c.protest_VERDICT_DATE >= :sd) AND (c.protest_VERDICT_DATE <= :ed) ORDER BY c.protest_VERDICT_DATE desc";
                                break;
                        }
                        break;
                    case 04: //производство по материалам
                        switch (stage)
                        {
                            case 113:
                                stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_DATE,g.USERNAME,c.essence,c.VALIDITY_DATE FROM m_case@{0} c inner join groupcontent@{0} g on c.judge_id = g.GROUPCONTENTID WHERE (VERDICT_DATE >= :sd) AND (VERDICT_DATE <= :ed) ORDER BY c.VERDICT_DATE desc";
                                break;
                        }
                        break;
                    case 05: //административные
                        switch (stage)
                        {
                            case 110: //Первый пересмотр
                                stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.LAW_ARTICLES1, c.TAKE_LAW_EFFECT_DATE FROM adm1_case@{0} c  INNER JOIN   GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed)  ORDER BY c.DECREE_DATE desc"; //AND (adm1_case.DECREE_ID NOT IN (70100005,70100006))
                                break;
                            case 111: //Второй пересмотр
                                stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.LAW_ARTICLES1, c.DECREE_DATE FROM adm2_case@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed)  ORDER BY c.DECREE_DATE desc"; //AND (adm2_case.DECREE_ID NOT IN (70030007,70030008))
                                break;
                            case 113: //Первая инстанция
                                stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.ESSENCE, c.TAKE_LAW_EFFECT_DATE FROM adm_case@{0} c INNER JOIN   GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed) ORDER BY c.DECREE_DATE desc";
                                break;
                            case 115: //Надзор
                                stmt = "SELECT c.id, c.full_number, c.verdict_date, g.username, c.law_article, c.verdict_date FROM a33_proceeding@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.judge_study_assist_id = g.GROUPCONTENTID WHERE (verdict_date >= :sd) AND (verdict_date <= :ed)  ORDER BY c.verdict_date desc";
                                break;
                        }
                        break;
                }
                stmt = string.Format(stmt,SDPLinkName);
                

                #endregion

                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.Transaction = ot;
                    command.CommandType = CommandType.Text;
                    command.CommandText = stmt;
                    command.CommandTimeout = 60;

                    OracleParameter startDateParam = new OracleParameter()
                    {
                        DbType = DbType.DateTime,
                        Direction = ParameterDirection.Input,
                        ParameterName = ":sd",
                        Value = startDate
                    };

                    OracleParameter endDateParam = new OracleParameter()
                    {
                        DbType = DbType.DateTime,
                        Direction = ParameterDirection.Input,
                        ParameterName = ":ed",
                        Value = endDate
                    };

                    command.Parameters.Add(startDateParam);
                    command.Parameters.Add(endDateParam);

                    OracleDataReader reader = command.ExecuteReader();
                    int last_item_type = 0;
                    InfoListItem item = null;


                    //Logging.Log("list stmt",stmt);
                    
                    while (reader.Read()) //Проходим по всем рассмотренным делам
                    {
                        InfoListItem temp_item = new InfoListItem()
                        {
                            id = reader.GetInt32(0),
                            number = reader.GetString(1),
                            date = reader.GetDateTime(2),
                            Judge = reader.GetString(3),
                            info = !reader.IsDBNull(4) ? reader.GetString(4): "",
                        };
                        
                        //Вот эта хитрая конструкция нужна из-за того, что в деле может быть несколько человек (характерно для уголовных дел)
                        //и запрос возвращает одинаковые записи для каждого подсудимого, отличается только его имя и статьи, поэтому
                        //если видим, что id дела совпадает с предыдущим, то просто модифицируем информацию
                        if ((last_item_type != 0) && (temp_item.id == item.id))
                        {
                            switch (last_item_type)
                            {
                                case 1: //not published
                                    ((NotPublishedListItem)result.NotPublished[result.NotPublished.Count - 1]).info += " \n"+temp_item.info;
                                break;
                                
                            }

                            continue;
                        }

                        item = temp_item;
                        
                        if (result.Judges.IndexOf(item.Judge) == -1) result.Judges.Add(item.Judge);
                            

                        if (reader[5] != DBNull.Value)
                        {
                            item.validityDate = reader.GetDateTime(5);
                        }

                        item.vidpr = vidpr;
                        item.stage = stage;

                        oraIdAgora.Value = item.id;
                        OracleDataReader oraReader = oraCommand.ExecuteReader();
                        //Ищем дело в БСР
                        //Вернуться может не одна запись, возможны дубли, поэтому проходим по всему результату и ищем опубликованный
                        //Если опубликованных нет, то берём последний.

                        OracleNumber OracleCaseId = new OracleNumber();
                        DateTime publishDate = new DateTime();
                        bool foundPub = false;
                        string p_annot = item.info;

                        if (!oraReader.HasRows) {

                            item.inBSR = false;
                            item.docStatus = "Не загружен";
                  
                        }
                        else {

                            int oraCount = 0;
                            while (oraReader.Read())
                            {
                                oraCount++;
                                if (foundPub) continue; //Найдено опубликованное. Остальные итерации просто для подсчёта

                                object PRPUB = oraReader[1];

                                if (PRPUB != DBNull.Value && (decimal)PRPUB == 1)
                                {
                                    foundPub = true;
                                    if (oraReader[3].ToString() != "") {
                                    	p_annot = oraReader[3].ToString();
                                    }
                                }

                                 OracleCaseId = oraReader.GetOracleNumber(0);


                                 if (oraReader[2] != DBNull.Value)
                                     publishDate = oraReader.GetDateTime(2);
                                 else
                                     publishDate = new DateTime();
                            }

                            if (oraCount > 1) item.hasCopies = true;
                            item.inBSR = true;

                        }


                        oraReader.Close();


                        if (foundPub) //Дело опубликовано
                        {
                           //Проверим вариант, когда дело опубликовано и находится в исключениях одновременно
                            if (exclusions[item.id] != null)
                            {
                                //Элемент в исключениях
                                ExclusionInfo ei = (ExclusionInfo)exclusions[item.id];
                                ExcludedListItem eli = new ExcludedListItem(item);
                                eli.reason = "(Опубликовано!) "+ei.reason;
                                eli.whoExcluded = ei.FIO;
                                eli.excludeId = ei.id;
                                result.Excluded.Add(eli);
                                last_item_type = 3;
                            } else {
	                        	PublishedListItem pli = new PublishedListItem(item);
	                            pli.comment = p_annot;
	                            pli.pubDate = publishDate;
	                            result.Published.Add(pli);
	                            last_item_type = 2;
                            }
                        }

                        else
                        {
                            if (item.inBSR)
                            {
                                //Определим статус приложенных документов
                                oraId.Value = OracleCaseId;
                                OracleDataReader docsReader = oraDocumentStatusCommand.ExecuteReader();

                                if (!docsReader.HasRows)
                                {
                                    item.docStatus = "Нет приложенных к делу документов";
                                }
                                else
                                {
                                    string statText = "";
                                    while (docsReader.Read())
                                    {
                                        string docstat = docsReader[0].ToString();
                                        switch (docstat)
                                        {
                                            case "1":
                                                statText += "Пропущен через обезличиватель, ";
                                                break;
                                            case "2":
                                                statText += "Документ в работе, ";
                                                break;
                                            case "3":
                                                statText += "Готов к публикации, ";
                                                break;

                                            default:
                                                statText += "Документ не обработан, ";
                                                break;
                                        }
                                    }

                                    item.docStatus = statText.Substring(0, statText.Length - 2); //Убираем запятую с пробелом
                                }
                            }
                            
                            
                            //Проверим исключения
                            bool Excluded = false;
                            if (exclusions[item.id] != null)
                            {
                                //Элемент в исключениях
                                ExclusionInfo ei = (ExclusionInfo)exclusions[item.id];
                                ExcludedListItem eli = new ExcludedListItem(item);
                                eli.reason = ei.reason;
                                eli.whoExcluded = ei.FIO;
                                eli.excludeId = ei.id;
                                result.Excluded.Add(eli);
                                last_item_type = 3;
                                Excluded = true;
                            }
                            else
                            {
                                if (vidpr == 2 && stage == 113) //Первая гражданская инстанция. Проверяем вдруг это материал
                                {
                                    string case_type = item.number.Substring(0, 1);
                                    if (case_type == "9" || case_type == "М")
                                    {
                                        //Нашли материал, решаем, что с ним делать
                                        if (!ShowCanceledCivil) continue;

                                        if (AutoExcludeCanceledCivil)
                                        {
                                            //Logging.Log("Get cases list. Auto exclude 9,M",item.number+ " show: "+ShowCanceledCivil.ToString()+ " exclude:"+AutoExcludeCanceledCivil.ToString());
                                        	ExcludedListItem eli = new ExcludedListItem(item);
                                            string exclReason1 ="Дело не рассматривалось по существу";
                                            eli.excludeId = DataModel._AddToExcluded(item.id, item.number, exclReason1, item.date, vidpr, stage, "auto");
                                            eli.reason = exclReason1;
                                            eli.whoExcluded = "bsr";
                                            result.Excluded.Add(eli);
                                            last_item_type = 3;
                                            continue;
                                        }
                                    }
                                }
                            	
                            	
                            	//Ищем по автоисключению BSR
                                oraIdAgora2.Value = item.id;
                                string exclReason = (string)oraCheckBSRExclusions.ExecuteScalar();

                                if (exclReason != null)
                                {
                                    ExcludedListItem eli = new ExcludedListItem(item);
                                    
                                    eli.excludeId = DataModel._AddToExcluded(item.id, item.number, exclReason, item.date, vidpr, stage, "bsr");
                                    eli.reason = exclReason;
                                    eli.whoExcluded = "bsr";
                                    
                                    result.Excluded.Add(eli);
                                    last_item_type = 3;
                                    Excluded = true;
                                }
                            }

                            if (!Excluded)
                            {
                                NotPublishedListItem npli = new NotPublishedListItem(item);
                                result.NotPublished.Add(npli);
                                last_item_type = 1;
                            }
                                
                        }   

                    }

                }

                ot.Commit();

                connection.Close();
            }



            ArrayList.Adapter(result.Judges).Sort();
            result.query_time = DateTime.Now - start_time;
            return result;
        }

        public static ObservableCollection<JudgeGroup> GetJudgeGroups(string SDPLinkName)
        {
            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;


            ObservableCollection<JudgeGroup> result = new ObservableCollection<JudgeGroup>();
           
           	string bsr_pass = mySettings.bsrPass;
            string bsr_db = mySettings.bsrBase;
            string bsr_login = mySettings.bsrUser;

            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;

            using (OracleConnection connection = new OracleConnection(bsrConnStr))
            {

                connection.Open();

                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format("SELECT groupcontentid, username FROM groupcontent@{0} WHERE va_code is not null AND va_code <> '' ORDER BY username",SDPLinkName);



                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        JudgeGroup jg = new JudgeGroup();
                        jg.id = reader.GetInt32(0);
                        jg.name = reader.GetString(1);
                        result.Add(jg);
                    }
                }

                connection.Close();
            }

            return result;
        }

        public static List<PublishListItem> GetUnpublishedCases(ObservableCollection<JudgeGroup> groups, bool FirstGrAfterConsideration, bool FirstAdmAfterConsideration, bool ShowCanceledCivil, bool AutoExcludeCanceledCivil, bool ShowDaysAfterConsideration, int pubDays, string SDPLinkName)
        {
            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;

            List<PublishListItem> result = new List<PublishListItem>();

            if (groups.Count == 0) throw new ArgumentException("Список групп судей пуст");

            string judges = "(";
            Hashtable groupNames = new Hashtable(groups.Count);
            foreach (JudgeGroup item in groups)
            {
                judges += (item.id + ",");
                groupNames.Add(item.id, item.name);
            }

            judges = judges.Substring(0,judges.Length - 1) + ")";

            DateTime start_date = DateTime.Now.AddDays(-1*mySettings.scan_days);
            DateTime end_date = DateTime.Now;

            #region connectionStrings

            FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
            {
                Charset = "WIN1251",
                UserID = mySettings.exclUser,
                Password = mySettings.exclPass,
                Database = mySettings.exclBase,
                ServerType = FbServerType.Default
            };

            string bsr_pass = mySettings.bsrPass;
            string bsr_db = mySettings.bsrBase;
            string bsr_login = mySettings.bsrUser;

            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;

            #endregion

            #region GetExclusions

        
            List<int> exclusions = new List<int>();
            using (FbConnection connection = new FbConnection(exclConStr.ToString()))
            {
                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT CASE_ID FROM PUBLISH_EXCEPTIONS";

                    FbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        exclusions.Add(reader.GetInt32(0));
                    }
                    reader.Close();
                }
				
                //Logging.Log("exclusions found");
                
                connection.Close();
            }

            #endregion

            using (OracleConnection connection = new OracleConnection(bsrConnStr))
            {
                connection.Open();

                OracleTransaction ot = connection.BeginTransaction();
                
                #region GetCanceledCases
                
                List<SDPCaseInfo> cases = GetCanceledCases(connection, ot, SDPLinkName, start_date,end_date,judges);
                
                foreach (SDPCaseInfo cs in cases) {       	
                	int days_remains = 0;
	                if (ShowDaysAfterConsideration)
	                    days_remains = 99;
	                else
	                    days_remains = -99;
	                result.Add(new PublishListItem(cs.id, days_remains, cs.number, (string)groupNames[cs.judge_id], cs.vidpr, cs.stage, true, true,ShowDaysAfterConsideration,pubDays,true));
                }
                
                
                
                #endregion
                
                
                #region GetCasesList
            

	            cases = new List<SDPCaseInfo>();
	
	
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 1, 113, start_date, end_date, judges));
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 1, 114, start_date, end_date, judges));
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 1, 115, start_date, end_date, judges));

                if (FirstGrAfterConsideration)
                    cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 2, 1130, start_date, end_date, judges));
                else
                    cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 2, 113, start_date, end_date, judges));
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 2, 114, start_date, end_date, judges));
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 2, 115, start_date, end_date, judges));

                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 5, 110, start_date, end_date, judges));
                cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 5, 111, start_date, end_date, judges));
                
                
                if (FirstAdmAfterConsideration)
                	cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 5, 1130, start_date, end_date, judges));
				else 
					cases.AddRange(GetCasesFromSDP(connection, ot, SDPLinkName, 5, 113, start_date, end_date, judges));
	           
	            #endregion
	                
                
                FbConnection ExclConnection = new FbConnection(exclConStr.ToString());
                ExclConnection.Open();


                    //Начинаем искать неопубликованные дела


					int last_id = 0; //id последнего обработанного дела. вдруг дубли.
                    
                    
                    foreach (SDPCaseInfo cs in cases)
                    {
                    	
                    	if (cs.id == last_id) continue;
 
                        if (exclusions.IndexOf(cs.id) == -1)
                        {
                            //Дело не в исключениях
                            
                            if (cs.vidpr == 2 && cs.stage == 113) //Первая гражданская инстанция. Будем отсеивать материалы 9- и М-
                            {
                                string case_type = cs.number.Substring(0, 1);
                                if (case_type == "9" || case_type == "М")
                                {
                                    //Нашли материал, решаем, что с ним делать
                                    if (!ShowCanceledCivil) continue;

                                    if (AutoExcludeCanceledCivil)
                                    {
                                    	//Logging.Log("Get unpublished cases. Auto exclude 9,M",cs.number+ " show: "+ShowCanceledCivil.ToString()+ " exclude:"+AutoExcludeCanceledCivil.ToString());
                                    	DataModel._AddToExcluded(cs.id, cs.number, "Дело не рассматривалось по существу", cs.date, cs.vidpr, cs.stage, "auto");
                                        continue;
                                    }
                                }
                            }
                                
                               
                                if (cs.inBSR) //Дело загружено в БСР
                                {

                                 //Проверим на правила автоисключения БСР
                                    using (OracleCommand autoExclCommand = new OracleCommand())
                                    {
                                        autoExclCommand.Connection = connection;
                                        autoExclCommand.Transaction = ot;
                                        autoExclCommand.CommandType = CommandType.Text;
                                        autoExclCommand.CommandText = string.Format("SELECT de.prich FROM  BSR.BSRP b left join bsr.rub_docum r on b.id_docum = r.id_docum inner join rub_docum_exception de on r.rubrikat = de.vncode WHERE ID_AGORA = {0}",cs.id);

                                        string denyreason = (string)autoExclCommand.ExecuteScalar();

                                        if (denyreason != null)
                                        {
                                            //Что-то нашли, добавляем в исключения
                                            DataModel._AddToExcluded(cs.id, cs.number, denyreason, cs.date, cs.vidpr, cs.stage, "bsr");
                                            continue;
                                        }
                                    }

                                }


                                int days_remains = 0;
                                if (ShowDaysAfterConsideration)
                                    days_remains = (int)((end_date.Date - cs.date.Date).TotalDays);
                                else
                                    days_remains = (pubDays - (int)((end_date.Date - cs.date.Date).TotalDays));
                                result.Add(new PublishListItem(cs.id, days_remains, cs.number, (string)groupNames[cs.judge_id], cs.vidpr, cs.stage, cs.inBSR, cs.ready_to_publish,ShowDaysAfterConsideration,pubDays));

                            }
                        
                    }


                



                ExclConnection.Close();
                ot.Commit();
                connection.Close();

            }

            return result;
        }

        public static SummaryInfo GetSummaryInfo(DateTime startDate, DateTime endDate, bool useMaterials, bool ShowCanceledCivil, bool FirstCriminalValidity, int pubDays, string SDPLinkName)
        {
            SummaryInfo result = new SummaryInfo();

            DateTime start_time = DateTime.Now;

            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;

            #region connectionStrings

            FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
            {
                Charset = "WIN1251",
                UserID = mySettings.exclUser,
                Password = mySettings.exclPass,
                Database = mySettings.exclBase,
                ServerType = FbServerType.Default
            };


            string bsr_pass = mySettings.bsrPass;
            string bsr_db = mySettings.bsrBase;
            string bsr_login = mySettings.bsrUser;

            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;

            #endregion

            #region Выбираем из нашей базы исключения
            
            Hashtable exclusions = new Hashtable();
            using (FbConnection connection = new FbConnection(exclConStr.ToString()))
            {
                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS";

                    FbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string dumbDate = reader.GetString(4);
                        ExclusionInfo ei = new ExclusionInfo(){
                            id = reader.GetInt32(0),
                            case_id = reader.GetInt32(1),
                            number = reader.GetString(2),
                            reason = reader.GetString(3),
                            date = new DateTime(int.Parse(dumbDate.Substring(0,4)),int.Parse(dumbDate.Substring(4,2)),int.Parse(dumbDate.Substring(6,2))),
                            vidpr = reader.GetInt32(5),
                            stage = reader.GetInt32(6),
                            FIO = reader.GetString(7)
                        };
                        exclusions.Add(ei.case_id,ei);
                    }
                }

                connection.Close();
            }

            #endregion



            using (OracleConnection connection = new OracleConnection(bsrConnStr))
            {
                connection.Open();

                OracleTransaction ot = connection.BeginTransaction();
         
                int[][] stages;
                int stage_count = 0;
                if (ShowCanceledCivil) {
	                stages = new int[][] {
	                    new int[] {113, 114, 115}, //УГ
	                    new int[] {113, 114, 115}, //ГР
	                    new int[] {113}, //М
	                    new int[] {110,111,113,115}, //АДМ
	                    new int[] {113} //Гр. возвраты
	                };
                	stage_count = 5;
                } else {
                	stages = new int[][] {
	                    new int[] {113, 114, 115}, //УГ
	                    new int[] {113, 114, 115}, //ГР
	                    new int[] {113}, //М
	                    new int[] {110,111,113,115} //АДМ
	                };
                	stage_count = 4;
                }

                
                //Logging.Log("summary first mode",FirstCriminalValidity.ToString());
                
                for (int vidpr = 0; vidpr < stage_count; vidpr++)
                {
                    foreach (int stage in stages[vidpr])
                    {
                        
                    	bool ValidityCheck = (!FirstCriminalValidity && (vidpr == 1 || vidpr == 3 || vidpr == 0) && stage == 113) || (FirstCriminalValidity && vidpr == 0 && stage == 113);
                    	
                    	//Logging.Log("summary new stage", "vidpr: "+vidpr.ToString()+" stage:"+stage.ToString()+" validity check: "+ValidityCheck.ToString());
                    	
                    	if (!useMaterials && vidpr == 2) continue;
                    	
                         SummaryInfoItem info = new SummaryInfoItem();

                        #region Выбираем нужный запрос

                        string stmt = "";
                        switch (vidpr)
                        {
                            case 0: //уголовные
                                switch (stage)
                                {

                                    case 113: //Первая инстанция
                                        stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_DATE, c.VALIDITY_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM U1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.VERDICT_DATE >= :sd) AND (c.VERDICT_DATE <= :ed) ORDER BY c.ID, c.VERDICT_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                    case 114: //Кассация
                                        stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_II_DATE, c.VERDICT_II_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM U2_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.id is not null) AND (c.VERDICT_II_DATE >= :sd) AND (c.VERDICT_II_DATE <= :ed) AND (c.VERDICT_II_ID = 50190000) ORDER BY c.ID, c.VERDICT_II_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                    case 115: //Единый Надзор
                                        stmt = "SELECT c.ID, c.proceeding_full_number, c.protest_verdict_date, c.protest_verdict_date, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM u33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.protest_VERDICT_DATE >= :sd) AND (c.protest_VERDICT_DATE <= :ed) AND (c.verdict_by_protest = 11050001) ORDER BY c.ID, c.protest_VERDICT_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                }
                                break;
                            case 1: //гражданские
                                switch (stage)
                                {
                                    case 113: //Первая инстанция
                                        stmt = "SELECT c.ID, c.FULL_NUMBER, c.RESULT_DATE, c.VALIDITY_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM G1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.DECISION_ID = 50580000 or c.CASE_TYPE_ID = 50520004) AND (c.RESULT_DATE >= :sd) AND (c.RESULT_DATE <= :ed) ORDER BY c.ID, c.DECISION_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                    case 114: //Аппеляция
                                        stmt = "SELECT c.ID, c.Z_FULLNUMBER, c.DRESALT, c.DRESALT, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM GR2_DELO@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.DRESALT >= :sd) AND (c.DRESALT <= :ed) AND (c.IDRESH NOT IN (911000001,911000002,911000003,911000004,911000005,911000006,50440013)) ORDER BY c.ID, c.DRESALT desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                    case 115: //Кассация
                                        stmt = "SELECT c.ID, c.proceeding_full_number, c.protest_verdict_date, c.protest_verdict_date, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM g33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.protest_VERDICT_DATE >= :sd) AND (c.protest_VERDICT_DATE <= :ed) ORDER BY c.ID, c.protest_VERDICT_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                }
                                break;
                            case 2: //производство по материалам
                                switch (stage)
                                {
                                    case 113:
                                        stmt = "SELECT c.ID, c.FULL_NUMBER, c.VERDICT_DATE,c.VALIDITY_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM m_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (VERDICT_DATE >= :sd) AND (VERDICT_DATE <= :ed) ORDER BY c.ID, c.VERDICT_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                }
                                break;
                            case 3: //административные
                                switch (stage)
                                {
                                    case 110: //Первый пересмотр
                                        stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, c.DECREE_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC  FROM adm1_case@{0} c  LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.DECREE_DATE >= :sd) AND (c.DECREE_DATE <= :ed)  ORDER BY c.ID, c.DECREE_DATE desc, NVL(b.PRPUB,0) DESC"; //AND (adm1_case.DECREE_ID NOT IN (70100005,70100006))
                                        break;
                                    case 111: //Второй пересмотр
                                        stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, c.DECREE_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM adm2_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.DECREE_DATE >= :sd) AND (c.DECREE_DATE <= :ed)  ORDER BY c.ID, c.DECREE_DATE desc, NVL(b.PRPUB,0) DESC"; //AND (adm2_case.DECREE_ID NOT IN (70030007,70030008))
                                        break;
                                    case 113: //Первая инстанция
                                        stmt = "SELECT c.ID, c.CASE_NUMBER, c.DECREE_DATE, c.TAKE_LAW_EFFECT_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM adm_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.DECREE_DATE >= :sd) AND (c.DECREE_DATE <= :ed) ORDER BY c.ID, c.DECREE_DATE desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                    case 115: //Надзор
                                        stmt = "SELECT c.id, c.full_number, c.verdict_date, c.verdict_date, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM a33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE (c.verdict_date >= :sd) AND (c.verdict_date <= :ed)  ORDER BY c.ID, c.verdict_date desc, NVL(b.PRPUB,0) DESC";
                                        break;
                                }
                                break;
                             case 4://Возвраты
                                case 113: 
                                    stmt = "SELECT c.ID, c.FULL_NUMBER, c.RESULT_DATE, c.VALIDITY_DATE, b.ID_DOCUM, b.PRPUB, b.DATEPUBLIC FROM G1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID = b.ID_AGORA WHERE c.DECISION_ID <> 50580000 AND c.CASE_TYPE_ID <> 50520004 AND (c.RESULT_DATE >= :sd) AND (c.RESULT_DATE <= :ed) ORDER BY c.ID, c.DECISION_DATE desc, NVL(b.PRPUB,0) DESC";
                                    break;
                        }
                        stmt = string.Format(stmt,SDPLinkName);
                        #endregion


                        using (OracleCommand command = new OracleCommand())
                        {
                            command.Connection = connection;
                            command.Transaction = ot;
                            command.CommandType = CommandType.Text;
                            command.CommandText = stmt;
                            command.CommandTimeout = 60;

                            OracleParameter startDateParam = new OracleParameter()
                            {
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = ":sd",
                                Value = startDate
                            };

                            OracleParameter endDateParam = new OracleParameter()
                            {
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                ParameterName = ":ed",
                                Value = endDate
                            };

                            command.Parameters.Add(startDateParam);
                            command.Parameters.Add(endDateParam);

                            OracleDataReader reader = command.ExecuteReader();


                            int last_id = 0;
                            while (reader.Read())
                            {                                
                                int case_id = reader.GetInt32(0);

                                if (case_id == last_id) { continue; } //На случай дублей в БСР
                                else { last_id = case_id; }

                                info.completed++;

                                DateTime considered = reader.GetDateTime(2); //Дата рассмотрения

                                DateTime valididty_date = considered;

                               
                                if (reader[3] != DBNull.Value) // Дата вступления в зак. силу
                                {
                                    valididty_date = reader.GetDateTime(3);
                                }

                                if (!reader.IsDBNull(4))
                                {
                                    info.imported++;
                                }

                                
                                SummaryInfoLates unpubl = new SummaryInfoLates();
                                unpubl.case_number = reader.GetString(1);
                                
                                if (exclusions[case_id] != null) //Дело есть в исключениях
                                {
                                    
                                	ExclusionInfo ei = (ExclusionInfo)exclusions[case_id];
                                    
                                    #region Грязный хак для приведения причин автоисключения БСР к нашему виду
                                    if (ei.reason.IndexOf("п.5.1") != -1) ei.reason = "Затрагивающие безопасность государства";
                                    else if (ei.reason.IndexOf("п.5.2") != -1) ei.reason = "Возникающие из семейно-правовых отношений, в том числе по делам об усыновлении (удочерении) ребенка, другим делам, затрагивающим права и законные интересы несовершеннолетних";
									else if (ei.reason.IndexOf("п.5.3") != -1) ei.reason = "О преступлениях против половой неприкосновенности и половой свободы личности";
									else if (ei.reason.IndexOf("п.5.4") != -1) ei.reason = "Об ограничении дееспособности гражданина или о признании его недееспособным";
									else if (ei.reason.IndexOf("п.5.5") != -1) ei.reason = "О принудительной госпитализации гражданина в психиатрический стационар и принудительном психиатрическом освидетельствовании";
									else if (ei.reason.IndexOf("п.5.6") != -1) ei.reason = "О внесении исправлений или изменений в запись актов гражданского состояния";
									else if (ei.reason.IndexOf("п.5.7") != -1) ei.reason = "Об установлении фактов, имеющих юридическое значение";
									else if (ei.reason.IndexOf("По решению судьи") != -1) ei.reason = "По решению судьи";
                                    #endregion
                                    
                                    if (!info.denied.ContainsKey(ei.reason)) info.denied.Add(ei.reason, 1);
                                    	else info.denied[ei.reason]++;
                                   
                                    if (ei.reason != "Дело не рассматривалось по существу") {
                                    	info.not_published_denied++;
                                    	info.not_published++;
                                    }
                                    	/*else {
                                    		Logging.Log("summary не рассматривалось по существу",reader[1].ToString());
                                    	}*/
                                }
                                else
                                {
                                    if (reader[5] != DBNull.Value && reader[6] != DBNull.Value && reader.GetInt32(5) == 1) //Дело опубликовано
                                    {
                                       	info.published++;
                                       	bool isInvalid = false;
                                       	if ((reader[3] == DBNull.Value || valididty_date > DateTime.Now)) //Не вступило в зак. силу
                                        {
                                       		if (ValidityCheck) {
                                       			info.published_invalid++;
                                       			//Logging.Log("summary published invalid",reader[1].ToString());
                                       			
                                       		}
                                       		
                                            isInvalid = true;
                                        } 
                                    	
                                       	DateTime pub_date = reader.GetDateTime(6);
                                       	unpubl.pub_date = pub_date;
                                       	
                                    	if (!isInvalid) { //Для расчёта срока по закону нас интересут только чтобы у дела была дата вступления в 
                                        	
	                                        
	                                        if ((valididty_date.AddMonths(1) - pub_date).Days < 0 ) {
	                                        	info.published_too_late_global++;
	                                        	unpubl.late_global = true;
	                                        	unpubl.control_date_global = valididty_date;
	                                        	unpubl.days_count_global = (pub_date-valididty_date).Days;
	                                        	
	                                        	//Logging.Log("summary late global",reader[1].ToString()+" valid date: "+valididty_date.ToString());
	                                        }
	
                                       	}
                                       	
                                       	if (!isInvalid || !ValidityCheck){
                                       	
	                                        if (!ValidityCheck)
	                                        { //Гражданские и административные от даты рассмотрения
	                                            valididty_date = considered;
	                                        }
	                                        int pub_time_local = (pub_date - valididty_date).Days;
	                                        if (pub_time_local > pubDays) {
	                                        	info.published_too_late_local++;
	                                        	unpubl.days_count_local = pub_time_local;
	                                        	unpubl.control_date_local = valididty_date;
	                                        	unpubl.late_local = true;
	                                        	//Logging.Log("summary late local","case num: "+reader[1].ToString()+" base date: "+valididty_date+" pub date: "+pub_date+" days: "+pub_time_local);
	                                        }
                                        	
                                       	}
  
                                    }
                                    else //Дело не опубликовано
                                    {
                                        info.not_published++;
										bool isInvalid = false;
                                        if (reader[3] == DBNull.Value || valididty_date > DateTime.Now) //Не вступило в зак. силу
                                        {
                                            if (ValidityCheck)
                                            {
                                                info.not_published_invalid++;
                                                isInvalid = true;
                                                
                                              //  Logging.Log("summary published invalid unpub",reader[1].ToString());
                                            }
                                        }
                                        
                                        if (!isInvalid || !ValidityCheck) {
                                        
                                        	if ((valididty_date.AddMonths(1) - DateTime.Now).Days < 0 ) {
                                        		info.not_published_and_late_global++;
                                        		unpubl.days_count_global = (DateTime.Now - valididty_date).Days;
                                        		unpubl.late_global = true;
                                        		unpubl.control_date_global = valididty_date;
                                        		
                                        		//Logging.Log("summary late global unpub",reader[1].ToString());
                                        	}
                                        	
                                        	
                                        	if (!ValidityCheck)
	                                        { //Гражданские и административные от даты рассмотрения
	                                            valididty_date = considered;
	                                        }
                                        	
                                            int pub_time_local = (DateTime.Now - valididty_date).Days;
                                            if (pub_time_local > pubDays) {
                                            	info.not_published_and_late_local++;
                                            	unpubl.days_count_local = pub_time_local;
                                            	unpubl.control_date_local = valididty_date;
                                            	unpubl.late_local = true;
                                            	
                                            	//Logging.Log("summary late local unpub","case num: "+reader[1].ToString()+" base date: "+valididty_date+" pub date: "+DateTime.Now+" days: "+pub_time_local);
                                            }
                                        }

                                    } 
                                }
                                
                                
                                if (unpubl.late_local || unpubl.late_global) result.SaveLate(vidpr+1,stage,unpubl);
                                
                            }

                        }

                        result.SaveInfo(vidpr+1, stage, info);


                    } // foreach
                } // for

                ot.Commit();
                connection.Close();
            }

            return result;
        }

        private enum excl_type
        {
            manual, auto
        }

        

        private struct SDPCaseInfo
        {
            public int id;
            public string number;
            public DateTime date;
            public int judge_id;
            public int stage;
            public int vidpr;
            public bool inBSR;
            public bool ready_to_publish;
        }

        public static void RemoveFromExcluded(int[] ids)
        {
            using (FbConnection connection = new FbConnection())
            {
                AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;
                FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
                {
                    Charset = "WIN1251",
                    UserID = mySettings.exclUser,
                    Password = mySettings.exclPass,
                    Database = mySettings.exclBase,
                    ServerType = FbServerType.Default
                };

                connection.ConnectionString = exclConStr.ToString();
                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM PUBLISH_EXCEPTIONS WHERE id = @id";

                    FbParameter param = new FbParameter()
                    {
                        DbType = DbType.Int32,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@id"
                    };

                    command.Parameters.Add(param);

                    foreach (int i in ids) {
                        param.Value = i;
                        command.ExecuteNonQuery();
                    }

                }

                connection.Close();
            }
        }

        public static ObservableCollection<ExclusionInfo> GetExcludedCases(int vidpr = 0, int stage = 0)
        {
            ObservableCollection<ExclusionInfo> result = new ObservableCollection<ExclusionInfo>();
            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;
            using (FbConnection connection = new FbConnection())
            {
                FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
                {
                    Charset = "WIN1251",
                    UserID = mySettings.exclUser,
                    Password = mySettings.exclPass,
                    Database = mySettings.exclBase,
                    ServerType = FbServerType.Default
                };

                connection.ConnectionString = exclConStr.ToString();
                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.Connection = connection;

                    if (vidpr > 0 && stage > 0)
                        command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS WHERE vidpr = @vidpr AND stage = @stage";
                    else
                    {
                        if (vidpr > 0 && stage == 0)
                            command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS WHERE vidpr = @vidpr";
                        else if (vidpr == 0 && stage > 0)
                            command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS WHERE stage = @stage";
                        else
                            command.CommandText = "SELECT * FROM PUBLISH_EXCEPTIONS";
                    }

                    command.CommandText += " ORDER BY id desc";

                    FbParameter vidprParam = new FbParameter() {
                        Direction = ParameterDirection.Input,
                        DbType = DbType.Int32,
                        ParameterName = "@vidpr"
                    };
                    if (vidpr > 0) vidprParam.Value = vidpr.ToString();
                    else vidprParam.Value = null;

                    command.Parameters.Add(vidprParam);

                    FbParameter stageParam = new FbParameter()
                    {
                        Direction = ParameterDirection.Input,
                        DbType = DbType.Int32,
                        ParameterName = "@stage"
                    };

                    if (stage > 0) stageParam.Value = stage.ToString();
                    else stageParam.Value = null;

                    command.Parameters.Add(stageParam);

                    FbDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string dateint = reader[4].ToString();
                        DateTime datedocum = new DateTime(int.Parse(dateint.Substring(0,4)),int.Parse(dateint.Substring(4,2)), int.Parse(dateint.Substring(6,2)));

                        ExclusionInfo info = new ExclusionInfo()
                        {
                            id = reader.GetInt32(0),
                            case_id = reader.GetInt32(1),
                            number = reader[2].ToString(),
                            reason = reader[3].ToString(),
                            date = datedocum,
                            vidpr = reader.GetInt32(5),
                            stage = reader.GetInt32(6),
                            FIO = reader[7].ToString()
                        };

                        result.Add(info);
                    }
                }

                connection.Close();
            }

            return result;
        }

        
        private static List<SDPCaseInfo> GetCanceledCases(OracleConnection connection, OracleTransaction ot, string SDPLinkName, DateTime start_date, DateTime end_date, string judges) {
        	List<SDPCaseInfo> result = new List<SDPCaseInfo>();
        	
        	using (OracleCommand command = new OracleCommand())
            {
                command.Connection = connection;
                command.Transaction = ot;
                command.CommandType = CommandType.Text;
                command.CommandText = string.Format("SELECT c.ID, c.FULL_NUMBER, JUDGE_ID FROM U1_CASE@{0} c WHERE (SELECT COUNT(*) FROM bsr.bsrp b WHERE b.id_agora = c.ID AND b.rubrikat = 100 AND b.prpub = 1 AND (b.datepublic >=:sd) AND (b.datepublic <=:ed)) > 0 AND c.VALIDITY_DATE is null AND c.VERDICT_DATE is not null AND c.JUDGE_ID IN {1}", SDPLinkName, judges);
                OracleParameter sd = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType = DbType.Date,
                    Value = start_date,
                    ParameterName = ":sd"
                };

                command.Parameters.Add(sd);

                OracleParameter ed = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType = DbType.Date,
                    Value = end_date,
                    ParameterName = ":ed"
                };

                command.Parameters.Add(ed);


               /* DateTime bench_start = DateTime.Now;*/
                OracleDataReader reader = command.ExecuteReader();
               /* Logging.Log("query benchmark", "query: " + command.CommandText + "\n running:" + (DateTime.Now - bench_start).TotalMilliseconds.ToString() + "мс");
                bench_start = DateTime.Now;*/

                while (reader.Read())
                {
                    SDPCaseInfo info = new SDPCaseInfo();
                    info.id = reader.GetInt32(0);
                    info.number = reader[1].ToString();
                    info.date = new DateTime();
                    info.judge_id = reader.GetInt32(2);
                    info.stage = 113;
                    info.vidpr = 1;
                    result.Add(info);
                }

                reader.Close();

               /* Logging.Log("fetch data benchmark", (DateTime.Now - bench_start).TotalMilliseconds.ToString() + "мс");*/
            }

            return result;
        	
        }
        
        private static List<SDPCaseInfo> GetCasesFromSDP(OracleConnection connection, OracleTransaction ot, string SDPLinkName, int case_type, int case_stage, DateTime start_date, DateTime end_date, string judges)
        {
            List<SDPCaseInfo> result = new List<SDPCaseInfo>();
            
            string stmt = "";
            switch (case_type) {
		        case 01: //уголовные
			        switch (case_stage) {
				
				        case 113: //Первая инстанция и апелляция
                            stmt = "SELECT ID, FULL_NUMBER, VALIDITY_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM U1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (VALIDITY_DATE >=:sd) AND (VALIDITY_DATE <= :ed) and VERDICT_DATE is not null AND JUDGE_ID IN {1}";
				        break;
				        case 114: //Кассация
					        stmt = "SELECT ID, FULL_NUMBER, VERDICT_II_DATE, SPEAKER, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM U2_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA  WHERE (id is not null) AND (VERDICT_II_DATE >= :sd) AND (VERDICT_II_DATE <= :ed) AND (VERDICT_II_ID = 50190000) AND SPEAKER in {1}";
				        break;
				        case 115: //Единый Надзор
					        stmt = "SELECT ID, proceeding_full_number, protest_verdict_date, presiding_judge, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM u33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA  WHERE (protest_VERDICT_DATE >= :sd) AND (protest_VERDICT_DATE <= :ed) AND (verdict_by_protest = 11050001) AND presiding_judge in {1}";
				        break;
			        }
		        break;
		        case 02: //гражданские
			        switch (case_stage) { 
				        case 113: //Первая инстанция
					        stmt = "SELECT ID, CASE_FULL_NUMBER, VALIDITY_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM G1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (VALIDITY_DATE >= :sd) AND (VALIDITY_DATE <= :ed) AND JUDGE_ID IN {1}";                   
                        break;
                        case 1130:
                        //Просили, чтобы показывалось сразу после рассмотрения 
                        	stmt = "SELECT ID, CASE_FULL_NUMBER, RESULT_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM G1_CASE@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (RESULT_DATE >= :sd) AND (RESULT_DATE <= :ed)  AND JUDGE_ID IN {1}";
                        case_stage = 113;
                        break;
				        case 114: //Аппеляция
					        stmt = "SELECT ID, Z_FULLNUMBER, DRESALT, IDDOC, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM GR2_DELO@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (DRESALT >= :sd) AND (DRESALT <= :ed) AND (IDRESH NOT IN (911000001,911000002,911000003,911000004,911000005,911000006,50440041,50440042,50440043,50440044,50440012,50440013)) and IDDOC in {1}";
				        break;
				        case 115: //Кассация
					        stmt = "SELECT ID, proceeding_full_number, protest_verdict_date, presiding_judge, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM g33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (protest_VERDICT_DATE >= :sd) AND (protest_VERDICT_DATE <= :ed) and presiding_judge in {1}";
				        break;
			        }
		        break;
		        case 05: //административные
			        switch (case_stage) {
				        case 110: //Первый пересмотр
					        stmt = "SELECT ID, CASE_NUMBER, DECREE_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM adm1_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed) and JUDGE_ID in {1}"; // AND (adm1_case.DECREE_ID NOT IN (70100005,70100006))
				        break;
				        case 111: //Второй пересмотр
					        stmt = "SELECT ID, CASE_NUMBER, DECREE_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1)  FROM adm2_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed) and JUDGE_ID in {1}"; // AND (adm2_case.DECREE_ID NOT IN (70030007,70030008))
				        break;
				        case 113: //Первая инстанция после вступления в зак силу
					        stmt = "SELECT ID, CASE_NUMBER, TAKE_LAW_EFFECT_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM adm_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (TAKE_LAW_EFFECT_DATE >= :sd) AND (TAKE_LAW_EFFECT_DATE <= :ed) and JUDGE_ID in {1}";
				        break;
				       	case 1130://Первая инстанция сразу после рассмотрения
				        	stmt = "SELECT ID, CASE_NUMBER, DECREE_DATE, JUDGE_ID, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM adm_case@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE (DECREE_DATE >= :sd) AND (DECREE_DATE <= :ed) and JUDGE_ID in {1}";
				        break;
				        case 115: //Надзор
					        stmt = "SELECT id, full_number, verdict_date, judge_study_assist_id, b.PRPUB, b.ID_DOCUM, (SELECT 1 FROM BSR.TEXT_DOCUM t WHERE t.ID_DOCUM = b.ID_DOCUM AND PRPUB = 3 AND ROWNUM =1) FROM a33_proceeding@{0} c LEFT JOIN bsr.bsrp b ON c.ID=b.ID_AGORA WHERE verdict_date >= :sd AND verdict_date <= :ed and judge_study_assist_id in {1}";
				        break;
			        }
		        break;
	        }

            stmt = string.Format(stmt, SDPLinkName, judges);

            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = connection;
                command.Transaction = ot;
                command.CommandType = CommandType.Text;
                command.CommandText = stmt;

                OracleParameter sd = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType = DbType.Date,
                    Value = start_date,
                    ParameterName = ":sd"
                };

                command.Parameters.Add(sd);

                OracleParameter ed = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType = DbType.Date,
                    Value = end_date,
                    ParameterName = ":ed"
                };

                command.Parameters.Add(ed);


                //DateTime bench_start = DateTime.Now;
                OracleDataReader reader = command.ExecuteReader();
               // Logging.Log("query benchmark", "query: " + stmt + "\n running:" + (DateTime.Now - bench_start).TotalMilliseconds.ToString() + "мс");
                /*bench_start = DateTime.Now;*/

                while (reader.Read())
                {
                	if (reader[4] != DBNull.Value && reader.GetInt32(4) == 1) continue; //Дело опубликовано. Пропускаем
                	
                	SDPCaseInfo info = new SDPCaseInfo();
                    info.id = reader.GetInt32(0);
                    info.number = reader[1].ToString();
                    info.date = reader.GetDateTime(2);
                    info.judge_id = reader.GetInt32(3);
                    info.stage = case_stage;
                    info.vidpr = case_type;
                    
                    if (reader.IsDBNull(5)) info.inBSR = false;
                    else info.inBSR = true;
                    
                    if (reader[6] != DBNull.Value) info.ready_to_publish = true;
                    else info.ready_to_publish = false;
                    
                    result.Add(info);
                }
                reader.Close();

                /*Logging.Log("fetch data benchmark", (DateTime.Now - bench_start).TotalMilliseconds.ToString() + "мс");*/
            }

            return result;
        }

        private static int _AddToExcluded(int id, string CaseNumber, string message, DateTime datedocum, int vidpr, int stage, string username)
        {
            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;
            int result = 0;
            using (FbConnection connection = new FbConnection())
            {
                string excl_pass = mySettings.exclPass;
                string excl_db = mySettings.exclBase;
                string excl_login = mySettings.exclUser;

                FbConnectionStringBuilder conStr = new FbConnectionStringBuilder()
                {
                    Charset = "WIN1251",
                    UserID = excl_login,
                    Password = excl_pass,
                    Database = excl_db,
                    ServerType = FbServerType.Default
                };
                connection.ConnectionString = conStr.ToString();

                connection.Open();

                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format("DELETE FROM PUBLISH_EXCEPTIONS WHERE CASE_ID = {0}", id);
                    command.ExecuteNonQuery();
                }
                
                
                if (message.Length > 255) {
                	Logging.Log("Add to excluded",string.Format("Дело {0} Слишком длинная причина исключения. Пришлось обрезать до 255 символов",CaseNumber));
                	message = message.Substring(0,255);
                }
                
                if (CaseNumber.Length > 64) {
                	Logging.Log("Add to excluded",string.Format("Дело {0} Слишком длинный номер дела. Пришлось обрезать до 64 символов",CaseNumber));
                	CaseNumber = CaseNumber.Substring(0,64);
                }
                
                if (username.Length > 255) {
                	Logging.Log("Add to excluded",string.Format("Дело {0} Слишком длинное имя пользователя. Пришлось обрезать до 255 символов",CaseNumber));
                	username = username.Substring(0,255);
                }
                
                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format("INSERT INTO PUBLISH_EXCEPTIONS (ID,CASE_ID,CASE_NUMBER,REASON,DATEDOCUM,VIDPR,STAGE,FIO) VALUES (GEN_ID(GEN_PUBLISH_EXCEPTIONS_ID, 1),{0},'{1}','{2}',{3},'{4}','{5}','{6}')",
                                                        new object[] { id, CaseNumber, message, datedocum.ToString("yyyyMMdd"), vidpr, stage, username});
                    command.ExecuteNonQuery();
                }

                using (FbCommand command = new FbCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = string.Format("SELECT ID FROM PUBLISH_EXCEPTIONS WHERE CASE_ID = {0}", id);
                    result =  (int)command.ExecuteScalar();
                }

                connection.Close();
            }

            return result;
        }

        public static bool AddToExcluded(string SDPLinkname, ExcludeInfo info, bool FirstGrAfterConsideration = false)
        {
            #region PrepareArguments

            DateTime datedocum = DateTime.Now.AddDays(info.item.DaysCount - 30); //Вычисляем дату рассмотрения на основе информации об оставшемся количестве дней
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string username = wi.Name;

            #endregion


            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;

                List<SDPCaseInfo> cases = new List<SDPCaseInfo>();

                string bsr_pass = mySettings.bsrPass;
	            string bsr_db = mySettings.bsrBase;
	            string bsr_login = mySettings.bsrUser;
	
	            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;
                
	            using (OracleConnection connection = new OracleConnection(bsrConnStr))
                {
                    connection.Open();

                    //Получим номер и дату
                    int vidpr = info.item.vidpr;
                    int stage = info.item.stage;
                    string stmt = "";
		            switch (vidpr) {
			            case 1: //уголовные
				            switch (stage) {
					            case 113: //Первая инстанция и апелляция
						            stmt = "SELECT VALIDITY_DATE, VERDICT_DATE FROM U1_CASE@{0} WHERE ID={1}";
					            break;
					            case 114: //Кассация
                                stmt = "SELECT VERDICT_II_DATE FROM U2_CASE@{0} WHERE id={1}";
					            break;
					            case 115: //Единый Надзор
                                stmt = "SELECT protest_verdict_date FROM u33_proceeding@{0} WHERE id={1}";
					            break;
				            }
			            break;
			            case 2: //гражданские
				            switch (stage) {
					            case 113: //Первая инстанция
			            		case 1130:
				            		stmt = "SELECT RESULT_DATE FROM G1_CASE@{0} WHERE id={1}";
					            break;
					            case 114: //Аппеляция
                                stmt = "SELECT DRESALT FROM GR2_DELO@{0} WHERE  id={1}";
					            break;
					            case 115: //Кассация
                                stmt = "SELECT protest_verdict_date FROM g33_proceeding@{0} WHERE  id={1}";
					            break;
				            }
			            break;
			           case 4: //материалы
			            	switch (stage) {
			            	case 113:
			            		stmt = "SELECT VERDICT_DATE FROM m_case@{0} WHERE id={1}"; 
			            		break;
			            	}
			            break;
			            case 5: //административные
				            switch (stage) {
					            case 110: //Первый пересмотр
                                    stmt = "SELECT DECREE_DATE FROM adm1_case@{0} WHERE  id={1}";
					            break;
					            case 111: //Второй пересмотр
                                	stmt = "SELECT DECREE_DATE FROM adm2_case@{0} WHERE  id={1}";
					            break;
					            case 113: //Первая инстанция
					            case 1130:
                                	stmt = "SELECT DECREE_DATE FROM adm_case@{0} WHERE  id={1}";
					            break;
					            case 115: //Надзор
                                	stmt = "SELECT verdict_date FROM a33_proceeding@{0} WHERE  id={1}";
					            break;
				            }
			            break;
		            }

                    stmt = string.Format(stmt, SDPLinkname, info.item.id);

                    using (OracleCommand command = new OracleCommand())
                    {
                        command.CommandType = CommandType.Text;
                        command.Connection = connection;
                        command.CommandText = stmt;

                        OracleDataReader reader = command.ExecuteReader();
                        if (!reader.Read()) throw new Exception(string.Format("Дело с ID {0} и номером {1} не найдено в базе денных",info.item.id,info.item.CaseNumber));

                        
                        if ((vidpr == 1 || vidpr == 2) && stage == 113 && reader[0] == DBNull.Value) {
                        	datedocum = reader.GetDateTime(1);
                        }
                        else {
                        	datedocum = reader.GetDateTime(0);
                        }
                        
                    }

                    connection.Close();


                DataModel._AddToExcluded(info.item.id, info.item.CaseNumber, info.message, datedocum, info.item.vidpr, info.item.stage, username);
            }

            return true;
        }
        
        public static NotPublishedListItem GetCaseInfo(int id, int vidpr, int stage, bool canceled, string SDPLinkname) {
        	
		    string stmt = "";
            switch (vidpr)
            {
                case 01: //уголовные
                    switch (stage)
                    {
                        
                        case 113: //Первая инстанция
                            stmt = "SELECT c.FULL_NUMBER, c.VERDICT_DATE, g.USERNAME, d.NAME || ' ' ||  d.LAW_ARTICLE, c.VALIDITY_DATE, c.RETURN_OFFICE_DATE FROM U1_CASE@{0} c JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID INNER JOIN U1_DEFENDANT@{0} d ON d.CASE_ID = c.ID WHERE c.ID = :id";

                            break;
                        case 114: //Кассация
                            stmt = "SELECT c.FULL_NUMBER, c.VERDICT_II_DATE, g.USERNAME, d.NAME || ' ' ||  d.LOW_ARTICLE_I, c.VERDICT_II_DATE, c.CASE_GET_BACK FROM U2_CASE@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.SPEAKER = g.GROUPCONTENTID JOIN  U2_DEFENDANT@{0} d ON d.CASE_ID = c.ID WHERE c.ID = :id";
                            break;
                        case 115: //Единый Надзор
                            stmt = "SELECT c.proceeding_full_number, c.protest_verdict_date, g.USERNAME, d.name || ' ' || d.law_article, c.protest_verdict_date,, c.PROTEST_DELIVER_DATE FROM u33_proceeding@{0} inner JOIN GROUPCONTENT@{0} ON c.presiding_judge = g.GROUPCONTENTID JOIN  u33_parts@{0} d ON d.proc_id = c.ID WHERE c.ID = :id";
                            break;
                    }
                    break;
                case 02: //гражданские
                    switch (stage)
                    {
                        case 113: //Первая инстанция
                    	case 1130:
                            stmt = "SELECT c.FULL_NUMBER, c.RESULT_DATE, g.USERNAME, c.REQ_ESSENCE, c.VALIDITY_DATE, c.RETURN_OFFICE_DATE FROM G1_CASE@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                        case 114: //Аппеляция
                            stmt = "SELECT c.Z_FULLNUMBER, c.DRESALT, g.USERNAME, c.ESSENCE_STATEMENT, c.DRESALT, c.CASE_GET_BACK FROM GR2_DELO@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.IDDOC = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                        case 115: //Кассация
                            stmt = "SELECT c.proceeding_full_number, c.protest_verdict_date, g.USERNAME, coalesce(co.essence_statement,' '), c.protest_verdict_date, c.PROTEST_DELIVER_DATE FROM g33_proceeding@{0} c inner JOIN GROUPCONTENT@{0} g ON c.presiding_judge = g.GROUPCONTENTID inner join g33_complaint@{0} co on c.id = co.proc_id WHERE c.ID = :id";
                            break;
                    }
                    break;
                case 04: //производство по материалам
                    switch (stage)
                    {
                        case 113:
                            stmt = "SELECT c.FULL_NUMBER, c.VERDICT_DATE,g.USERNAME,c.essence,c.VALIDITY_DATE, c.RETURN_OFFICE_DATE FROM m_case@{0} c inner join groupcontent@{0} g on c.judge_id = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                    }
                    break;
                case 05: //административные
                    switch (stage)
                    {
                        case 110: //Первый пересмотр
                            stmt = "SELECT c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.LAW_ARTICLES1, c.TAKE_LAW_EFFECT_DATE, c.CASE_GET_BACK FROM adm1_case@{0} c  INNER JOIN   GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                        case 111: //Второй пересмотр
                            stmt = "SELECT c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.LAW_ARTICLES1, c.DECREE_DATE, c.CASE_GET_BACK FROM adm2_case@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                        case 113: //Первая инстанция
                        case 1130:
                            stmt = "SELECT c.CASE_NUMBER, c.DECREE_DATE, g.USERNAME, c.ESSENCE, c.TAKE_LAW_EFFECT_DATE, c.CASE_GET_BACK FROM adm_case@{0} c INNER JOIN   GROUPCONTENT@{0} g ON c.JUDGE_ID = g.GROUPCONTENTID WHERE c.ID = :id";
                            break;
                        case 115: //Надзор
                            stmt = "SELECT c.full_number, c.verdict_date, g.username, c.law_article, c.verdict_date, c.CASE_RETURN_DATE FROM a33_proceeding@{0} c INNER JOIN GROUPCONTENT@{0} g ON c.judge_study_assist_id = g.GROUPCONTENTID WHERE c.id = :id";
                            break;
                    }
                    break;
            }    	
        
            if (stmt == "") throw new Exception(string.Format("Странные значения параметров vidpr: {0}, stage: {1}",vidpr,stage));
            stmt = string.Format(stmt,SDPLinkname);
            
            #region connectionStrings
            AppSettings mySettings = ((App)System.Windows.Application.Current).mySettings;

            string bsr_pass = mySettings.bsrPass;
            string bsr_db = mySettings.bsrBase;
            string bsr_login = mySettings.bsrUser;

            string bsrConnStr = "Data Source=" + bsr_db + ";Password=" + bsr_pass + ";User ID=" + bsr_login;

            #endregion
            
            InfoListItem res = null;
            NotPublishedListItem result = null;
            
            using (OracleConnection conn = new OracleConnection(bsrConnStr)) {
            	conn.Open();
            	
            	
            	using (OracleCommand command = new OracleCommand()){
            	
					command.Connection = conn;
					command.CommandType = CommandType.Text;
					command.CommandText = stmt;
					
					OracleParameter sdpId = new OracleParameter() {
						DbType = DbType.Int32,
						Value = id,
						Direction = ParameterDirection.Input,
						ParameterName = ":id"
					};
					
					command.Parameters.Add(sdpId);
					
					OracleDataReader reader = command.ExecuteReader();
					while(reader.Read()){
						if (res != null) {
							res.info += ", "+reader.GetString(3);
							break;
						}
						
						res = new InfoListItem()
                        {
                            id = id,
                            number = reader.GetString(0),
                            date = reader.GetDateTime(1),
                            Judge = reader.GetString(2),
                            info = reader.GetString(3),                                 
                        };
						
						if (reader[4] != DBNull.Value)
                        {
                            res.validityDate = reader.GetDateTime(4);
                        }
						
						if (reader[5] != DBNull.Value)
                        {
                            res.returnDate = reader.GetDateTime(5);
                        }
						
                        res.vidpr = vidpr;
                        res.stage = stage;
					}
					
					reader.Close();
            	}
            	
            	
            	res.canceled = canceled;
            	result = new NotPublishedListItem(res);
            	
            	OracleCommand oraCommand = new OracleCommand()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ID_DOCUM, PRPUB, DATEPUBLIC, P_ANNOT FROM bsr.BSRP WHERE id_agora=:id"
                };

                OracleParameter oraIdAgora = new OracleParameter()
                {
                    Direction = ParameterDirection.Input,
                    DbType  = DbType.Int32,
                    ParameterName = ":id",
                    Value = id
                };

                oraCommand.Parameters.Add(oraIdAgora);
                    
                OracleDataReader oraReader = oraCommand.ExecuteReader();
                //Ищем дело в БСР
                //Вернуться может не одна запись, возможны дубли, поэтому проходим по всему результату и ищем опубликованный
                //Если опубликованных нет, то берём последний.

                OracleNumber OracleCaseId = new OracleNumber();
                DateTime publishDate = new DateTime();
                bool foundPub = false;
                string p_annot = "";

                if (!oraReader.HasRows) {

                    result.inBSR = false;
                    result.docStatus = "Не загружен";
          
                }
                else {

                    int oraCount = 0;
                    while (oraReader.Read())
                    {
                        oraCount++;
                        if (foundPub) continue; //Найдено опубликованное. Остальные итерации просто для подсчёта

                        object PRPUB = oraReader[1];

                        if (PRPUB != DBNull.Value && (decimal)PRPUB == 1)
                        {
                            foundPub = true;
                            p_annot = oraReader[3].ToString();
                            result.docStatus  = "Опубликовано";
                        }

                         OracleCaseId = oraReader.GetOracleNumber(0);


                         if (oraReader[2] != DBNull.Value)
                             publishDate = oraReader.GetDateTime(2);
                         else
                             publishDate = new DateTime();
                    }

                    if (oraCount > 1) result.hasCopies = true;
                    result.inBSR = true;

                }
                oraReader.Close();    
                
                //Проверяем статус приложенных документов
                if (result.inBSR && !foundPub)
                {
                	OracleCommand oraDocumentStatusCommand = new OracleCommand()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT PRPUB FROM BSR.TEXT_DOCUM WHERE ID_DOCUM = :id"
                    };

                    OracleParameter oraId = new OracleParameter() {
                        Direction = ParameterDirection.Input,
                        OracleType = System.Data.OracleClient.OracleType.Number,
                        ParameterName = ":id",
                        Value = id
                    };
                    oraDocumentStatusCommand.Parameters.Add(oraId);
                	
                    //Определим статус приложенных документов
                    oraId.Value = OracleCaseId;
                    OracleDataReader docsReader = oraDocumentStatusCommand.ExecuteReader();

                    result.ReadyToPublish = false;
                    if (!docsReader.HasRows)
                    {
                        result.docStatus = "Нет приложенных к делу документов";
                    }
                    else
                    {
                        string statText = "";
                        while (docsReader.Read())
                        {
                            string docstat = docsReader[0].ToString();
                            switch (docstat)
                            {
                                case "1":
                                    statText += "Пропущен через обезличиватель, ";
                                    break;
                                case "2":
                                    statText += "Документ в работе, ";
                                    break;
                                case "3":
                                    statText += "Готов к публикации, ";
                                    result.ReadyToPublish = true;
                                    break;

                                default:
                                    statText += "Документ не обработан, ";
                                    break;
                            }
                        }

                        result.docStatus = statText.Substring(0, statText.Length - 2); //Убираем запятую с пробелом
                    }
                }

            	
                conn.Close();
            	
            }
         
			return result;            
        }
    }

    class SecurityFunctions
    {

        static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt In My Wounds Burns Like Hell Today");

        public static string EncryptString(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.LocalMachine);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
    
    class ExcelExport {
    	public static bool ExportPeriod(DateTime query_time, DateTime sDate, DateTime eDate, GetCasesListResult res, string vidpr, string stage, BackgroundWorker export_worker, System.Windows.Controls.ListView PublishedList) 
    	{
    		Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();;
            try
            {

                int overal_count = res.Published.Count + res.NotPublished.Count + res.Excluded.Count;
                int progress = 0;

                if (excelApp == null)
                {
                    throw new Exception("Не могу подключиться к MS Excel. Возможно он не установлен");
                }
                
                Workbook wb = excelApp.Workbooks.Add(1);


                while (wb.Worksheets.Count != 1) wb.Worksheets.Delete();

                //Не опубликованные
                Worksheet ws = (Worksheet)wb.Worksheets[1];
                 Microsoft.Office.Interop.Excel.Range r = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("A1", System.Reflection.Missing.Value);
				r.EntireColumn.NumberFormat = "@";
				System.Runtime.InteropServices.Marshal.ReleaseComObject(r);

                ws.Cells[1, 1] = string.Format("Запрос выполнен {0}",query_time.ToString("dd.MM.yyyy HH:mm:ss"));
                ws.Cells[2, 1] = string.Format("Неопубликованные дела за период: {0} по {1}", sDate.ToString("dd.MM.yyyy"), eDate.ToString("dd.MM.yyyy"));
                
                
                
                
                ws.Cells[3, 1] = string.Format("{0} {1}", vidpr, stage);

                ws.Cells[4, 1] = "Номер";
                ws.Cells[4, 2] = "Дата рассмотрения";
                ws.Cells[4, 3] = "Дата вступления в законную силу";
                ws.Cells[4, 4] = "Судья";
                ws.Cells[4, 5] = "Комментарий";
                ws.Cells[4, 6] = "Статус";

                int row = 5;


                ws.Name = string.Format("Неопубликованные дела ({0})", res.NotPublished.Count);

                foreach (NotPublishedListItem item in res.NotPublished)
                {
                    ws.Cells[row, 1] = item.number;
                    ws.Cells[row, 2] = item.date.ToString("dd.MM.yyyy");
                    
                    if (item.validityDate == new DateTime())
                    {
                        ws.Cells[row, 3] = "Не задано";
                    }
                    else
                    {
                        ws.Cells[row, 3] = item.validityDate.ToString("dd.MM.yyyy");
                    }
                    
                    ws.Cells[row, 4] = item.Judge;
                    ws.Cells[row, 5] = item.info;
                    ws.Cells[row, 6] = item.docStatus;
                    row++;
                    if (++progress % 10 == 0)
                    {
                        export_worker.ReportProgress((int)(100.0 * progress / overal_count));
                    }
                }


                //Опубликованные
                wb.Worksheets.Add();
                ws = (Worksheet)wb.Worksheets[1];
                r = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("A1", System.Reflection.Missing.Value);
				r.EntireColumn.NumberFormat = "@";
				System.Runtime.InteropServices.Marshal.ReleaseComObject(r);

                ws.Cells[1, 1] = string.Format("Запрос выполнен {0}", query_time.ToString("dd.MM.yyyy HH:mm:ss"));
                ws.Cells[2, 1] = string.Format("Опубликованные дела за период: {0} по {1}", sDate.ToString("dd.MM.yyyy"), eDate.ToString("dd.MM.yyyy"));
                ws.Cells[3, 1] = string.Format("{0} {1}", vidpr, stage);

                ws.Cells[4, 1] = "Номер";
                ws.Cells[4, 2] = "Дата рассмотрения";
                ws.Cells[4, 3] = "Дата вступления в законную силу";
                ws.Cells[4, 4] = "Дата публикации";
                ws.Cells[4, 5] = "Срок публикации";
                ws.Cells[4, 6] = "Судья";
                ws.Cells[4, 7] = "Комментарий";

                row = 5;
                ObservableCollection<PublishedListItem> pub = (ObservableCollection<PublishedListItem>)PublishedList.ItemsSource;

                ws.Name = string.Format("Опубликованные дела ({0})", res.Published.Count);

                foreach (PublishedListItem item in res.Published)
                {
                    ws.Cells[row, 1] = item.number;
                    ws.Cells[row, 2] = item.date.ToString("dd.MM.yyyy");
                    if (item.validityDate == new DateTime())
                    {
                        ws.Cells[row, 3] = "Не задано";
                    }
                    else
                    {
                        ws.Cells[row, 3] = item.validityDate.ToString("dd.MM.yyyy");
                    }
                    ws.Cells[row, 4] = item.pubDate.ToString("dd.MM.yyyy");
                    if (item.PublicationLength == -10000)
                    {
                        ws.Cells[row, 5] = "Невозможно вычислить";
                    }
                    else
                    {
                        ws.Cells[row, 5] = item.PublicationLength;
                    }
                    ws.Cells[row, 6] = item.Judge;
                    ws.Cells[row, 7] = item.comment;
                    if (++progress % 10 == 0)
                    {
                        export_worker.ReportProgress((int)(100.0 * progress / overal_count));
                    }
                    row++;
                }

                //Исключенные
                wb.Worksheets.Add();
                ws = (Worksheet)wb.Worksheets[1];
                r = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("A1", System.Reflection.Missing.Value);
				r.EntireColumn.NumberFormat = "@";
				System.Runtime.InteropServices.Marshal.ReleaseComObject(r);


                ws.Cells[1, 1] = string.Format("Запрос выполнен {0}", query_time.ToString("dd.MM.yyyy HH:mm:ss"));
                ws.Cells[2, 1] = string.Format("Исключенные дела за период: {0} по {1}", sDate.ToString("dd.MM.yyyy"), eDate.ToString("dd.MM.yyyy"));
                ws.Cells[3, 1] = string.Format("{0} {1}", vidpr, stage);

                ws.Cells[4, 1] = "Номер";
                ws.Cells[4, 2] = "Дата рассмотрения";
                ws.Cells[4, 3] = "Судья";
                ws.Cells[4, 4] = "Причина исключения";
                ws.Cells[4, 5] = "Кто исключил";
                ws.Cells[4, 6] = "Статус";
                row = 5;


                ws.Name = string.Format("Исключенные дела ({0})", res.Excluded.Count);

                foreach (ExcludedListItem item in res.Excluded)
                {
                    ws.Cells[row, 1] = item.number;
                    ws.Cells[row, 2] = item.date.ToString("dd.MM.yyyy");
                    ws.Cells[row, 3] = item.Judge;
                    ws.Cells[row, 4] = item.reason;
                    ws.Cells[row, 5] = item.whoExcluded;
                    ws.Cells[row, 6] = item.docStatus;
                    if (++progress % 10 == 0)
                    {
                        export_worker.ReportProgress((int)(100.0 * progress / overal_count));
                    }
                    row++;
                }

                excelApp.Visible = true;


            }
            catch (Exception ex)
            {
                if (excelApp != null) excelApp.Visible = true;
                Logging.Log("export to excel", ex.Message + " " + ex.StackTrace);
                return false;
            }
            
            return true;
    	}
    	
    	public static bool ExportSummary(bool first_mode, bool show_canceled, bool show_late, DateTime sDate, DateTime eDate, SummaryInfo info, bool UseMaterials, int pub_days){
    		Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                if (excelApp == null)
                {
                    throw new Exception("Не могу подключиться к MS Excel. Возможно он не установлен");
                }


				excelApp.SheetsInNewWorkbook = 1;
                Workbook wb = excelApp.Workbooks.Add(System.Reflection.Missing.Value);
                while (wb.Worksheets.Count != 1) wb.Worksheets.Delete();
                
                Worksheet late_cases_ws = null;
                int current_late = 0;
                if (show_late) {
                	late_cases_ws  = wb.Worksheets.Add();
                	Microsoft.Office.Interop.Excel.Range r = (Microsoft.Office.Interop.Excel.Range)late_cases_ws.get_Range("A1", System.Reflection.Missing.Value);
					r.EntireColumn.NumberFormat = "@";
					System.Runtime.InteropServices.Marshal.ReleaseComObject(r);
                	late_cases_ws.Name = "C нарушением";
                	late_cases_ws.Cells[1,1] = "Список дел с нарушенным сроком публикации";
                	late_cases_ws.Cells[3,1] = "Номер дела";
                	late_cases_ws.Cells[3,2] = "Дата по регламенту";
                	late_cases_ws.Cells[3,3] = "Количество дней";
                	late_cases_ws.Cells[3,4] = "Опоздание по регламенту";
                	late_cases_ws.Cells[3,5] = "Дата согласно выбранного режима";
                	late_cases_ws.Cells[3,6] = "Количество дней";
                	late_cases_ws.Cells[3,7] = "Опоздание согласно локально установленного срока";
                	late_cases_ws.Cells[3,8] = "Дата публикаци";
                }
                
                
                int[][] stages;
                int stage_count = 0;
                if (show_canceled) {
	                stages = new int[][] {
	                    new int[] {0,0,0,0},
	                    new int[] {113, 114, 115}, //УГ
	                    new int[] {113, 114, 115}, //ГР
	                    new int[] {113}, //М
	                    new int[] {113,110,111,115}, //АДМ
	                    new int[] {113} //гр. возвраты
	                };
                	stage_count = 5;
                }
                else {
                	stages = new int[][] {
	                    new int[] {0,0,0,0},
	                    new int[] {113, 114, 115}, //УГ
	                    new int[] {113, 114, 115}, //ГР
	                    new int[] {113}, //М
	                    new int[] {113,110,111,115} //АДМ
	                };
                	stage_count = 4;
                }

                for (int vidpr = 0; vidpr <= stage_count; vidpr++)
                {
                	
                	if (!UseMaterials && vidpr == 3) continue;
                    
                	
                	
                	Worksheet ws = wb.Worksheets.Add();

                    ws.Cells[1, 1] = string.Format("Запрос выполнен {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    ws.Cells[2, 1] = string.Format("Общие данные за период: {0} по {1}", sDate.ToString("dd.MM.yyyy"), eDate.ToString("dd.MM.yyyy"));
					
                    if (first_mode)
	                	ws.Cells[2,2] = "Локальный срок считается с момента, предусмотренного Законом";
	                else
	                	ws.Cells[2,2] = "Локальный срок считается с даты вступления в законную силу";
                    
                    int summary_col = 0;
                    switch (vidpr)
                    {
                        case 0: //Итого
                            ws.Cells[4, 2] = "Уголовные";
                            ws.Cells[4, 3] = "Гражданские";
                            ws.Cells[4, 4] = "Материалы";
                            ws.Cells[4, 5] = "Административные";
                            ws.Cells[4, 6] = "Всего";
                            ws.Name = "Итого";
                            summary_col = 6;
                            break;
                        case 1:
                            ws.Cells[4, 2] = "1-я инстанция";
                            ws.Cells[4, 3] = "Апелляция (обл. суд)";
                            ws.Cells[4, 4] = "Кассация";
                            ws.Cells[4, 5] = "Всего";
                            summary_col = 5;
                            ws.Name = "Уголовные";
                            break;
                        case 2:
                        	ws.Cells[4, 2] = "1-я инстанция";
                            ws.Cells[4, 3] = "Апелляция (обл. суд)";
                            ws.Cells[4, 4] = "Кассация";
                            ws.Cells[4, 5] = "Всего";
                            summary_col = 5;
                            ws.Name = "Гражданские";
                            break;
                        case 3:
                            ws.Cells[4, 2] = "Материалы";
                            ws.Cells[4, 3] = "Всего";
                            ws.Name = "Материалы";
                            summary_col = 3;
                            break;
                        case 4:
                            ws.Cells[4, 2] = "1-я инстанция";
                            ws.Cells[4, 3] = "1-й пересмотр";
                            ws.Cells[4, 4] = "2-й пересмотр";
                            ws.Cells[4, 5] = "Надзор";
                            ws.Cells[4, 6] = "Всего";
                            ws.Name = "Административные";
                            summary_col = 6;
                            break;
                        case 5:
                            ws.Cells[4, 2] = "Материалы";
                            ws.Cells[4, 3] = "Всего";
                            ws.Name = "Возвращенные материалы";
                            summary_col = 3;
                            break;
                    }
                    
                    ws.Cells[5, 1] = "- общая информация -";
                    (ws.Cells[5, 1] as Microsoft.Office.Interop.Excel.Range).Font.Bold = true;
                    ws.Cells[6, 1] = "Рассмотрено всего";
                    ws.Cells[7, 1] = "Рассмотрено по существу";
                    ws.Cells[8, 1] = "По существу не рассматривалось";
                    ws.Cells[9, 1] = "Импортировано в БСР";
                    ws.Cells[10, 1] = "Не импортировано в БСР";
                    ws.Cells[11, 1] = "- опубликовано -";
                    (ws.Cells[11, 1] as Microsoft.Office.Interop.Excel.Range).Font.Bold = true;
                    ws.Cells[12, 1] = "Опубликовано всего";
                    ws.Cells[13, 1] = "из них с нарушением срока по регламенту (1 мес.)";
                    ws.Cells[14, 1] = string.Format("из них с нарушением локально установленного срока ({0} дней)",pub_days);
                    ws.Cells[15, 1] = "из них не вступивших в законную силу";
                    ws.Cells[16, 1] = "- не опубликовано -";
                    (ws.Cells[16, 1] as Microsoft.Office.Interop.Excel.Range).Font.Bold = true;
                    ws.Cells[17, 1] = "Не опубликовано всего";
                    ws.Cells[18, 1] = string.Format("из них не обработаны (\"зеленые\", \"желтые\" и \"красные\")",pub_days);
                    ws.Cells[19, 1] = "из них не вступило в законную силу";
                    ws.Cells[20, 1] = "из них с нарушением срока по регламенту (1 мес.)";
                    ws.Cells[21, 1] = string.Format("из них с нарушением локально установленного срока ({0} дней)",pub_days);
                    ws.Cells[22, 1] = "из них запрещено к публикации (кроме \"не рассм. по существу\")";
                    ws.Cells[23, 1] = " - запрещено к публикации -";
                    (ws.Cells[23, 1] as Microsoft.Office.Interop.Excel.Range).Font.Bold = true;

                    SummaryInfoItem sum = info.GetInfo(vidpr, 0);

                    PrintSummaryCommonInfo(ws, sum, summary_col);

                    Dictionary<string, int> all_deny_reasons = new Dictionary<string, int>();

                    int i = 0;
                    foreach (string reason in sum.denied.Keys)
                    {
                        if (reason != "Дело не рассматривалось по существу")
                        {
                            ws.Cells[24 + i, 1] = reason;
                            ws.Cells[24 + i, summary_col] = sum.denied[reason];
                            all_deny_reasons.Add(reason, i);
                            i++;
                        }
                    }

                    if (vidpr == 0)
                    {
                        for (i = 1; i <= 4; i++)
                        {
                            sum = info.GetInfo(i, 0);
                            PrintSummaryCommonInfo(ws, sum, 1 + i);

                            foreach (string reason in sum.denied.Keys)
                            {
                                if (reason != "Дело не рассматривалось по существу")
                                {
                                    ws.Cells[24 + all_deny_reasons[reason], 1 + i] = sum.denied[reason];
                                }
                            }
                        }
                        
                        if (!UseMaterials) { //Удаляем столбец с материалами
                       		Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("D1", System.Reflection.Missing.Value);
							range.EntireColumn.Delete(System.Reflection.Missing.Value);
							System.Runtime.InteropServices.Marshal.ReleaseComObject(range);
                        }
                    }
                    else {
                        for (i = 0; i < stages[vidpr].Length; i++)
                        {
                        	
                        	#region Выводим опоздавших
		                	if (show_late && vidpr != 0) {
		                		
                        		List<SummaryInfoLates> lates = info.GetLates(vidpr,stages[vidpr][i]);
                        		if (lates != null) {
	                        		foreach (SummaryInfoLates late in lates) {
	                        			late_cases_ws.Cells[4+current_late,1] = late.case_number;
	                        			
	                        			if (late.late_global) {
	                        				late_cases_ws.Cells[4+current_late,2] = late.control_date_global.ToShortDateString();
	                        				late_cases_ws.Cells[4+current_late,3] = late.days_count_global;
	                        				if (late.late_global) late_cases_ws.Cells[4+current_late,4] = "да";
	                        			}
	                        			else late_cases_ws.Cells[4+current_late,4] = "нет";
		
	                        			
	                        			if (late.late_local) {
	                        				late_cases_ws.Cells[4+current_late,5] = late.control_date_local.ToShortDateString();
		                        			late_cases_ws.Cells[4+current_late,6] = late.days_count_local;
		                        			late_cases_ws.Cells[4+current_late,7] = "да";
	                        			}
	                        			else late_cases_ws.Cells[4+current_late,7] = "нет";
	                        			
	                        			if (late.pub_date != new DateTime()) late_cases_ws.Cells[4+current_late,8] = late.pub_date.ToShortDateString();
	                        			current_late++;
	                        		}
                        		}
		                		
		                		
		                	}
		                	#endregion
                        	
                        	
                            sum = info.GetInfo(vidpr, stages[vidpr][i]);
                            PrintSummaryCommonInfo(ws, sum, 2 + i);

                            foreach (string reason in sum.denied.Keys)
                            {
                                if (reason != "Дело не рассматривалось по существу")
                                {
                                    ws.Cells[24 + all_deny_reasons[reason], 2+i] = sum.denied[reason];
                                }
                            }

                        }
                    }
	
                    //делаем первую колонку пошире
                    Microsoft.Office.Interop.Excel.Range r = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("A1", System.Reflection.Missing.Value);
					r.EntireColumn.ColumnWidth = 80;
					System.Runtime.InteropServices.Marshal.ReleaseComObject(r);

                    /*if (vidpr != stage_count)
                    {
                        wb.Worksheets.Add();
                    }*/
                }
                wb.Worksheets[wb.Worksheets.Count].Delete();
                excelApp.Visible = true;

            }
            catch (Exception ex)
            {
                if (excelApp != null) excelApp.Visible = true;
                Logging.Log("export summary to excel", ex.Message + " " + ex.StackTrace);
                return false;
            }
    		return true;
    	}
    	
    	private static void PrintSummaryCommonInfo(Worksheet ws, SummaryInfoItem info, int col)
        {
            ws.Cells[6, col] = info.completed;
            
            int canceled = info.denied.ContainsKey("Дело не рассматривалось по существу") ? info.denied["Дело не рассматривалось по существу"] : 0;

            ws.Cells[7, col] = info.completed - canceled;
            ws.Cells[8, col] = canceled;
            ws.Cells[9, col] = info.imported;
            ws.Cells[10, col] = info.completed - info.imported;
            ws.Cells[12, col] = info.published;
            ws.Cells[13, col] = info.published_too_late_global;
            ws.Cells[14, col] = info.published_too_late_local;
            ws.Cells[15, col] = info.published_invalid;
            
            ws.Cells[17, col] = info.not_published;
            ws.Cells[18, col] = info.not_published - info.not_published_invalid - info.not_published_denied;
            ws.Cells[19, col] = info.not_published_invalid;
            ws.Cells[20, col] = info.not_published_and_late_global;
            ws.Cells[21, col] = info.not_published_and_late_local;
            ws.Cells[22, col] = info.not_published_denied;
        }
    	
    	
    	public static bool Check() {
    		//Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
    		Type excelApp = Type.GetTypeFromProgID("Excel.Application");
    		if (excelApp == null) {
    			return false;
    		}
    		return true;
    	}
    	
    	public static bool ExportMainWindow(ObservableCollection<PublishListItem> info, bool ShowDaysAfterConsideration, int pub_days) {
    		Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            try
            {
                if (excelApp == null)
                {
                    throw new Exception("Не могу подключиться к MS Excel. Возможно он не установлен");
                }



                Workbook wb = excelApp.Workbooks.Add(1);
                while (wb.Worksheets.Count != 1) wb.Worksheets.Delete();
                
                Worksheet ws = (Worksheet)wb.Worksheets[1];
                
                
                Microsoft.Office.Interop.Excel.Range r = (Microsoft.Office.Interop.Excel.Range)ws.get_Range("B1", System.Reflection.Missing.Value);
				r.EntireColumn.NumberFormat = "@";
				System.Runtime.InteropServices.Marshal.ReleaseComObject(r);
                
                
                ws.Cells[1,1] = string.Format("Неопубликованные дела на  {0}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                
                if (ShowDaysAfterConsideration)
                	ws.Cells[3,1] = "Дней со дня рассмотрения";
                else 
                	ws.Cells[3,1] = string.Format("Осталось дней из {0}",pub_days);
                ws.Cells[3,2] = "Номер дела";
                ws.Cells[3,3] = "Судья";
                ws.Cells[3,4] = "Инстанция";
                ws.Cells[3,5] = "Этап";
                ws.Cells[3,6] = "Статус";
                
                int i=1;
                foreach (PublishListItem item in info) {
                	ws.Cells[3+i,1] = item.DaysCount;
                	ws.Cells[3+i,2] = item.CaseNumber;
                	ws.Cells[3+i,3] = item.JudgeName;
                	
                	
                		switch (item.vidpr) {
                		case 1:
                			ws.Cells[3+i,4] = "Уголовная";
                			break;
            			case 2:
            			ws.Cells[3+i,4] = "Гражданская";
            			break;
            			case 4:
                			ws.Cells[3+i,4] = "Производство по материалам";
                			break;
                		case 5:
                			ws.Cells[3+i,4] = "Административная";
                			break;
                	}
                		
                	switch (item.stage) {
                		case 110:
                			ws.Cells[3+i,5] = "Первый пересмотр";
                			break;
                		case 111:
                			ws.Cells[3+i,5] = "Второй пересмотр";
                			break;
                		case 113:
                			ws.Cells[3+i,5] = "1-я инстанция";
                			break;
                		case 114:
                			ws.Cells[3+i,5] = "Апелляция (обл. суд)";
                			break;
                		case 115:
                			ws.Cells[3+i,5] = "Кассация";
                			break;
                	}
                	
                	if (item.canceled) {
                		ws.Cells[3+i,6] = "Отменено!";
						ws.Cells[3+i,6].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255,176,222,255));
                	}
                	else {
	                	if (!item.inBSR) {
	                		ws.Cells[3+i,6] = "Не загружено";
							ws.Cells[3+i,6].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255,225,162,166));
	                	}
                		else {
                			if (item.ReadyToPublish) {
                				ws.Cells[3+i,6] = "Готово к публикации";
								ws.Cells[3+i,6].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.SpringGreen);
                			} else {
                				ws.Cells[3+i,6] = "Документ не обработан";
								ws.Cells[3+i,6].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(255,230,237,190));
                			}
                			
                		}
                	}
                	
                	i++;
                }
                
                	
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                if (excelApp != null) excelApp.Visible = true;
                Logging.Log("export unpublished to excel", ex.Message + " " + ex.StackTrace);
                return false;
            }
    		return true;
    	}
    	
    }
}
