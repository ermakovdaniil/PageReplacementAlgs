using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using PageReplacementAlgs;

namespace OS_KP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private SeriesCollection seriesCol;
        public SeriesCollection SeriesCol {
            get { return seriesCol; }
            set
            {
                seriesCol = value;
                OnPropertyChanged();
            }
        }

        private int memoryCount;
        public int MemoryCount
        {
            get { return memoryCount; }
            set
            {
                memoryCount = value;
                OnPropertyChanged();
            }
        }

        private string inputedPages;
        public string InputedPages
        {
            get { return inputedPages; }
            set
            {
                inputedPages = value;
                OnPropertyChanged();
            }
        }

        private static List<int> pages;

        private int nruInteraptions;
        private int lruInteraptions;
        private int fifoInteraptions;
        private int scInteraptions;
        private int clockInteraptions;

        private string nruLog;
        private string lruLog;
        private string fifoLog;
        private string scLog;
        private string clockLog;

        private void CreateBarChartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SeriesCol = new SeriesCollection { };
                string[] stringSeparators = { "\r", "\n", " ", "\t" };
                pages = InputedPages.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(n => int.Parse(n))
                                    .ToList();

                if(pages.Count == 0)
                {
                    throw new NullReferenceException();
                }

                Algorithms.NRU(pages, MemoryCount, out nruInteraptions, out nruLog);
                NRUTextBox.Text = nruLog;
                SeriesCol.Add(new ColumnSeries
                {
                    ColumnPadding = 20,
                    Title = "NRU",
                    Values = new ChartValues<int> { nruInteraptions }
                });

                Algorithms.LRU(pages, MemoryCount, out lruInteraptions, out lruLog);
                LRUTextBox.Text = lruLog;
                SeriesCol.Add(new ColumnSeries
                {
                    ColumnPadding = 20,
                    Title = "LRU",
                    Values = new ChartValues<int> { lruInteraptions }
                });

                Algorithms.FIFO(pages, MemoryCount, out fifoInteraptions, out fifoLog);
                FIFOTextBox.Text = fifoLog;
                SeriesCol.Add(new ColumnSeries
                {
                    ColumnPadding = 20,
                    Title = "FIFO",
                    Values = new ChartValues<int> { fifoInteraptions }
                });

                Algorithms.SC(pages, MemoryCount, out scInteraptions, out scLog);
                SCTextBox.Text = scLog;
                SeriesCol.Add(new ColumnSeries
                {
                    ColumnPadding = 20,
                    Title = "Вторая попытка",
                    Values = new ChartValues<int> { scInteraptions }
                });

                Algorithms.Clock(pages, MemoryCount, out clockInteraptions, out clockLog);
                ClockTextBox.Text = clockLog;
                SeriesCol.Add(new ColumnSeries
                {
                    ColumnPadding = 20,
                    Title = "Часы",
                    Values = new ChartValues<int> { clockInteraptions }
                });

                barChart.Series = SeriesCol;
            }
            catch (FormatException)
            {
                MessageBox.Show("Исходные данные содержат некорректные значения.\n" +
                                "Данные не должны содержать букв и спец. символов.",
                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Значение было недопустимо малым или недопустимо большим.",
                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Вы не ввели исходные данные.", 
                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HelpMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данная программа реализует алгоритмы по замещению страниц: \n" +
                            "NRU, LRU, FIFO, " + @"""Вторая попытка"", ""Часы""." + "\n" +
                            "Программа отображает количество прерываний алгоритмов на гистограмме для сравнения и " +
                            "показывает пошаговое выполнение алгоритмов.\n" +
                            "Ввод очереди страниц осуществляется через пробел.\n" +
                            "Ввод количества мест в памяти осуществляется с помощью счётчика.\n" +
                            "В программе реализованы функции открытия и сохранения\n" +
                            "исходных данных в файл формата .txt, сохранения результатов в файл формата PDF.\n" +
                            "Автор:  Ермаков Даниил Игоревич\n" +
                            "Группа: 494\n" +
                            "Учебное заведение: СПбГТИ (ТУ)", "Справка о программе",
                             MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("В файле должны содержаться только числа.\n" +
                                "Первое число является количеством мест в памяти.\n" +
                                "Все последующие числа являются страницами.\n", 
                                "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                string fileInputPath;
                string pageString;
                int temp;
                var dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                bool? result = dlg.ShowDialog();
                if (result == true)
                {
                    fileInputPath = dlg.FileName;
                    PagesTextBox.Clear();
                    FileWork.OpenFile(fileInputPath, out pageString, out temp);             
                    MemoryCountUpDown.Value = temp;   
                    PagesTextBox.Text = pageString;
                }
                else
                {
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Данные в файле содержат некорректные значения.\n" +
                                "Данные не должны содержать букв и спец. символов.",
                                "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveInitialDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string fileOutputPath;
                var dlg = new SaveFileDialog();
                dlg.DefaultExt = ".txt";
                dlg.Filter = "Text documents (.txt)|*.txt";
                dlg.FileName = "Исходные данные " + DateTime.Now.ToString().Replace(':', '_');
                var result = dlg.ShowDialog();
                if (result == true)
                {
                    fileOutputPath = dlg.FileName;
                    string text = MemoryCount.ToString() + " " + InputedPages;
                    System.IO.File.WriteAllText(fileOutputPath, text);
                }
                else
                {
                    return;
                }
            }          
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Выбранный файл предназначен только для чтения.", "Ошибка!");
            }
        }

        public byte[] EncodeVisual()
        {
            var visual = barChart;
            var encoder = new PngBitmapEncoder();
            var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                var bit = stream.ToArray();
                stream.Close();
                return bit;
            }
        }

        private void SaveResultMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                var dlg = new SaveFileDialog();
                dlg.DefaultExt = ".pdf";
                dlg.Filter = "Pdf Files|*.pdf";
                dlg.FileName = "Результаты алгоритмов " + DateTime.Now.ToString().Replace(':', '_');
                var result = dlg.ShowDialog();
                if (result == true)
                {
                    FileWork.CreatePDF(dlg.FileName, EncodeVisual(), nruLog, lruLog, fifoLog, scLog, clockLog);
                }
                else
                {
                    return;
                }
            }          
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Выбранный файл предназначен только для чтения.", "Ошибка!");
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion
    }
}
