#pragma once
#include "Enums.h"
#include "LemmatizeResult.h"
#include "Adapter.h"

namespace TalesGenerator { namespace Text {

	public ref class TextAnalyzer
	{
	private:
		bool m_isDisposed;
		Adapter* m_pAdapter;
		turglem m_tlem;

	internal:
		PartOfSpeech GetPartOfSpeech(LemmatizeResult^ result, System::UInt32 formId);

		Grammem GetGrammem(LemmatizeResult^ result, System::UInt32 formId);

		System::String^ GetTextByFormId(LemmatizeResult^ result, System::UInt32 formId);

		System::Collections::Generic::IEnumerable<System::String^>^ GetTextByGrammem(LemmatizeResult^ result, Grammem grammem);

	public:
		TextAnalyzer(AdapterKind adapter);
		~TextAnalyzer(void);
		!TextAnalyzer(void);

		void Load(System::String^ languageDictPath, System::String^ paradigmsDictPath, System::String^ predictionDictPath);

		System::Collections::Generic::IEnumerable<LemmatizeResult^>^ Lemmatize(System::String^ input);

		System::Collections::Generic::IEnumerable<LemmatizeResult^>^ Lemmatize(System::String^ input, bool usePrediction);
	};
}}

