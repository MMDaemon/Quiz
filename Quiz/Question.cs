﻿using System.Collections.Generic;

namespace Quiz
{
    public class Question
    {

        private string _correctAnswer;
        public string Text { get; }
        public IEnumerable<string> Answers { get; }
        public string Difficulty { get; }

        public Question(string text, string correctAnswer, IEnumerable<string> otherAnswers, int difficulty)
        {
            Text = text;

            _correctAnswer = correctAnswer;

            List<string> answers = new List<string>();
            answers.Add(correctAnswer);
            answers.AddRange(otherAnswers);
            answers.Shuffle();
            Answers = answers;

            Difficulty = $"Schwierigkeitsgrad {difficulty} / 5";
        }

        public bool IsCorrect(string answer)
        {
            return answer.Equals(_correctAnswer);
        }

    }
}
