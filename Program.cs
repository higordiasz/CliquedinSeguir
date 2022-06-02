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
using System.Collections.Generic;

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
                bool account = false;
                CliquedinAPI.Models.Retorno.ContaRetorno conta = null;
                while (true)
                {
                    try
                    {
                        UserAgent = uaController.GetUa();
                        Console.WriteLine("UserAgent: " + UserAgent);
                        Console.WriteLine("Buscando conta...");
                        if (!account)
                        {
                            conta = await Plat.GetAccount();
                            account = true;
                        }
                        if (conta.Conta != null)
                        {
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
                                string[] data = GetSaveData(Plat, conta.Conta.Username.ToLower());
                                if (data != null)
                                {
                                    Console.WriteLine("Recuperando cookie anterior...");
                                    if (proxy == null)
                                    {
                                        Console.WriteLine("Não foi possivel puxar um proxy do servidor...");
                                        Console.WriteLine("Aguardando 30 segundo para tentar novamente...");
                                        await Task.Delay(TimeSpan.FromSeconds(30));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            //Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, data[0], data[1], $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                            //Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, data[0], data[1], $"http://gate.dc.smartproxy.com:20000/", "sp51276865", "20180102");
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
                                            logada = await Conta.IsLogged();
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Erro ao criar instancia do bot ... (Proxy)");
                                            Console.WriteLine("Tentando novamente...");
                                            await Task.Delay(TimeSpan.FromSeconds(5));
                                        }
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
                                            Console.WriteLine("Não foi possivel puxar um proxy do servidor...");
                                            Console.WriteLine("Aguardando 30 segundo para tentar novamente...");
                                            await Task.Delay(TimeSpan.FromSeconds(30));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                //Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                                //Insta i = new(conta.Conta.Username.ToLower(), conta.Conta.Password, $"http://gate.dc.smartproxy.com:20000/", "sp51276865", "20180102");
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
                                                        if (login.Status == -998)
                                                        {
                                                            Console.WriteLine("Não foi possivel carregar a pagina do instagram..");
                                                            Console.WriteLine("Tentando novamente...");
                                                            await Task.Delay(TimeSpan.FromSeconds(15));
                                                        }
                                                        else
                                                        {
                                                            await Task.Delay(TimeSpan.FromSeconds(15));
                                                            leave = true;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                                Console.WriteLine("Erro ao criar instancia do bot ... (Proxy)");
                                                Console.WriteLine("Tentando novamente...");
                                                await Task.Delay(TimeSpan.FromSeconds(5));
                                            }
                                        }
                                    }
                                }
                                if (logada)
                                {
                                    try
                                    {
                                        Console.WriteLine("Buscando ID da conta...");
                                        var id = await Plat.GetAccountID(conta.Conta.Username);
                                        if (id.Status != 1)
                                        {
                                            Console.WriteLine("Não foi possivel localizar a conta na cliquedin...");
                                            Console.WriteLine("Buscando informações da conta...");
                                            var data2 = await Conta.GetDataFromPerfil(Plat);
                                            if (data2.Status == 1)
                                            {
                                                Console.WriteLine("Sucesso ao recuperar informações...");
                                                Console.WriteLine("Registrando a conta na cliquedin...");
                                                var cad = await Plat.RegisteAccount(conta.Conta.Username, data2.Gender, data2.Response, await Conta.LastPostDate(Plat));
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
                                                        account = false;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Erro ao cadastrar a conta...");
                                                        account = false;
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Erro ao cadastrar a conta...");
                                                    account = false;
                                                }
                                            }
                                            else
                                            {
                                                Console.Write(data2.Response);
                                                await Task.Delay(TimeSpan.FromSeconds(15));
                                                account = false;
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
                                    catch
                                    {
                                        account = false;
                                    }
                                }
                                else
                                {
                                    account = false;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Não foi possivel localizar conta na cliquedin...");
                            Console.WriteLine("Aguardando 1 minuto para continuar...");
                            await Task.Delay(TimeSpan.FromSeconds(60));
                            account = false;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.Write("\n\n\n\n");
                        Console.WriteLine("Err Message: ");
                        Console.WriteLine(err.Message);
                        Console.WriteLine("Err StackTrace: ");
                        Console.WriteLine(err.StackTrace);
                        Console.WriteLine("Err Source: ");
                        Console.WriteLine(err.Source);
                        Console.WriteLine("Err Message: ");
                        Console.WriteLine(err.Data);
                        Console.Write("\n\n\n\n");
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
            Dictionary<string, int> waitValues = await Plat.GetMinMax();
            DeleteDate(Plat, conta.conta.Username.ToLower());
            SaveDate(Plat, conta.conta.Username.ToLower(), UserAgent, conta.insta.CookieString(), conta.insta.GetClaim());
            try
            {
                Random rand = new();
                int nTask = 0;
                while (nTask < 99)
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
                                            if (seguir.Status == -882)
                                            {
                                                Console.WriteLine(seguir.Response);
                                                Console.WriteLine("Tentando relogar na conta ...");
                                                //conta.insta = new(conta.conta.Username.ToLower(), conta.conta.Password, $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                                //conta.insta = new(conta.conta.Username.ToLower(), conta.conta.Password, $"http://gate.dc.smartproxy.com:20000/", "sp51276865", "20180102");
                                                conta.insta = new(conta.conta.Username.ToLower(), conta.conta.Password);
                                                var login = await conta.Login(Plat);
                                                if (login.Status == 1)
                                                {
                                                    Console.WriteLine("Login realizado com sucesso...");
                                                    Console.WriteLine("Continuando com as tarefas...");
                                                    DeleteDate(Plat, conta.conta.Username.ToLower());
                                                    SaveDate(Plat, conta.conta.Username.ToLower(), UserAgent, conta.insta.CookieString(), conta.insta.GetClaim());
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Não foi possivel realizar o login na conta...");
                                                    Console.WriteLine(login.Status);
                                                    await Task.Delay(TimeSpan.FromSeconds(3));
                                                    DeleteDate(Plat, conta.conta.Username.ToLower());
                                                    return;
                                                }
                                            }
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
                                            DeleteDate(Plat, conta.conta.Username.ToLower());
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(seguir.Response);
                                            await Task.Delay(TimeSpan.FromSeconds(20));
                                            DeleteDate(Plat, conta.conta.Username.ToLower());
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
                                            if (curtir.Status == -992)
                                            {
                                                Console.WriteLine(curtir.Response);
                                                Console.WriteLine("Tentando relogar na conta ...");
                                                conta.insta = new(conta.conta.Username.ToLower(), conta.conta.Password, $"http://{proxy.IP}:{proxy.Port}/", proxy.User, proxy.Pass);
                                                //conta.insta = new(conta.conta.Username.ToLower(), conta.conta.Password, $"http://gate.dc.smartproxy.com:20000/", "sp51276865", "20180102");
                                                var login = await conta.Login(Plat);
                                                if (login.Status == 1)
                                                {
                                                    Console.WriteLine("Login realizado com sucesso...");
                                                    Console.WriteLine("Continuando com as tarefas...");
                                                    DeleteDate(Plat, conta.conta.Username.ToLower());
                                                    SaveDate(Plat, conta.conta.Username.ToLower(), UserAgent, conta.insta.CookieString(), conta.insta.GetClaim());
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Não foi possivel realizar o login na conta...");
                                                    Console.WriteLine(login.Status);
                                                    await Task.Delay(TimeSpan.FromSeconds(3));
                                                    DeleteDate(Plat, conta.conta.Username.ToLower());
                                                    return;
                                                }
                                            }
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
                                            DeleteDate(Plat, conta.conta.Username.ToLower());
                                            return;
                                        }
                                        else
                                        {
                                            Console.WriteLine(curtir.Response);
                                            await Task.Delay(TimeSpan.FromSeconds(20));
                                            DeleteDate(Plat, conta.conta.Username.ToLower());
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
                                        DeleteDate(Plat, conta.conta.Username.ToLower());
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
                                        DeleteDate(Plat, conta.conta.Username.ToLower());
                                        return;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        int min = waitValues.GetValueOrDefault("min") > 0 ? waitValues.GetValueOrDefault("min") : 10;
                        int max = waitValues.GetValueOrDefault("max") > min ? waitValues.GetValueOrDefault("max") : min + 30;
                        int delay = rand.Next(min, max);
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
                await Plat.SendFinaly(conta.conta.Username.ToLower());
            }
            catch (Exception err)
            {
                Console.WriteLine("Erro ao realizar as tarefas...");
                Console.WriteLine("Error: " + err.Message);
                await Task.Delay(TimeSpan.FromSeconds(20));
            }
            return;
        }

        static string[] GetSaveData(Cliquedin cliquedin, string username)
        {
            /*
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
            */
            try
            {
                string[] retorno = new string[2];
                string cookie = cliquedin.GetCookie(username).Result;
                if (!String.IsNullOrEmpty(cookie))
                {
                    string[] arr = cookie.Split(";");
                    string cookieToReturn = "";
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (i != (arr.Length - 1))
                        {
                            cookieToReturn += arr[i];
                            if (i < (arr.Length - 2))
                                cookieToReturn += ";";
                        }
                    }
                    retorno[0] = cookieToReturn;
                    retorno[1] = arr[^1];
                    return retorno;
                }
                return null;
            }
            catch (Exception err)
            {
                Console.WriteLine("Erro GetDate: " + err.Message);
                return null;
            }
        }

        static void SaveDate(Cliquedin cliquedin, string username, string ua, string cookie, string claim)
        {
            /*string dir = Directory.GetCurrentDirectory();
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
            */
            try
            {
                string cookieToSend = cookie + ";" + claim;
                cliquedin.SaveCookie(username, cookieToSend);
                return;
            }
            catch
            {
                return;
            }
        }

        static void DeleteDate(Cliquedin cliquedin, string username)
        {
            /*
            string dir = Directory.GetCurrentDirectory();
            try
            {
                if (!Directory.Exists($@"{dir}/Conta"))
                    Directory.CreateDirectory($@"{dir}/Conta");
            }
            catch { }
            if (File.Exists($@"{dir}/Conta/{username}.arka"))
                File.Delete($@"{dir}/Conta/{username}.arka");
            if (File.Exists($@"{dir}/Conta/{username}-ua.arka"))
                File.Delete($@"{dir}/Conta/{username}-ua.arka");
            if (File.Exists($@"{dir}/Conta/{username}-claim.arka"))
                File.Delete($@"{dir}/Conta/{username}-claim.arka");
            */
            try
            {
                cliquedin.DeleteCookie(username);
                return;
            }
            catch
            {
                return;
            }
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
