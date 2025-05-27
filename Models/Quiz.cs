// Quiz.cs - Cleaned up to remove duplicate Question class
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace StudyChem.Models
{
    public class Quiz
    {
        public string Title { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public static Quiz LoadFromFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var questions = JsonConvert.DeserializeObject<List<Question>>(json);
            return new Quiz { Title = Path.GetFileNameWithoutExtension(filePath), Questions = questions };
        }

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

        public void Shuffle()
        {
            var rnd = new Random();
            Questions = Questions.OrderBy(q => rnd.Next()).ToList();
        }
    }
}
