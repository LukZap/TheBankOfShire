namespace ShireBank.Repos
{
    public struct ConcurentResult
    {
        public float? BalanceChange { get; set; }
        public bool AccountWasDeleted { get; set; }
        public bool AccountBalanceWasUpdated { get; set; }
    }
}
