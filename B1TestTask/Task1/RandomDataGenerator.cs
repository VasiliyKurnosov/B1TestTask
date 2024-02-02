using System;
using System.Text;

namespace B1TestTask.Task1
{
    public class RandomDataGenerator
    {
        private const string _englishLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _russianLetters = "абвгдеёжзийклмнопрстуфхцчшщьыъэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЬЫЪЭЮЯ";

        private readonly Random _random = new Random();

        public DateTime GenerateDate(DateTime startDate, DateTime endDate)
        {
            int daysRange = (endDate - startDate).Days;
            return startDate.AddDays(_random.Next(daysRange + 1));
        }

        public string GenerateEnglishString(int length)
        {
            return GenerateString(length, _englishLetters);
        }

        public string GenerateRussianString(int length)
        {
            return GenerateString(length, _russianLetters);
        }

        public int GenerateEvenNumber(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue + 1) & ~1;
        }

        public double GenerateDouble(int minValue, int maxValue, int decimalPlacesNumber)
        {
            double value = _random.NextDouble() * (maxValue - minValue) + minValue;
            return Math.Round(value, decimalPlacesNumber);
        }

        private string GenerateString(int length, string characters)
        {
            var resultStringBuilder = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                resultStringBuilder.Append(characters[_random.Next(characters.Length)]);
            }
            return resultStringBuilder.ToString();
        }
    }
}
