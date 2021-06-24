using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace WordPredictions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker backgroundWorker;
        private List<String> wordsFromWebservice;
        private String pendingMessage;

        public MainWindow()
        {
            InitializeComponent();

            wordsFromWebservice = new List<string>();

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchFromSQLiteAndWebService();
        }

        private void inputTexbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchFromSQLiteAndWebService();
        }

        private void SearchFromSQLiteAndWebService()
        {
            var words = inputTexbox.Text.Split(' ');
            var searchingWord = words[words.Length - 1];

            // Fetch data from SQLite
            try
            {
                var adapter = new SQLiteAdapter();
                var wordsFromDB = adapter.getWords(searchingWord);
                SQLiteListBox.Items.Clear();
                foreach (var item in wordsFromDB)
                {
                    SQLiteListBox.Items.Add(item);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                MessageBox.Show("Something wrong with SQLite database");
            }

            // Fetch data from Web Service
            if (!backgroundWorker.IsBusy) backgroundWorker.RunWorkerAsync(searchingWord);
            else
            {
                pendingMessage = searchingWord;
                backgroundWorker.CancelAsync();
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                try
                {
                    var caller = new WebServiceCaller();
                    wordsFromWebservice = caller.getWords(e.Argument.ToString());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    MessageBox.Show("Something wrong with Web Service");
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (pendingMessage == null)
            {
                WebServiceListBox.Items.Clear();
                foreach (var item in wordsFromWebservice)
                {
                    WebServiceListBox.Items.Add(item);
                }
            }
            else
            {
                backgroundWorker.RunWorkerAsync(pendingMessage);
                pendingMessage = null;
            }
        }

        private void SQLiteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReplaceWord(e.AddedItems);
        }

        private void WebServiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReplaceWord(e.AddedItems);
        }

        private void ReplaceWord(IList list)
        {
            if (list.Count > 0)
            {
                var words = inputTexbox.Text.Split(' ');
                var searchWord = words[words.Length - 1];
                words[words.Length - 1] = list[0] + " ";
                inputTexbox.Text = string.Join(" ", words);
                inputTexbox.Focus();
                inputTexbox.SelectionStart = inputTexbox.Text.Length;
            }
        }
    }
}
