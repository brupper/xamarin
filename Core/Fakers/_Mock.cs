using System;
using System.Collections.Generic;
using System.Text;

namespace Brupper.Fakers
{
    public class Generator
    {
        #region Declarations

        private const string Vowels = "aeiou"; //Full list: aeiouy
        private const string Consonants = "bcdfghjklmnprstvz"; //Full list: bcdfghjklmnpqrstvwxz

        private static List<string> NameFormats = new List<string>()
        {
            "Cvc",
            "Cvcc", //Kish
            "Cvcv",
            "Cvcvc",
            "Cvc Cvc",
            "Cvc Cvcv", //Tau Ceti
            "Vcvcvcv", //Eridani
            "Cvcvvs", //Sirius
            "Cvccvccv", //Macragge
            "Cvcvccvvcv" //Betelgeuse
        };

        private const double PostfixChance = 0.3;

        private static List<string> Postfixes = new List<string>()
        {
            "Alpha",
            "Beta",
            "Gamma",
            "Primus",
            "Secundus",
            "IX",
            "X"
        };

        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region Methods

        public static string CreateName()
        {
            string name = string.Empty;

            //Select a random NameFormat while respecting probability densities
            string nameFormat = NameFormats[rnd.Next(0, NameFormats.Count)];

            //Generate the letters one by one
            for (int i = 0; i < nameFormat.Length; i++)
            {
                if (nameFormat[i] == 'c')
                {
                    name += GetRandomLetter(Consonants);
                }
                else if (nameFormat[i] == 'C')
                {
                    name += GetRandomLetter(Consonants).ToString().ToUpper();
                }
                else if (nameFormat[i] == 'v')
                {
                    name += GetRandomLetter(Vowels);
                }
                else if (nameFormat[i] == 'V')
                {
                    name += GetRandomLetter(Vowels).ToString().ToUpper();
                }
                else
                {
                    name += nameFormat[i];
                }
            }

            //Add a postfix
            if (rnd.NextDouble() <= PostfixChance)
            {
                name += " " + Postfixes[rnd.Next(0, Postfixes.Count)];
            }

            return name;
        }

        private static char GetRandomLetter(string input)
        {
            if (input == string.Empty)
                return ' ';
            return input[rnd.Next(0, input.Length)];
        }

        public static int CreateRandomInteger(int minValue = 0, int maxValue = int.MaxValue) => rnd.Next(minValue, maxValue);

        public static string CreateRandomIntegerAsString(int length)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.Next(0, 10).ToString());
            }

            return sb.ToString();
        }

        #endregion
    }
}
