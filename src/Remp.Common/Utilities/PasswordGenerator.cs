using System.Security.Cryptography;

namespace Remp.Common.Utilities;

public static class PasswordGenerator
{
    // Generate a strong random password: length >= 12 recommended
    public static string Generate(int length = 12)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$?*";
        Span<char> buffer = stackalloc char[length];

        for (int i = 0; i < length; i++)
        {
            buffer[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }

        // Ensure complexity by appending required categories if you want:
        // (Keep simple here; Identity options may enforce more)
        return new string(buffer);
    }
}