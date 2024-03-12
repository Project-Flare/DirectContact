using System.Text.RegularExpressions;
using Zxcvbn;

namespace DirectContactClient.Backend
{
    internal static class RegistrationManager
    {
        public enum PasswordStrength
        {
            None, // interpret None as invalid password
            Unacceptable,
            Weak,
            Good,
            Excellent
        }

        public static PasswordStrength EvaluatePassword(string userPassword)
        {
            // Just invalid input
            if (string.IsNullOrEmpty(userPassword) || string.IsNullOrWhiteSpace(userPassword))
                return PasswordStrength.None;

            // Entropy is estimated using Dan Wheeler's zxcvbn algorithm.
            int entropy = (int)Math.Log2(
                Core.EvaluatePassword(userPassword).Guesses
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

            // (90, +inf) - excellent
            return PasswordStrength.Excellent;
        }

        public static bool IsUserNameValid(string userName)
        {
            // Just to be safe
            if (string.IsNullOrEmpty(userName))
                return false;

            // Contains alphanumerical symbols ONLY
            Regex regex = new Regex(@"^([a-z]|[A-Z]|[0-9])*$");
            if (!regex.IsMatch(userName))
                return false;

            // 1 symbol at least
            regex = new Regex(@"[a-z]|[A-Z]");
            if (!regex.Match(userName).Success)
                return false;

            // 32 symbols maximum
            if (userName.Count() > 32)
                return false;

            // User name matches all the requirements (can be sent to check for server)
            return true;
        }
    }
}
