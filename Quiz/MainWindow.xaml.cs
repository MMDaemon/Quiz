using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace Quiz
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Question> _questions;

        public MainWindow()
        {
            _questions = new List<Question>();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    {
                        reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',', ';');

                            Question question = new Question { Text = values[0], Answers = new string[] { values[1], values[2], values[3], values[4] }, Difficulty = int.Parse(values[5]) };
                            _questions.Add(question);
                        }
                    }
                    _questions = _questions.OrderBy((question) => question.Difficulty).ToList();
                }
            }
        }
    }
}
