using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalesGenerator.Text;


namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			TextAnalyzer textAnalyzer = new TextAnalyzer(AdapterKind.RussianCp1251Adapter);

			textAnalyzer.Load(
				@"C:\Projects\Git\libturglem\Dictionaries\dict_russian.auto",
				@"C:\Projects\Git\libturglem\Dictionaries\paradigms_russian.bin",
				@"C:\Projects\Git\libturglem\Dictionaries\prediction_russian.auto");

			var results = textAnalyzer.Lemmatize("Любить", true);
			LemmatizeResult result = results.FirstOrDefault();

			PartOfSpeech partOfSpeech = result.GetPartOfSpeech(0);
			result.GetTextByFormId(0);

			

			string form = result.GetTextByGrammem((ulong)(Grammem.Plural | Grammem.PastTense | Grammem.ThirdPerson));
		}
	}
}
