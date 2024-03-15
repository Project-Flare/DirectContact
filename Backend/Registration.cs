using Flare;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zxcvbn;
using static Flare.ServerMessage;

namespace Backend
{
    public enum PasswordStrength
    {
        None, // interpret None as invalid password
        Unacceptable,
        Weak,
        Good,
        Excellent
    }

    public class Registration
    {
        public string Username { get => _username; }
        public string Password { get => _password; }
        
        private string _username;
        private string _password;

        public Registration()
        {
            _username = string.Empty;
            _password = string.Empty;
        }

        public bool TrySetPassword(string password)
        {
            var evaluation = EvaluatePassword(password);
            switch (evaluation)
            {
                case PasswordStrength.None:
                    return false;
                case PasswordStrength.Unacceptable:
                    return false;
                case PasswordStrength.Weak:
                    return false;
                case PasswordStrength.Good:
                    this._password = password;
                    return true;
                case PasswordStrength.Excellent:
                    this._password = password;
                    return true;
                default:
                    return false;
            }
        }

        public PasswordStrength EvaluatePassword(string password)
        {
            // just invalid input
            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
                return PasswordStrength.None;

            // Entropy is estimated using Dan Wheeler's zxcvbn algorithm.
            int entropy = (int)Math.Log2(
                Core.EvaluatePassword(password).Guesses
                );

            // [0;50] - unacceptable
            if (entropy <= 50)
                return PasswordStrength.Unacceptable;

            // (50; 70] - weak
            if (entropy <= 70)
                return PasswordStrength.Weak;

            // (70, 90] - good
            if (entropy <= 90)
                return PasswordStrength.Good;

            return PasswordStrength.Excellent;
        }

        public bool TrySetUsername(string username)
        {
            if (!UsernameValid(username))
                return false;

            this._username = username;
            return true;
        }

        public bool UsernameValid(string username)
        {
            // Just to be safe
            if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                return false;

            // Username must contain only ASCII characters
            if (!ContainsOnlyAscii(username))
                return false;

            // Must contain alphanumerical symbols only
            Regex regex = new Regex(@"^[\d\w]{1,32}$", RegexOptions.IgnoreCase);
            if (!regex.IsMatch(username))
                return false;

            return true;
        }

        public bool ContainsOnlyAscii(string str)
        {
            // C# moment
            foreach (char c in str)
                if (!char.IsAscii(c))
                    return false;
            return true;
        }

        public RegisterRequest? FormRegistrationRequest()
        {
            // Don't send request if username or password is not set
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
                return null;

            // Form register request protobuf
            RegisterRequest request = new RegisterRequest
            {
                Username = _username,
                Password = _password
            };

            return request;
        }
    }
}
