using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Collections.ObjectModel;

namespace RegexProgect
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int counter = 0;
        bool Searching;
        bool Stop = false;
        ObservableCollection<URL> urls = new ObservableCollection<URL>();
        public string maxUrl;
        public int max = 0;
        public MainWindow()
        {
            InitializeComponent();
            lvUrls.ItemsSource = urls; // Привязываем список к ListView
        }

        private async void BtnSearch_Click(object sender, RoutedEventArgs e) // Кнопка поиска
        {
            tbCounter.Text = "Searching...";
            await WorkingMethod();

            if (!Searching && !Stop)
            {
                tbCounter.Text = "Matches: " + counter.ToString();
                counter = 0;
                tbMaxUrl.Text = "Max matches at: " + maxUrl + " (" + max + ")";
            }
            Stop = false;
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e) // Кнопка остановки поиска
        {
            if (Searching)
            {
                Stop = true;
                Searching = false;
                tbCounter.Text = "Canceled.";
            }
        }

        private Task WorkingMethod()
        {
            return Task.Run(() =>
            {
                Dispatcher.Invoke(() => urls.Clear()); // Очищаем список url-ов
                Searching = true;
                string regtext = null;
                string filename = null;
                Dispatcher.Invoke(() => filename = tbFileName.Text);
                Dispatcher.Invoke(() => regtext = tbRegex.Text);
                Regex regex = new Regex(regtext); // Создаем регулярное выражение для поиска
                Stream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs);
                while (!sr.EndOfStream && Searching)
                {
                    string url = sr.ReadLine(); // Считываем адрес из файла
                    Dispatcher.Invoke(() => urls.Add(new URL(url)));
                    WebRequest req = WebRequest.Create(url);
                    WebResponse resp = req.GetResponse(); // Открываем страницу
                    Stream stream = resp.GetResponseStream();
                    StreamReader srd = new StreamReader(stream);
                    string s = srd.ReadToEnd(); // Считываем текст со страницы
                    MatchCollection matches = regex.Matches(s); // ищем совпадения
                    if (max < matches.Count) // Записываем url с максимальным количеством совпадений
                    {
                        max = matches.Count;
                        maxUrl = url;
                    }
                    counter += matches.Count;
                    srd.Close();
                    stream.Close();
                }
                sr.Close();
                fs.Close();
                Searching = false;
            });
        }
    }
}
