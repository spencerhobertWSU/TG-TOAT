using System.Security.Cryptography;

namespace MvcMovie.Helpers;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
    private static char Delimiter = ';';

    /// <summary>
    /// Hashes the password.
    /// </summary>
    /// <param name="password">The password to be hashed</param>
    /// <returns>The hashed password</returns>
    string IPasswordHasher.Hash(string password)
    {
        // Grab the salt and hash
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, _hashAlgorithmName, KeySize);
        
        // Return them combined (with a delimiter)
        return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    /// <summary>
    /// Checks if the password input, during login, is the same as the stored hashed password.
    /// </summary>
    /// <param name="passwordHash">The hashed password stored in the database</param>
    /// <param name="inputPassword">The password that was input during login</param>
    /// <returns>If the input password is the same as the hashed password in the database</returns>
    /// <exception cref="NotImplementedException"></exception>
    bool IPasswordHasher.Verify(string passwordHash, string inputPassword)
    {
        // Grab the salt and hash from the hashed password
        var elements = passwordHash.Split(Delimiter);
        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);

        // Hash the input password
        var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iterations, _hashAlgorithmName, KeySize);

        // Return whether they're equal
        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}
