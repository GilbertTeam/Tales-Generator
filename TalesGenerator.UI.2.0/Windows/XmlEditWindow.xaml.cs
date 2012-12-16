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

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for XmlEditWindow.xaml
	/// </summary>
	public partial class XmlEditWindow : Window
	{
		public XmlEditWindow(string xml)
		{
			InitializeComponent();

			txtXml.Text = xml;
		}

		public string Result
		{
			get;
			set;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Result = txtXml.Text;
			this.DialogResult = true;
			this.Close();
		}

		private void txtXml_TextChanged(object sender, TextChangedEventArgs e)
		{
			Result = txtXml.Text;
		}
	}
}
