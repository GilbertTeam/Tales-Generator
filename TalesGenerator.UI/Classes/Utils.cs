using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TalesGenerator.Core;

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
}
