using B1TestTask.Commands;
using System;
using System.ComponentModel;
using System.IO;

namespace B1TestTask.Task1
{
    public class GeneratorViewModel : INotifyPropertyChanged
    {
        public RelayCommand GenerateFilesCommand { get; set; }
        public RelayCommand UniteFilesCommand { get; set; }
        public RelayCommand ImportDataCommand { get; set; }
        public string RemovePattern
        {
            get { return _removePattern; }
            set
            {
                _removePattern = value;
                OnPropertyChanged(nameof(RemovePattern));
            }
        }
        public int RemovedRowsNumber
        {
            get { return _removedRowsNumber; }
            set
            {
                _removedRowsNumber = value;
                OnPropertyChanged(nameof(RemovedRowsNumber));
            }
        }
        public int ImportedRowsNumber
        {
            get { return _importedRowsNumber; }
            set
            {
                _importedRowsNumber = value;
                OnPropertyChanged(nameof(ImportedRowsNumber));
            }
        }
        public int RowsNumber
        {
            get { return _rowsNumber; }
            set
            {
                _rowsNumber = value;
                OnPropertyChanged(nameof(RowsNumber));
            }
        }
        public long IntegerNumbersSum
        {
            get { return _integerNumbersSum; }
            set
            {
                _integerNumbersSum = value;
                OnPropertyChanged(nameof(IntegerNumbersSum));
            }
        }
        public double RealNumbersSum
        {
            get { return _realNumbersSum; }
            set
            {
                _realNumbersSum = value;
                OnPropertyChanged(nameof(RealNumbersSum));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        private const string _filesDirectoryPath = "files";
        private const string _unitedFileName = "united_file.txt";

        private readonly string _connectionString;

        private string _removePattern = "";
        private int _removedRowsNumber;
        private int _rowsNumber;
        private int _importedRowsNumber;
        private long _integerNumbersSum;
        private double _realNumbersSum;

        public GeneratorViewModel(string connectionString)
        {
            _connectionString = connectionString;
            GenerateFilesCommand = new RelayCommand(param => GenerateFiles());
            UniteFilesCommand = new RelayCommand(param => UniteFiles());
            ImportDataCommand = new RelayCommand(param => ImportData());
        }

        private void GenerateFiles()
        {
            string filesNamePattern = "file";
            int filesNumber = 100;
            int rowsInFileNumber = 100000;
            RowsFilesManager.CreateRowsFiles(_filesDirectoryPath, filesNamePattern, filesNumber, rowsInFileNumber);
        }

        private void UniteFiles()
        {
            RemovedRowsNumber = RowsFilesManager.UniteFilesFromDirectory(_filesDirectoryPath, _unitedFileName, _removePattern);
        }

        private void ImportData()
        {
            var database = new Database(_connectionString);
            var rows = File.ReadAllLines(_unitedFileName);
            RowsNumber = rows.Length;
            var importProgress = new Progress<int>(importedRowsNumber =>
            {
                ImportedRowsNumber = importedRowsNumber;
            });
            database.ImportRows(rows, importProgress);

            IntegerNumbersSum = database.CalculateIntegerNumbersSum();
            RealNumbersSum = database.CalculateRealNumbersAverage();
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
