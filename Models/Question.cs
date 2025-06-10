// Question.cs - Centralized class definition
using System;
using System.Collections.Generic;

namespace StudyChem.Models
{
    public class Question
    {
        /// <summary>
        /// Question system information.
        /// </summary>
        public string Prompt { get; set; }
        public string Answer { get; set; }
        public int Points { get; set; } = 1;
        public List<string> Options { get; set; } = new List<string>();
        public string Type { get; set; } = "MCQ";

        //Checks the selected Quiz.
        public bool Check(string input)
        {
            return input.Trim().Equals(Answer.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
