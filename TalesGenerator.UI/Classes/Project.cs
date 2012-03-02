using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.ComponentModel;
using System.Xml;

using TalesGenerator.Core;
using MindFusion.Diagramming.Wpf;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Input;


namespace TalesGenerator.UI.Classes
{
	class Project : INotifyPropertyChanged
	{
		#region Fields

		Diagram _diagram;

		Network _network;

		string _path;

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Contructors

		public Project()
		{
			_diagram = null;
			_network = null;
			_path = "";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Диагрмма текущего проекта
		/// </summary>
		public Diagram Diagram
		{
			get { return _diagram; }
			set 
			{
				if (value == null)
					throw new ArgumentNullException("Diagram");
				if (_diagram != value)
				{
					_diagram = value;

					OnPropertyChanged("Diagram");
				}
			}

		}

		/// <summary>
		/// Модель текущего проекта
		/// </summary>
		public Network Network
		{
			get { return _network; }
			set 
			{
				if (_network != value)
				{
					_network = value;

					OnPropertyChanged("Network");
				}
			}
		}

		/// <summary>
		/// Путь текущего проекта
		/// </summary>
		public string Path
		{
			get { return _path; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("Path");
				if (_path != value)
				{
					_path = value;

					OnPropertyChanged("Path");
				}
			}
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

		/// <summary>
		/// Новый проект
		/// </summary>
		public void NewProject()
		{
			Diagram = new Diagram();
			Network = new Network();
			Path = "";
		}

		/// <summary>
		/// Сохранить проект
		/// </summary>
		public void Save()
		{
			CheckObjects();
			
			FileStream writer = new FileStream(Path, FileMode.OpenOrCreate);
			SaveToStream(writer);
			writer.Close();
		}

		/// <summary>
		/// Загрузить проект
		/// </summary>
		public void Load()
		{

			if (Path == "")
				throw new ArgumentException("Path");
			FileStream reader = new FileStream(Path, FileMode.Open);
			LoadFromStream(reader);
			reader.Close();
		}

		/// <summary>
		/// Сохранить проект в поток
		/// </summary>
		/// <param name="stream">Поток для сохранения</param>
		public void Save(Stream stream)
		{
			CheckObjects();
			SaveToStream(stream);
		}

		/// <summary>
		/// Загрузить проект из потока
		/// </summary>
		/// <param name="stream">Поток для загрузки</param>
		public void Load(Stream stream)
		{
			LoadFromStream(stream);
		}

		/// <summary>
		/// Сохранить проект в поток
		/// </summary>
		/// <param name="stream">Поток для сохранения</param>
		protected void SaveToStream(Stream stream)
		{
			//_network.Save(stream);
			stream.Close();

			//Diagram.SaveToXml(_path);
			XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
			XElement xEl = new XElement("TalesGeneratorProject");
			xDoc.AddFirst(xEl);
			//_network.SaveToXDocument(xDoc);
			_network.SaveToXDocument(xDoc);
			DiagramSerializer diagSr = new DiagramSerializer(Diagram);
			diagSr.SaveToXDocument(xDoc);
			
			xDoc.Save(_path);
			//XElement xNetwork = new XElement("Network");
			//_network.SaveToElement(xNetwork);
			//xDoc.Root.Add(xNetwork);
			//xDoc.Save(_path);
			//stream.Position = 0;
			//XDocument xDoc = XDocument.Load(stream);
			//stream.Close();
			//string presentation = Diagram.SaveToString(SaveToStringFormat.Xml, true);
			
			//MemoryStream ms = new MemoryStream();
			//BinaryFormatter formatter = new BinaryFormatter();
			//formatter.Serialize(ms, Diagram);
			//string presentation = Convert.ToBase64String(ms.ToArray());
			//Diagram.SaveToXml(_path);
			//XmlDocument doc = new XmlDocument();
			//xPresentation.Add(presentation);
			//xDoc.Root.Add(xPresentation);
			//xDoc.Save(_path);

		}

		/// <summary>
		/// Загрузить проект из потока
		/// </summary>
		/// <param name="stream">Поток для загрузки</param>
		protected void LoadFromStream(Stream stream)
		{
			stream.Close();
			XDocument xDoc = XDocument.Load(_path);
			XElement xEl = xDoc.Element("TalesGeneratorProject");
			_network = Network.LoadFromXDocument(xDoc);
			DiagramSerializer diagSr = new DiagramSerializer(_diagram);
			diagSr.LoadFromXDocument(xDoc, _network);
			//_network = Network.LoadFromXDocument(xDoc);
			//Diagram.LoadFromXml(_path);
			//Diagram.IsEnabled = true;
			
			//XElement xNetwork = xDoc.Root.Element("Network");
			//_network = Network.LoadFromElement(xNetwork);
			//XDocument xDoc = XDocument.Load(stream);
			//XElement xPresentation = xDoc.Element("Presentation");
			//Diagram.LoadFromString(xPresentation.Value);
		}

		private void CheckObjects()
		{
			if (Path == "")
				throw new ArgumentException("Path");
			if (Diagram == null)
				throw new ArgumentNullException("Diagram");
			if (Network == null)
				throw new ArgumentNullException("Network");
		}

		#endregion



	}
}
