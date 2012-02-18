using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public abstract class NetworkObject : SerializableObject, INotifyPropertyChanged
	{
		#region Fields

		/// <summary>
		/// Сеть, которой принадлежит объект.
		/// </summary>
		protected readonly Network _network;

		/// <summary>
		/// Уникальный (в рамках сети) идентификатор объекта.
		/// </summary>
		private int _id;
		#endregion

		#region Properties

		/// <summary>
		/// Возвращает сеть, которой принадлежит данный объект.
		/// </summary>
		public Network Parent
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
			_id = int.Parse(xElement.Attribute("id").Value);
		}

		internal override void SaveToXml(XElement xElement)
		{
			xElement.Add(new XAttribute("id", _id));
		}
		#endregion
	}
}
