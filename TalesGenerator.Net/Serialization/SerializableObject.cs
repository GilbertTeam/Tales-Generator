using System.Xml.Linq;

namespace TalesGenerator.Net.Serialization
{
	public abstract class SerializableObject
	{
		#region Fields

		internal const string Namespace = "http://www.gilbertteam.com";

		public static readonly XNamespace XNamespace = Namespace;
		#endregion

		#region Methods

		/// <summary>
		/// Возвращает XML представление объекта.
		/// </summary>
		/// <returns></returns>
		public abstract XElement GetXml();

		/// <summary>
		/// Сохраняет XML представление объекта.
		/// </summary>
		/// <param name="xElement">Элемент, в котором должно быть сохранено XML представление объекта.</param>
		public abstract void SaveToXml(XElement xElement);

		/// <summary>
		/// Загружает объект на основе XML представления.
		/// </summary>
		/// <param name="xElement"></param>
		public abstract void LoadFromXml(XElement xElement);
		#endregion
	}
}
