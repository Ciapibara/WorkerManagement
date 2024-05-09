using System.Text;

namespace WorkerManagement.Functions
{
  internal static class PasswordGenerator
  {
    private const string allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GeneratePassword(int length)
    {
      Random random = new();
      StringBuilder password = new();

      for (int i = 0; i < length; i++)
      {
        int index = random.Next(allowedCharacters.Length);
        password.Append(allowedCharacters[index]);
      }

      return password.ToString();
    }
  }
}