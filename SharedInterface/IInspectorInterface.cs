using System.ServiceModel;

namespace SharedInterface
{
    [ServiceContract]
    public interface IInspectorInterface
    {
        [OperationContract]
        string GetFullSummary();

        [OperationContract]
        void StartInspection();

        [OperationContract]
        void FinishInspection();
    }
}