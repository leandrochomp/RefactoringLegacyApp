using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.CreditProviders
{
    public class SecuritySpecialistCreditProvider : ICreditProvider
    {
        public (bool RequireCreditCheck, int Credit) GetCredits(Candidate candidate)
        {
            return (false, 0);
        }

        public string NameRequirement { get; } = "SecuritySpecialist";
    }
}
