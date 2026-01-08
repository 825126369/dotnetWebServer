namespace GameLogic
{
    public class AuthenticationResultData
    {
        public int nErrorCode { get; set; }
        public string token { get; set; }
        public ulong expires { get; set; }
    }
}
