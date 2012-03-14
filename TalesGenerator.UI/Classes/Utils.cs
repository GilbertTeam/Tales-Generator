using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TalesGenerator.Core;
using MindFusion.Diagramming.Wpf;
using System.Windows.Data;
using System.Windows;
using System.Xml.Linq;

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
					result = NetworkEdgeType.Agent;
					break;
				case "реципиент":
					result = NetworkEdgeType.Recipient;
					break;
				case "is_a":
					result = NetworkEdgeType.IsA;
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
					res = "#is_a";
					break;
				case NetworkEdgeType.Agent:
					res = "#агент";
					break;
				case NetworkEdgeType.Recipient:
					res = "#реципиент";
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

		/// <summary>
		/// Сохранение прямоугольниа в XElement
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="xEl"></param>
		public static void SaveRectToXElement(Rect rect, XElement xEl)
		{
			xEl.Add(new XAttribute("X", rect.X.ToString()));
			xEl.Add(new XAttribute("Y", rect.Y.ToString()));
			xEl.Add(new XAttribute("Width", rect.Width.ToString()));
			xEl.Add(new XAttribute("Height", rect.Height.ToString()));
		}

		/// <summary>
		/// Загрузка прямоугольник из XElement'a
		/// </summary>
		/// <param name="xEl"></param>
		/// <returns></returns>
		public static Rect LoadRectFromXElement(XElement xEl)
		{
			double x = Double.Parse(xEl.Attribute("X").Value);
			double y = Double.Parse(xEl.Attribute("Y").Value);
			double width = Convert.ToDouble(xEl.Attribute("Width").Value);
			double height = Convert.ToDouble(xEl.Attribute("Height").Value);
			return new Rect(x, y, width, height);
		}

		/// <summary>
		/// Сохранение точки в XElement
		/// </summary>
		/// <param name="point"></param>
		/// <param name="xEl"></param>
		public static void SavePointToXElement(Point point, XElement xEl)
		{
			xEl.Add(new XAttribute("X", Convert.ToString(point.X)));
			xEl.Add(new XAttribute("Y", Convert.ToString(point.Y)));
		}

		/// <summary>
		/// Загрузка точки из XElement'a
		/// </summary>
		/// <param name="xEl"></param>
		/// <returns></returns>
		public static Point LoadPointFromXElement(XElement xEl)
		{
			double x = Convert.ToDouble(xEl.Attribute("X").Value);
			double y = Convert.ToDouble(xEl.Attribute("Y").Value);
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
			return Utils.ConvertType(type);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{

			String type = value as String;
			return Utils.ConvertType(type);
		}
	}

	#endregion
}
