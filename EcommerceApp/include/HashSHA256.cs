using System;
using System.Security.Cryptography;
using System.Text;

namespace EcommerceApp {
    public static class HashSHA256 {
        // method that returns a password hashed in SHA256
        // used in login and signup
        public static string HashPassword(string password) {
            using (var sha256 = SHA256.Create()) {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

    }
}