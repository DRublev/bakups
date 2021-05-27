using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPT.Helpers
{
    public static class Translitter
    {
		private static Dictionary<string, string> transliter = new Dictionary<string, string>()
		{
			{"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"},
			{"д", "d"}, {"е", "e"}, {"ё", "e"}, {"ж", "zh"},
			{"з", "z"}, {"и", "i"}, {"й", "i"}, {"к", "k"},
			{"л", "l"}, {"м", "m"}, {"н", "n"}, {"о", "o"},
			{"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
			{"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "c"},
			{"ч", "ch"}, {"ш", "sh"}, {"щ", "sch"}, {"ы", "i"},
			{"э", "e"}, {"ю", "yu"}, {"я", "ya"}, {" ", " "},
			{"А", "A"}, {"Б", "B"}, {"В", "V"}, {"Г", "G"},
			{"Д", "D"}, {"Е", "E"}, {"Ё", "E"}, {"Ж", "Zh"},
			{"З", "Z"}, {"И", "I"}, {"Й", "I"}, {"К", "K"},
			{"Л", "L"}, {"М", "M"}, {"Н", "N"}, {"О", "O"},
			{"П", "P"}, {"Р", "R"}, {"С", "S"}, {"Т", "T"},
			{"У", "U"}, {"Ф", "F"}, {"Х", "H"}, {"Ц", "C"},
			{"Ч", "Ch"}, {"Ш", "Sh"}, {"Щ", "Sch"}, {"Ы", "I"},
			{"Э", "E"}, {"Ю", "Yu"}, {"Я", "Ya"}
		};

		/// <summary>
		/// Translit source string from RU to EN
		/// </summary>
		/// <param name="source">String to translit</param>
		/// <returns></returns>
		public static string Translit(string source)
		{
			string translitted = String.Empty;

			for(int i = 0; i < source.Length; i++)
			{
				var character = source[i].ToString();
				string translittedChar = String.Empty;

				if (transliter.ContainsKey(character))
				{
					translittedChar = transliter[character].ToString();
					translitted += translittedChar;
				}
			}

			return translitted;
		}
	}
}
