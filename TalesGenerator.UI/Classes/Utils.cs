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

		public static void SaveRectToXElement(Rect rect, XElement xEl)
		{
			xEl.Add(new XAttribute("X", rect.X.ToString()));
			xEl.Add(new XAttribute("Y", rect.Y.ToString()));
			xEl.Add(new XAttribute("Width", rect.Width.ToString()));
			xEl.Add(new XAttribute("Height", rect.Height.ToString()));
		}

		public static Rect LoadRectFromXElement(XElement xEl)
		{
			double x = Double.Parse(xEl.Attribute("X").Value);
			double y = Double.Parse(xEl.Attribute("Y").Value);
			//double width = Double.Parse(xEl.Attribute("Width").Value);
			double width = Convert.ToDouble(xEl.Attribute("Width").Value);
			//double height = Double.Parse(xEl.Attribute("Height").Value);
			double height = Convert.ToDouble(xEl.Attribute("Height").Value);
			return new Rect(x, y, width, height);
		}
	}

	[ValueConversion(typeof(String), typeof(NetworkEdgeType))]
	public class NetworkEdgeTypeStringConverter : IValueConverter
	{
	public object  Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{
		NetworkEdgeType type = (NetworkEdgeType)value;
		return Utils.ConvertType(type);
	}

	public object  ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	{

		String type = value as String;
		return Utils.ConvertType(type);
	}
}

}
