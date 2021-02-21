using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Quiz
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _currentQuestionID;
        private List<Question> _questions;

        private Question _currentQuestion;
        public Question CurrentQuestion
        {
            get => _currentQuestion;
            set => SetField(ref _currentQuestion, value, nameof(CurrentQuestion));
        }

        private string _questionCount;
        public string QuestionCount
        {
            get => _questionCount;
            set => SetField(ref _questionCount, value, nameof(QuestionCount));
        }

        public MainWindow()
        {
            _questions = new List<Question>();
            InitializeComponent();
            this.DataContext = this;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void SetQuestionID(int questionID)
        {
            answer1.IsEnabled = true;
            answer2.IsEnabled = true;
            answer3.IsEnabled = true;
            answer4.IsEnabled = true;

            answer1.Background = Brushes.LightGray;
            answer2.Background = Brushes.LightGray;
            answer3.Background = Brushes.LightGray;
            answer4.Background = Brushes.LightGray;

            _currentQuestionID = questionID;
            CurrentQuestion = _questions[_currentQuestionID];
            QuestionCount = $"Frage {_currentQuestionID+1} / {_questions.Count}";
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
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

                            Question question = new Question(values[0], values[1], new string[] { values[2], values[3], values[4] }, int.Parse(values[5]));
                            _questions.Add(question);
                        }
                    }
                    _questions = _questions.OrderBy((question) => question.Difficulty).ToList();
                    SetQuestionID(0);
                }
            }
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button answerButton = (System.Windows.Controls.Button)sender;
            if (CurrentQuestion.IsCorrect(answerButton.Content.ToString())){
                answerButton.Background = Brushes.Green;
            }
            else
            {
                answerButton.Background = Brushes.Red;
            }

            if(CurrentQuestion.IsCorrect(answer1.Content.ToString()))
            {
                answer1.Background = Brushes.Green;
            }
            if (CurrentQuestion.IsCorrect(answer2.Content.ToString()))
            {
                answer2.Background = Brushes.Green;
            }
            if (CurrentQuestion.IsCorrect(answer3.Content.ToString()))
            {
                answer3.Background = Brushes.Green;
            }
            if (CurrentQuestion.IsCorrect(answer4.Content.ToString()))
            {
                answer4.Background = Brushes.Green;
            }

            if(_currentQuestionID + 1 < _questions.Count)
            {
                next.IsEnabled = true;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            next.IsEnabled = false;
            SetQuestionID(_currentQuestionID + 1);
        }
    }
}
