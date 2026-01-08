using DTBWebServer.ExcelCS;
using DTBWebServer.GameLogic;

namespace DTBWebServer.Authentication
{
    public class AuthenticationResultData : ResultBase
    {
        public string token { get; set; }
        public ulong expires { get; set; }
    }

}
