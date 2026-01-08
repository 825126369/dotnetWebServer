namespace DTBWebServer.DataBase
{
    public class DataBaseConnectConfig
    {
        public string server { get; set; }
        public string port { get; set; }
        //
        public string user { get; set; }
        public string password { get; set; }
        public string database { get; set; }

        public string GetConnectStr()
        {
            return $"server={server};port={port};user={user};password={password};database={database}";
        }
    }
}
