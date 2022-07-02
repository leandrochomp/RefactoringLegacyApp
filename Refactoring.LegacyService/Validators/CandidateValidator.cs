using System;
using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.Validators
{
    public class CandidateValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public CandidateValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public static bool HasCreditCheckAndLimitIsLess500(Candidate candidate)
        {
            return candidate.RequireCreditCheck && candidate.Credit < 500;
        }

        public static bool HasValidEmail(string email)
        {
            return email.Contains("@") || email.Contains(".");
        }

        public static bool HasValidFullName(string firname, string surname)
        {
            return !string.IsNullOrEmpty(firname) && !string.IsNullOrEmpty(surname);
        }

        public bool IsAtLeast21YearsOld(DateTime dateOfBirth)
        {
            var now = _dateTimeProvider.DateTimeNow;
            var age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            return age >= 21;
        }
    }
}
