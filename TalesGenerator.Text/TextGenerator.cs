using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using MoreLinq;
using TalesGenerator.Net;
using TalesGenerator.Net.Collections;
using TalesGenerator.TaleNet;

namespace TalesGenerator.Text
{
	public class TextGenerator
	{
		#region Fields

		private readonly TextAnalyzer _textAnalyzer;

		private readonly TextParser _textParser;

		private readonly TemplateParser _templateParser;

		private TextGenerationContext _currentContext;
		#endregion

		#region Constructors

		public TextGenerator(TextAnalyzer textAnalyzer)
		{
			if (textAnalyzer == null)
			{
				throw new ArgumentNullException("textAnalyzer");
			}

			_textAnalyzer = textAnalyzer;
			_textParser = new TextParser(_textAnalyzer);
			_templateParser = new TemplateParser(_textAnalyzer);
		}
		#endregion

		#region Methods

		private IEnumerable<IEnumerable<SentenceToken>> Shingle(IEnumerable<SentenceToken> tokens, int maxCount)
		{
			List<IEnumerable<SentenceToken>> shingles = new List<IEnumerable<SentenceToken>>();

			foreach (SentenceToken token in tokens)
			{
				shingles.Add(new[] { token });
			}

			return shingles;
		}

		//private FunctionConflictSet GetConflictSet(TaleGenerationInfo taleInfo, FunctionType functionType)
		//{
		//	FunctionConflictSet conflictSet = taleInfo.ConflictSets.FirstOrDefault(set => set.FunctionType == functionType);

		//	if (conflictSet != null &&
		//		taleInfo.CanToogleConflictSets)
		//	{
		//		taleInfo.CanToogleConflictSets = false;

		//		foreach (FunctionConflictSet otherConflictSet in taleInfo.ConflictSets)
		//		{
		//			if (!otherConflictSet.Closed)
		//			{
		//				otherConflictSet.Next();

		//				break;
		//			}

		//			otherConflictSet.Reset();
		//		}
		//	}

		//	return conflictSet;
		//}

		private void ResolveSentence(
			IEnumerable<SentenceToken> sentenceTokens,
			IEnumerable<NetworkNode> contextNodes,
			ICollection<NetworkNode> resultCollection)
		{
			Contract.Assume(_currentContext != null);

			foreach (NetworkNode contextNode in contextNodes)
			{
				var contextTokens = _textParser.Parse(contextNode.Name).First();
				int shingleLength = contextTokens.Count();

				foreach (var shingle in Shingle(sentenceTokens, shingleLength))
				{
					if (shingle.SequenceEqual(contextTokens, new SentenceTokenDefaultEqualityComparer()))
					{
						if (!resultCollection.Contains(contextNode))
						{
							resultCollection.Add(contextNode);
						}
					}
				}
			}
		}

		private bool AreHaveCommonAncestor(NetworkNode firstNode, NetworkNode secondNode, params NetworkNode[] stopNodes)
		{
			if (secondNode.IsInherit(firstNode, false))
			{
				return true;
			}

			NetworkNode baseNode = firstNode.BaseNode;

			while (baseNode != null &&
				Array.IndexOf(stopNodes, baseNode) == -1)
			{
				if (secondNode.IsInherit(baseNode, false))
				{
					return true;
				}

				baseNode = baseNode.BaseNode;
			}

			return false;
		}

		private ITemplateParserContext ReplaceFunctionContext(FunctionNode function)
		{
			Contract.Assume(_currentContext != null);

			TemplateParserDictionaryContext dictionaryContext = new TemplateParserDictionaryContext();
			// TODO: Необходимо добавить учет связи Is-Instance.
			NetworkNode[] stopNodes =
			{
				function.Network.Nodes.GetNode("Персонаж"),
				function.Network.Nodes.GetNode("Положительный персонаж"),
				function.Network.Nodes.GetNode("Отрицательный персонаж"),
				function.Network.Nodes.GetNode("Место"),
				function.Network.Nodes.GetNode("Действие")
			};
			Action<IEnumerable<NetworkNode>, IEnumerable<NetworkNode>, NetworkEdgeType> fillDictionaryAction =
				(functionNodes, contextNodes, contextType) =>
				{
					foreach (NetworkNode functionNode in functionNodes)
					{
						NetworkNode contextNode = contextNodes.FirstOrDefault(
							node => AreHaveCommonAncestor(functionNode, node, stopNodes));

						if (contextNode != null &&
							!functionNodes.Contains(contextNode))
						{
							dictionaryContext.Add(contextType, contextNode);
						}
						else
						{
							dictionaryContext.Add(contextType, functionNode);
						}
					}
				};

			// TODO: Пока не уверен, должны ли унаследованные контекстные вершины
			//       также появляться в списке.

			fillDictionaryAction(
				function.Agents,
				_currentContext.ResolvedPersons,
				NetworkEdgeType.Agent);

			fillDictionaryAction(
				function.Recipients,
				_currentContext.ResolvedPersons,
				NetworkEdgeType.Recipient);

			fillDictionaryAction(
				function.Locatives,
				_currentContext.ResolvedLocatives,
				NetworkEdgeType.Locative);

			fillDictionaryAction(
				function.Actions,
				_currentContext.ResolvedActions,
				NetworkEdgeType.Action);

			//fillDictionaryAction(function.Recipients, _currentContext.Persons, NetworkEdgeType.Recipient);
			//fillDictionaryAction(function.Locatives, _currentContext.Locatives, NetworkEdgeType.Locative);
			//fillDictionaryAction(function.Actions, _currentContext.Actions, NetworkEdgeType.Action);

			return dictionaryContext;
		}

		private string GenerateText(FunctionNode function)
		{
			Contract.Assume(_currentContext != null);

			// 1. Сначала выполняется операция замещения контекста.

			ITemplateParserContext parserContext = ReplaceFunctionContext(function);

			// 2. Запомним контекстные вершины текущей функции.
			//    В последующем они будут использоваться для определения
			//    соблюдения принципа монотонности.

			_currentContext.CurrentContextNodes.AddRange(parserContext.SelectMany(pair => pair.Value));

			// 3. Затем текст функции генерируется с учетом измененного контекста.

			if (parserContext.Count == 0)
			{
				// TODO: Контекст генерации должен заполняться в любом случае.
				throw new InvalidOperationException();
				//return _templateParser.Parse(function).Text;
			}
			else
			{
				return _templateParser.Parse(function, parserContext).Text;
			}
		}

		private void ResolveText(TalesNetwork talesNetwork, string text)
		{
			_currentContext = new TextGenerationContext(talesNetwork, text);

			// 1. Сначала входной текст разбиваем на предложения,
			//    а каждое предложение в свою очередь на лексемы.

			IEnumerable<IEnumerable<SentenceToken>> textTokens = _textParser.Parse(text);

			// 2. Затем среди лексем входного текста выполняется поиск отдельных
			//    концептов сети (персонажей, локативов и т.д.).

			foreach (var sentenceTokens in textTokens)
			{
				ResolveSentence(sentenceTokens, talesNetwork.Persons, _currentContext.ResolvedPersons);
				ResolveSentence(sentenceTokens, talesNetwork.Locatives, _currentContext.ResolvedLocatives);
				ResolveSentence(sentenceTokens, talesNetwork.Actions, _currentContext.ResolvedActions);
			}

			// 3. Затем выполняется поиск функций, которые содержат привязавшиеся концепты.
			//    Одновременно для каждой функции рассчитывается числовой коэффициент,
			//    отображающий ее уровень релевантности контексту, содержащемуся во входном тексте.

			foreach (TaleNode tale in talesNetwork.Tales)
			{
				// TODO: Функции базовой сказки не учитываются при формировании 
				//       набора привязавшихся функций и списка генерации.

				if (!(tale.BaseNode is TaleNode))
				{
					continue;
				}

				foreach (FunctionNode function in tale.Functions)
				{
					double resolvedFunctionNodes =
						function.Agents.Count(agent => _currentContext.ResolvedPersons.Contains(agent)) +
						function.Recipients.Count(recipient => _currentContext.ResolvedPersons.Contains(recipient)) +
						function.Locatives.Count(locative => _currentContext.ResolvedLocatives.Contains(locative)) +
						function.Actions.Count(action => _currentContext.ResolvedActions.Contains(action));

					if (resolvedFunctionNodes != 0)
					{
						double generalFunctionNodes =
							function.Agents.Count() +
							function.Recipients.Count() +
							function.Locatives.Count() +
							function.Actions.Count();
						double functionRelevanceLevel = resolvedFunctionNodes / generalFunctionNodes;

						_currentContext.ResolvedFunctions.Add(new FunctionGenerationInfo(function, functionRelevanceLevel));
					}
				}
			}

			// 4. Затем составляется отсортированный
			//    в соответсвии уровнем релевантности список сказок.

			Dictionary<TaleNode, double> taleRelevanceLevels = new Dictionary<TaleNode, double>();

			foreach (FunctionGenerationInfo functionResolveContext in _currentContext.ResolvedFunctions)
			{
				TaleNode currentTale = functionResolveContext.Function.Tale;
				TaleNode baseTale = currentTale.BaseNode as TaleNode;

				// TODO: Базовые сказки пока не попадают в список генерации.
				if (baseTale == null)
				{
					continue;
				}

				double taleRelevanceLevel;

				if (!taleRelevanceLevels.TryGetValue(currentTale, out taleRelevanceLevel))
				{
					taleRelevanceLevel = 0.0;
				}

				taleRelevanceLevel += functionResolveContext.RelevanceLevel / baseTale.Functions.Count();

				taleRelevanceLevels[currentTale] = taleRelevanceLevel;
			}

			List<KeyValuePair<TaleNode, double>> list = taleRelevanceLevels.ToList();

			// Список сортируется в порядке уменьшения уровня релевантности.

			list.Sort((firstPair, secondPair) => secondPair.Value.CompareTo(firstPair.Value));

			foreach (var pair in list)
			{
				_currentContext.Tales.Add(new TaleGenerationInfo(pair.Key, pair.Value));
			}
		}

		private void GetChildContextNode(
			IDictionary<FunctionNode, TemplateParserResult> parserResults,
			TaleNode currentTaleNode,
			FunctionNode baseFunctionNode,
			NetworkEdgeType edgeType,
			TemplateParserDictionaryContext newParserContext)
		{
			TemplateParserResult baseFunctionNodeParserResult = parserResults[baseFunctionNode];
			var currentTaleContextNodes = new List<NetworkNode>();
			var baseTaleContextNodes = baseFunctionNode.OutgoingEdges.GetEdges(edgeType).Select(edge => edge.EndNode);
			bool found = false;

			foreach (FunctionNode functionNode in currentTaleNode.Functions)
			{
				currentTaleContextNodes.AddRange(functionNode.OutgoingEdges.GetEdges(edgeType).Select(edge => edge.EndNode));
			}

			foreach (NetworkNode baseTaleContextNode in baseTaleContextNodes)
			{
				if (currentTaleContextNodes.Any(node => node.IsInherit(baseTaleContextNode, false)))
				{
					NetworkNode currentNode = null;

					while (true)
					{
						NetworkEdge isAEdge = baseTaleContextNode.OutgoingEdges.GetEdge(NetworkEdgeType.IsA);

						if (isAEdge != null)
						{
							currentNode = isAEdge.StartNode;

							NetworkNode childNode = currentTaleContextNodes.FirstOrDefault(node => node == currentNode || node.BaseNode == currentNode);

							if (childNode != null)
							{
								found = true;
								newParserContext.Add(edgeType, childNode);
								break;
							}
						}

						if (isAEdge == null &&
							currentNode != null)
						{
							found = true;
							newParserContext.Add(edgeType, currentNode);
							break;
						}
					}
				}
			}

			if (!found)
			{
				newParserContext.Add(edgeType, baseTaleContextNodes);
			}
		}

		private string GenerateText(IEnumerable<FunctionNode> functionNodes)
		{
			StringBuilder textBuilder = new StringBuilder();

			foreach (FunctionNode functionNode in functionNodes)
			{
				textBuilder.AppendFormat("{0}{1}", _templateParser.Parse(functionNode).Text, Environment.NewLine);
			}

			return textBuilder.ToString();
		}

		public string GenerateText(TaleNode taleNode)
		{
			if (taleNode == null)
			{
				throw new ArgumentNullException("taleNode");
			}

			return GenerateText(taleNode.Functions);
		}

		public string GenerateText(TalesNetwork talesNetwork, string text)
		{
			Contract.Requires<ArgumentNullException>(talesNetwork != null);
			Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(text));

			StringBuilder textBuilder = new StringBuilder();

			// 1. Сначала проверим, не изменился ли контекст.

			if (_currentContext == null ||
				_currentContext.Network != talesNetwork ||
				string.Compare(_currentContext.Text, text) != 0)
			{
				// Если контекст изменился или его не было, необходимо выполнить этап анализа.

				ResolveText(talesNetwork, text);
			}

			// 2. Найдем сказку, для которой еще остались неиспользованные сочетания функций в конфликтных наборах.

			TaleGenerationInfo currentTaleInfo = _currentContext.Tales.GetCurrentTale();

			if (currentTaleInfo == null)
			{
				return null;
			}
			//TaleGenerationInfo currentTaleInfo = _currentContext.GenerationList.FirstOrDefault(
			//	taleInfo => taleInfo.ConflictSets.Any(
			//		conflictSet => !conflictSet.Closed));

			// Если такой сказки нет, то необходимо сбросить все индексы во всех конфликтных наборах
			// и начать процесс генерации c первой сказки в списке генерации.

			//if (currentTaleInfo == null)
			//{
			//	_currentContext.GenerationList.ForEach(taleInfo =>
			//		taleInfo.ConflictSets.ForEach(conflictSet =>
			//			conflictSet.Reset()));

			//	currentTaleInfo = _currentContext.GenerationList.First();
			//}

			//Contract.Assume(currentTaleInfo != null);

			// TODO: Необходимо вручную задавать режим переключения конфликтных наборов.
			//currentTaleInfo.CanToogleConflictSets = true;

			// Запомним текущую сказку, сценарий и выберем первую функцию.

			TaleNode currentTale = currentTaleInfo.Tale;
			IEnumerable<FunctionType> scenario = currentTale.Scenario;
			FunctionNode firstFunction = currentTale.Functions.FirstOrDefault();

			// Перед началом процесса генерации необходимо очистить список текущих контекстных вершин,
			// на основе которых будет определяться выполнение принципа монотонности.

			_currentContext.CurrentContextNodes.Clear();

			if (firstFunction == null)
			{
				// TODO: Должна быть выполнена генерация для вершины базовой (сценарной) сказки с выполнением замещения контекста.
				throw new NotImplementedException();
			}

			// Отдельно выполним процесс генерации для первой функции.

			textBuilder.AppendLine(GenerateText(firstFunction));

			// Процесс генерации начинается со второй по счету функции.

			foreach (FunctionType functionType in scenario.Skip(1))
			{
				// 1. Определим наличие конфликтного набора для текущего типа функции.

				// TODO: Возникнут проблемы, если в сказке несколько функций с одним типом.
				FunctionConflictSet conflictSet = currentTaleInfo.ConflictSets.FirstOrDefault(set => set.FunctionType == functionType);

				if (conflictSet != null)
				{
					// Если конфликтный набор нашелся, то
					// процесс генерации выполняется для текущей функции этого набора.

					textBuilder.AppendLine(GenerateText(conflictSet.CurrentFunction.Function));
					continue;
				}

				// 2. В противном случае осуществляется попытка поиска функции с нужным типом во входном наборе.

				var resolvedFunctions = _currentContext.ResolvedFunctions
					.Where(functionContext => functionContext.Function.FunctionType == functionType)
					.Select(functionContext => functionContext.Function);

				if (resolvedFunctions.Any())
				{
					if (resolvedFunctions.Count() == 1)
					{
						textBuilder.AppendLine(GenerateText(resolvedFunctions.First()));
						continue;
					}
					else
					{
						// Если функций с необходимым типом во входном наборе больше 1, то
						// необходимо сформировать конфликтный набор.

						ICollection<NetworkNode> currentContextNodes = _currentContext.CurrentContextNodes;
						Func<FunctionNode, int> getContextNodesCount =
							function =>
							{
								return
									function.Agents.Count(agent => currentContextNodes.Contains(agent)) +
									function.Recipients.Count(recipient => currentContextNodes.Contains(recipient)) +
									function.Locatives.Count(locative => currentContextNodes.Contains(locative));
							};
						List<FunctionGenerationInfo> sortedFunctionsList = resolvedFunctions
							.Select(function => new FunctionGenerationInfo(function, getContextNodesCount(function)))
							.ToList();

						// Функции в конфликтном наборе упорядочиваются в соответсвии с принципом монотонности.

						sortedFunctionsList.Sort(
							(firstFunc, secondFunc) => secondFunc.RelevanceLevel.CompareTo(firstFunc.RelevanceLevel));

						currentTaleInfo.ConflictSets.Add(new FunctionConflictSet(sortedFunctionsList));

						// Процесс генерации выполняется для функции, которая больше всего
						// удовлетворяет принципу монотонности, т.е. для первой функции из конфликтного набора.

						textBuilder.AppendLine(GenerateText(sortedFunctionsList.First().Function));
						continue;
					}
				}

				// 3. Если во входном наборе не оказалось функций с нужным типом, выполняем процесс генерации для функции текущей сказки.
				textBuilder.AppendLine(GenerateText(currentTale.Functions.First(function => function.FunctionType == functionType)));
			}

			return textBuilder.ToString();
		}
		#endregion
	}
}
