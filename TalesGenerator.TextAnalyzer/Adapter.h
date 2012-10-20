#pragma once

namespace TalesGenerator { namespace Text {

	class Adapter
	{
	public:
		virtual MAFSA::l_string String2Letters(const char *s) = 0;
		virtual std::string Letters2String(const MAFSA_letter *s, size_t sz) = 0;
		virtual MAFSA::l_string Binary2Letters(const char *s, size_t sz) = 0;
		virtual std::string Letters2Binary(const MAFSA_letter *s, size_t sz) = 0;
		virtual uint32_t GetMaxLetter() = 0;
	};

	class RussianUtf8Adapter : public Adapter
	{
	public:
		virtual MAFSA::l_string String2Letters(const char *s)
		{
			return russian_utf8_adapter::string2letters(s);
		}

		virtual std::string Letters2String(const MAFSA_letter *s, size_t sz)
		{
			return russian_utf8_adapter::letters2string(s, sz);
		}

		virtual MAFSA::l_string Binary2Letters(const char *s, size_t sz)
		{
			return russian_utf8_adapter::binary2letters(s, sz);
		}

		virtual std::string Letters2Binary(const MAFSA_letter *s, size_t sz)
		{
			return russian_utf8_adapter::letters2binary(s, sz);
		}

		virtual uint32_t GetMaxLetter()
		{
			return russian_utf8_adapter::max_letter;
		}
	};

	class RussianCp1251Adapter : public Adapter
	{
	public:
		virtual MAFSA::l_string String2Letters(const char *s)
		{
			return russian_cp1251_adapter::string2letters(s);
		}

		virtual std::string Letters2String(const MAFSA_letter *s, size_t sz)
		{
			return russian_cp1251_adapter::letters2string(s, sz);
		}

		virtual MAFSA::l_string Binary2Letters(const char *s, size_t sz)
		{
			return russian_cp1251_adapter::binary2letters(s, sz);
		}

		virtual std::string Letters2Binary(const MAFSA_letter *s, size_t sz)
		{
			return russian_cp1251_adapter::letters2binary(s, sz);
		}

		virtual uint32_t GetMaxLetter()
		{
			return russian_cp1251_adapter::max_letter;
		}
	};

	class RussianKoi8rAdapter : public Adapter
	{
	public:
		virtual MAFSA::l_string String2Letters(const char *s)
		{
			return russian_koi8r_adapter::string2letters(s);
		}

		virtual std::string Letters2String(const MAFSA_letter *s, size_t sz)
		{
			return russian_koi8r_adapter::letters2string(s, sz);
		}

		virtual MAFSA::l_string Binary2Letters(const char *s, size_t sz)
		{
			return russian_koi8r_adapter::binary2letters(s, sz);
		}

		virtual std::string Letters2Binary(const MAFSA_letter *s, size_t sz)
		{
			return russian_koi8r_adapter::letters2binary(s, sz);
		}

		virtual uint32_t GetMaxLetter()
		{
			return russian_koi8r_adapter::max_letter;
		}
	};
}}

