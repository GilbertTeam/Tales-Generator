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
	/// Interaction logic for StringEditWindow.xaml !!!НЕ ДОПИСАНО - пока не нужен :)
	/// </summary>
	public partial class StringEditWindow : Window
	{
		String Str { get; set; }

		public StringEditWindow(string str)
		{
			InitializeComponent();
			Str = str;

			Binding binding = new Binding();
			binding.Source = Str;
			StringTextBox.SetBinding(TextBox.TextProperty, binding);
		}

	}
}
