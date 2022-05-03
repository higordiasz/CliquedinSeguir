namespace CliquedinSeguir.Models.Proxy
{
    public class Proxy
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }

        /// <summary>
        /// Create new instace of proxy models
        /// </summary>
        /// <param name="ip">Proxy ip</param>
        /// <param name="port">Proxy port</param>
        /// <param name="user">Proxy username</param>
        /// <param name="pass">Proxy password</param>
        public Proxy(string ip, string port, string user, string pass)
        {
            IP = ip;
            Port = port;
            User = user;
            Pass = pass;
        }
    }
}
