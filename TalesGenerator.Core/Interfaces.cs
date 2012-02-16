using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TalesGenerator.Core
{
	/// <summary>
	/// Семантическая сеть
	/// </summary>
	public interface ISemanticNetwork
	{
		/// <summary>
		/// 
		/// </summary>
		bool Dirty { get; set; }
		/// <summary>
		/// Имя сети (непонятно пока, надо ли)
		/// </summary>
		string Name { get; set; }
		/// <summary>
		/// Вершины
		/// </summary>
		INodes Nodes { get; }
		/// <summary>
		/// Дуги
		/// </summary>
		IEdges Edges { get; }
		/// <summary>
		/// Отмена последнего действия (на будущее)
		/// </summary>
		void Undo();
		/// <summary>
		/// Возврат последнего действия (на будущее)
		/// </summary>
		void Redo();
		/// <summary>
		/// Загружает сеть из файла
		/// </summary>
		/// <param name="path">Путь к файлу</param>
		void Load(string path);
		/// <summary>
		/// Загружает сеть из потока
		/// </summary>
		/// <param name="reader">Поток</param>
		void Load(Stream reader);
		/// <summary>
		/// Сохраняет сеть в файл
		/// </summary>
		/// <param name="path">Путь к файлу</param>
		void Save(string path);
		/// <summary>
		/// Сохраняет сеть в поток
		/// </summary>
		/// <param name="writer">Поток</param>
		void Save(Stream writer);
	}

	/// <summary>
	/// Вершины
	/// </summary>
	public interface INodes
	{
	}

	/// <summary>
	/// Вершина
	/// </summary>
	public interface INode
	{
	}

	/// <summary>
	/// Дуги
	/// </summary>
	public interface IEdges
	{
	}

	/// <summary>
	/// Дуга
	/// </summary>
	public interface IEdge
	{
	}


}
