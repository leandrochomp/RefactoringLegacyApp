using System;
using Refactoring.LegacyService.DataAccess;
using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Repositories;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService
{
    public class CandidateService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPositionRepository _positionRepository;
        private readonly ICandidateCreditService _candidateCreditService;
        private readonly ICandidateDataAccess _candidateDataAccess;

        public CandidateService(
            IDateTimeProvider dateTimeProvider,
            IPositionRepository positionRepository,
            ICandidateCreditService candidateCreditService,
            ICandidateDataAccess candidateDataAccess)
        {
            _dateTimeProvider = dateTimeProvider;
            _positionRepository = positionRepository;
            _candidateCreditService = candidateCreditService;
            _candidateDataAccess = candidateDataAccess;
        }

        public CandidateService() :
            this(new DateTimeProvider(),
                 new PositionRepository(),
                 new CandidateCreditServiceClient(),
                 new CandidateDataAccessProxy())
        {
        }

        public bool AddCandidate(string firstName, string surName, string email, DateTime dateOfBirth, int positionId)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surName))
            {
                return false;
            }

            if (!email.Contains("@") || !email.Contains("."))
            {
                return false;
            }

            var now = _dateTimeProvider.DateTimeNow;
            int age = now.Year - dateOfBirth.Year;

            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
            {
                age--;
            }

            if (age < 18)
            {
                return false;
            }

            var position = _positionRepository.GetById(positionId);

            var candidate = new Candidate
            {
                Position = position,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firstName,
                Surname = surName
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

            if (candidate.RequireCreditCheck && candidate.Credit < 500)
            {
                return false;
            }

            _candidateDataAccess.AddCandidate(candidate);

            return true;
        }
    }
}
