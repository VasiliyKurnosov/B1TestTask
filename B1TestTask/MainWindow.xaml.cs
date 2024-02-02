using B1TestTask.Task1;
using B1TestTask.Task2;
using System.Windows;

namespace B1TestTask
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            task1Control.DataContext = new GeneratorViewModel("Data Source=..\\..\\..\\Task1\\Database\\task1.db");
            task2Control.DataContext = new RecordsViewModel("Data Source=..\\..\\..\\Task2\\Database\\task2.db");
        }
    }
}
