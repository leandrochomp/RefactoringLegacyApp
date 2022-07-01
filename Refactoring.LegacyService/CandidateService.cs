using System;
using Refactoring.LegacyService.CreditProviders;
using Refactoring.LegacyService.DataAccess;
using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Repositories;
using Refactoring.LegacyService.Services;
using Refactoring.LegacyService.Validators;

namespace Refactoring.LegacyService
{
    public class CandidateService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ICandidateDataAccess _candidateDataAccess;
        private readonly CandidateValidator _candidateValidator;
        private readonly CreditProviderFactory _creditProviderFactory;

        public CandidateService(
            IPositionRepository positionRepository,
            ICandidateDataAccess candidateDataAccess,
            CandidateValidator candidateValidator,
            CreditProviderFactory creditProviderFactory)
        {
            _positionRepository = positionRepository;
            _candidateDataAccess = candidateDataAccess;
            _candidateValidator = candidateValidator;
            _creditProviderFactory = creditProviderFactory;
        }

        public CandidateService() :
            this(new PositionRepository(),
                 new CandidateDataAccessProxy(),
                 new CandidateValidator(new DateTimeProvider()),
                 new CreditProviderFactory(new CandidateCreditServiceClient()))
        {
        }

        public bool AddCandidate(string firname, string surname, string email, DateTime dateOfBirth, int positionId)
        {
            if (!CandidateProvidedDataIsValid(firname, surname, email, dateOfBirth))
            {
                return false;
            }

            var position = _positionRepository.GetById(positionId);

            var candidate = new Candidate
            {
                Position = position,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            ApplyCredits(position, candidate);

            if (CandidateValidator.HasCreditCheckAndLimitIsLess500(candidate))
            {
                return false;
            }

            _candidateDataAccess.AddCandidate(candidate);
            return true;
        }

        private void ApplyCredits(Position position, Candidate candidate)
        {
            var provider = _creditProviderFactory.GetProviderByPositionName(position.Name);
            var (requireCreditCheck, credit) = provider.GetCredits(candidate);
            candidate.RequireCreditCheck = requireCreditCheck;
            candidate.Credit = credit;
        }

        private bool CandidateProvidedDataIsValid(string firname, string surname, string email, DateTime dateOfBirth)
        {
            if (!CandidateValidator.HasValidFullName(firname, surname))
            {
                return false;
            }

            if (!CandidateValidator.HasValidEmail(email))
            {
                return false;
            }

            if (!_candidateValidator.IsAtLeast21YearsOld(dateOfBirth))
            {
                return false;
            }

            return true;
        }
    }
}
