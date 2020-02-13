
namespace SharedInterface
{
    [System.ServiceModel.ServiceContract]
    public interface ICustomerInterface
    {
        [System.ServiceModel.OperationContract]
        uint? OpenAccount(string firstName, string lastName, float debtLimit);

        [System.ServiceModel.OperationContract]
        float Withdraw(uint account, float amount);

        [System.ServiceModel.OperationContract]
        void Deposit(uint account, float amount);

        [System.ServiceModel.OperationContract]
        string GetHistory(uint account);

        [System.ServiceModel.OperationContract]
        bool CloseAccount(uint account);
    }
}