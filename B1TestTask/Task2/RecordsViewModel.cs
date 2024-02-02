using B1TestTask.Commands;
using B1TestTask.Task2.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace B1TestTask.Task2
{
    public class RecordsViewModel
    {
        public ObservableCollection<ClassRecord> ClassRecords { get; set; }
        public ObservableCollection<TwoDigitBalanceAccountRecord> TwoDigitBalanceAccountRecords { get; set; }
        public ObservableCollection<Record> Records { get; set; }
        public ObservableCollection<string> LoadedFilesNames { get; set; }
        public RelayCommand LoadFileCommand { get; set; }

        private readonly Database _database;

        public RecordsViewModel(string connectionString)
        {
            TwoDigitBalanceAccountRecords = new ObservableCollection<TwoDigitBalanceAccountRecord>();
            ClassRecords = new ObservableCollection<ClassRecord>();
            _database = new Database(connectionString);
            LoadedFilesNames = new ObservableCollection<string>();
            LoadFileCommand = new RelayCommand(param => LoadFile());
            LoadRecords();
        }

        private void LoadRecords()
        {
            Records = new ObservableCollection<Record>(_database.GetAllRecords());
            TwoDigitBalanceAccountRecords.Clear();
            ClassRecords.Clear();

            var classTotals = _database.GetClassTotals().ToList();
            var twoDigitBalanceAccountTotals = _database.GetTwoDigitBalanceAccountTotals().ToList();

            int i = 0;
            int j = 0;
            foreach (var classTotalRecord in classTotals)
            {
                var classRecord = new ClassRecord();
                classRecord.Total = classTotalRecord;
                var twoDigitBalanceAccountRecords = new List<TwoDigitBalanceAccountRecord>();
                while (i < twoDigitBalanceAccountTotals.Count
                    && twoDigitBalanceAccountTotals[i].BalanceAccount / 10 == classTotalRecord.BalanceAccount)
                {
                    var twoDigitBalanceAccountRecord = new TwoDigitBalanceAccountRecord();
                    twoDigitBalanceAccountRecord.Total = twoDigitBalanceAccountTotals[i];
                    var records = new List<Record>();
                    while (j < Records.Count
                        && Records[j].BalanceAccount / 100 == twoDigitBalanceAccountTotals[i].BalanceAccount)
                    {
                        records.Add(Records[j]);
                        j++;
                    }
                    twoDigitBalanceAccountRecord.Records = records;
                    twoDigitBalanceAccountRecords.Add(twoDigitBalanceAccountRecord);
                    i++;
                }
                classRecord.TwoDigitBalanceAccountRecords = twoDigitBalanceAccountRecords;
                ClassRecords.Add(classRecord);
            }
        }

        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var records = ExcelFileReader.ReadFile(filePath);
                _database.InsertRecords(records);
                LoadRecords();
                LoadedFilesNames.Add(filePath.Split('\\')[^1]);
            }
        }
    }
}
