using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.CreditProviders
{
    public interface ICreditProvider
    {
        (bool RequireCreditCheck, int Credit) GetCredits(Candidate candidate);

        public string NameRequirement { get; }
    }
}
