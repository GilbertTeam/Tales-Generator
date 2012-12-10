using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Linq;
using TalesGenerator.Net.Serialization;

namespace TalesGenerator.Net
{
	public abstract class NetworkObject : SerializableObject, INotifyPropertyChanged
	{
		#region Fields

		/// <summary>
		/// Уникальный (в рамках сети) идентификатор объекта.
		/// </summary>
		private int _id;

		/// <summary>
		/// Сеть, которой принадлежит объект.
		/// </summary>
		private readonly Network _network;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает сеть, которой принадлежит данный объект.
		/// </summary>
		public Network Network
		{
			get { return _network; }
		}

		/// <summary>
		/// Возвращает уникальный идентификатор данного объекта.
		/// </summary>
		public int Id
		{
			get
			{
				return _id;
			}
		}
		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region Constructors

		protected NetworkObject(Network parent)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			_network = parent;
			_id = _network.NextId;
		}
		#endregion

		#region Methods

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		internal override void LoadFromXml(XElement xElement)
		{
			XAttribute xIdAttribute = xElement.Attribute("id");

			if (xIdAttribute == null)
			{
				throw new SerializationException();
			}
			else
			{
				if (!int.TryParse(xIdAttribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _id))
				{
					throw new SerializationException();
				}
			}
		}

		internal override void SaveToXml(XElement xElement)
		{
			xElement.Add(new XAttribute("id", _id));
		}
		#endregion
	}
}
