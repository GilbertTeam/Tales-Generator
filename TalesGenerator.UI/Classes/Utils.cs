using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using TalesGenerator.Core;
using MindFusion.Diagramming.Wpf;
using System.Windows.Data;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Media;

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
			}

			return res;
		}

		public static ShapeNode FindNodeByUid(Diagram diagram, int id)
		{
			ShapeNode result = null;

			foreach (ShapeNode node in diagram.Nodes)
			{
				if (Int32.Parse(node.Uid) == id)
				{
					result = node;
					break;
				}
			}

			return result;
		}

		public static DiagramItem FindItemByUid(Diagram diagram, int id)
		{
			DiagramItem result = null;

			foreach (DiagramItem item in diagram.Items)
			{
				if (Convert.ToInt32(item.Uid) == id)
				{
					result = item;
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Сохранение прямоугольниа в XElement
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="xEl"></param>
		public static void SaveRectToXElement(Rect rect, XElement xEl)
		{
			xEl.Add(new XAttribute("X", rect.X.ToString(CultureInfo.InvariantCulture)));
			xEl.Add(new XAttribute("Y", rect.Y.ToString(CultureInfo.InvariantCulture)));
			xEl.Add(new XAttribute("Width", rect.Width.ToString(CultureInfo.InvariantCulture)));
			xEl.Add(new XAttribute("Height", rect.Height.ToString(CultureInfo.InvariantCulture)));
		}

		/// <summary>
		/// Загрузка прямоугольник из XElement'a
		/// </summary>
		/// <param name="xEl"></param>
		/// <returns></returns>
		public static Rect LoadRectFromXElement(XElement xEl)
		{
			double x = Double.Parse(xEl.Attribute("X").Value,CultureInfo.InvariantCulture);
			double y = Double.Parse(xEl.Attribute("Y").Value, CultureInfo.InvariantCulture);
			double width = Convert.ToDouble(xEl.Attribute("Width").Value, CultureInfo.InvariantCulture);
			double height = Convert.ToDouble(xEl.Attribute("Height").Value, CultureInfo.InvariantCulture);
			return new Rect(x, y, width, height);
		}

		/// <summary>
		/// Сохранение точки в XElement
		/// </summary>
		/// <param name="point"></param>
		/// <param name="xEl"></param>
		public static void SavePointToXElement(Point point, XElement xEl)
		{
			xEl.Add(new XAttribute("X", Convert.ToString(point.X, CultureInfo.InvariantCulture)));
			xEl.Add(new XAttribute("Y", Convert.ToString(point.Y, CultureInfo.InvariantCulture)));
		}

		/// <summary>
		/// Загрузка точки из XElement'a
		/// </summary>
		/// <param name="xEl"></param>
		/// <returns></returns>
		public static Point LoadPointFromXElement(XElement xEl)
		{
			double x = Convert.ToDouble(xEl.Attribute("X").Value, CultureInfo.InvariantCulture);
			double y = Convert.ToDouble(xEl.Attribute("Y").Value, CultureInfo.InvariantCulture);
			return new Point(x, y);
		}
	}

	#region ValueConverters

	[ValueConversion(typeof(String), typeof(NetworkEdgeType))]
	public class NetworkEdgeTypeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			NetworkEdgeType type = (NetworkEdgeType)value;

			DiagramLink link = parameter as DiagramLink;
			if (link != null)
			{
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
					default:
						str = "IsA";
						break;
				}
				SolidColorBrush brush = App.Current.FindResource(str + "Brush") as SolidColorBrush;
				Pen pen = App.Current.FindResource(str + "Pen") as Pen;

				link.HeadPen = pen;
				link.Stroke = brush;
				link.Brush = App.Current.FindResource("LinkFillBrush") as SolidColorBrush;
				//link.StrokeThickness = App.StrokeThickness;
				link.TextBrush = brush;
				link.FontWeight = FontWeights.Bold;
				
			}

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
