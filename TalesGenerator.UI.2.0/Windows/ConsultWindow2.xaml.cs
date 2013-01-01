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

using TalesGenerator.TaleNet;
using TalesGenerator.Text;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for ConsultWindow2.xaml
	/// </summary>
	public partial class ConsultWindow2 : Window
	{
		#region fields

		TalesNetwork _network;

		private readonly TextGenerator _textGenerator;
		#endregion

		#region ctor

		public ConsultWindow2(TalesNetwork network)
		{
			InitializeComponent();

			_network = network;

			cmbTale.ItemsSource = _network.Tales;
			if (cmbTale.HasItems)
				cmbTale.SelectedIndex = 0;

			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);
			textAnalyzer.Load(
				@"Dictionaries\Russian\Dictionary.auto",
				@"Dictionaries\Russian\Paradigms.bin",
				@"Dictionaries\Russian\PredictionDictionary.auto");

			_textGenerator = new TextGenerator(textAnalyzer);
		}
		#endregion

		#region methods

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			AnswerTextBox.Text = _textGenerator.GenerateText((TaleNode)cmbTale.SelectedItem);
		}

		#endregion
	}
}
