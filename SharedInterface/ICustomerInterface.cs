using System.ServiceModel;

namespace SharedInterface
{
    [ServiceContract]
    public interface ICustomerInterface
    {
        [OperationContract]
        uint? OpenAccount(string firstName, string lastName, float debtLimit);

        [OperationContract]
        float Withdraw(uint account, float amount);

        [OperationContract]
        void Deposit(uint account, float amount);

        [OperationContract]
        string GetHistory(uint account);

        [OperationContract]
        bool CloseAccount(uint account);
    }
}