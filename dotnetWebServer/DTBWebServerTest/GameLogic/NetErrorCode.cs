namespace DTBWebServer.GameLogic
{
    public class ResultBase
    {
        public int nErrorCode { get; set; }
        public string errorMsg { get; set; }
    }

    public static class NetErrorCode
    {
        public const int Success = 0;
        public const int Error = 1;
        
        //---具体的错误码
        public const int UserNotExist = 2;
        public const int DbOpError = 3;
        public const int ParamError = 4;
        public const int ServerInnerEexception = 5;
        public const int PeopleLimitError = 6;
        public const int ActivityNoOpen = 7;
    }
}
