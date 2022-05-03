using CliquedinAPI;
using CliquedinAPI.Controllers;
using CliquedinSeguir.Models.Proxy;
using System;
using System.Collections.Generic;
using System.IO;

namespace CliquedinSeguir.Helpers.Proxy
{
    static public class ProxyHelper
    {
        static public Models.Proxy.Proxy LoadProxyFromFile()
        {
            string dir = Directory.GetCurrentDirectory();
            if (File.Exists($@"{dir}\Proxy\proxy.txt"))
            {
                string[] linhas = File.ReadAllLines($@"{dir}\Proxy\proxy.txt");
                if (linhas.Length == 4)
                {
                    Models.Proxy.Proxy ret = new(linhas[0], linhas[1], linhas[2], linhas[3]);
                    return ret;
                }
                return null;
            }
            return null;
        }

        static public Models.Proxy.Proxy LoadProxyFromCliquedin(Cliquedin Plat)
        {
            List<string> proxy = Plat.GetProxy().Result;
            var array = proxy.ToArray();
            if (proxy.Count > 1)
            {
                Models.Proxy.Proxy ret = new(array[0], array[1], array[2], array[3]);
                return ret;
            }
            return null;
        }
    }
}
