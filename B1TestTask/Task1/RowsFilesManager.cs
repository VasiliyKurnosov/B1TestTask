using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace B1TestTask.Task1
{
    public static class RowsFilesManager
    {
        public static void CreateRowsFiles(string directoryPath, string filesNamePattern, int filesNumber,
            int rowsInFileNumber)
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
            Directory.CreateDirectory(directoryPath);
            for (int i = 0; i < filesNumber; i++)
            {
                var rows = CreateRows(rowsInFileNumber);
                File.AppendAllLines(directoryPath + "\\" + filesNamePattern + i + ".txt", rows);
            }
        }

        public static int UniteFilesFromDirectory(string filesDirectoryPath, string newFilePath,
            string rowsToRemovePattern)
        {
            if (!Directory.Exists(filesDirectoryPath))
            {
                throw new DirectoryNotFoundException(filesDirectoryPath);
            }

            int removedRowsNumber = 0;
            using (var writeStream = new StreamWriter(newFilePath))
            {
                foreach (string filePath in Directory.GetFiles(filesDirectoryPath))
                {
                    var rows = File.ReadAllLines(filePath);
                    foreach (var row in rows)
                    {
                        if (row.Contains(rowsToRemovePattern))
                        {
                            removedRowsNumber++;
                            continue;
                        }
                        writeStream.WriteLine(row);
                    }
                }
            }
            return removedRowsNumber;
        }

        private static IEnumerable<string> CreateRows(int rowsNumber)
        {
            var generator = new RandomDataGenerator();
            var startDate = DateTime.Today.AddYears(-5);
            int decimalPlacesNumber = 8;
            for (int i = 0; i < rowsNumber; i++)
            {
                var rowBuilder = new StringBuilder();
                rowBuilder.Append(generator.GenerateDate(startDate, DateTime.Today).ToString("dd.MM.yyyy"));
                rowBuilder.Append("||");
                rowBuilder.Append(generator.GenerateEnglishString(10));
                rowBuilder.Append("||");
                rowBuilder.Append(generator.GenerateRussianString(10));
                rowBuilder.Append("||");
                rowBuilder.Append(generator.GenerateEvenNumber(1, 100_000_000));
                rowBuilder.Append("||");
                rowBuilder.Append(generator.GenerateDouble(1, 20, decimalPlacesNumber).ToString($"F{decimalPlacesNumber}"));
                rowBuilder.Append("||");
                yield return rowBuilder.ToString();
            }
        }
    }
}
