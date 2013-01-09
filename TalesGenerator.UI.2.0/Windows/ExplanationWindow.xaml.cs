using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TalesGenerator.TaleNet;
using TalesGenerator.Text;

namespace TalesGenerator.UI.Windows
{
	/// <summary>
	/// Interaction logic for ExplanationWindow.xaml
	/// </summary>
	public partial class ExplanationWindow : Window
	{
		TextGeneratorContext _context;

		private ExplanationWindow()
		{
			InitializeComponent();

			_context = null;
		}

		public ExplanationWindow(TextGeneratorContext context)
			: this()
		{
			_context = context;

			BuildTree();
		}


		protected void BuildTree()
		{
			if (_context == null)
				return;

			int index = 1;

			foreach (var tale in _context.Tales)
			{
				BuildTale(tale, index);

				index++;
			}
		}

		private void BuildTale(TaleGenerationInfo tale, int index)
		{
			if (tale == null)
				return;

			StringBuilder headerBuilder = new StringBuilder();
			headerBuilder.Append("{0}. ");
			headerBuilder.Append(tale.Tale.Name);
			headerBuilder.Append(". ");
			headerBuilder.Append("Коэффициент релевантности: {1}");

			TreeViewItem item = new TreeViewItem();
			item.Header = String.Format(headerBuilder.ToString(), index.ToString(), tale.RelevanceLevel.ToString());

			int setIndex = 1;

			foreach (var conflictSet in tale.ConflictSets)
			{
				BuildConfictSet(conflictSet, item, setIndex);

				setIndex++;
			}

			treeExlanation.Items.Add(item);

		}

		private void BuildConfictSet(FunctionConflictSet conflictSet, TreeViewItem item, int setIndex)
		{
			if (conflictSet == null || item == null)
				return;

			StringBuilder headerBuidller = new StringBuilder();
			headerBuidller.Append("Конфликтный набор №{0}. ");
			headerBuidller.Append("Функция ");
			headerBuidller.Append(conflictSet.FunctionType.ToString());
			headerBuidller.Append(".");

			TreeViewItem setItem = new TreeViewItem();
			setItem.Header = String.Format(headerBuidller.ToString(), setIndex);

			item.Items.Add(setItem);

			conflictSet.Reset();

			while (conflictSet.ClosedEx == false)
			{
				FunctionGenerationInfo info = conflictSet.CurrentFunction;

				headerBuidller.Clear();
				headerBuidller.Append("Функция \"{0}\". ");
				headerBuidller.Append("Коэффициент релевантности: {1}.");

				TreeViewItem functionItem = new TreeViewItem();
				functionItem.Header = String.Format(headerBuidller.ToString(), info.Function.Name, info.RelevanceLevel);
				setItem.Items.Add(functionItem);

				conflictSet.NextEx();
			}

			conflictSet.Reset();
		}
	}
}
