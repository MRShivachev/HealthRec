namespace HealthRec.Services.Common.Contracts;

public interface ISecurityCodeGenerator
{
    public string GenerateUniqueSecurityCode();
}