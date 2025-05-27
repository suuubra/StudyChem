using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace StudyChem.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public List<UserResult> Results { get; set; } = new List<UserResult>();

        public bool VerifyPassword(string password)
        {
            return PasswordHash == Hash(password);
        }

        public void Save()
        {
            string path = $"Data/users/{Username}.json";
            Directory.CreateDirectory("Data/users");
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static User Load(string username)
        {
            string path = $"Data/users/{username}.json";
            if (!File.Exists(path)) return null;
            return JsonConvert.DeserializeObject<User>(File.ReadAllText(path));
        }

        public static string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }

    public class UserResult
    {
        public DateTime Timestamp { get; set; }
        public double Score { get; set; }
    }
}
