using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.DataAccess
{
    public static class CandidateDataAccess
    {
        public static void AddCandidate(Candidate candidate)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["applicationDatabase"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "uspAddCandidate"
                };

                var firstNameParameter = new SqlParameter("@Firstname", SqlDbType.VarChar, 50) { Value = candidate.Firstname };
                command.Parameters.Add(firstNameParameter);
                var surnameParameter = new SqlParameter("@Surname", SqlDbType.VarChar, 50) { Value = candidate.Surname };
                command.Parameters.Add(surnameParameter);
                var dateOfBirthParameter = new SqlParameter("@DateOfBirth", SqlDbType.DateTime) { Value = candidate.DateOfBirth };
                command.Parameters.Add(dateOfBirthParameter);
                var emailAddressParameter = new SqlParameter("@EmailAddress", SqlDbType.VarChar, 50) { Value = candidate.EmailAddress };
                command.Parameters.Add(emailAddressParameter);
                var requireCreditCheckParameter = new SqlParameter("@RequireCreditCheck", SqlDbType.Bit) { Value = candidate.RequireCreditCheck };
                command.Parameters.Add(requireCreditCheckParameter);
                var creditParameter = new SqlParameter("@Credit", SqlDbType.Int) { Value = candidate.Credit };
                command.Parameters.Add(creditParameter);
                var positionIdParameter = new SqlParameter("@PositionId", SqlDbType.Int) { Value = candidate.Position.Id };
                command.Parameters.Add(positionIdParameter);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
