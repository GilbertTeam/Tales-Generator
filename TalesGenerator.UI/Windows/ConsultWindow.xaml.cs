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

using TalesGenerator.Core;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for ConsultWindow.xaml
	/// </summary>
	public partial class ConsultWindow : Window
	{
		Network _network;
		//Reasoner _reasoner;

		public ConsultWindow(Network network)
		{
			InitializeComponent();

			_network = network;

			//_reasoner = new Reasoner(_network);

			QuestionTextBox.Focus();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//AnswerTextBox.Text = _reasoner.Confirm(QuestionTextBox.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.ErrorMsgCaption, MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}
	}
}
