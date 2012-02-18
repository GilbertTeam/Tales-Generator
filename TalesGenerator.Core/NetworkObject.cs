using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Linq;

namespace TalesGenerator.Core
{
	public abstract class NetworkObject : SerializableObject, INotifyPropertyChanged
	{
		#region Fields

		private string _id;
		protected readonly Network _network;
		#endregion

		#region Properties

		public Network Parent
		{
			get { return _network; }
		}

		public string Id
		{
			get
			{
				throw new NotImplementedException();
			}

			private set
			{
				
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
			Id = xElement.Attribute("id").Value;
		}

		internal override void SaveToXml(XElement xElement)
		{
			xElement.Add(new XAttribute("id", Id));
		}
		#endregion
	}
}
