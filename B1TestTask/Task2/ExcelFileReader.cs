using B1TestTask.Task2.Models;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace B1TestTask.Task2
{
    public static class ExcelFileReader
    {
        static ExcelFileReader()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public static IEnumerable<Record> ReadFile(string filePath)
        {
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var data = reader.AsDataSet();
                    var dataTable = data.Tables[0];
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string? cellString = row[0].ToString();
                        if (cellString?.Length == 4 && Int32.TryParse(cellString, out int balanceAccount))
                        {
                            var record = new Record
                            {
                                BalanceAccount = balanceAccount,
                                IncomingBalanceActive = Convert.ToDecimal(row[1]),
                                IncomingBalancePassive = Convert.ToDecimal(row[2]),
                                TurnoverDebit = Convert.ToDecimal(row[3]),
                                TurnoverCredit = Convert.ToDecimal(row[4]),
                                OutgoingBalanceActive = Convert.ToDecimal(row[5]),
                                OutgoingBalancePassive = Convert.ToDecimal(row[6]),
                            };
                            yield return record;
                        }
                    }
                }
            }
        }
    }
}
