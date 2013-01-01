#include "stdafx.h"
#include "TextAnalyzer.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace TalesGenerator { namespace Text {

	TextAnalyzer::TextAnalyzer(AdapterKind adapterKind)
	{
		switch (adapterKind)
		{
			case AdapterKind::RussianUtf8Adapter:
				m_pAdapter = new RussianUtf8Adapter();
				break;

			case AdapterKind::RussianCp1251Adapter:
				m_pAdapter = new RussianCp1251Adapter();
			break;

			case AdapterKind::RussianKoi8rAdapter:
				m_pAdapter = new RussianKoi8rAdapter();
			break;
		}
	}

	TextAnalyzer::~TextAnalyzer(void)
	{
		if (m_isDisposed)
		{
			return;
		}

		this->!TextAnalyzer();
		m_isDisposed = true;
	}

	TextAnalyzer::!TextAnalyzer(void)
	{
		delete m_pAdapter;
		::turglem_close(m_tlem);
	}

	PartOfSpeech TextAnalyzer::GetPartOfSpeech(LemmatizeResult^ result, UInt32 formId)
	{
		return static_cast<PartOfSpeech>(::turglem_paradigms_get_part_of_speech(m_tlem->paradigms, result->Paradigm, formId));
	}

	Grammem TextAnalyzer::GetGrammem(LemmatizeResult^ result, UInt32 formId)
	{
		return static_cast<Grammem>(::turglem_paradigms_get_grammem(m_tlem->paradigms, result->Paradigm, formId));
	}

	String^ TextAnalyzer::GetTextByFormId(LemmatizeResult^ result, UInt32 formId)
	{
		size_t size;
		const int bufferSize = 1024;
		MAFSA_letter buffer[bufferSize];

		IntPtr pSourceText = Marshal::StringToHGlobalAnsi(result->SourceText);
		const char* pszSourceText = static_cast<const char*>(pSourceText.ToPointer());

		try
		{
			MAFSA::l_string sourceText = m_pAdapter->String2Letters(pszSourceText);

			size = ::turglem_build_form(
				m_tlem,
				sourceText.data(),
				sourceText.size(),
				buffer,
				bufferSize,
				result->Paradigm,
				result->SourceForm,
				formId
			);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceText);
		}

		String^ text = gcnew String(m_pAdapter->Letters2String(buffer, size).data());

		return text;
	}

	IEnumerable<String^>^ TextAnalyzer::GetTextByGrammem(LemmatizeResult^ result, Grammem grammem)
	{
		List<String^>^ textList = gcnew List<String^>();
		size_t formCount = ::turglem_paradigms_get_form_count(m_tlem->paradigms, result->Paradigm);
		uint64_t grammemValue = static_cast<uint64_t>(grammem);

		for (uint32_t formId = 0; formId < formCount; formId++)
		{
			uint64_t currentGrammem = ::turglem_paradigms_get_grammem(m_tlem->paradigms, result->Paradigm, formId);

#ifdef _DEBUG
			String^ text = GetTextByFormId(result, formId);
#endif

			if (grammemValue == (currentGrammem & grammemValue))
			{
				textList->Add(GetTextByFormId(result, formId));
			}
		}

		return textList;
	}

	void TextAnalyzer::Load(String^ languageDictPath, String^ paradigmsDictPath, String^ predictionDictPath)
	{
		if (String::IsNullOrEmpty(languageDictPath))
		{
			throw gcnew ArgumentException("languageDictPath");
		}
		if (String::IsNullOrEmpty(paradigmsDictPath))
		{
			throw gcnew ArgumentException("paradigmsDictPath");
		}
		if (String::IsNullOrEmpty(predictionDictPath))
		{
			throw gcnew ArgumentException("predictionDictPath");
		}

		int errorDescription = 0;
		int errorStringNumber = 0;

		IntPtr pLanguageDictPath = Marshal::StringToHGlobalAnsi(languageDictPath);
		IntPtr pParadigmsDictPath = Marshal::StringToHGlobalAnsi(paradigmsDictPath);
		IntPtr pPredictionDictPath = Marshal::StringToHGlobalAnsi(predictionDictPath);
		const char* pszLanguageDictPath = static_cast<const char*>(pLanguageDictPath.ToPointer());
		const char* pszParadigmsDictPath = static_cast<const char*>(pParadigmsDictPath.ToPointer());
		const char* pszPredictionDictPath = static_cast<const char*>(pPredictionDictPath.ToPointer());

		try
		{
			if (0 == (m_tlem = ::turglem_load(pszLanguageDictPath, pszPredictionDictPath, pszParadigmsDictPath, &errorStringNumber, &errorDescription)))
			{
				const char* pszErrorDescription = turglem_error_what_string(errorDescription);
				const char* pszErrorStringNumber = turglem_error_no_string(errorStringNumber);
				const char* pszErrorString = strerror(errno);

				throw gcnew InvalidOperationException(
					String::Format("Error loading lemmatizer: ({0}/{1}): error loading {2}: {3}: {4}.",
						errorStringNumber,
						errorDescription,
						gcnew String(pszErrorDescription),
						gcnew String(pszErrorStringNumber),
						gcnew String(pszErrorString))
				);
			}
		}
		finally
		{
			Marshal::FreeHGlobal(pLanguageDictPath);
			Marshal::FreeHGlobal(pParadigmsDictPath);
			Marshal::FreeHGlobal(pPredictionDictPath);
		}
	}

	IEnumerable<LemmatizeResult^>^ TextAnalyzer::Lemmatize(String^ input)
	{
		return Lemmatize(input, true);
	}

	IEnumerable<LemmatizeResult^>^ TextAnalyzer::Lemmatize(String^ input, bool usePrediction)
	{
		if (String::IsNullOrEmpty(input))
		{
			throw gcnew ArgumentException("input");
		}

		List<LemmatizeResult^>^ results = gcnew List<LemmatizeResult^>();
		IntPtr pInput = Marshal::StringToHGlobalAnsi(input);
		const char* pszInput = static_cast<const char*>(pInput.ToPointer());

		try
		{
			MAFSA::l_string source = m_pAdapter->String2Letters(pszInput);

			if (!source.empty())
			{
				size_t formCount;
				const int bufferSize = 1024;
				int buffer[2 * bufferSize];

				formCount = ::turglem_lemmatize(
					m_tlem,
					source.data(),
					source.size(),
					buffer,
					bufferSize,
					m_pAdapter->GetMaxLetter(),
					usePrediction
				);

				for (size_t i = 0; i < formCount; i++)
				{
					uint32_t paradigm = buffer[i * 2];
					uint32_t sourceForm = buffer[i * 2 + 1];
					size_t formsCount = ::turglem_paradigms_get_form_count(m_tlem->paradigms, paradigm);

					results->Add(gcnew LemmatizeResult(this, input, static_cast<UInt32>(paradigm), static_cast<UInt32>(sourceForm), static_cast<UInt32>(formsCount)));
				}
			}
		}
		finally
		{
			Marshal::FreeHGlobal(pInput);
		}

		return results;
	}
}}