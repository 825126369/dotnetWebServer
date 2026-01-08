namespace DTBWebServer.Authentication
{
    
    public class JwtConfig
    {
        //签发人
        public string Issuer { get; set; }
        //哪类消费者
        public string Audience { get; set; }
        //
        public string SigningKey { get; set; }
        public string pwd { get; set; }
    }

}
