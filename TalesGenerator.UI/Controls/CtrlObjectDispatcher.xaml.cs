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
using System.Windows.Navigation;
using System.Windows.Shapes;

using TalesGenerator.UI.Classes;
using TalesGenerator.Core;

namespace TalesGenerator.UI.Controls
{
	/// <summary>
	/// Interaction logic for CtrlObjectDispatcher.xaml
	/// </summary>
	public partial class CtrlObjectDispatcher : UserControl
	{
		#region Contructors

		public CtrlObjectDispatcher()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		public void SetNetwork(Network network)
		{
			NetworkObjectsTree.CurrentNetwork = network;
		}

		#endregion
	}
}
