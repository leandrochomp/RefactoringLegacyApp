using System;

namespace Refactoring.LegacyService.Services
{
    public interface IDateTimeProvider
    {
        public DateTime DateTimeNow { get; }
    }
}