using System.Collections.Generic;

namespace Quiz
{
    class Question
    {
        public string Text { get; set; }
        public IEnumerable<string> Answers { get; set; }
        public int Difficulty { get; set; }

    }
}
