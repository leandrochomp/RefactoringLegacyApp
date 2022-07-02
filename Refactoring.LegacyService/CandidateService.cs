using System;
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
        private readonly ICandidateCreditService _candidateCreditService;
        private readonly ICandidateDataAccess _candidateDataAccess;
        private readonly CandidateValidator _candidateValidator;

        public CandidateService(
            IPositionRepository positionRepository,
            ICandidateCreditService candidateCreditService,
            ICandidateDataAccess candidateDataAccess,
            CandidateValidator candidateValidator)
        {
            _positionRepository = positionRepository;
            _candidateCreditService = candidateCreditService;
            _candidateDataAccess = candidateDataAccess;
            _candidateValidator = candidateValidator;
        }

        public CandidateService() :
            this(new PositionRepository(),
                 new CandidateCreditServiceClient(),
                 new CandidateDataAccessProxy(),
                 new CandidateValidator(new DateTimeProvider()))
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

            if (position.Name == "SecuritySpecialist")
            {
                // No credit check
                candidate.RequireCreditCheck = false;

            }
            else if (position.Name == "FeatureDeveloper")
            {
                // Do credit check and double credit
                candidate.RequireCreditCheck = true;
                var credit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
                credit *= 2;
                candidate.Credit = credit;
            }
            else
            {
                // Do credit check
                candidate.RequireCreditCheck = true;
                var credit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
                candidate.Credit = credit;
            }

            if (CandidateValidator.HasCreditCheckAndLimitIsLess500(candidate))
            {
                return false;
            }

            _candidateDataAccess.AddCandidate(candidate);
            return true;
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
