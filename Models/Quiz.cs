
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace StudyChem.Models
{
    /// <summary>
    /// Quizzing System, using Newtonsoft.Json to deserializeobject
    /// </summary>
    public class Quiz
    {
        //Title, Questions
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        //Quiz loading system
        public static Quiz LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var questions = JsonConvert.DeserializeObject<List<Question>>(json);
            return new Quiz { Title = Path.GetFileNameWithoutExtension(filePath), Questions = questions };
        }
        //Quiz folder setup
        //Plus Preloaded quizzes.
        public static Dictionary<string, Quiz> LoadAllPreloadedQuizzes()
        {
            var quizzes = new Dictionary<string, Quiz>();
            string[] quizFiles = Directory.GetFiles(AppConstants.QuizDataFolder, "*.json");
            foreach (var file in quizFiles)
            {
                var quiz = LoadFromFile(file);
                quizzes[quiz.Title] = quiz;
            }
            return quizzes;
        }
        /// <summary>
        /// Shuffles the questions
        /// </summary>
        public void Shuffle()
        {
            var rnd = new Random();
            Questions = Questions.OrderBy(q => rnd.Next()).ToList();
        }
    }
}
