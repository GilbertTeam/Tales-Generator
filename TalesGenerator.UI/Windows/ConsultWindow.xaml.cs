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
	/// Interaction logic for ConsultWindow.xaml
	/// </summary>
	public partial class ConsultWindow : Window
	{
		public ConsultWindow()
		{
			InitializeComponent();

			QuestionTextBox.Focus();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
