using B1TestTask.Task2.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace B1TestTask.Task2
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertRecords(IEnumerable<Record> records)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                foreach (var record in records)
                {
                    var formatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-US");
                    string incomingBalanceActiveString = record.IncomingBalanceActive.ToString(formatProvider);
                    string incomingBalancePassiveString = record.IncomingBalancePassive.ToString(formatProvider);
                    string debitString = record.TurnoverDebit.ToString(formatProvider);
                    string creditString = record.TurnoverCredit.ToString(formatProvider);
                    string sqlExpression = @$"INSERT INTO turnovers
(balance_account, incoming_balance_active, incoming_balance_passive, debit, credit)
VALUES({record.BalanceAccount}, {incomingBalanceActiveString}, {incomingBalancePassiveString},
{debitString}, {creditString})
ON CONFLICT DO UPDATE SET incoming_balance_active = incoming_balance_active + {incomingBalanceActiveString},
incoming_balance_passive = incoming_balance_passive + {incomingBalancePassiveString},
debit = debit + {debitString}, credit = credit + {creditString};";

                    var insertRowCommand = new SqliteCommand(sqlExpression, connection);
                    insertRowCommand.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Record> GetAllRecords()
        {
            return GetRecordsFromTable("full_turnovers");
        }

        public IEnumerable<Record> GetTwoDigitBalanceAccountTotals()
        {
            return GetRecordsFromTable("full_turnovers_grouped_by_balance_account");
        }

        public IEnumerable<Record> GetClassTotals()
        {
            return GetRecordsFromTable("full_turnovers_grouped_by_class");
        }

        public Record GetTotalRecord()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression = @"SELECT ifnull(SUM(incoming_balance_active), 0),
ifnull(SUM(incoming_balance_passive), 0), ifnull(SUM(debit), 0), ifnull(SUM(credit), 0),
ifnull(SUM(outgoing_balance_active), 0), ifnull(SUM(outgoing_balance_passive), 0) FROM full_turnovers";

                var selectCommand = new SqliteCommand(sqlExpression, connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        var record = new Record
                        {
                            BalanceAccount = 0,
                            IncomingBalanceActive = reader.GetDecimal(0),
                            IncomingBalancePassive = reader.GetDecimal(1),
                            TurnoverDebit = reader.GetDecimal(2),
                            TurnoverCredit = reader.GetDecimal(3),
                            OutgoingBalanceActive = reader.GetDecimal(4),
                            OutgoingBalancePassive = reader.GetDecimal(5)
                        };
                        return record;
                    }
                }
            }
            return new Record();
        }

        public IEnumerable<string> GetClassDescriptions()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression = "SELECT description FROM classes";

                var selectCommand = new SqliteCommand(sqlExpression, connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string className = reader.GetString(0);
                            yield return className;
                        }
                    }
                }
            }
        }

        private IEnumerable<Record> GetRecordsFromTable(string tableName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                string sqlExpression = $"SELECT * FROM {tableName}";

                var selectCommand = new SqliteCommand(sqlExpression, connection);
                using (var reader = selectCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var record = new Record
                            {
                                BalanceAccount = reader.GetInt32(0),
                                IncomingBalanceActive = reader.GetDecimal(1),
                                IncomingBalancePassive = reader.GetDecimal(2),
                                TurnoverDebit = reader.GetDecimal(3),
                                TurnoverCredit = reader.GetDecimal(4),
                                OutgoingBalanceActive = reader.GetDecimal(5),
                                OutgoingBalancePassive = reader.GetDecimal(6)
                            };
                            yield return record;
                        }
                    }
                }
            }
        }
    }
}
