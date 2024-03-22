using System.Security.Cryptography;

namespace CommyAPI.Authentication
{
    public static class AuthGeneral
    {
        public const string salt = "DqIhIveCRFtS_Ra40L4_NqV6prnndMIRTptHjokv";
        public static string GenerateSaltedHash(string password)
        {
            var sha = SHA256.Create();
            string? hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password + salt)).ToString();
            if (hash == null) throw new AuthGeneralException("ハッシュ値の計算に失敗しました。");
            return hash;
        }

        public static bool Authenticate(string sourceHash, string password)
        {
            var sha = SHA256.Create();
            string? hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(salt + password + salt)).ToString();
            if (hash == null) throw new AuthGeneralException("ハッシュ値の計算に失敗しました。");
            return (sourceHash == hash);
        }
    }

    public class AuthGeneralException : Exception
    {
        public AuthGeneralException(string? message) : base(message)
        {
        }
    }
}
