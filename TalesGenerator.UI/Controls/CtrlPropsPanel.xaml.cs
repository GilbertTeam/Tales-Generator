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

using TalesGenerator.Net;
using TalesGenerator.UI.Classes;

namespace TalesGenerator.UI.Controls
{
	/// <summary>
	/// Контекст PropsPanel
	/// </summary>
	enum PropsPanelContext
	{
		/// <summary>
		/// Никакой
		/// </summary>
		None = 0,
		/// <summary>
		/// Вершина
		/// </summary>
		Node = 1,
		/// <summary>
		/// Отношение
		/// </summary>
		Edge = 2
	}

	/// <summary>
	/// Interaction logic for CtrlPropsPanel.xaml
	/// </summary>
	public partial class PropsPanel : UserControl
	{
		#region Fields

		NetworkEdge _edge = null;
		NetworkNode _node = null;

		PropsPanelContext _currentContex;

		#endregion

		#region Constructors

		/// <summary>
		/// Инициализирует панель
		/// </summary>
		public PropsPanel()
		{
			InitializeComponent();

			_currentContex = PropsPanelContext.None;

			SetVisibilities();

		}

		#endregion

		#region Properties

		/// <summary>
		/// Выбранная дуга
		/// </summary>
		public NetworkEdge Edge
		{
			get { return _edge; }
			set
			{
				SetEdge(value);
			}
		}

		/// <summary>
		/// Выбранная вершина
		/// </summary>
		public NetworkNode Node
		{
			get { return _node; }
			set
			{
				SetNode(value);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Установить дугу
		/// </summary>
		/// <param name="value">Дуга</param>
		private void SetEdge(NetworkEdge value)
		{
			if (value == null)
			{
				_currentContex = PropsPanelContext.None;
				SetVisibilities();
				return;
			}

			if (_currentContex == PropsPanelContext.Edge && value == _edge)
				return;

			_edge = value;
			
			//синхронизация комбобокса
			int i = 0;
			switch (_edge.Type)
			{
				case NetworkEdgeType.Agent: 
					i = 1;
					break;
				case NetworkEdgeType.Recipient:
					i = 2;
					break;
				default:
					i = 0;
					break;
			}
			LinkTypeCombo.SelectedIndex = i;

			_currentContex = PropsPanelContext.Edge;
			SetVisibilities();
		}

		/// <summary>
		/// Уставновить вершину
		/// </summary>
		/// <param name="value"></param>
		private void SetNode(NetworkNode value)
		{
			if (value == null)
			{
				_currentContex = PropsPanelContext.None;
				SetVisibilities();
				return;
			}

			if (_currentContex == PropsPanelContext.Node && value == _node)
				return;

			_node = value;
			_currentContex = PropsPanelContext.Node;
			Binding binding = new Binding();
			binding.Path = new PropertyPath("Name");
			binding.Source = _node;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
			NodeText.SetBinding(TextBox.TextProperty, binding);
			SetVisibilities();
		}

		/// <summary>
		/// Установить видимости
		/// </summary>
		private void SetVisibilities()
		{
			string tag = _currentContex == PropsPanelContext.None ? "" : 
				_currentContex == PropsPanelContext.Edge ? "Node" : "Link";
			foreach (FrameworkElement control in this.StackItems.Children)
			{
				string controlTag = control.Tag as string;
				if (controlTag == tag || (tag == "" && (controlTag == "Node" || controlTag == "Link")))
				{
					control.Visibility = System.Windows.Visibility.Collapsed;
				}
				else
				{
					control.Visibility = System.Windows.Visibility.Visible;
				}
			}
		}

		#endregion

		#region EventHandlers

		private void LinkTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_currentContex != PropsPanelContext.Edge)
				return;

			ComboBoxItem item = LinkTypeCombo.SelectedItem as ComboBoxItem;
			if (item == null)
				return;

			_edge.Type = Utils.ConvertType(item.Content as string) ;
		}

		#endregion
	}
}
