using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Refactoring.LegacyService.Services
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    [ServiceContract(ConfigurationName = "LegacyApp.ICandidateTestService")]
    public interface ICandidateCreditService
    {
        [OperationContract(Action = "http://xxx.com/ICandidateTestService/GetCredit")]
        int GetCredit(string firstname, string surname, DateTime dateOfBirth);
    }

    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public interface ICandidateCreditServiceChannel : ICandidateCreditService, IClientChannel
    {
    }

    [DebuggerStepThrough]
    [GeneratedCode("System.ServiceModel", "4.0.0.0")]
    public partial class CandidateCreditServiceClient : ClientBase<ICandidateCreditService>, ICandidateCreditService
    {
        private ICandidateCreditServiceChannel _candidateTestServiceChannelImplementation;
        public CandidateCreditServiceClient() { }

        public CandidateCreditServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        { }

        public CandidateCreditServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        { }

        public CandidateCreditServiceClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        { }

        public int GetCredit(string firstname, string surname, DateTime dateOfBirth)
        {
            return base.Channel.GetCredit(firstname, surname, dateOfBirth);
        }
    }
}
