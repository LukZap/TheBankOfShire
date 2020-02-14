namespace SharedInterface
{
    public class Constants
    {
        public const string BankBaseAddress = "http://localhost:6999";
        public const string ServiceName = "ShireBank";
        public const string InspectorServiceEndpoint = "Inspector";

        public static string FullBankAddress => BankBaseAddress + "/" + ServiceName;
        public static string FullInspectorAddress => BankBaseAddress + "/" + InspectorServiceEndpoint;

    }
}