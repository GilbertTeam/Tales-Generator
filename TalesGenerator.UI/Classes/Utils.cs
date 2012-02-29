using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TalesGenerator.Core;
using System.Windows.Data;

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
