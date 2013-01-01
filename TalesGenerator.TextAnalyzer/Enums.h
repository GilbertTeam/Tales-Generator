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
		/// Существительное.
		/// </summary>
		NOUN				= 0,

		/// <summary>
		/// Прилагательное.
		/// </summary>
		ADJ_FULL			= 1,

		/// <summary>
		/// Глагол.
		/// </summary>
		VERB				= 2,

		/// <summary>
		/// Местоимение.
		/// </summary>
		PRONOUN				= 3,

		/// <summary>
		/// Местоименное прилагательное.
		/// </summary>
		PRONOUN_P			= 4,

		/// <summary>
		/// Местоимение-предикатив.
		/// </summary>
		PRONOUN_PREDK		= 5,

		/// <summary>
		/// Количественное числительное.
		/// </summary>
		NUMERAL				= 6,

		/// <summary>
		/// Порядковое числительное.
		/// </summary>
		NUMERAL_P			= 7,

		/// <summary>
		/// Наречие.
		/// </summary>
		ADV					= 8,

		/// <summary>
		/// Предикатив.
		/// </summary>
		PREDK				= 9,

		/// <summary>
		/// Предлог.
		/// </summary>
		PREP				= 10,

		/// <summary>
		/// Послелог.
		/// </summary>
		POSL				= 11,

		/// <summary>
		/// Союз.
		/// </summary>
		CONJ				= 12,

		/// <summary>
		/// Междометие.
		/// </summary>
		INTERJ				= 13,

		/// <summary>
		/// Вводное слово.
		/// </summary>
		INP					= 14,

		/// <summary>
		/// Фраза.
		/// </summary>
		PHRASE				= 15,

		/// <summary>
		/// Частица.
		/// </summary>
		PARTICLE			= 16,

		/// <summary>
		/// Краткое прилагательное.
		/// </summary>
		ADJ_SHORT			= 17,

		/// <summary>
		/// Причастие.
		/// </summary>
		PARTICIPLE			= 18,

		/// <summary>
		/// Деепричастие.
		/// </summary>
		ADVERB_PARTICIPLE	= 19,

		/// <summary>
		/// Краткое причастие.
		/// </summary>
		PARTICIPLE_SHORT	= 20,

		/// <summary>
		/// Инфинитив.
		/// </summary>
		INFINITIVE			= 21,

		UNKNOWN				= 22,
	};

	[System::FlagsAttribute]
	public enum class Grammem : System::UInt64
	{
		None				= 0x0,

		/// <summary>
		/// Множественное число.
		/// </summary>
		Plural				= 0x1,

		/// <summary>
		/// Единственное число.
		/// </summary>
		Singular			= 0x2,

		/// <summary>
		/// Именительный падеж.
		/// </summary>
		Nominativ			= 0x4,

		/// <summary>
		/// Родительный падеж.
		/// </summary>
		Genitiv				= 0x8,

		/// <summary>
		/// Дательный падеж.
		/// </summary>
		Dativ				= 0x10,

		/// <summary>
		/// Винительный падеж.
		/// </summary>
		Accusativ			= 0x20,

		/// <summary>
		/// Творительный падеж.
		/// </summary>
		Instrumentalis		= 0x40,

		/// <summary>
		/// Предложный падеж.
		/// </summary>
		Locativ				= 0x80,

		/// <summary>
		/// Звательный падеж.
		/// </summary>
		Vocativ				= 0x100,

		/// <summary>
		/// Мужской род.
		/// </summary>
		Masculinum			= 0x200,

		/// <summary>
		/// Женский род.
		/// </summary>
		Feminum				= 0x400,

		/// <summary>
		/// Средний род.
		/// </summary>
		Neutrum				= 0x800,

		/// <summary>
		/// Мужской-женский род.
		/// </summary>
		MascFem				= 0x1000,

		/// <summary>
		/// Краткая форма.
		/// </summary>
		ShortForm			= 0x2000,

		/// <summary>
		/// Настоящее время.
		/// </summary>
		PresentTense		= 0x4000,

		/// <summary>
		/// Будущее время.
		/// </summary>
		FutureTense			= 0x8000,

		/// <summary>
		/// Прошедшее время.
		/// </summary>
		PastTense			= 0x10000,

		/// <summary>
		/// Первое лицо.
		/// </summary>
		FirstPerson			= 0x20000,

		/// <summary>
		/// Второе лицо.
		/// </summary>
		SecondPerson		= 0x40000,

		/// <summary>
		/// Третье лицо.
		/// </summary>
		ThirdPerson			= 0x80000,

		/// <summary>
		/// Повелительное наклонение.
		/// </summary>
		Imperative			= 0x100000,

		/// <summary>
		/// Одушевленный.
		/// </summary>
		Animative			= 0x200000,

		/// <summary>
		/// Неодушевленный.
		/// </summary>
		NonAnimative		= 0x400000,

		/// <summary>
		/// Сравнительный.
		/// </summary>
		Comparative			= 0x800000,

		/// <summary>
		/// Совершенный вид.
		/// </summary>
		Perfective			= 0x1000000,

		/// <summary>
		/// Несовершенный вид.
		/// </summary>
		NonPerfective		= 0x2000000,

		/// <summary>
		/// Непереходный глагол.
		/// </summary>
		NonTransitive		= 0x4000000,

		/// <summary>
		/// Переходный глагол.
		/// </summary>
		Transitive			= 0x8000000,

		/// <summary>
		/// Действительный залог.
		/// </summary>
		ActiveVoice			= 0x10000000,

		/// <summary>
		/// Страдательный залог.
		/// </summary>
		PassiveVoice		= 0x20000000,

		/// <summary>
		/// Несклоняемый.
		/// </summary>
		Indeclinable		= 0x40000000,

		/// <summary>
		/// Инициализм (аббревиатура из первых букв).
		/// </summary>
		Initialism			= 0x80000000,

		/// <summary>
		/// Отчество.
		/// </summary>
		Patronymic			= 0x100000000,

		/// <summary>
		/// Топоним.
		/// </summary>
		Toponym				= 0x200000000,

		/// <summary>
		/// Организация.
		/// </summary>
		Organisation		= 0x400000000,

		/// <summary>
		/// Качественный.
		/// </summary>
		Qualitative			= 0x800000000,

		/// <summary>
		/// 
		/// </summary>
		DeFactoSingTantum	= 0x1000000000,

		/// <summary>
		/// Вопросительный.
		/// </summary>
		Interrogative		= 0x2000000000,

		/// <summary>
		/// Указательный.
		/// </summary>
		Demonstrative		= 0x4000000000,

		/// <summary>
		/// Имя.
		/// </summary>
		Name				= 0x8000000000,

		/// <summary>
		/// Фамилия.
		/// </summary>
		Surname				= 0x10000000000,

		/// <summary>
		/// Безличный.
		/// </summary>
		Impersonal			= 0x20000000000,

		/// <summary>
		/// Сленг.
		/// </summary>
		Slang				= 0x40000000000,

		/// <summary>
		/// Опечатка.
		/// </summary>
		Misprint			= 0x80000000000,

		/// <summary>
		/// Разговорный.
		/// </summary>
		Colloquial			= 0x100000000000,

		/// <summary>
		/// Притяжательный.
		/// </summary>
		Possessive			= 0x200000000000,

		/// <summary>
		/// Архаизм.
		/// </summary>
		Archaism			= 0x400000000000,

		/// <summary>
		/// 
		/// </summary>
		SecondCase			= 0x800000000000,

		/// <summary>
		/// Поэзия.
		/// </summary>
		Poetry				= 0x1000000000000,

		/// <summary>
		/// Профессия.
		/// </summary>
		Profession			= 0x2000000000000,

		/// <summary>
		/// Превосходный.
		/// </summary>
		Superlative			= 0x4000000000000,

		/// <summary>
		/// 
		/// </summary>
		Positive			= 0x8000000000000
	};
}}