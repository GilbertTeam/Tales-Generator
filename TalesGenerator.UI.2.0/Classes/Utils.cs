using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using TalesGenerator.Net;
using System.Windows.Data;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;
using System.Globalization;

using Gt.Controls.Diagramming;

namespace TalesGenerator.UI.Classes
{
	class Utils
	{
		/// <summary>
		/// Преобразует тип дуги из строкового представления в элемент перечисления
		/// </summary>
		/// <param name="type">Строковое представление типа дуги</param>
		/// <returns>Элемент перечисления</returns>
		public static NetworkEdgeType ConvertType(string type)
		{
			NetworkEdgeType result = NetworkEdgeType.IsA;

			switch (type)
			{
				case "агент":
				case "#агент":
					result = NetworkEdgeType.Agent;
					break;
				case "реципиент":
				case "#реципиент":
					result = NetworkEdgeType.Recipient;
					break;
				case "is_a":
				case "#is_a":
					result = NetworkEdgeType.IsA;
					break;
				case "#локатив":
					result = NetworkEdgeType.Locative;
					break;
				case "#следовать":
					result = NetworkEdgeType.Follow;
					break;
				case "#цель":
					result = NetworkEdgeType.Goal;
					break;
				case "#is_instance":
					result = NetworkEdgeType.IsInstance;
					break;
			}

			return result;
		}

		/// <summary>
		/// Преобразует тип дуги из элемента перечисления в строку
		/// </summary>
		/// <param name="type">Элемент перечисления</param>
		/// <returns>Строковое представление</returns>
		public static string ConvertType(NetworkEdgeType type)
		{
			string res = "";

			switch (type)
			{
				case NetworkEdgeType.IsA:
					res = Properties.Resources.IsALabel;
					break;
				case NetworkEdgeType.Agent:
					res = Properties.Resources.AgentLabel;
					break;
				case NetworkEdgeType.Recipient:
					res = Properties.Resources.RecipientLabel;
					break;
				case NetworkEdgeType.Locative:
					res = Properties.Resources.LocativeLabel;
					break;
				case NetworkEdgeType.Follow:
					res = Properties.Resources.FollowLabel;
					break;
				case NetworkEdgeType.Goal:
					res = Properties.Resources.GoalLabel;
					break;
				case NetworkEdgeType.IsInstance:
					res = Properties.Resources.IsInstanceLabel;
					break;
			}

			return res;
		}

		public static string ConvertToResourcesType(NetworkEdgeType type)
		{
			string res = "";

			switch (type)
			{
				case NetworkEdgeType.IsA:
					res = Properties.Resources.IsAResourceLabel;
					break;
				case NetworkEdgeType.Agent:
					res = Properties.Resources.AgentResourceLabel;
					break;
				case NetworkEdgeType.Recipient:
					res = Properties.Resources.RecipientResourceLabel;
					break;
				case NetworkEdgeType.Locative:
					res = Properties.Resources.LocativeResourceLabel;
					break;
				case NetworkEdgeType.Follow:
					res = Properties.Resources.FollowResourceLabel;
					break;
				case NetworkEdgeType.Goal:
					res = Properties.Resources.GoalResourceLabel;
					break;
				case NetworkEdgeType.IsInstance:
					res = Properties.Resources.IsInstanceResourceLabel;
					break;
			}

			return res;
		}

		public static DiagramItem FindItemByUserData(Diagram diagram, int id)
		{
			DiagramItem result = null;

			foreach (var node in diagram.Nodes)
			{
				int parseResult;
				if (Int32.TryParse(node.UserData, out parseResult) == true)
				{
					if (parseResult == id)
					{
						result = node;
						break;
					}
				}
			}

			foreach (var edge in diagram.Edges)
			{
				int parseResult;
				if (Int32.TryParse(edge.UserData, out parseResult) == true)
				{
					if (parseResult == id)
					{
						result = edge;
						break;
					}
				}
			}

			return result;
		}

		public static void UpdateNodeStyle(DiagramNode node)
		{
			if (node == null)
				return;

			SolidColorBrush brush = App.Current.TryFindResource("DefaultNodeBackgroundBrush") as SolidColorBrush;
			Pen pen = App.Current.FindResource("DefaultNodeBorderPen") as Pen;

			node.BorderPen = pen;
			node.Background = brush;
			node.Label.FontWeight = FontWeights.Bold;
		}

		public static void UpdateEdgeStyleFromType(DiagramEdge edge, NetworkEdgeType type)
		{
			if (edge != null)
			{
				edge.Label.Text = Utils.ConvertType(type);

				Style style = new Style();
				Setter setter = new Setter();
				string str;
				switch (type)
				{
					case NetworkEdgeType.Agent:
						str = "Agent";
						break;
					case NetworkEdgeType.Recipient:
						str = "Recipient";
						break;
					case NetworkEdgeType.Goal:
						str = "Goal";
						break;
					case NetworkEdgeType.Locative:
						str = "Locative";
						break;
					case NetworkEdgeType.Follow:
						str = "Follow";
						break;
					case NetworkEdgeType.IsInstance:
						str = "IsInstance";
						break;
					default:
						str = "IsA";
						break;
				}
				SolidColorBrush brush = App.Current.TryFindResource(str + "Brush") as SolidColorBrush;
				Pen pen = App.Current.FindResource(str + "Pen") as Pen;

				edge.BorderPen = pen;
				edge.Background = brush;
				edge.Label.Foreground = brush;
				edge.Label.FontWeight = FontWeights.Bold;
			}
		}
	}

	#region ValueConverters

	//[ValueConversion(typeof(DataTemplate), typeof(NetworkEdgeType))]
	//public class NetworkEdgeTypeDataTemplateConverter : IValueConverter
	//{
	//    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	//    {
	//        NetworkEdgeType type = (NetworkEdgeType)value;

	//        DataTemplate result = null;

	//        string str;
	//        switch (type)
	//        {
	//            case NetworkEdgeType.Agent:
	//                str = "Agent";
	//                break;
	//            case NetworkEdgeType.Recipient:
	//                str = "Recipient";
	//                break;
	//            case NetworkEdgeType.Goal:
	//                str = "Goal";
	//                break;
	//            case NetworkEdgeType.Locative:
	//                str = "Locative";
	//                break;
	//            case NetworkEdgeType.Follow:
	//                str = "Follow";
	//                break;
	//            case NetworkEdgeType.IsInstance:
	//                str = "IsInstance";
	//                break;
	//            default:
	//                str = "IsA";
	//                break;
	//        }

	//        str += "TemplateKey";

	//        result = Application.Current.TryFindResource(str) as DataTemplate;

	//        return result;
	//    }

	//    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	//    {
	//        return null;
	//    }
	//}

	[ValueConversion(typeof(String), typeof(NetworkEdgeType))]
	public class NetworkEdgeTypeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			NetworkEdgeType type = (NetworkEdgeType)value;

			DiagramEdge edge = parameter as DiagramEdge;
			Utils.UpdateEdgeStyleFromType(edge, type);

			return Utils.ConvertType(type);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			String type = value as String;
			return Utils.ConvertType(type);
		}
	}

	[ValueConversion(typeof(String), typeof(String))]
	public class NodeNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string str = (string)value;
			string res = str.Length == 0 ? Properties.Resources.UnnamedLinkLabel : str;
			return res;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string str = (string)value;
			string res = str == Properties.Resources.UnnamedLinkLabel ? "" : str;
			return res;
		}
	}

	#endregion
}
