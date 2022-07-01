using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.CreditProviders
{
    public class FeatureDeveloperCreditProvider : ICreditProvider
    {
        private readonly ICandidateCreditService _candidateCreditService;

        public FeatureDeveloperCreditProvider(ICandidateCreditService candidateCreditService)
        {
            _candidateCreditService = candidateCreditService;
        }

        public (bool RequireCreditCheck, int Credit) GetCredits(Candidate candidate)
        {
            var credit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
            return (true, credit * 2);
        }

        public string NameRequirement { get; } = "FeatureDeveloper";
    }
}
