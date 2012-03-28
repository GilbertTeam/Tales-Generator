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
		public static readonly DependencyProperty ValueProperty;

		static StringEditWindow()
		{
			ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(StringEditWindow));
		}

		public StringEditWindow(string str)
		{
			InitializeComponent();
			Value = str;

			Binding binding = new Binding();
			binding.Source = this;
			binding.Path = new PropertyPath("Value");
			StringTextBox.SetBinding(TextBox.TextProperty, binding);

			StringTextBox.Focus();
		}

		public string Value
		{
			get { return (string)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Value = StringTextBox.Text;
			this.DialogResult = true;
			this.Close();
		}

	}
}
