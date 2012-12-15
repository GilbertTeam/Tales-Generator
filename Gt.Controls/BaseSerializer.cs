using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Xml.Linq;
using System.Globalization;

namespace Gt.Controls
{
	public abstract class BaseSerializer
	{
#region methods

		public void WriteElement(XElement xBase, string name, string value)
		{
			if (value == null)
				return;
			XElement xEl = new XElement(name);
			xEl.Value = value;
			xBase.Add(xEl);
		}

		public void WriteElement(XElement xBase, string name, double value)
		{
			WriteElement(xBase, name, value.ToString(CultureInfo.InvariantCulture));
		}

		public void WriteElement(XElement xBase, string name, int value)
		{
			WriteElement(xBase, name, (double)value);
		}

		public void WriteElement(XElement xBase, string name, Point value)
		{
			XElement xEl = new XElement(name);
			SerializationUtils.SavePointToXElement(value, xEl);
			xBase.Add(xEl);
		}

		public void WriteElement(XElement xBase, string name, Rect value)
		{
			XElement xEl = new XElement(name);
			SerializationUtils.SaveRectToXElement(value, xEl);
			xBase.Add(xEl);
		}
		public bool ReadElement(XElement xBase, string name, out string result)
		{
			result = "";
			XElement xEl = xBase.Element(name);
			if (xEl != null)
			{
				result = xEl.Value;
				return true;
			}
			return false;
		}

		public bool ReadElement(XElement xBase, string name, out double result)
		{
			result = 0;
			XElement xEl = xBase.Element(name);
			if (xEl != null)
			{
				result = Convert.ToDouble(xEl.Value, CultureInfo.InvariantCulture);
				return true;
			}
			return false;
		}

		public bool ReadElement(XElement xBase, string name, out int result)
		{
			result = 0;
			XElement xEl = xBase.Element(name);
			if (xEl != null)
			{
				result = Convert.ToInt32(xEl.Value, CultureInfo.InvariantCulture);
				return true;
			}
			return false;
		}

		public bool ReadElement(XElement xBase, string name, out Point result)
		{
			result = new Point();
			XElement xEl = xBase.Element(name);
			if (xEl != null)
			{
				result = SerializationUtils.LoadPointFromXElement(xEl);
				return true;
			}
			return false;
		}

		public bool ReadElement(XElement xBase, string name, out Rect result)
		{
			result = new Rect();
			XElement xEl = xBase.Element(name);
			if (xEl != null)
			{
				result = SerializationUtils.LoadRectFromXElement(xEl);
				return true;
			}
			return false;
		}

#endregion
	}
}
