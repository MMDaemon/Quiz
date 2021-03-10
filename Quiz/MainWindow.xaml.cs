using System;
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
        private static Random rng = new Random();

        private const string LOAD_QUIZ = "Quiz laden";
        private const string CONFIRM = "Antwort bestätigen";
        private const string NEXT = "Nächste Frage";
        private const string RESULT = "Ergebnis anzeigen";

        private int _correctCount = 0;
        private Dictionary<int, int> _correctByDifficultyCount;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _currentQuestionID;
        private List<Question> _questions;

        private string _currentAnswer;

        private string _nextButtonText = LOAD_QUIZ;
        public string NextButtonText
        {
            get => _nextButtonText;
            set => SetField(ref _nextButtonText, value, nameof(NextButtonText));
        }

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

        public bool _jokersEnabled = false;
        public bool JokersEnabled
        {
            get => _jokersEnabled;
            set => SetField(ref _jokersEnabled, value, nameof(JokersEnabled));
        }
        public string FiftyFifty => "50/50";

        public string _resultText;
        public string ResultText
        {
            get => _resultText;
            set => SetField(ref _resultText, value, nameof(ResultText));
        }

        public MainWindow()
        {
            _questions = new List<Question>();
            InitializeComponent();
            this.DataContext = this;

            _correctByDifficultyCount = new Dictionary<int, int>()
            {
                {1,0},{2,0},{3,0},{4,0},{5,0}
            };
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

        private void LoadQuiz()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "csv files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _questions.Clear();
                    using (var reader = new StreamReader(openFileDialog.FileName, System.Text.Encoding.UTF8))
                    {
                        reader.ReadLine();
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(';');

                            Question question = new Question(values[0], values[1], new string[] { values[2], values[3], values[4] }, int.Parse(values[5]));
                            _questions.Add(question);
                        }
                    }
                    _questions = _questions.OrderBy((question) => question.DifficultyText).ToList();

                    JokersEnabled = true;

                    SetQuestionID(0);
                }
            }
        }

        private void SetQuestionID(int questionID)
        {
            _currentAnswer = null;
            next.IsEnabled = false;
            NextButtonText = CONFIRM;

            cover1.Visibility = Visibility.Visible;
            cover2.Visibility = Visibility.Visible;
            cover3.Visibility = Visibility.Visible;
            cover4.Visibility = Visibility.Visible;

            answer1.IsEnabled = true;
            answer2.IsEnabled = true;
            answer3.IsEnabled = true;
            answer4.IsEnabled = true;

            ResetButtonColors();

            _currentQuestionID = questionID;
            CurrentQuestion = _questions[_currentQuestionID];
            QuestionCount = $"Frage {_currentQuestionID + 1} / {_questions.Count}";
        }

        private void ResetButtonColors()
        {
            answer1.Background = Brushes.LightGray;
            answer2.Background = Brushes.LightGray;
            answer3.Background = Brushes.LightGray;
            answer4.Background = Brushes.LightGray;
        }

        private void EvaluateAnswer()
        {
            if (CurrentQuestion.IsCorrect(_currentAnswer))
            {
                _correctCount++;
                _correctByDifficultyCount[CurrentQuestion.Difficulty]++;
            }

            cover1.Visibility = Visibility.Hidden;
            cover2.Visibility = Visibility.Hidden;
            cover3.Visibility = Visibility.Hidden;
            cover4.Visibility = Visibility.Hidden;

            SetAnswerColor(answer1);
            SetAnswerColor(answer2);
            SetAnswerColor(answer3);
            SetAnswerColor(answer4);

            NextButtonText = NEXT;
            if (_currentQuestionID + 1 >= _questions.Count)
            {
                NextButtonText = RESULT;
            }
        }

        private void SetAnswerColor(System.Windows.Controls.Button answer)
        {
            if (answer.Content.ToString() == _currentAnswer)
            {
                answer.Background = Brushes.Red;
            }

            if (CurrentQuestion.IsCorrect(answer.Content.ToString()))
            {
                answer.Background = Brushes.Green;
            }
        }

        private void DisableTwoAnwsers()
        {
            List<System.Windows.Controls.Button> answers = new List<System.Windows.Controls.Button>() { answer1, answer2, answer3, answer4 };

            int removedAnswers = 0;
            while (removedAnswers < 2)
            {
                int randomNumber = rng.Next(4);
                if (!CurrentQuestion.IsCorrect(answers[randomNumber].Content.ToString()))
                {
                    answers[randomNumber].IsEnabled = false;
                    removedAnswers++;
                }
            }
        }

        private void ShowResult()
        {
            ResultText =
                $"Richtige Schwierigkeit 1 Antworten: {_correctByDifficultyCount[1]}/{_questions.Count(question => question.Difficulty == 1)}\r" +
                $"Richtige Schwierigkeit 2 Antworten: {_correctByDifficultyCount[2]}/{_questions.Count(question => question.Difficulty == 2)}\r" +
                $"Richtige Schwierigkeit 3 Antworten: {_correctByDifficultyCount[3]}/{_questions.Count(question => question.Difficulty == 3)}\r" +
                $"Richtige Schwierigkeit 4 Antworten: {_correctByDifficultyCount[4]}/{_questions.Count(question => question.Difficulty == 4)}\r" +
                $"Richtige Schwierigkeit 5 Antworten: {_correctByDifficultyCount[5]}/{_questions.Count(question => question.Difficulty == 5)}\r" +
                $"Richtige Antworten: {_correctCount}/{_questions.Count}";
            result.Visibility = Visibility.Visible;
        }

        private void Cover_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button cover = (System.Windows.Controls.Button)sender;
            cover.Visibility = Visibility.Hidden;
        }

        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (NextButtonText != NEXT)
            {
                ResetButtonColors();

                System.Windows.Controls.Button answerButton = (System.Windows.Controls.Button)sender;
                answerButton.Background = Brushes.SkyBlue;

                _currentAnswer = answerButton.Content.ToString();

                next.IsEnabled = true;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (NextButtonText)
            {
                case LOAD_QUIZ:
                    LoadQuiz();
                    break;
                case CONFIRM:
                    EvaluateAnswer();
                    break;
                case NEXT:
                    SetQuestionID(_currentQuestionID + 1);
                    break;
                case RESULT:
                    ShowResult();
                    break;
            }
        }

        private void Joker_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button joker = (System.Windows.Controls.Button)sender;

            joker.IsEnabled = false;

            if (joker.Content?.ToString() == FiftyFifty)
            {
                DisableTwoAnwsers();
            }
        }
    }
}
