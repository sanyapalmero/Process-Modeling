using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace process_modeling
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public class Table//класс таблица
        {
            public string ProcessName { get; set; }//имя процесса
            public List<string> ProcessUsage { get; set; }//список строк, содержащий информацию о использовании процесса (в таблице отображается как строка)
        }

        public class Process //класс Процесс
        {
            public string ProcName;//имя процесса
            public int TimeWait;//время ожидания
            public int TimeUsage;//время выполнения
        }

        ObservableCollection<Table> ProcessesCollection = new ObservableCollection<Table>();//коллекция процессов
        List<Process> ListProc = new List<Process>();//список объектов класса Process
        List<DataGridTextColumn> ListColumns = new List<DataGridTextColumn>();//список объектов класса DataGridTextColumn

        int ProcCount = 1;//количество процессов
        int ColumnCount = 1;//количество колонок
        int allTime = 0;//общее время выполнения процессов

        private void AddRow(int time)//метод, выполняющий обработку значений процесса и добавление строки в таблицу
        {
            var values = new List<string>();//список строк, содержащий информацию об использовании процесса

            for (int i = 0; i < time; i++)//цикл нужен для того, чтобы динамично создавать колонки в зависимости от time
            {
                var textColumn = new DataGridTextColumn();//создание объекта textColumn
                var column = "ProcessUsage[" + (ColumnCount - 1) + "]";//свойство колонки ProcessUsage0, ProcessUsage1 ...
                textColumn.Header = ColumnCount;//значение, которое отображается в названии колонки, т.е. 1, 2, 3...
                textColumn.Binding = new Binding(column);//каждая колонка будет иметь Name="CurrentStatus<порядковый номер>"
                DataGrid.Columns.Add(textColumn);//добавление объекта textColumn в коллекцию DataGrid
                ListColumns.Add(textColumn);//добавление объекта textColumn в список ListColumns
                ColumnCount++;//увеличиваем кол-во колонок
            }

            if (ProcCount == 1)//если процесс первый
            {
                for (int k = 0; k < time; k++)
                {
                    values.Add("И");//в список values добавляем "И" ровно столько раз, какое у него процессорное время т.е. time
                }
            }
            else//если процесс НЕ первый
            {
                for (int j = 0; j < allTime; j++)
                {
                    values.Add("Г");//в список values добавляем "Г" ровно столько раз, чему равняется общее время (оно равно общему времени предыдущего процесса)
                }
                for (int k = 0; k < time; k++)
                {
                    values.Add("И");//затем в этот же список values добавляем "И" ровно столько раз, какое у него процессорное время т.е. time
                }
            }

            var process = new Process//создание объекта и заполнение полей
            {
                ProcName = "Процесс " + ProcCount,//имя процесса
                TimeWait = allTime,//время ожидания
                TimeUsage = time//время выполнения
            };
            ListProc.Add(process);//сохранение объекта в список ListProc для дальнейших вычислений

            var tableRow = new Table//создание объекта и заполнение полей
            {
                ProcessName = "Процесс " + ProcCount,//имя процесса
                ProcessUsage = values//использование процесса (список значений "И" или "Г")
            };

            ProcessesCollection.Add(tableRow);//добавление объекта tableRow в коллекцию
            DataGrid.ItemsSource = ProcessesCollection;//добавление коллекции в DataGrid

            allTime += time;//к общему времени добавлем время текущего процесса
            ProcCount++;//увеличиваем кол-во процессов
        }

        private void ProcAdd(object sender, RoutedEventArgs e)//Кнопка "Добавить процесс"
        {
            int time;//локальная переменная для хранения значения времени
            string processTimeField = TimeValue.Text;//получение процессорного времени
            if (String.IsNullOrEmpty(processTimeField))//если поле было пустое
            {
                time = 1;//время будет 1
            }
            else
            {
                time = int.Parse(processTimeField);//сохранение строки в time типа int
            }
            AddRow(time);//вызов метода AddRow
            TimeValue.Clear();
        }

        private void ProcDel(object sender, RoutedEventArgs e)//сброс процессов
        {
            //очистка таблицы
            ProcessesCollection.Clear();//очистка коллекции
            //установка глобальных переменных в исходные значения
            ProcCount = 1;
            ColumnCount = 1;
            allTime = 0;
            //очистка колонок
            for (int i = 0; i < ListColumns.Count; i++)
            {
                DataGrid.Columns.Remove(ListColumns[i]);
            }
            //очистка значений времени
            ListProc.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)//вычисление средних значений
        {
            double all = ListProc.Count;//количество всех процессов
            double timewait = 0;
            double timedoit = 0;
            double sredtimewait = 0;
            double sredtimedoit = 0;
            for (int i = 0; i < ListProc.Count; i++)
            {
                timewait += ListProc[i].TimeWait;
                timedoit += ListProc[i].TimeUsage;
            }
            sredtimewait = timewait / all;
            sredtimedoit = (timedoit + timewait) / all;
            TimeWaiting.Content = sredtimewait;
            TimeDoing.Content = sredtimedoit;
        }

        
    }
}
