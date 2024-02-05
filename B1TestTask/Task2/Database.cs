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
                    InsertRecordIntoTable(record, "records", connection);
                }
            }
        }

        public void InsertTwoDigitBalanceAccountTotals(IEnumerable<Record> twoDigitBalanceAccountTotals)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                foreach (var record in twoDigitBalanceAccountTotals)
                {
                    InsertRecordIntoTable(record, "two_digit_balance_account_totals", connection);
                }
            }
        }

        public void InsertClassTotals(IEnumerable<Record> classTotals)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                foreach (var record in classTotals)
                {
                    InsertRecordIntoTable(record, "class_totals", connection);
                }
            }
        }

        public IEnumerable<Record> GetRecords()
        {
            return GetRecordsFromTable("records");
        }

        public IEnumerable<Record> GetTwoDigitBalanceAccountTotals()
        {
            return GetRecordsFromTable("two_digit_balance_account_totals");
        }

        public IEnumerable<Record> GetClassTotals()
        {
            return GetRecordsFromTable("class_totals");
        }

        private void InsertRecordIntoTable(Record record, string tableName, SqliteConnection connection)
        {
            var formatProvider = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            string incomingBalanceActiveString = record.IncomingBalanceActive.ToString(formatProvider);
            string incomingBalancePassiveString = record.IncomingBalancePassive.ToString(formatProvider);
            string turnoverDebitString = record.TurnoverDebit.ToString(formatProvider);
            string turnoverCreditString = record.TurnoverCredit.ToString(formatProvider);
            string outgoingBalanceActiveString = record.OutgoingBalanceActive.ToString(formatProvider);
            string outgoingBalancePassiveString = record.OutgoingBalancePassive.ToString(formatProvider);

            string sqlExpression = @$"INSERT INTO {tableName}
(balance_account, incoming_balance_active, incoming_balance_passive, turnover_debit,turnover_credit,
outgoing_balance_active, outgoing_balance_passive)
VALUES({record.BalanceAccount}, {incomingBalanceActiveString}, {incomingBalancePassiveString},
{turnoverDebitString}, {turnoverCreditString}, {outgoingBalanceActiveString}, {outgoingBalancePassiveString})
ON CONFLICT DO UPDATE SET incoming_balance_active = incoming_balance_active + {incomingBalanceActiveString},
incoming_balance_passive = incoming_balance_passive + {incomingBalancePassiveString},
turnover_debit = turnover_debit + {turnoverDebitString}, turnover_credit = turnover_credit + {turnoverCreditString},
outgoing_balance_active = outgoing_balance_active + {outgoingBalanceActiveString},
outgoing_balance_passive = outgoing_balance_passive + {outgoingBalancePassiveString};";

            var insertRowCommand = new SqliteCommand(sqlExpression, connection);
            insertRowCommand.ExecuteNonQuery();
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
