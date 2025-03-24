using System.Security.Cryptography;
using HealthRec.Data;
using HealthRec.Services.Common.Contracts;

namespace HealthRec.Services.Common.Internals;

internal class SecurityCodeCodeGenerator : ISecurityCodeGenerator
{
    private readonly HealthRecDbContext context;

    public SecurityCodeCodeGenerator(HealthRecDbContext context)
    {
        this.context = context;
    }

    public string GenerateUniqueSecurityCode()
    {
        string securityCode;
        bool isUnique;

        do
        {
            // Generate a secure random 8-digit code
            securityCode = this.GenerateRandomDigits(8);

            // Check if it's already in use
            isUnique = !this.context.Patients.Any(p => p.Code == securityCode);
        }
        while (!isUnique);

        return securityCode;
    }

    private string GenerateRandomDigits(int length)
    {
        // Use cryptographically secure random number generator
        using var rng = RandomNumberGenerator.Create();
        byte[] data = new byte[length];
        rng.GetBytes(data);

        // Convert each byte to a digit (0-9)
        // Note: We use modulo 10 to get a single digit (0-9) from each byte
        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = (char)('0' + (data[i] % 10));
        }

        return new string(chars);
    }
}