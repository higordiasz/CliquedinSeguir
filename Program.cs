using System;

using Instagram;

using CliquedinAPI;
using CliquedinAPI.Controllers;

using CliquedinSeguir.Models.Contas;
using CliquedinSeguir.Controllers.Ua;
using CliquedinSeguir.Controllers.Arka;

using CliquedinSeguir.Models.Proxy;
using CliquedinSeguir.Helpers.Proxy;

using System.Threading.Tasks;
using System.IO;
using CliquedinSeguir.Helpers;

namespace CliquedinSeguir
{
    class Program
    {
        static Cliquedin Plat { get; set; }
        static string EmailCLiquedin { get; set; }
        static string SenhaCliquedin { get; set; }
        static string UserAgent { get; set; }
        static Proxy proxy { get; set; }
        static async Task Main()
        {
            if (!await LicenseController.License())
            {
                Console.WriteLine("Licença de uso expirada.");
                await Task.Delay(TimeSpan.FromHours(100));
                return;
            }
            _ = LicenseController.Open();
            EmailCLiquedin = "cosme_junior16@hotmail.com";
            SenhaCliquedin = "123456";
            Plat = new(EmailCLiquedin, SenhaCliquedin);
            Console.WriteLine("Realizando login na Cliquedin...");
            var resd = await Plat.Login();
            if (resd.Status == 1)
            {
                Console.WriteLine("Login realizado...");
                while (true)
                {
                    try
                    {
                        UserAgent = uaController.GetUa();
                        Console.WriteLine("UserAgent: " + UserAgent);
                        Console.WriteLine("Buscando conta...");
                        var conta = await Plat.GetAccount();
                        BotAccounts Conta = new();
                        Conta.conta = conta.Conta;
                        bool logada = false;
                        if (String.IsNullOrEmpty(Conta.conta.Username))
                        {
                            Console.WriteLine("Não possui contas no momento...");
                            Console.WriteLine("Aguardando 5 minutos para tentar novamente...");
                            await Task.Delay(TimeSpan.FromMinutes(5));
                        }
                        else
                        {
                            Console.WriteLine("Checando se possui cookie...");
                            try
                            {
                                proxy = ProxyHelper.LoadProxyFromCliquedin(Plat);
                            }
                            catch
                            {
                                Console.WriteLine("Erro ao puxar proxy...");
                            }
                            if (HaveCookie(conta.Conta.Username))
                            {
                                Console.WriteLine("Recuperando cookie anterior...");
                                string[] data = GetSaveData(conta.Conta.Username.ToLower());
                                if (proxy == null)
                                {
                                    Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, data[0], data[1]);
                                    bool checkUserAgent = true;
                                    while (checkUserAgent)
                                    {
                                        try
                                        {
                                            i.SetuserAgent(UserAgent);
                                            checkUserAgent = false;
                                        }
                                        catch
                                        {
                                            UserAgent = uaController.GetUa();
                                        }
                                    }
                                    Conta.insta = i;
                                    logada = await Conta.IsLogged();
                                }
                                else
                                {
                                    Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, data[0], data[1], $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                    bool checkUserAgent = true;
                                    while (checkUserAgent)
                                    {
                                        try
                                        {
                                            i.SetuserAgent(UserAgent);
                                            checkUserAgent = false;
                                        }
                                        catch
                                        {
                                            UserAgent = uaController.GetUa();
                                        }
                                    }
                                    Conta.insta = i;
                                    logada = await Conta.IsLogged();
                                }
                            }
                            if (!logada)
                            {
                                bool leave = false;
                                while (!leave)
                                {
                                    Console.WriteLine("Realizando Login na conta...");
                                    Console.WriteLine($"Username: {conta.Conta.Username} | Password: {conta.Conta.Password}");
                                    if (proxy == null)
                                    {
                                        Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password);
                                        bool checkUserAgent = true;
                                        while (checkUserAgent)
                                        {
                                            try
                                            {
                                                i.SetuserAgent(UserAgent);
                                                checkUserAgent = false;
                                            }
                                            catch
                                            {
                                                UserAgent = uaController.GetUa();
                                            }
                                        }
                                        Conta.insta = i;
                                        var login = await Conta.Login(Plat);
                                        if (login.Status == 1)
                                        {
                                            Console.WriteLine("Login realizado com sucesso...");
                                            logada = true;
                                            leave = true;
                                        }
                                        else
                                        {
                                            if (login.Status == -995)
                                            {
                                                Console.WriteLine("Erro ao logar...");
                                                Console.WriteLine(login.Response);
                                                Console.WriteLine("Buscando novo proxy...");
                                                await Task.Delay(TimeSpan.FromSeconds(25));
                                                try
                                                {
                                                    proxy = ProxyHelper.LoadProxyFromCliquedin(Plat);
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("Erro ao puxar proxy...");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Erro ao logar...");
                                                Console.WriteLine(login.Response);
                                                await Task.Delay(TimeSpan.FromSeconds(15));
                                                leave = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                        bool checkUserAgent = true;
                                        while (checkUserAgent)
                                        {
                                            try
                                            {
                                                i.SetuserAgent(UserAgent);
                                                checkUserAgent = false;
                                            }
                                            catch
                                            {
                                                UserAgent = uaController.GetUa();
                                            }
                                        }
                                        Conta.insta = i;
                                        var login = await Conta.Login(Plat);
                                        if (login.Status == 1)
                                        {
                                            Console.WriteLine("Login realizado com sucesso...");
                                            logada = true;
                                            leave = true;
                                        }
                                        else
                                        {
                                            if (login.Status == -995)
                                            {
                                                Console.WriteLine("Erro ao logar...");
                                                Console.WriteLine(login.Response);
                                                Console.WriteLine("Buscando novo proxy...");
                                                await Task.Delay(TimeSpan.FromSeconds(25));
                                                try
                                                {
                                                    proxy = ProxyHelper.LoadProxyFromCliquedin(Plat);
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("Erro ao puxar proxy...");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Erro ao logar...");
                                                Console.WriteLine(login.Response);
                                                await Task.Delay(TimeSpan.FromSeconds(15));
                                                leave = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if (logada)
                            {
                                Console.WriteLine("Buscando ID da conta...");
                                var id = await Plat.GetAccountID(conta.Conta.Username);
                                if (id.Status != 1)
                                {
                                    Console.WriteLine("Não foi possivel localizar a conta na cliquedin...");
                                    Console.WriteLine("Buscando informações da conta...");
                                    var data = await Conta.GetDataFromPerfil(Plat);
                                    if (data.Status == 1)
                                    {
                                        Console.WriteLine("Sucesso ao recuperar informações...");
                                        Console.WriteLine("Registrando a conta na cliquedin...");
                                        var cad = await Plat.RegisteAccount(conta.Conta.Username, data.Gender, data.Response);
                                        if (cad)
                                        {
                                            Console.WriteLine("Sucesso ao cadastrar a conta...");
                                            await Task.Delay(TimeSpan.FromSeconds(15));
                                            id = await Plat.GetAccountID(conta.Conta.Username);
                                            if (id.Status == 1)
                                            {
                                                Console.WriteLine("Rodando tarefas na conta...");
                                                Conta.conta.ContaID = Conta.conta.Username;
                                                await RodarConta(Conta);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Erro ao cadastrar a conta...");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Erro ao cadastrar a conta...");
                                        }
                                    }
                                    else
                                    {
                                        Console.Write(data.Response);
                                        await Task.Delay(TimeSpan.FromSeconds(15));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Conta localizada...");
                                    Console.WriteLine("Rodando tarefas na conta...");
                                    Conta.conta.ContaID = Conta.conta.Username;
                                    await RodarConta(Conta);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                        Console.WriteLine(err.StackTrace);
                        Console.WriteLine(err.Source);
                        Console.WriteLine("Erro ao rodar o bot: " + err.Message);
                        await Task.Delay(TimeSpan.FromSeconds(30));
                    }
                    Console.WriteLine("Aguardando 1 minuto para mudar de conta...");
                    await Task.Delay(TimeSpan.FromSeconds(60));
                    Console.Clear();
                }
            }
            else
            {
                Console.WriteLine("Não foi possivel realizar login na cliquedin...");
                await Task.Delay(TimeSpan.FromSeconds(999999));
            }
        }

        static async Task RodarConta(BotAccounts conta)
        {
            SaveDate(conta.conta.Username.ToLower(), UserAgent, conta.insta.CookieString(), conta.insta.GetClaim());
            try
            {
                Random rand = new();
                int nTask = 0;
                while (nTask < 90)
                {
                    Console.Clear();
                    if (nTask > 0 && nTask % 10 == 0)
                    {
                        Console.WriteLine("Assistindo 3minutos de stories de famosos...");
                        _ = await conta.RelaxSystem(3, Plat);
                        Console.WriteLine("Continuando a realizar as tarefas...");
                    }
                    Console.WriteLine("Buscando tarefa para realizar...");
                    var task = await Plat.GetTask(conta.conta.Username);
                    if (task.Status != 1)
                    {
                        int check = 0;
                        while (check < 3 && task.Status != 1)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(25));
                            check++;
                            task = await Plat.GetTask(conta.conta.Username);
                        }
                    }
                    if (task.Status == 1)
                    {
                        Console.WriteLine("Tarefa encontrada...");
                        string target;
                        string taskID;
                        switch (task.Tipo)
                        {
                            case "seguir":
                                if (task.Json.name.ToString().IndexOf("instagram.com") > -1)
                                {
                                    dynamic array = task.Json.name.ToString().Split("/");
                                    target = array[array.Length - 1] == "" ? (string)array[array.Length - 2] : (string)array[array.Length - 1];
                                }
                                else
                                    target = task.Json.name.ToString();
                                Console.WriteLine($"Seguindo o perfil '{target}'...");
                                taskID = task.Json.id.ToString();
                                var seguir = await conta.FollowUser(target, Plat);
                                if (seguir.Status == 1)
                                {
                                    Console.WriteLine("Sucesso ao seguir o perfil...");
                                    Console.WriteLine("Confirmando a tarefa...");
                                    var confirm = await Plat.ConfirmTask(taskID, conta.conta.Username);
                                    if (confirm.Status == 1)
                                        Console.WriteLine("Sucesso ao confirmar a tarefa...");
                                    else
                                        Console.WriteLine("Erro ao confirmar a tarefa...");
                                    nTask++;
                                }
                                else
                                {
                                    if (seguir.Status <= 2)
                                    {
                                        if (seguir.Status == -3)
                                        {
                                            Console.WriteLine("Conta com bloqueio temporario...");
                                            Console.WriteLine("Enviando para o servidor e mudando de conta...");
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(5));
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(seguir.Response);
                                            Console.WriteLine("Pulando a tarefa...");
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Plat.SendPrivateOrNotExistTask(taskID);
                                        }
                                    }
                                    else
                                    {
                                        if (seguir.Status == 3)
                                        {
                                            Console.WriteLine(seguir.Response);
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(3));
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(seguir.Response);
                                            await Task.Delay(TimeSpan.FromSeconds(20));
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "curtir":
                                if (task.Json.name.ToString().IndexOf("instagram.com") > -1)
                                {
                                    dynamic array = task.Json.name.ToString().Split("/");
                                    target = array[array.Length - 1] == "" ? (string)array[array.Length - 2] : (string)array[array.Length - 1];
                                }
                                else
                                    target = task.Json.name.ToString();
                                Console.WriteLine($"Curtindo a publicação '{target}'...");
                                taskID = task.Json.id.ToString();
                                var curtir = await conta.LikeMediaShortCode(target, Plat);
                                if (curtir.Status == 1)
                                {
                                    Console.WriteLine("Sucesso ao curtir a publicação...");
                                    Console.WriteLine("Confirmando a tarefa...");
                                    var confirm = await Plat.ConfirmTask(taskID, conta.conta.Username);
                                    if (confirm.Status == 1)
                                        Console.WriteLine("Sucesso ao confirmar a tarefa...");
                                    else
                                        Console.WriteLine("Erro ao confirmar a tarefa...");
                                    nTask++;
                                }
                                else
                                {
                                    if (curtir.Status <= 2)
                                    {
                                        if (curtir.Status == -3)
                                        {
                                            Console.WriteLine("Conta com bloqueio temporario...");
                                            Console.WriteLine("Enviando para o servidor e mudando de conta...");
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(5));
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(curtir.Response);
                                            Console.WriteLine("Pulando a tarefa...");
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Plat.SendPrivateOrNotExistTask(taskID);
                                        }
                                    }
                                    else
                                    {
                                        if (curtir.Status == 3)
                                        {
                                            Console.WriteLine(curtir.Response);
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(3));
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(curtir.Response);
                                            await Task.Delay(TimeSpan.FromSeconds(20));
                                            return;
                                        }
                                    }
                                }
                                break;
                            case "stories":
                                if (task.Json.name.ToString().IndexOf("instagram.com") > -1)
                                {
                                    dynamic array = task.Json.name.ToString().Split("/");
                                    target = array[array.Length - 1] == "" ? (string)array[array.Length - 2] : (string)array[array.Length - 1];
                                }
                                else
                                    target = task.Json.name.ToString();
                                Console.WriteLine($"Assistindo stories do perfil '{target}'...");
                                taskID = task.Json.id.ToString();
                                var stories = await conta.SeeStoryByUsername(target, Plat);
                                if (stories.Status == 1)
                                {
                                    Console.WriteLine("Sucesso ao assistir stories...");
                                    Console.WriteLine("Confirmando a tarefa...");
                                    var confirm = await Plat.ConfirmTask(taskID, conta.conta.Username);
                                    if (confirm.Status == 1)
                                        Console.WriteLine("Sucesso ao confirmar a tarefa...");
                                    else
                                        Console.WriteLine("Erro ao confirmar a tarefa...");
                                    nTask++;
                                }
                                else
                                {
                                    if (stories.Status <= 2)
                                    {
                                        if (stories.Status == -3)
                                        {
                                            Console.WriteLine("Conta com bloqueio temporario...");
                                            Console.WriteLine("Enviando para o servidor e mudando de conta...");
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(5));
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(stories.Response);
                                            Console.WriteLine("Pulando a tarefa...");
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Plat.SendPrivateOrNotExistTask(taskID);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(stories.Response);
                                        await Task.Delay(TimeSpan.FromSeconds(20));
                                        return;
                                    }
                                }
                                break;
                            case "comentar":
                                if (task.Json.name.ToString().IndexOf("instagram.com") > -1)
                                {
                                    dynamic array = task.Json.name.ToString().Split("/");
                                    target = array[array.Length - 1] == "" ? (string)array[array.Length - 2] : (string)array[array.Length - 1];
                                }
                                else
                                    target = task.Json.name.ToString();
                                string[] comentarios;
                                try
                                {
                                    comentarios = task.Json.comments.ToObject<string[]>(); ;
                                }
                                catch
                                {
                                    comentarios = new string[] { "Top !!", "Nice !!", "Muito Bom !!!" };
                                }
                                int position = rand.Next(0, comentarios.Length);
                                Console.WriteLine($"Seguindo o perfil '{target}'...");
                                taskID = task.Json.id.ToString();
                                var comentar = await conta.CommentMediaShotcode(target, comentarios[position], Plat);
                                if (comentar.Status == 1)
                                {
                                    Console.WriteLine("Sucesso ao seguir o perfil...");
                                    Console.WriteLine("Confirmando a tarefa...");
                                    var confirm = await Plat.ConfirmTask(taskID, conta.conta.Username);
                                    if (confirm.Status == 1)
                                        Console.WriteLine("Sucesso ao confirmar a tarefa...");
                                    else
                                        Console.WriteLine("Erro ao confirmar a tarefa...");
                                    nTask++;
                                }
                                else
                                {
                                    if (comentar.Status <= 2)
                                    {
                                        if (comentar.Status == -3)
                                        {
                                            Console.WriteLine("Conta com bloqueio temporario...");
                                            Console.WriteLine("Enviando para o servidor e mudando de conta...");
                                            await Plat.SendBlockTemp(conta.conta.Username);
                                            await Task.Delay(TimeSpan.FromSeconds(5));
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(comentar.Response);
                                            Console.WriteLine("Pulando a tarefa...");
                                            await Plat.JumpTask(taskID, conta.conta.Username);
                                            await Plat.SendPrivateOrNotExistTask(taskID);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(comentar.Response);
                                        await Task.Delay(TimeSpan.FromSeconds(20));
                                        return;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        int delay = rand.Next(150, 400);
                        Console.WriteLine($"Aguardando {delay} segundos para continuar...");
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                    }
                    else
                    {
                        Console.WriteLine("Não foi popssivel localizar tarefa no momento...");
                        int delay = rand.Next(60, 100);
                        Console.WriteLine($"Aguardando {delay} segundos para continuar...");
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine("Erro ao realizar as tarefas...");
                Console.WriteLine("Error: " + err.Message);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
            return;
        }

        static string[] GetSaveData(string username)
        {
            string dir = Directory.GetCurrentDirectory();
            try
            {
                if (!Directory.Exists($@"{dir}/Conta"))
                    Directory.CreateDirectory($@"{dir}/Conta");
            }
            catch { }
            string[] retorno = new string[3];
            if (File.Exists($@"{dir}/Conta/{username}.arka"))
                retorno[0] = File.ReadAllText($@"{dir}/Conta/{username}.arka");
            if (File.Exists($@"{dir}/Conta/{username}-ua.arka"))
                retorno[1] = File.ReadAllText($@"{dir}/Conta/{username}-ua.arka");
            if (File.Exists($@"{dir}/Conta/{username}-claim.arka"))
                retorno[2] = File.ReadAllText($@"{dir}/Conta/{username}-claim.arka");
            return retorno;
        }

        static void SaveDate(string username, string ua, string cookie, string claim)
        {
            string dir = Directory.GetCurrentDirectory();
            try
            {
                if (!Directory.Exists($@"{dir}/Conta"))
                    Directory.CreateDirectory($@"{dir}/Conta");
            }
            catch { }
            if (File.Exists($@"{dir}/Conta/{username}.arka"))
                File.Delete($@"{dir}/Conta/{username}.arka");
            File.WriteAllText($@"{dir}/Conta/{username}.arka", cookie);
            if (File.Exists($@"{dir}/Conta/{username}-ua.arka"))
                File.Delete($@"{dir}/Conta/{username}-ua.arka");
            File.WriteAllText($@"{dir}/Conta/{username}-ua.arka", ua);
            if (File.Exists($@"{dir}/Conta/{username}-claim.arka"))
                File.Delete($@"{dir}/Conta/{username}-claim.arka");
            File.WriteAllText($@"{dir}/Conta/{username}-claim.arka", claim);
        }

        static bool HaveCookie(string username)
        {
            var dir = Directory.GetCurrentDirectory();
            if (File.Exists($@"{dir}/Conta/{username}.arka") && File.Exists($@"{dir}/Conta/{username}-ua.arka") && File.Exists($@"{dir}/Conta/{username}-claim.arka"))
                return true;
            return false;
        }

    }
}
