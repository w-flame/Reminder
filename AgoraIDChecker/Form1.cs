using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OracleClient;
using System.Drawing;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using ListViewSorter;

namespace AgoraIDChecker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (listView1.CheckedIndices.Count > 0) SameNumbersFixButton.Enabled = true;
			else SameNumbersFixButton.Enabled = false;
		}

		private void InProgressPanelToggle (bool show)
		{
			if (show)
			{
				groupBox1.Enabled = false;
				tabControl.Enabled = false;
				Size ps = inProgressPanel.Size;
				Size fs = this.Size;
				cancelButton.Enabled = true;

				inProgressPanel.Left = (fs.Width - ps.Width) / 2;
				inProgressPanel.Top = (fs.Height - ps.Height) / 2;
				inProgressPanel.Show();
			}
			else
			{
				groupBox1.Enabled = true;
				tabControl.Enabled = true;
				inProgressPanel.Hide();
			}
		}

		private void Form1_Resize(object sender, EventArgs e)
		{
			if (inProgressPanel.Visible)
			{
				Size ps = inProgressPanel.Size;
				Size fs = this.Size;


				inProgressPanel.Left = (fs.Width - ps.Width) / 2;
				inProgressPanel.Top = (fs.Height - ps.Height) / 2;
			}
		}

		private struct SearchParam
		{
			public string db;
			public string login;
			public string pass;
			public DateTime start;
			public DateTime end;
		}

		private struct NumbersFixParam
		{
			public SearchParam sp;
			public List<NumbersResultElement> elems;
		}

		private struct NumbersResultElement
		{
			public int id;
			public int id_agora;
			public string number;
			public string replace_number;
		}

		private struct JudgesResultElement
		{
			public OracleNumber id; //ID дела
			public string number; //Номер дела
			public string sdp_judge; //Судья в сдп
			public string bsr_judge; //Судья в БСР
			public OracleNumber right_group; //Идентификатор группы БСР для судьи из СДП
		}

		private struct JudgesFixParam
		{
			public SearchParam sp;
			public List<JudgesResultElement> elems;
		}
		
		private struct ExclusionSearchParam{
			public SearchParam bsr;
			public SearchParam excl;
		}
		
		private struct ExclusionSyncParam{
			public SearchParam sp;
			public List<ExclusionElement> elems;
		}
		
		private struct ExclusionElement {
			public int id; //agora_id
			public string number; // case number
			public string bsr_reason; //причина исключения в БСР
			public string ext_reason; //причина исключения в служебной БД
			public string FIO; //кто запретил
			public bool inBSR; //дело загружено в БСР
			public DateTime date; //дата рассмотрения
		}

		private void SameNumbersSearchButton_Click(object sender, EventArgs e)
		{
			if (startDate.Value > endDate.Value)
			{
				errorProvider1.SetError(startDate, "Начальная дата не может быть больше конечной");
				return;
			}
			else
			{
				errorProvider1.SetError(startDate, "");
			}
			InProgressPanelToggle(true);

			SearchParam param = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};

			

			SearchWorker.RunWorkerAsync(param);

		}

		private void SearchWorker_DoWork(object sender, DoWorkEventArgs e)
		{

			List<NumbersResultElement> result = new List<NumbersResultElement>();
			SearchParam param = (SearchParam)e.Argument;
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();
					for (int i = 1; i <= 11; i++)
					{
						if (SearchWorker.CancellationPending) break;

						string stmt = "";
						switch (i)
						{
							case 1:
								stmt = "SELECT c.ID, c.FULL_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM U1_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.FULL_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 2:
								stmt = "SELECT c.ID, c.FULL_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM U2_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.FULL_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.VERDICT_II_DATE >= :startDate) AND (c.VERDICT_II_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 3:
								stmt = "SELECT c.ID, c.proceeding_full_number, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM u33_proceeding@IBASEDATA c INNER JOIN bsr.bsrp b on c.proceeding_full_number = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 4:
								stmt = "SELECT c.ID, c.FULL_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM G1_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.FULL_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.RESULT_DATE >= :startDate) AND (c.RESULT_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 5:
								stmt = "SELECT c.ID, c.Z_FULLNUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM GR2_DELO@IBASEDATA c INNER JOIN bsr.bsrp b on c.Z_FULLNUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.DRESALT >= :startDate) AND (c.DRESALT <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 6:
								stmt = "SELECT c.ID, c.proceeding_full_number, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM g33_proceeding@IBASEDATA c INNER JOIN bsr.bsrp b on c.proceeding_full_number = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 7:
								stmt = "SELECT c.ID, c.FULL_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM M_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.FULL_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 8:
								stmt = "SELECT c.ID, c.CASE_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM ADM1_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.CASE_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 9:
								stmt = "SELECT c.ID, c.CASE_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM ADM2_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.CASE_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 10:
								stmt = "SELECT c.ID, c.CASE_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM ADM_CASE@IBASEDATA c INNER JOIN bsr.bsrp b on c.CASE_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;
							case 11:
								stmt = "SELECT c.ID, c.FULL_NUMBER, nvl(b.ID_AGORA,0), b2.NUMDOCUM FROM a33_proceeding@IBASEDATA c INNER JOIN bsr.bsrp b on c.FULL_NUMBER = b.NUMDOCUM LEFT JOIN bsr.bsrp b2 ON c.ID = b2.ID_AGORA  WHERE (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate) AND ((c.ID <> b.ID_AGORA) OR (b.ID_AGORA IS NULL))  ORDER BY c.ID desc";
								break;

						}


						using (OracleCommand command = new OracleCommand())
						{
							command.Connection = conn;
							command.CommandType = CommandType.Text;
							command.CommandText = stmt;


							OracleParameter startDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.start,
								ParameterName = ":startDate"
							};

							command.Parameters.Add(startDate);

							OracleParameter endDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.end,
								ParameterName = ":endDate"
							};

							command.Parameters.Add(endDate);

							OracleDataReader reader = command.ExecuteReader();

							while (reader.Read())
							{
								NumbersResultElement elem = new NumbersResultElement()
								{
									id = reader.GetInt32(0),
									number = reader.GetString(1),
									id_agora = reader.GetInt32(2)
								};

								if (reader[3] != DBNull.Value)
								{
									elem.replace_number = reader.GetString(3);
								}

								result.Add(elem);
							}

							reader.Close();

						}

					}
					conn.Close();
				}


				e.Result = result;

			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = null;
			}
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			cancelButton.Enabled = false;
			if (SearchWorker.IsBusy)
			{
				SearchWorker.CancelAsync();
			}

			if (FixWorker.IsBusy)
			{
				FixWorker.CancelAsync();
			}

			if (JudgesSearchWorker.IsBusy)
			{
				JudgesSearchWorker.CancelAsync();
			}

			if (JudgesFixWorker.IsBusy)
			{
				JudgesFixWorker.CancelAsync();
			}
			
			if (ExclusionsSearchWorker.IsBusy) 
			{
				ExclusionsSearchWorker.CancelAsync();
			}
		}

		private void SearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result != null)
			{
				List<NumbersResultElement> elems = (List<NumbersResultElement>)e.Result;
				listView1.BeginUpdate();
				listView1.Items.Clear();


				foreach (NumbersResultElement item in elems)
				{
					ListViewItem lvi = new ListViewItem()
					{
						Text = item.number,
						Tag=item
					};

					if (item.replace_number == null) lvi.Checked = true;
					
					lvi.SubItems.Add(item.id.ToString());
					lvi.SubItems.Add(item.id_agora.ToString());
					lvi.SubItems.Add(item.replace_number);
					
					listView1.Items.Add(lvi);
					
				}
				listView1.EndUpdate();
			}

			InProgressPanelToggle(false);
		}

		private void SameNumbersFixButton_Click(object sender, EventArgs e)
		{
			List<NumbersResultElement> elems = new List<NumbersResultElement>();
			foreach (ListViewItem item in listView1.CheckedItems)
			{
				elems.Add((NumbersResultElement)item.Tag);
			}

			InProgressPanelToggle(true);

			SearchParam sp = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};

			NumbersFixParam param = new NumbersFixParam();
			param.sp = sp;
			param.elems = elems;

			FixWorker.RunWorkerAsync(param);
		}

		private void FixWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			SearchParam param = ((NumbersFixParam)e.Argument).sp;
			List<NumbersResultElement> elems = ((NumbersFixParam)e.Argument).elems;

			List<string> result = new List<string>();
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();

					using (OracleCommand comm = new OracleCommand())
					{
						comm.Connection = conn;
						comm.CommandType = CommandType.Text;
						comm.CommandText = "UPDATE bsr.bsrp SET ID_AGORA = :id WHERE numdocum = :num";

						OracleParameter idPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":id"
						};

						comm.Parameters.Add(idPar);

						OracleParameter numPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							DbType = DbType.String,
							ParameterName = ":num"
						};

						comm.Parameters.Add(numPar);

						foreach (NumbersResultElement item in elems)
						{
							if (FixWorker.CancellationPending) break;
							idPar.Value = item.id;
							numPar.Value = item.number;
							comm.ExecuteNonQuery();
							result.Add(item.number);
						}
					}

					conn.Close();
				}
				e.Result = result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: "+ex.Message);
				e.Result = result;
			}
		}

		private void FixWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result != null)
			{
				List<string> changed_elems = (List<string>)e.Result;
				listView1.BeginUpdate();
				foreach (string item in changed_elems)
				{
					int item_index = -1;
					foreach (ListViewItem lvi in listView1.Items)
					{
						if (item == lvi.Text)
						{
							item_index = lvi.Index;
							break;
						}
					}
					if (item_index != -1) listView1.Items.RemoveAt(item_index);
				}
				listView1.EndUpdate();
			}
			InProgressPanelToggle(false);
		}

		#region Вкладка 2
		
		private void DifferentJudgesSearchButton_Click(object sender, EventArgs e)
		{
			if (startDate.Value > endDate.Value)
			{
				errorProvider1.SetError(startDate, "Начальная дата не может быть больше конечной");
				return;
			}
			else
			{
				errorProvider1.SetError(startDate, "");
			}
			InProgressPanelToggle(true);

			SearchParam param = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};



			JudgesSearchWorker.RunWorkerAsync(param);
		}

		private void JudgesSearchWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			List<JudgesResultElement> result = new List<JudgesResultElement>();
			SearchParam param = (SearchParam)e.Argument;
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();
					for (int i = 1; i <= 11; i++)
					{
						if (JudgesSearchWorker.CancellationPending) break;

						string stmt = "";
						switch (i)
						{
							case 1:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN U1_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.JUDGE_ID = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.JUDGE_ID = u2.ID_JUDGE WHERE c.JUDGE_ID <> u.ID_JUDGE AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
							case 2:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN U2_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.SPEAKER = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.SPEAKER = u2.ID_JUDGE WHERE c.SPEAKER <> u.ID_JUDGE AND (c.VERDICT_II_DATE >= :startDate) AND (c.VERDICT_II_DATE <= :endDate)";
								break;
							case 3:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN u33_proceeding@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.presiding_judge = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.presiding_judge = u2.ID_JUDGE WHERE c.presiding_judge <> u.ID_JUDGE AND (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate)";
								break;
							case 4:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN G1_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.JUDGE_ID = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.JUDGE_ID = u2.ID_JUDGE WHERE c.JUDGE_ID <> u.ID_JUDGE AND (c.RESULT_DATE >= :startDate) AND (c.RESULT_DATE <= :endDate)";
								break;
							case 5:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN GR2_DELO@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.IDDOC = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.IDDOC = u2.ID_JUDGE WHERE c.IDDOC <> u.ID_JUDGE AND (c.DRESALT >= :startDate) AND (c.DRESALT <= :endDate)";
								break;
							case 6:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN g33_proceeding@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.presiding_judge = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.presiding_judge = u2.ID_JUDGE WHERE c.presiding_judge <> u.ID_JUDGE AND (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate)";
								break;
							case 7:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN M_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.judge_id = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.judge_id = u2.ID_JUDGE WHERE c.judge_id <> u.ID_JUDGE AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
							case 8:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN ADM1_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.judge_id = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.judge_id = u2.ID_JUDGE WHERE c.judge_id <> u.ID_JUDGE AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 9:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN ADM2_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.judge_id = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.judge_id = u2.ID_JUDGE WHERE c.judge_id <> u.ID_JUDGE AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 10:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN ADM_CASE@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.judge_id = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.judge_id = u2.ID_JUDGE WHERE c.judge_id <> u.ID_JUDGE AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 11:
								stmt = "SELECT b.ID_DOCUM, b.NUMDOCUM, gc1.USERNAME, gc2.USERNAME, u2.ID_GROUP FROM BSR.BSRP b INNER JOIN a33_proceeding@IBASEDATA c ON c.ID = b.ID_AGORA INNER JOIN bsr.USERS_GROUP u ON b.ID_GROUP = u.ID_GROUP INNER JOIN GROUPCONTENT@IBASEDATA gc1 ON c.judge_study_assist_id = gc1.GROUPCONTENTID LEFT JOIN GROUPCONTENT@IBASEDATA gc2 ON u.ID_JUDGE = gc2.GROUPCONTENTID INNER JOIN bsr.USERS_GROUP u2 ON c.judge_study_assist_id = u2.ID_JUDGE WHERE c.judge_study_assist_id <> u.ID_JUDGE AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
						}


						using (OracleCommand command = new OracleCommand())
						{
							command.Connection = conn;
							command.CommandType = CommandType.Text;
							command.CommandText = stmt;


							OracleParameter startDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.start,
								ParameterName = ":startDate"
							};

							command.Parameters.Add(startDate);

							OracleParameter endDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.end,
								ParameterName = ":endDate"
							};

							command.Parameters.Add(endDate);

							OracleDataReader reader = command.ExecuteReader();

							while (reader.Read())
							{
								JudgesResultElement elem = new JudgesResultElement()
								{
									id = reader.GetOracleNumber(0),
									number = reader.GetString(1),
									sdp_judge = reader.GetString(2),
									right_group = reader.GetOracleNumber(4)
								};

								if (reader[3] != DBNull.Value)
								{
									elem.bsr_judge = reader.GetString(3);
								}
								else
								{
									elem.bsr_judge = "";
								}

								result.Add(elem);
							}

							reader.Close();

						}

					}
					conn.Close();
				}


				e.Result = result;

			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = null;
			}
		}

		private void JudgesSearchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result != null)
			{
				List<JudgesResultElement> elems = (List<JudgesResultElement>)e.Result;
				listView2.BeginUpdate();
				listView2.Items.Clear();


				foreach (JudgesResultElement item in elems)
				{
					ListViewItem lvi = new ListViewItem()
					{
						Text = item.number,
						Tag = item,
						Checked = true
					};
					

					lvi.SubItems.Add(item.sdp_judge);
					if (item.bsr_judge == "")
						lvi.SubItems.Add("Неизвестно");
					else
						lvi.SubItems.Add(item.bsr_judge);

					listView2.Items.Add(lvi);

				}
				listView2.EndUpdate();
			}

			InProgressPanelToggle(false);
		}

		private void listView2_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			if (listView2.CheckedIndices.Count > 0) DifferentJudgesFixButton.Enabled = true;
			else DifferentJudgesFixButton.Enabled = false;
		}

		private void DifferentJudgesFixButton_Click(object sender, EventArgs e)
		{
			List<JudgesResultElement> elems = new List<JudgesResultElement>();
			foreach (ListViewItem item in listView2.CheckedItems)
			{
				elems.Add((JudgesResultElement)item.Tag);
			}

			InProgressPanelToggle(true);

			SearchParam sp = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};

			JudgesFixParam param = new JudgesFixParam();
			param.sp = sp;
			param.elems = elems;

			JudgesFixWorker.RunWorkerAsync(param);
		}

		private void JudgesFixWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			SearchParam param = ((JudgesFixParam)e.Argument).sp;
			List<JudgesResultElement> elems = ((JudgesFixParam)e.Argument).elems;

			List<string> result = new List<string>();
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();

					using (OracleCommand comm = new OracleCommand())
					{
						comm.Connection = conn;
						comm.CommandType = CommandType.Text;
						comm.CommandText = "UPDATE bsr.bsrp SET ID_GROUP = :grp WHERE ID_DOCUM = :id";

						OracleParameter idPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":id"
						};

						comm.Parameters.Add(idPar);

						OracleParameter groupPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":grp"
						};

						comm.Parameters.Add(groupPar);

						foreach (JudgesResultElement item in elems)
						{
							if (FixWorker.CancellationPending) break;
							idPar.Value = item.id;
							groupPar.Value = item.right_group;
							comm.ExecuteNonQuery();
							result.Add(item.number);
						}
					}

					conn.Close();
				}
				e.Result = result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = result;
			}
		}

		private void JudgesFixWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Result != null)
			{
				List<string> changed_elems = (List<string>)e.Result;
				listView2.BeginUpdate();
				foreach (string item in changed_elems)
				{
					int item_index = -1;
					foreach (ListViewItem lvi in listView2.Items)
					{
						if (item == lvi.Text)
						{
							item_index = lvi.Index;
							break;
						}
					}
					if (item_index != -1) listView2.Items.RemoveAt(item_index);
				}
				listView2.EndUpdate();
			}
			InProgressPanelToggle(false);
		}

		#endregion
		
		#region Вкладка 3
        
        void DifferentJudges2SearchButtonClick(object sender, EventArgs e)
        {
			    if (startDate.Value > endDate.Value)
			{
				errorProvider1.SetError(startDate, "Начальная дата не может быть больше конечной");
				return;
			}
			else
			{
				errorProvider1.SetError(startDate, "");
			}
			InProgressPanelToggle(true);

			SearchParam param = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};



			JudgesSearch2Worker.RunWorkerAsync(param);    	
        }
        
        void JudgesSearch2WorkerDoWork(object sender, DoWorkEventArgs e)
        {
        	List<JudgesResultElement> result = new List<JudgesResultElement>();
			SearchParam param = (SearchParam)e.Argument;
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();
					for (int i = 1; i <= 11; i++)
					{
						if (JudgesSearchWorker.CancellationPending) break;

						string stmt = "";
						switch (i)
						{
							case 1:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM U1_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.JUDGE_ID INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
							case 2:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM U2_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.SPEAKER INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.VERDICT_II_DATE >= :startDate) AND (c.VERDICT_II_DATE <= :endDate)";
								break;
							case 3:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM u33_proceeding@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.presiding_judge INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate)";
								break;
							case 4:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM G1_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.JUDGE_ID INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.RESULT_DATE >= :startDate) AND (c.RESULT_DATE <= :endDate)";
								break;
							case 5:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM GR2_DELO@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.IDDOC INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.DRESALT >= :startDate) AND (c.DRESALT <= :endDate)";
								break;
							case 6:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM g33_proceeding@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.presiding_judge INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.protest_verdict_date >= :startDate) AND (c.protest_verdict_date <= :endDate)";
								break;
							case 7:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM M_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.judge_id INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
							case 8:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM ADM1_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.judge_id INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 9:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM ADM2_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.judge_id INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 10:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM ADM_CASE@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.judge_id INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.DECREE_DATE >= :startDate) AND (c.DECREE_DATE <= :endDate)";
								break;
							case 11:
								stmt = "SELECT b.id_docum, b.numdocum, u.username, g1.name, g2.id FROM a33_proceeding@IBASEDATA c INNER JOIN bsr.BSRP b ON c.ID = b.id_agora INNER JOIN GROUPS_V2 g1 ON b.id_group = g1.id INNER JOIN USER_PROPERTIES_V2 up ON up.value = c.judge_study_assist_id INNER JOIN USERS_V2 u ON up.id_user = u.id INNER JOIN GROUPS_V2 g2 ON u.username = g2.name WHERE up.name = 'SdpJudgeId' AND g1.id <> g2.id AND (c.VERDICT_DATE >= :startDate) AND (c.VERDICT_DATE <= :endDate)";
								break;
						}


						using (OracleCommand command = new OracleCommand())
						{
							command.Connection = conn;
							command.CommandType = CommandType.Text;
							command.CommandText = stmt;


							OracleParameter startDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.start,
								ParameterName = ":startDate"
							};

							command.Parameters.Add(startDate);

							OracleParameter endDate = new OracleParameter()
							{
								Direction = ParameterDirection.Input,
								DbType = DbType.DateTime,
								Value = param.end,
								ParameterName = ":endDate"
							};

							command.Parameters.Add(endDate);

							OracleDataReader reader = command.ExecuteReader();

							while (reader.Read())
							{
								JudgesResultElement elem = new JudgesResultElement()
								{
									id = reader.GetOracleNumber(0),
									number = reader.GetString(1),
									sdp_judge = reader.GetString(2),
									right_group = reader.GetOracleNumber(4)
								};

								if (reader[3] != DBNull.Value)
								{
									elem.bsr_judge = reader.GetString(3);
								}
								else
								{
									elem.bsr_judge = "";
								}

								result.Add(elem);
							}

							reader.Close();

						}

					}
					conn.Close();
				}


				e.Result = result;

			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = null;
			}
        }
        
        void JudgesSearch2WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	if (e.Result != null)
			{
				List<JudgesResultElement> elems = (List<JudgesResultElement>)e.Result;
				judgeFix2List.BeginUpdate();
				judgeFix2List.Items.Clear();


				foreach (JudgesResultElement item in elems)
				{
					ListViewItem lvi = new ListViewItem()
					{
						Text = item.number,
						Tag = item,
						Checked = true
					};
					

					lvi.SubItems.Add(item.sdp_judge);
					if (item.bsr_judge == "")
						lvi.SubItems.Add("Неизвестно");
					else
						lvi.SubItems.Add(item.bsr_judge);

					judgeFix2List.Items.Add(lvi);

				}
				judgeFix2List.EndUpdate();
			}

			InProgressPanelToggle(false);
        }	
        
        void JudgeFix2ListItemChecked(object sender, ItemCheckedEventArgs e)
        {
        	if (judgeFix2List.CheckedIndices.Count > 0) DifferentJudges2FixButton.Enabled = true;
			else DifferentJudges2FixButton.Enabled = false;
        }
        
        void DifferentJudges2FixButtonClick(object sender, EventArgs e)
        {
        	List<JudgesResultElement> elems = new List<JudgesResultElement>();
			foreach (ListViewItem item in judgeFix2List.CheckedItems)
			{
				elems.Add((JudgesResultElement)item.Tag);
			}

			InProgressPanelToggle(true);

			SearchParam sp = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};

			JudgesFixParam param = new JudgesFixParam();
			param.sp = sp;
			param.elems = elems;

			JudgesFix2Worker.RunWorkerAsync(param);
        }
            
        void JudgesFix2WorkerDoWork(object sender, DoWorkEventArgs e)
        {
        	SearchParam param = ((JudgesFixParam)e.Argument).sp;
			List<JudgesResultElement> elems = ((JudgesFixParam)e.Argument).elems;

			List<string> result = new List<string>();
			OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();

					using (OracleCommand comm = new OracleCommand())
					{
						comm.Connection = conn;
						comm.CommandType = CommandType.Text;
						comm.CommandText = "UPDATE bsr.bsrp SET ID_GROUP = :grp WHERE ID_DOCUM = :id";

						OracleParameter idPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":id"
						};

						comm.Parameters.Add(idPar);

						OracleParameter groupPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":grp"
						};

						comm.Parameters.Add(groupPar);

						foreach (JudgesResultElement item in elems)
						{
							if (FixWorker.CancellationPending) break;
							idPar.Value = item.id;
							groupPar.Value = item.right_group;
							comm.ExecuteNonQuery();
							result.Add(item.number);
						}
					}

					conn.Close();
				}
				e.Result = result;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = result;
			}
        }
        
        void JudgesFix2WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	if (e.Result != null)
			{
				List<string> changed_elems = (List<string>)e.Result;
				judgeFix2List.BeginUpdate();
				foreach (string item in changed_elems)
				{
					int item_index = -1;
					foreach (ListViewItem lvi in judgeFix2List.Items)
					{
						if (item == lvi.Text)
						{
							item_index = lvi.Index;
							break;
						}
					}
					if (item_index != -1) judgeFix2List.Items.RemoveAt(item_index);
				}
				judgeFix2List.EndUpdate();
			}
			InProgressPanelToggle(false);
        }
        
        #endregion
		
        
        #region Вкладка 4
        
        void ExclusionsSearchWorkerDoWork(object sender, DoWorkEventArgs e)
        {
        	SearchParam bsr_db = ((ExclusionSearchParam)e.Argument).bsr;
        	SearchParam ext_db = ((ExclusionSearchParam)e.Argument).excl;
        	
        	OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = bsr_db.db;
			connStr.UserID = bsr_db.login;
			connStr.Password = bsr_db.pass;
			
			FbConnectionStringBuilder exclConStr = new FbConnectionStringBuilder()
            {
                Charset = "WIN1251",
                UserID = ext_db.login,
                Password = ext_db.pass,
                Database = ext_db.db,
                ServerType = FbServerType.Default
            };
			
			List<ExclusionElement> result = new List<Form1.ExclusionElement>();
			Hashtable ext_exclusions = new Hashtable();
			
			try
			{
				
				
            	
	            using (FbConnection connection = new FbConnection(exclConStr.ToString()))
	            {
	                connection.Open();
	
	                using (FbCommand command = new FbCommand())
	                {
	                    command.Connection = connection;
	                    command.CommandType = CommandType.Text;
	                    command.CommandText = "SELECT CASE_ID, CASE_NUMBER, REASON, FIO FROM PUBLISH_EXCEPTIONS";
	
	                    FbDataReader reader = command.ExecuteReader();
	                    while (reader.Read())
	                    {
	                        
	                    	ExclusionElement ee = new ExclusionElement(){
	                    		id = reader.GetInt32(0),
	                    		number = reader.GetString(1),
	                    		ext_reason = reader.GetString(2),
	                    		bsr_reason = null,
	                    		FIO = reader.GetString(3)
	                    	};
	                    	
	                    	
	                    	if (ee.FIO == "bsr") continue; //Автоисключения
	                    	
	                        ext_exclusions.Add(ee.id,ee);
	                        
	                        if (ExclusionsSearchWorker.CancellationPending) return;
	                    }
	                }
	
	                connection.Close();
	            }
	            
	            using (OracleConnection conn = new OracleConnection(connStr.ToString())) {
	            
	            	conn.Open();
	            	
	            	using (OracleCommand command = new OracleCommand()){
	            		command.Connection = conn;
	            		command.CommandType = CommandType.Text;
	            		command.CommandText = "SELECT ID_AGORA, NUMDOCUM, DATEDOCUM, PRPUB, P_ANNOT, USERMOD FROM BSR.BSRP WHERE ID_AGORA = :id AND DATEDOCUM >= :startDate AND DATEDOCUM <= :endDate";
	            		
	            		OracleParameter id = new OracleParameter() {
	            			DbType = DbType.Int32,
	            			Direction = ParameterDirection.Input,
	   						ParameterName = ":id"
	            		};
	            		
	            		OracleParameter sd = new OracleParameter() {
	            			DbType = DbType.Date,
	            			Direction = ParameterDirection.Input,
	            			ParameterName = ":startDate",
	            			Value = bsr_db.start
	            		};
	            		
	            		OracleParameter ed = new OracleParameter() {
	            			DbType = DbType.Date,
	            			Direction = ParameterDirection.Input,
	            			ParameterName = ":endDate",
	            			Value = bsr_db.end
	            		};
	            		
	            		command.Parameters.Add(id);
	            		command.Parameters.Add(sd);
	            		command.Parameters.Add(ed);
	            		
	            		ArrayList keys = new ArrayList();
	            		keys.AddRange(ext_exclusions.Keys);
	            		
	            		foreach (int elem in keys) {
	            			id.Value = ((ExclusionElement)ext_exclusions[elem]).id;
		            		OracleDataReader reader = command.ExecuteReader();
		            		
		            		while (reader.Read()) {
		            			int case_id = reader.GetInt32(0);
		            			string case_number = reader.GetString(1);
		            			DateTime dt = reader.GetDateTime(2);
		            			
		            			int status = 0;
		            			if (!reader.IsDBNull(3)) {
		            				status = reader.GetInt32(3);
		            			}
		            			
		            			string reason = "";
		            			if (!reader.IsDBNull(4) && status != 0) {
		            				reason = reader.GetString(4);
		            			}
		            				
		            			string user = reader.GetString(5);	
		            			
		            			ExclusionElement eel = ((ExclusionElement)ext_exclusions[case_id]);
		            			eel.date = dt;
		            			
		            			if (status == 2 || status == 0) {
		            				eel.bsr_reason = reason;
		            				eel.inBSR = true;
		            			}
		            			
		            			ext_exclusions[case_id] = eel;
		            				
		            			if (ExclusionsSearchWorker.CancellationPending) return;	            			
		            		}
		            		
		            		reader.Close();
		            		
	            		}
	            		
	            	}
	            	
	            	conn.Close();
	            }
				
	            ArrayList ks = new ArrayList();
	            ks.AddRange(ext_exclusions.Keys);
	            ks.Sort();
	            foreach (int element in ks) {
	            	if (((ExclusionElement)ext_exclusions[element]).inBSR) {
	            		result.Add((ExclusionElement)ext_exclusions[element]);
	            	}
	            }
	            e.Result = result;
				
			} catch (Exception ex) {
				MessageBox.Show("Ошибка: " + ex.Message);
				e.Result = result;
			}
			
			

        }
        
        List<ExclusionElement> exclusions = new List<Form1.ExclusionElement>();
        
        void FillExclusionsList() {
        	
        	ExclusionsList.BeginUpdate();
        	ExclusionsList.Items.Clear();
			ExclusionsList.Sorting = SortOrder.None;

				foreach (ExclusionElement item in exclusions)
				{
					ListViewItem lvi = new ListViewItem()
					{
						Text = item.number,
						Tag = item,
						Checked = true
					};
					
					lvi.SubItems.Add(item.date.ToShortDateString());
					
					if (item.bsr_reason == null) {
						lvi.SubItems.Add("Отсутствует");
					} else {
						
						lvi.SubItems.Add(item.bsr_reason);
					
					}
					
					if (item.ext_reason == null) {
						lvi.SubItems.Add("Отсутствует");
					} else {
						lvi.SubItems.Add(item.ext_reason);
					}
					
					lvi.SubItems.Add(item.FIO);
					

					ExclusionsList.Items.Add(lvi);

				}
				ExclusionsList.Sorting = SortOrder.Ascending;
				ExclusionsList.EndUpdate();
        	
        }
        
        
        void ExclusionsSearchWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	if (e.Result != null)
			{
        		exclusions = (List<ExclusionElement>)e.Result;
        		FillExclusionsList();
			}
			InProgressPanelToggle(false);	
       }
         

        void ExclusionSyncSearchButtonClick(object sender, EventArgs e)
        {
        	InProgressPanelToggle(true);
        	
        	ExclusionSearchParam p = new ExclusionSearchParam(){
        		bsr = new SearchParam() {
        			db = oraBase.Text,
					login = oraLogin.Text,
					pass = oraPass.Text,
					start = startDate.Value,
					end = endDate.Value
        		},
        		excl = new SearchParam() {
        			db = exclBase.Text,
        			login = exclUser.Text,
        			pass = exclPass.Text
        		}
        	};
        	
        	ExclusionsSearchWorker.RunWorkerAsync(p);
        	
        }
         
        #endregion
        
        
		
		private void Form1_Load(object sender, EventArgs e)
		{
			oraBase.Text = Properties.Settings.Default.oraBase;
			oraLogin.Text = Properties.Settings.Default.oraLogin;
			startDate.Value = Properties.Settings.Default.startDate;
			
			lvwColumnSorter = new ListViewColumnSorter();
			ExclusionsList.ListViewItemSorter = lvwColumnSorter;
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Properties.Settings.Default.oraBase = oraBase.Text;
			Properties.Settings.Default.oraLogin = oraLogin.Text;
			Properties.Settings.Default.startDate = startDate.Value;
			Properties.Settings.Default.Save();
		}
        
       
        
        void CheckAllExclButtonClick(object sender, EventArgs e)
        {
        	ExclusionsList.BeginUpdate();
        	foreach (ListViewItem element in ExclusionsList.Items) {
        		element.Checked = true;
        	}
        	ExclusionsList.EndUpdate();
        }
        
        void UncheckAllExclButtonClick(object sender, EventArgs e)
        {
        	ExclusionsList.BeginUpdate();
        	foreach (ListViewItem element in ExclusionsList.Items) {
        		element.Checked = false;
        	}
        	ExclusionsList.EndUpdate();
        }
        
        void CheckBSRExclButtonClick(object sender, EventArgs e)
        {
        	ExclusionsList.BeginUpdate();
        	foreach (ListViewItem element in ExclusionsList.Items) {
        		if (((ExclusionElement)element.Tag).bsr_reason != null) 
        		{
        			element.Checked = true;
        		} else {
        			element.Checked = false;
        		}
        	}
        	ExclusionsList.EndUpdate();
        }
        
        void CheckServiceBDExclButtonClick(object sender, EventArgs e)
        {
        	ExclusionsList.BeginUpdate();
        	foreach (ListViewItem element in ExclusionsList.Items) {
        		if (((ExclusionElement)element.Tag).ext_reason != null) 
        		{
        			element.Checked = true;
        		} else {
        			element.Checked = false;
        		}
        	}
        	ExclusionsList.EndUpdate();
        }
        
            
        void HideCheckedExclButtonCheckedChanged(object sender, EventArgs e)
        {
        	if (!HideCheckedExclButton.Checked) return;
        	ExclusionsList.BeginUpdate();
        	foreach (ListViewItem element in ExclusionsList.Items) {
        		if (!element.Checked)
        		{
        			element.Remove();
        		}
        	}
        	ExclusionsList.EndUpdate();
        }
        
        
        
        
        void ShowAllExclButtonCheckedChanged(object sender, EventArgs e)
        {
        	if (!ShowAllExclButton.Checked) return;
        	FillExclusionsList();
        }
        
        
        private ListViewColumnSorter lvwColumnSorter;
        
        void ExclusionsListColumnClick(object sender, ColumnClickEventArgs e)
        {
        	ListView myListView = (ListView)sender;

		   // Determine if clicked column is already the column that is being sorted.
		   if ( e.Column == lvwColumnSorter.SortColumn )
		   {
		   // Reverse the current sort direction for this column.
		   		if (lvwColumnSorter.Order == SortOrder.Ascending)
		    	{
		    		lvwColumnSorter.Order = SortOrder.Descending;
		    	}
		    	else
		    	{
		    		lvwColumnSorter.Order = SortOrder.Ascending;
		    	}
		  	}
		   	else
		   	{
		    // Set the column number that is to be sorted; default to ascending.
		    	lvwColumnSorter.SortColumn = e.Column;
		    	lvwColumnSorter.Order = SortOrder.Ascending;
		   }
		
		   // Perform the sort with these new sort options.
		   myListView.Sort();
		   
		}
        
        void ExclusionSyncFixButtonClick(object sender, EventArgs e)
        {
        	List<ExclusionElement> elems = new List<Form1.ExclusionElement>(ExclusionsList.CheckedItems.Count);
        	foreach (ListViewItem item in ExclusionsList.CheckedItems) {
        		elems.Add((ExclusionElement)item.Tag);
        	}
        	
        	InProgressPanelToggle(true);
        	
        	SearchParam sp = new SearchParam()
			{
				db = oraBase.Text,
				login = oraLogin.Text,
				pass = oraPass.Text,
				start = startDate.Value,
				end = endDate.Value
			};
        	
        	ExclusionSyncParam param = new ExclusionSyncParam();
        	param.sp = sp;
        	param.elems = elems;
        	
        	ExclusionsSyncWorker.RunWorkerAsync(param);
        }
        
        void ExclusionsSyncWorkerDoWork(object sender, DoWorkEventArgs e)
        {
        	SearchParam param = ((ExclusionSyncParam)e.Argument).sp;
        	List<ExclusionElement> elems = ((ExclusionSyncParam)e.Argument).elems;
        	
        	OracleConnectionStringBuilder connStr = new OracleConnectionStringBuilder();
			connStr.DataSource = param.db;
			connStr.UserID = param.login;
			connStr.Password = param.pass;

			try
			{

				using (OracleConnection conn = new OracleConnection(connStr.ConnectionString))
				{
					conn.Open();

					using (OracleCommand comm = new OracleCommand())
					{
						comm.Connection = conn;
						comm.CommandType = CommandType.Text;
						comm.CommandText = "UPDATE bsr.bsrp SET PRPUB = 2, P_ANNOT = :reason WHERE ID_AGORA = :id";

						OracleParameter idPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.Number,
							ParameterName = ":id"
						};

						comm.Parameters.Add(idPar);

						OracleParameter reasonPar = new OracleParameter()
						{
							Direction = ParameterDirection.Input,
							OracleType = OracleType.VarChar,
							ParameterName = ":reason"
						};

						comm.Parameters.Add(reasonPar);

						foreach (ExclusionElement item in elems)
						{
							if (FixWorker.CancellationPending) break;
							idPar.Value = item.id;
							reasonPar.Value = item.ext_reason;
							comm.ExecuteNonQuery();
						}
					}

					conn.Close();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
			}
        }
        
        void ExclusionsSyncWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	ExclusionSyncSearchButtonClick(null,null);
        }
        
        void ExclusionsListItemChecked(object sender, ItemCheckedEventArgs e)
        {
        	if (ExclusionsList.CheckedIndices.Count > 0) ExclusionSyncFixButton.Enabled = true;
			else ExclusionSyncFixButton.Enabled = false;
        }
	}
	
	
}
