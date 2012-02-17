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

using Microsoft.Windows.Controls.Ribbon;
using TalesGenerator.Core;

namespace TalesGenerator.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		ISemanticNetwork _network;

		public MainWindow()
		{
			InitializeComponent();

			_network = null;
		}

		#region CommandHandlers

		private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void New_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void Save_AsExecuted(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void CloseProject_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = _network != null;
		}

		private void CloseProject_Executed(object sender, ExecutedRoutedEventArgs e)
		{

		}

		private void Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			this.Close();
		}

		#endregion

	}
}
