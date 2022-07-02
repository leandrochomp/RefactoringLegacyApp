using System;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Refactoring.LegacyService.DataAccess;
using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Repositories;
using Refactoring.LegacyService.Services;
using Refactoring.LegacyService.Validators;
using Xunit;

namespace Refactoring.LegacyService.Tests
{
    public class CandidateServiceTests
    {
        private readonly CandidateService _sut;
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private readonly IPositionRepository _positionRepository = Substitute.For<IPositionRepository>();
        private readonly ICandidateDataAccess _candidateDataAccess = Substitute.For<ICandidateDataAccess>();
        private readonly ICandidateCreditService _candidateCreditService = Substitute.For<ICandidateCreditService>();
        private readonly IFixture _fixture = new Fixture();

        public CandidateServiceTests()
        {
            _sut = new CandidateService(_positionRepository,
                                        _candidateCreditService,
                                        _candidateDataAccess, new CandidateValidator(_dateTimeProvider));
        }
        [Fact]
        public void AddCandidate_ShouldCreateCandidate_WhenAllParametersAreValid()
        {
            //Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string surName = "Sinclair";
            const string email = "bob.sinclair@gmail.com";
            var dateOfBirth = new DateTime(1991, 08, 07);
            var position = ReturnNewPosition(id);

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2022, 07, 01));
            _positionRepository.GetById(id).Returns(position);
            _candidateCreditService.GetCredit(firstName, surName, dateOfBirth).Returns(600);

            //Act
            var result = _sut.AddCandidate(firstName, surName, email, dateOfBirth, id);

            //Assert
            result.Should().BeTrue();
            _candidateDataAccess.Received(1).AddCandidate(Arg.Any<Candidate>());

        }

        [Theory]
        [InlineData("", "Sinclair", "bob.sinclair@gmail.com", 1991)]
        [InlineData("Bob", "", "bob.sinclair@gmail.com", 1991)]
        [InlineData("", "Sinclair", "bobcom", 1991)]
        [InlineData("", "Sinclair", "bob.sinclair@gmail.com", 2003)]
        public void AddCandidate_ShouldNotCreateCandidate_WhenInputDetailsAreInvalid(string firstName, string surname, string email, int yearOfBirth)
        {
            //Arrange
            const int id = 1;
            var dateOfBirth = new DateTime(yearOfBirth, 1, 1);
            var position = ReturnNewPosition(id);

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 02, 16));
            _positionRepository.GetById(Arg.Is(id)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(surname), Arg.Is(dateOfBirth)).Returns(600);

            //Act
            var result = _sut.AddCandidate(firstName, surname, email, dateOfBirth, id);

            //Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("RandomCandidateName", true, 600, 600)]
        [InlineData("FeatureDeveloper", true, 600, 1200)]
        [InlineData("SecuritySpecialist", false, 0, 0)]
        public void AddCandidate_ShouldCreateCandidateWithCorrectCreditLimit_WhenNameIndicatesDifferentClassification(string candidateName, bool hasCreditLimit, int initialCreditLimit, int finalCreditLimit)
        {
            //Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string lastName = "Sinclair";
            const string email = "bob.sinclair@gmail.com";
            var dateOfBirth = new DateTime(1991, 08, 07);
            var position = _fixture.Build<Position>()
                .With(p => p.Id, id)
                .With(p => p.Name, candidateName)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 2, 16));
            _positionRepository.GetById(Arg.Is(id)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(initialCreditLimit);

            // Act
            var result = _sut.AddCandidate(firstName, lastName, email, dateOfBirth, id);

            //Assert
            result.Should().BeTrue();
            _candidateDataAccess.Received()
                .AddCandidate(Arg.Is<Candidate>(candidate => candidate.RequireCreditCheck == hasCreditLimit && candidate.Credit == finalCreditLimit));
        }

        [Fact]
        public void AddCandidate_ShouldNotCreateCandidate_WhenCandidateHasCreditLimitAndCreditLimitIsLessThan500()
        {
            //Arrange
            const int id = 1;
            const string firstName = "Bob";
            const string surName = "Sinclair";
            const string email = "bob.sinclair@gmail.com";
            var dateOfBirth = new DateTime(1991, 08, 07);
            var position = ReturnNewPosition(id);

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 02, 16));
            _positionRepository.GetById(Arg.Is(id)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(surName), Arg.Is(dateOfBirth)).Returns(499);

            //Fact
            var result = _sut.AddCandidate(firstName, surName, email, dateOfBirth, id);

            //Assert
            result.Should().BeFalse();
        }

        private Position ReturnNewPosition(int id)
        {
            return _fixture.Build<Position>().With(p => p.Id, () => id).Create();
        }
    }
}
