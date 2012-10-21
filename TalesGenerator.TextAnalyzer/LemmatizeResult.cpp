#include "StdAfx.h"
#include "LemmatizeResult.h"
#include "TextAnalyzer.h"
using namespace System;
using namespace System::Collections::Generic;

namespace TalesGenerator { namespace Text {

	LemmatizeResult::LemmatizeResult(TextAnalyzer^ textAnalyzer, String^ sourceText, UInt32 paradigm, UInt32 sourceForm, UInt32 formsCount)
	{
		if (nullptr == textAnalyzer)
		{
			throw gcnew ArgumentNullException("textAnalyzer");
		}
		if (String::IsNullOrEmpty(sourceText))
		{
			throw gcnew ArgumentException("sourceText");
		}

		m_TextAnalyzer = textAnalyzer;
		m_SourceText = sourceText;
		m_Paradigm = paradigm;
		m_SourceForm = sourceForm;
		m_FormsCount = formsCount;
	}

	LemmatizeResult::~LemmatizeResult(void)
	{

	}

	Int32 LemmatizeResult::FormsCount::get()
	{
		return m_FormsCount;
	}

	String^ LemmatizeResult::SourceText::get()
	{
		return m_SourceText;
	}

	UInt32 LemmatizeResult::Paradigm::get()
	{
		return m_Paradigm;
	}

	UInt32 LemmatizeResult::SourceForm::get()
	{
		return m_SourceForm;
	}

	PartOfSpeech LemmatizeResult::GetPartOfSpeech()
	{
		return m_TextAnalyzer->GetPartOfSpeech(this, 0);
	}

	PartOfSpeech LemmatizeResult::GetPartOfSpeech(UInt32 formId)
	{
		return m_TextAnalyzer->GetPartOfSpeech(this, formId);
	}

	Grammem LemmatizeResult::GetGrammem()
	{
		return m_TextAnalyzer->GetGrammem(this, 0);
	}

	Grammem LemmatizeResult::GetGrammem(UInt32 formId)
	{
		return m_TextAnalyzer->GetGrammem(this, formId);
	}

	String^ LemmatizeResult::GetTextByFormId(UInt32 formId)
	{
		return m_TextAnalyzer->GetTextByFormId(this, formId);
	}

	IEnumerable<String^>^ LemmatizeResult::GetTextByGrammem(Grammem grammem)
	{
		return m_TextAnalyzer->GetTextByGrammem(this, grammem);
	}
}}
