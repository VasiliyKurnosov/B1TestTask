using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace B1TestTask.Task1
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ImportRows(IEnumerable<string> rows, IProgress<int> progress)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    int insertedRowsNumber = 0;
                    foreach (var row in rows)
                    {
                        var data = row.Split("||");
                        if (data.Length != 6)
                        {
                            throw new FormatException(row);
                        }

                        string realNumberString = double.Parse(data[4])
                            .ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
                        string sqlExpression = @$"INSERT INTO rows
(date, english_sequence, russian_sequence, integer_number, real_number)
VALUES('{data[0]}', '{data[1]}', '{data[2]}', {data[3]}, {realNumberString});";

                        var insertRowCommand = new SqliteCommand(sqlExpression, connection, transaction);
                        insertedRowsNumber += insertRowCommand.ExecuteNonQuery();
                        progress.Report(insertedRowsNumber);
                    }

                    transaction.Commit();
                }
            }
        }

        public long CalculateIntegerNumbersSum()
        {
            long sum = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression = "SELECT SUM(integer_number) FROM rows";
                var selectCommand = new SqliteCommand(sqlExpression, connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        sum = reader.GetInt64(0);
                    }
                }
            }
            return sum;
        }

        public double CalculateRealNumbersAverage()
        {
            double average = 0;
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression = "SELECT AVG(real_number) FROM rows";
                var selectCommand = new SqliteCommand(sqlExpression, connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        average = reader.GetDouble(0);
                    }
                }
            }
            return average;
        }
    }
}
