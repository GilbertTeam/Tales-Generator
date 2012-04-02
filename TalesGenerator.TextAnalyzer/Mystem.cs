using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TalesGenerator.TextAnalyzer
{
	public static class Mystem
	{
		#region Fields

		private const string InputFileName = "input.txt";

		private const string OutputFileName = "output.txt";
		#endregion

		#region Methods

		private static void RunMystem()
		{
			ProcessStartInfo psi = new ProcessStartInfo
			{
				FileName = @"Mystem\mystem.exe",
				Arguments = string.Format("-ni {0} {1}", InputFileName, OutputFileName),
				UseShellExecute = false,
				CreateNoWindow = true
			};

			Process mystemProcess = Process.Start(psi);
			mystemProcess.WaitForExit();
		}

		public static IEnumerable<WordInfo> Analyze(string text)
		{
			List<WordInfo> wordsInfo = new List<WordInfo>();
			Encoding encoding = Encoding.GetEncoding(1251);

			using (FileStream inputStream = File.Open(InputFileName, FileMode.Create))
			{

				byte[] buffer = encoding.GetBytes(text);

				inputStream.Write(buffer, 0, buffer.Length);
			}

			RunMystem();

			using (FileStream outputStream = File.Open(OutputFileName, FileMode.Open))
			{
				using (StreamReader outputReader = new StreamReader(outputStream, encoding))
				{
					while (!outputReader.EndOfStream)
					{
						string line = outputReader.ReadLine();

						int firstBracketIndex = line.IndexOf("{");
						int firstEqualSignIndex = line.IndexOf("=", firstBracketIndex + 1);
						string word = line.Substring(0, firstBracketIndex).ToLower();
						string normalForm = line.Substring(firstBracketIndex + 1, firstEqualSignIndex - firstBracketIndex - 1);
						bool fuzzy = normalForm.Contains("?");
						string speechPartString;

						//if (line.Contains("|"))
						//{
						//    int secondEqualSignIndex = line.IndexOf("=", firstEqualSignIndex + 1);
						//    speechPartString = line.Substring(firstEqualSignIndex + 1, secondEqualSignIndex - firstEqualSignIndex - 1);
						//}
						//else
						//{
						//    int firstCommaIndex = line.IndexOf(",", firstEqualSignIndex);
						//    speechPartString = line.Substring(firstEqualSignIndex + 1, firstCommaIndex - firstEqualSignIndex - 1);
						//}

						SpeechPart speechPart = SpeechPart.Undefined;

						normalForm = normalForm.Replace("?", string.Empty);

						//switch (speechPartString)
						//{
						//    case "S":
						//        speechPart = SpeechPart.Noun;
						//        break;
						//    case "V":
						//        speechPart = SpeechPart.Verb;
						//        break;
						//    case "ADV":
						//        speechPart = SpeechPart.Adverb;
						//        break;
						//    case "SPRO":
						//        speechPart = SpeechPart.Pronoun;
						//        break;
						//    case "PR":
						//        speechPart = SpeechPart.Preposition;
						//        break;
						//}

						WordInfo wordInfo = new WordInfo(word, normalForm, speechPart, fuzzy);
						wordsInfo.Add(wordInfo);
					}
				}
			}

			return wordsInfo;
		}
		#endregion
	}
}
