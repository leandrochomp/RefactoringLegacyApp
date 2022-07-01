using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.CreditProviders
{
    public class CreditProviderFactory
    {
        private readonly IReadOnlyDictionary<string, ICreditProvider> _creditProviders;

        public CreditProviderFactory(ICandidateCreditService candidateCreditService)
        {
            var creditProviderType = typeof(ICreditProvider);
            _creditProviders = creditProviderType.Assembly.ExportedTypes
                .Where(x => creditProviderType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x =>
                {
                    var parameterlessCtor = x.GetConstructors().SingleOrDefault(c => c.GetParameters().Length == 0);
                    return parameterlessCtor is not null ? Activator.CreateInstance(x) : Activator.CreateInstance(x, candidateCreditService);
                })
                .Cast<ICreditProvider>()
                .ToImmutableDictionary(x => x.NameRequirement, x => x);
        }

        public ICreditProvider GetProviderByPositionName(string positionName)
        {
            var provider = _creditProviders.GetValueOrDefault(positionName);
            return provider ?? DefaultCreditProvider();
        }

        private ICreditProvider DefaultCreditProvider()
        {
            return _creditProviders[string.Empty];
        }
    }
}
