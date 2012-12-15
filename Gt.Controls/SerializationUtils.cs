using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using System.Xml.Linq;
using System.Globalization;

namespace Gt.Controls
{
	public class SerializationUtils
	{
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
			double x = Double.Parse(xEl.Attribute("X").Value, CultureInfo.InvariantCulture);
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
}
