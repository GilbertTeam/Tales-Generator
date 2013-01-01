#pragma once

namespace TalesGenerator { namespace Text {

	public enum class AdapterKind
	{
		RussianUtf8Adapter,
		RussianCp1251Adapter,
		RussianKoi8rAdapter
	};

	public enum class PartOfSpeech : System::Byte
	{
		/// <summary>
		/// ���������������.
		/// </summary>
		NOUN				= 0,

		/// <summary>
		/// ��������������.
		/// </summary>
		ADJ_FULL			= 1,

		/// <summary>
		/// ������.
		/// </summary>
		VERB				= 2,

		/// <summary>
		/// �����������.
		/// </summary>
		PRONOUN				= 3,

		/// <summary>
		/// ������������ ��������������.
		/// </summary>
		PRONOUN_P			= 4,

		/// <summary>
		/// �����������-����������.
		/// </summary>
		PRONOUN_PREDK		= 5,

		/// <summary>
		/// �������������� ������������.
		/// </summary>
		NUMERAL				= 6,

		/// <summary>
		/// ���������� ������������.
		/// </summary>
		NUMERAL_P			= 7,

		/// <summary>
		/// �������.
		/// </summary>
		ADV					= 8,

		/// <summary>
		/// ����������.
		/// </summary>
		PREDK				= 9,

		/// <summary>
		/// �������.
		/// </summary>
		PREP				= 10,

		/// <summary>
		/// ��������.
		/// </summary>
		POSL				= 11,

		/// <summary>
		/// ����.
		/// </summary>
		CONJ				= 12,

		/// <summary>
		/// ����������.
		/// </summary>
		INTERJ				= 13,

		/// <summary>
		/// ������� �����.
		/// </summary>
		INP					= 14,

		/// <summary>
		/// �����.
		/// </summary>
		PHRASE				= 15,

		/// <summary>
		/// �������.
		/// </summary>
		PARTICLE			= 16,

		/// <summary>
		/// ������� ��������������.
		/// </summary>
		ADJ_SHORT			= 17,

		/// <summary>
		/// ���������.
		/// </summary>
		PARTICIPLE			= 18,

		/// <summary>
		/// ������������.
		/// </summary>
		ADVERB_PARTICIPLE	= 19,

		/// <summary>
		/// ������� ���������.
		/// </summary>
		PARTICIPLE_SHORT	= 20,

		/// <summary>
		/// ���������.
		/// </summary>
		INFINITIVE			= 21,

		UNKNOWN				= 22,
	};

	[System::FlagsAttribute]
	public enum class Grammem : System::UInt64
	{
		None				= 0x0,

		/// <summary>
		/// ������������� �����.
		/// </summary>
		Plural				= 0x1,

		/// <summary>
		/// ������������ �����.
		/// </summary>
		Singular			= 0x2,

		/// <summary>
		/// ������������ �����.
		/// </summary>
		Nominativ			= 0x4,

		/// <summary>
		/// ����������� �����.
		/// </summary>
		Genitiv				= 0x8,

		/// <summary>
		/// ��������� �����.
		/// </summary>
		Dativ				= 0x10,

		/// <summary>
		/// ����������� �����.
		/// </summary>
		Accusativ			= 0x20,

		/// <summary>
		/// ������������ �����.
		/// </summary>
		Instrumentalis		= 0x40,

		/// <summary>
		/// ���������� �����.
		/// </summary>
		Locativ				= 0x80,

		/// <summary>
		/// ���������� �����.
		/// </summary>
		Vocativ				= 0x100,

		/// <summary>
		/// ������� ���.
		/// </summary>
		Masculinum			= 0x200,

		/// <summary>
		/// ������� ���.
		/// </summary>
		Feminum				= 0x400,

		/// <summary>
		/// ������� ���.
		/// </summary>
		Neutrum				= 0x800,

		/// <summary>
		/// �������-������� ���.
		/// </summary>
		MascFem				= 0x1000,

		/// <summary>
		/// ������� �����.
		/// </summary>
		ShortForm			= 0x2000,

		/// <summary>
		/// ��������� �����.
		/// </summary>
		PresentTense		= 0x4000,

		/// <summary>
		/// ������� �����.
		/// </summary>
		FutureTense			= 0x8000,

		/// <summary>
		/// ��������� �����.
		/// </summary>
		PastTense			= 0x10000,

		/// <summary>
		/// ������ ����.
		/// </summary>
		FirstPerson			= 0x20000,

		/// <summary>
		/// ������ ����.
		/// </summary>
		SecondPerson		= 0x40000,

		/// <summary>
		/// ������ ����.
		/// </summary>
		ThirdPerson			= 0x80000,

		/// <summary>
		/// ������������� ����������.
		/// </summary>
		Imperative			= 0x100000,

		/// <summary>
		/// ������������.
		/// </summary>
		Animative			= 0x200000,

		/// <summary>
		/// ��������������.
		/// </summary>
		NonAnimative		= 0x400000,

		/// <summary>
		/// �������������.
		/// </summary>
		Comparative			= 0x800000,

		/// <summary>
		/// ����������� ���.
		/// </summary>
		Perfective			= 0x1000000,

		/// <summary>
		/// ������������� ���.
		/// </summary>
		NonPerfective		= 0x2000000,

		/// <summary>
		/// ������������ ������.
		/// </summary>
		NonTransitive		= 0x4000000,

		/// <summary>
		/// ���������� ������.
		/// </summary>
		Transitive			= 0x8000000,

		/// <summary>
		/// �������������� �����.
		/// </summary>
		ActiveVoice			= 0x10000000,

		/// <summary>
		/// ������������� �����.
		/// </summary>
		PassiveVoice		= 0x20000000,

		/// <summary>
		/// ������������.
		/// </summary>
		Indeclinable		= 0x40000000,

		/// <summary>
		/// ���������� (������������ �� ������ ����).
		/// </summary>
		Initialism			= 0x80000000,

		/// <summary>
		/// ��������.
		/// </summary>
		Patronymic			= 0x100000000,

		/// <summary>
		/// �������.
		/// </summary>
		Toponym				= 0x200000000,

		/// <summary>
		/// �����������.
		/// </summary>
		Organisation		= 0x400000000,

		/// <summary>
		/// ������������.
		/// </summary>
		Qualitative			= 0x800000000,

		/// <summary>
		/// 
		/// </summary>
		DeFactoSingTantum	= 0x1000000000,

		/// <summary>
		/// ��������������.
		/// </summary>
		Interrogative		= 0x2000000000,

		/// <summary>
		/// ������������.
		/// </summary>
		Demonstrative		= 0x4000000000,

		/// <summary>
		/// ���.
		/// </summary>
		Name				= 0x8000000000,

		/// <summary>
		/// �������.
		/// </summary>
		Surname				= 0x10000000000,

		/// <summary>
		/// ���������.
		/// </summary>
		Impersonal			= 0x20000000000,

		/// <summary>
		/// �����.
		/// </summary>
		Slang				= 0x40000000000,

		/// <summary>
		/// ��������.
		/// </summary>
		Misprint			= 0x80000000000,

		/// <summary>
		/// �����������.
		/// </summary>
		Colloquial			= 0x100000000000,

		/// <summary>
		/// ��������������.
		/// </summary>
		Possessive			= 0x200000000000,

		/// <summary>
		/// �������.
		/// </summary>
		Archaism			= 0x400000000000,

		/// <summary>
		/// 
		/// </summary>
		SecondCase			= 0x800000000000,

		/// <summary>
		/// ������.
		/// </summary>
		Poetry				= 0x1000000000000,

		/// <summary>
		/// ���������.
		/// </summary>
		Profession			= 0x2000000000000,

		/// <summary>
		/// ������������.
		/// </summary>
		Superlative			= 0x4000000000000,

		/// <summary>
		/// 
		/// </summary>
		Positive			= 0x8000000000000
	};
}}