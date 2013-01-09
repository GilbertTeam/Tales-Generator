using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using TalesGenerator.TaleNet;
using TalesGenerator.Text;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for ConsultWindow.xaml
	/// </summary>
	public partial class ConsultWindow : Window
	{
		#region Fields

		private readonly TalesNetwork _talesNetwork;

		private readonly TextGenerator _textGenerator;
		#endregion

		#region Constructors

		public ConsultWindow()
		{
			InitializeComponent();
		}

		public ConsultWindow(TalesNetwork talesNetwork)
			: this()
		{
			Contract.Requires<ArgumentNullException>(talesNetwork != null);

			_talesNetwork = talesNetwork;

			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);
			textAnalyzer.Load(
				@"Dictionaries\Russian\Dictionary.auto",
				@"Dictionaries\Russian\Paradigms.bin",
				@"Dictionaries\Russian\PredictionDictionary.auto");

			_textGenerator = new TextGenerator(textAnalyzer);

			QuestionTextBox.Focus();
		}
		#endregion

		#region Event Handlers

		private void QuestionTextBoxOnTextChanged(object sender, TextChangedEventArgs e)
		{
			btnStart.IsEnabled = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				statusText.Text = string.Empty;

				TextGeneratorContext result = _textGenerator.GenerateText(_talesNetwork, QuestionTextBox.Text);

				if (result == null)
				{
					btnStart.IsEnabled = false;
					statusText.Text = "Процесс генерации завершен";
				}
				else
				{
					AnswerTextBox.Text = result.OutputText;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Properties.Resources.ErrorMsgCaption, MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
		}
		#endregion
	}
}
