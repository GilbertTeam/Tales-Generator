#pragma once
#include "Enums.h"

namespace TalesGenerator { namespace Text {

	ref class TextAnalyzer;

	public ref class LemmatizeResult
	{
	private:
		TextAnalyzer^ m_TextAnalyzer;
		System::String^ m_SourceText;
		System::UInt32 m_Paradigm;
		System::UInt32 m_SourceForm;
		System::UInt32 m_FormsCount;

	internal:
		LemmatizeResult(TextAnalyzer^ textAnalyzer, System::String^ source, System::UInt32 paradigm, System::UInt32 sourceForm, System::UInt32 formsCount);

		property System::UInt32 Paradigm
		{
			System::UInt32 get();
		}

		property System::UInt32 SourceForm
		{
			System::UInt32 get();
		}

	public:
		~LemmatizeResult(void);

		property System::Int32 FormsCount
		{
			System::Int32 get();
		}

		property System::String^ SourceText
		{
			System::String^ get();
		}

		PartOfSpeech GetPartOfSpeech();

		PartOfSpeech GetPartOfSpeech(System::UInt32 formId);

		System::String^ GetTextByFormId(System::UInt32 formId);

		System::String^ GetTextByGrammem(Grammem grammem);
	};
}}

