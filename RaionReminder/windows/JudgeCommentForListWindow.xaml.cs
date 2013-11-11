
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace RaionReminder
{
	/// <summary>
	/// Interaction logic for JudgeCommentForListWindow.xaml
	/// </summary>
	public partial class JudgeCommentForListWindow : Window
	{
		public JudgeCommentForListWindow()
		{
			InitializeComponent();
		}
		
		
		public string comment="";
		public bool hideReasonWindow= false;
		
		void Exclude_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			comment = ReasonText.Text;
			hideReasonWindow = DontShow.IsChecked.Value;
			this.Close();
		}
	}
}