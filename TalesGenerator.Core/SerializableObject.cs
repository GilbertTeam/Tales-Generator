using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public abstract class SerializableObject
	{
		#region Fields

		internal const string Namespace = "http://www.giblertteam.com";

		internal static readonly XNamespace XNamespace = Namespace;
		#endregion

		#region Methods

		/// <summary>
		/// Возвращает XML представление объекта.
		/// </summary>
		/// <returns></returns>
		internal abstract XElement GetXml();

		/// <summary>
		/// Сохраняет XML представление объекта.
		/// </summary>
		/// <param name="xElement"></param>
		internal abstract void SaveToXml(XElement xElement);

		/// <summary>
		/// Загружает объект.
		/// </summary>
		/// <param name="xElement"></param>
		internal abstract void LoadFromXml(XElement xElement);
		#endregion
	}
}
