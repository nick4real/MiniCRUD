namespace MiniCRUD.WebApp.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        // Simple hash for demonstration purposes only. Use a secure hashing algorithm in production.
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
    }
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return HashPassword(password) == hashedPassword;
    }
}
