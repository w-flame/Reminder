
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
	/// Interaction logic for SummaryConfig.xaml
	/// </summary>
	public partial class SummaryConfig : Window
	{
		public SummaryConfig()
		{
			InitializeComponent();
		}

        public bool first_mode
        {
            set {
                    this.FirstMode.IsChecked = value;
                    this.SecondMode.IsChecked = !value;
            }
            get
            {
                return this.FirstMode.IsChecked.Value;
            }
        }
        
        public bool use_materials {
        	get {
        		return this.UseMaterialsCB.IsChecked.Value;
        	}
        }
        
        public bool show_late {
        	get {
        		return this.ShowLateCasesCB.IsChecked.Value;
        	}
        
        }
        
        public bool show_canceled {
        	get {
        		return this.ShowCanceledCivilCB.IsChecked.Value;
        	}
        	set {
        		this.ShowCanceledCivilCB.IsChecked = value;
        	}
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; 
            Close();
        }
	}
}