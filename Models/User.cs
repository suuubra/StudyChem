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

        public void Save()
        {
            Directory.CreateDirectory(AppConstants.UserDataFolder);
            string path = Path.Combine(AppConstants.UserDataFolder, Username + ".json");
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static User Load(string username)
        {
            string path = Path.Combine(AppConstants.UserDataFolder, username + ".json");
            if (!File.Exists(path)) return null;
            return JsonConvert.DeserializeObject<User>(File.ReadAllText(path));
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == Hash(password);
        }

        public static string Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }

    public class UserResult
    {
        public double Score { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
