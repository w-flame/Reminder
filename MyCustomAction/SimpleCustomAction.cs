using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

using FirebirdSql.Data.FirebirdClient;
using Microsoft.Deployment.WindowsInstaller;

namespace MyCustomAction
{
	public class SimpleCustomAction
	{
		
		private static string ReadValueFromConfigFile(XPathNavigator fileNavigator, string remainingPath)
        {
            string[] splittedXPath = remainingPath.Split(new[] { '/' }, 2);
            if (splittedXPath.Length == 0 || String.IsNullOrEmpty(remainingPath))
            {
                throw new Exception("Path incorrect.");
            }

            string xPathPart = splittedXPath[0];
            XPathNavigator nodeNavigator = fileNavigator.SelectSingleNode(xPathPart);

            if (splittedXPath.Length > 1)
            {
                // Recursion
                return ReadValueFromConfigFile(nodeNavigator, splittedXPath[1]);
            }
            else
            {
                return nodeNavigator.Value;
            }
        }
		
		private static void WriteValueToConfigFile(XPathNavigator fileNavigator, string remainingPath, string newValue)
        {
            string[] splittedXPath = remainingPath.Split(new[] { '/' }, 2);
            if (splittedXPath.Length == 0 || String.IsNullOrEmpty(remainingPath))
            {
                throw new Exception("Path incorrect.");
            }

            string xPathPart = splittedXPath[0];
            XPathNavigator nodeNavigator = fileNavigator.SelectSingleNode(xPathPart);

            if (splittedXPath.Length > 1)
            {
                // Recursion
                WriteValueToConfigFile(nodeNavigator, splittedXPath[1],newValue);
            }
            else
            {
            	nodeNavigator.SetValue(newValue);
            }
        }
		
		[CustomAction]
		public static ActionResult GetDBSettings(Session session)
		{
			try {
				string file_name = session["OLDCONFIGFILE"];
				
				XmlDocument configFile = new XmlDocument();
	
			    configFile.Load(file_name);
			
			    XPathNavigator fileNavigator = configFile.CreateNavigator();
			    
			    string path = "configuration/userSettings/RaionReminder.Properties.Settings/setting[@name=\"{0}\"]/value";
			    
			    session["DBPATH"] = ReadValueFromConfigFile(fileNavigator,string.Format(path,"exclBase"));
			    session["DBLOGIN"] = ReadValueFromConfigFile(fileNavigator,string.Format(path,"exclUser"));
			    
			    byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt In My Wounds Burns Like Hell Today");
			    byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(ReadValueFromConfigFile(fileNavigator,string.Format(path,"exclPass"))),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.LocalMachine);
			    
			    session["DBPASS"] = System.Text.Encoding.Unicode.GetString(decryptedData);
			    
			    if (bool.Parse(ReadValueFromConfigFile(fileNavigator,string.Format(path,"useLocalSettings"))) == true)
			    	session["DBUSE"] = "";
			    else 
			    	session["DBUSE"] = "1";
		    
		    } catch (Exception) {
				
				return ActionResult.Failure;
			}
		    return ActionResult.Success;
			
		}
		
		[CustomAction]
		public static ActionResult SaveDBSettings(Session session)
		{
			session.Log("action_parameters: "+session.CustomActionData.ToString());
			try
			{	
				XmlDocument configFile = new XmlDocument();
	
			    configFile.Load(session.CustomActionData["CONFFILE"]);
			
			    XPathNavigator fileNavigator = configFile.CreateNavigator();
			    
			    string path = "configuration/userSettings/RaionReminder.Properties.Settings/setting[@name=\"{0}\"]/value";
			    
			    WriteValueToConfigFile(fileNavigator, string.Format(path,"exclBase"),session.CustomActionData["DBPATH"]);
				WriteValueToConfigFile(fileNavigator, string.Format(path,"exclUser"),session.CustomActionData["DBLOGIN"]);
				
				string usedb = session.CustomActionData["DBUSE"];
			    if (usedb == "") //uselocal
			    	WriteValueToConfigFile(fileNavigator, string.Format(path,"useLocalSettings"),"True");
			    else
			    	WriteValueToConfigFile(fileNavigator, string.Format(path,"useLocalSettings"),"False");
			    
			    byte[] entropy = System.Text.Encoding.Unicode.GetBytes("Salt In My Wounds Burns Like Hell Today");
				byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(session.CustomActionData["DBPASS"]),
                entropy,
                System.Security.Cryptography.DataProtectionScope.LocalMachine);
			    
			    
				WriteValueToConfigFile(fileNavigator, string.Format(path,"exclPass"),Convert.ToBase64String(encryptedData));
				
				configFile.Save(session.CustomActionData["CONFFILE"]);
			}
			catch (Exception ex)
			{
				session.Log("savedb failed: "+ex.Message+ " " +ex.StackTrace);
				return ActionResult.Failure;
			}
			return ActionResult.Success;
		}
		
		[CustomAction]
		public static ActionResult CheckDBConnection(Session session)
		{
			try {
				FbConnectionStringBuilder conStr = new FbConnectionStringBuilder()
	            {
	                Charset = "WIN1251",
	                UserID = session["DBLOGIN"],
	                Password = session["DBPASS"],
	                Database = session["DBPATH"],
	                ServerType = FbServerType.Default
	            };
	
	            using (FbConnection connection = new FbConnection(conStr.ToString()))
	            {
	                    connection.Open();
	                    connection.Close();
	            }
			} catch (Exception exc) {			
				MessageBox.Show(string.Concat("Подключение не удалось: ",exc.Message),"Ошибка подключения",MessageBoxButtons.OK,MessageBoxIcon.Error,MessageBoxDefaultButton.Button1,0,false);
				
				return ActionResult.Success;
			}	
			MessageBox.Show("Подключение прошло успешно","Успешное подключение",MessageBoxButtons.OK,MessageBoxIcon.Information,MessageBoxDefaultButton.Button1,0,false);
			
			return ActionResult.Success;
		}
		
		[CustomAction]
		public static ActionResult UpdateDB(Session session)
		{
			session.Log("action_parameters: "+session.CustomActionData.ToString());
			try {
				FbConnectionStringBuilder conStr = new FbConnectionStringBuilder()
	            {
	                Charset = "WIN1251",
	                UserID = session.CustomActionData["DBLOGIN"],
	                Password = session.CustomActionData["DBPASS"],
	                Database = session.CustomActionData["DBPATH"],
	                ServerType = FbServerType.Default
	            };
	
	            using (FbConnection connection = new FbConnection(conStr.ToString()))
	            {
	                    connection.Open();
  
	                    try {
		                    using (FbCommand command = new FbCommand()) {
		                    	command.Connection = connection;
								command.CommandType = CommandType.Text;
								command.CommandText = "SELECT FIRST 1 NULL FROM SETTINGS";
								object o = command.ExecuteScalar();
		                    }
                    	} catch (Exception) {
	                    	
	                    	FbTransaction trans = connection.BeginTransaction();
	                    	
	                    	using (FbCommand command = new FbCommand()) {
		                    	command.Connection = connection;
								command.Transaction = trans;
	
								command.CommandType = CommandType.Text;
								command.CommandText = "CREATE TABLE SETTINGS (\"VERSION\" INTEGER NOT NULL, \"NAME\" VARCHAR(128) NOT NULL,\"VALUE\" VARCHAR(512))";
								command.ExecuteNonQuery();
								
								command.CommandText = "CREATE INDEX SETTINGS_IDX1 ON SETTINGS (\"NAME\")";
								command.ExecuteNonQuery();
								
								command.CommandText = "CREATE GENERATOR SETTINGSVERSION_GENERATOR";
								command.ExecuteNonQuery();
								
								command.CommandText = "SET GENERATOR SETTINGSVERSION_GENERATOR TO 1";
								command.ExecuteNonQuery();
		                    }
	                    	
	                    	trans.Commit();
							
						}							
	                    
	                    connection.Close();
	            }
			} catch (Exception exc) {			
				MessageBox.Show(string.Concat("Обновление БД не удалось: ",exc.Message),"Ошибка подключения",MessageBoxButtons.OK,MessageBoxIcon.Error,MessageBoxDefaultButton.Button1,0,false);
				
				return ActionResult.Failure;
			}	
			
			return ActionResult.Success;
		}

	}
}
