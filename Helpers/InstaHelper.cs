using System;
using System.Linq;
using System.Threading.Tasks;

using Instagram.Controllers.Login;
using Instagram.Controllers.Challenge;
using Instagram.Controllers.Follow;
using Instagram.Controllers.Media;
using Instagram.Controllers.PreLoad;
using Instagram.Controllers.Story;
using Instagram.Controllers.Profile;

using CliquedinSeguir.Models.Retorno;
using CliquedinSeguir.Models.Contas;
using S22.Imap;
using CliquedinAPI;
using CliquedinAPI.Controllers;

namespace CliquedinSeguir.Helpers
{
    public static class InstaHelper
    {
        private static string[] _famosos = new string[]
        {
            "cristiano",
            "alanzoka",
            "nike",
            "adidas",
            "mcdonjuan",
            "arianagrande",
            "bbb",
            "tvglobo",
            "imaginedragons",
            "luvadepedreiro",
            "neymarjr",
            "casimiro",
            "nyviestephan",
            "gabicattuzzo",
            "fred",
            "flamengo",
            "vascodagama",
            "corinthians",
            "nickiminaj",
            "instagram",
            "kyliejenner",
            "therock",
            "selenagomez",
            "kimkardashian",
            "beyonce",
            "justimbieber",
            "kendalljenner",
            "virat.kohli",
            "anitta",
            "tatawerneck",
            "larissamanoela",
            "brunamarquezine",
            "maisa",
            "gusttavolima",
            "marcelotwelve",
            "ronaldinho",
        };

        public async static Task<Retorno> RelaxSystem (this BotAccounts conta, int minutos, Cliquedin cliquedin)
        {
            int descanso = minutos / 3;
            Retorno ret = new()
            {
                Status = -1,
                Response = "Error"
            };
            try
            {
                Random rand = new();
                string celebridade = _famosos[rand.Next(0, _famosos.Length - 1)];
                for (int i = 0; i< 3; i++)
                {
                    _ = await conta.SeeStoryByUsername(celebridade, cliquedin);
                    await Task.Delay(TimeSpan.FromMinutes(descanso));
                    celebridade = _famosos[rand.Next(0, _famosos.Length - 1)];
                }
                ret.Status = 1;
                ret.Response = "Sucesso";
            } catch
            {
                ret.Status = -1;
                ret.Response = "Error";
            }
            return ret;
        }

        public async static Task<Retorno> FollowUser (this BotAccounts conta, string username, Cliquedin cliquedin, bool fisrt = true)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var follow = await conta.insta.FollowUserByUsername(username);
            if (follow.Status == 1)
            {
                ret.Status = 1;
                ret.Response = "Sucesso ao seguir o perfil";
                return ret;
            }
            else
            {
                if (follow.Status == -2)
                {
                    return await conta.CheckChallenge(cliquedin);
                } else
                {
                    if (follow.Status == -1)
                    {
                        if (fisrt)
                        {
                            return await conta.FollowUser(username, cliquedin, false);
                        }
                        else
                        {
                            ret.Status = follow.Status;
                            ret.Response = follow.Response;
                            return ret;
                        }
                    }
                    else
                    {
                        ret.Status = follow.Status;
                        ret.Response = follow.Response;
                        return ret;
                    }
                }
            }
        }
    
        public async static Task<Retorno> LikeMediaShortCode (this BotAccounts conta, string shotcode, Cliquedin cliquedin)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var follow = await conta.insta.LikeMediaByShortCode(shotcode);
            if (follow.Status == 1)
            {
                ret.Status = 1;
                ret.Response = "Sucesso ao curtir a publicação";
                return ret;
            }
            else
            {
                if (follow.Status == -2)
                {
                    return await conta.CheckChallenge(cliquedin);
                }
                else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }

        public async static Task<Retorno> CommentMediaShotcode(this BotAccounts conta, string shortcode, string comment, Cliquedin cliquedin)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var follow = await conta.insta.CommentMediaByShortCode(shortcode, comment);
            if (follow.Status == 1)
            {
                ret.Status = 1;
                ret.Response = "Sucesso ao comentar a publicação";
                return ret;
            }
            else
            {
                if (follow.Status == -2)
                {
                    return await conta.CheckChallenge(cliquedin);
                }
                else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }

        public async static Task<Retorno> SeeStoryByUsername (this BotAccounts conta, string username, Cliquedin cliquedin)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            Console.WriteLine("Iniciando isstema de stories");
            var follow = await conta.insta.SeeStoryFromUsername(username);
            if (follow.Status == 1)
            {
                ret.Status = 1;
                ret.Response = "Sucesso ao assistir o Story";
                return ret;
            }
            else
            {
                if (follow.Status == -2)
                {
                    return await conta.CheckChallenge(cliquedin);
                }
                else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }

        public async static Task<Retorno> Login (this BotAccounts conta, Cliquedin cliquedin)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var login = await conta.insta.ILogin();
            if (login.Status == 1)
            {
                ret.Response = "Sucesso ao fazer login";
                ret.Status = 1;
                return ret;
            }
            else
            {
                if (login.Status == -2)
                {
                    return await conta.CheckChallenge(cliquedin);
                }
                else
                {
                    if (ret.Status == -996)
                        await cliquedin.SendAccountBlock(conta.conta.Username, "UsuarioOuSenhaErrada");
                    if (ret.Status == -997)
                        await cliquedin.SendAccountBlock(conta.conta.Username, "BANIDA");
                    ret.Status = login.Status;
                    ret.Response = login.Response;
                    return ret;
                }
            }
        }
    
        public async static Task<bool> IsLogged(this BotAccounts conta)
        {
            var profile = await conta.insta.CheckLogin();
            if (profile.Status == 1) return true;
            return false;
        }

        public static async Task<RetornoData> GetDataFromPerfil(this BotAccounts conta, Cliquedin cliquedin)
        {
            RetornoData ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var login = await conta.insta.GetMyProfile();
            if (login.Status == 1)
            {
                ret.Gender = login.Gender;
                ret.Response = login.Response;
                ret.Status = 1;
                return ret;
            }
            else
            {
                if (login.Status == -2)
                {
                    var d = await conta.CheckChallenge(cliquedin);
                    ret.Response = d.Response;
                    ret.Status = d.Status;
                    return ret;
                }
                else
                {
                    ret.Status = login.Status;
                    ret.Response = login.Response;
                    return ret;
                }
            }
        }

        public static async Task<Retorno> CheckChallenge(this BotAccounts conta, Cliquedin cliquedin)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
            var challenge = await conta.insta.GetChallenge();
            if (challenge.Status == 1)
            {
                Console.WriteLine("Bloqueio: " + challenge.Response);
                switch (challenge.Response)
                {
                    case "AcknowledgeForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "AcknowledgeForm-SMS");
                        return ret;
                    case "UFACWWWBloksScreen":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "UFACWWWBloksScreen-SMS");
                        return ret;
                    case "ReviewLoginForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "ReviewLoginForm-SMS");
                        return ret;
                    case "SelfieCaptchaChallengeForm":
                        ret.Status = 11;
                        ret.Response = "Bloqueio: Selfie";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "SelfieCaptchaChallengeForm-SMS");
                        return ret;
                    case "SelectContactPointRecoveryForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "SelectContactPointRecoveryForm-SMS");
                        return ret;
                    case "RecaptchaChallengeForm":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: Recaptcha e SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "RecaptchaChallengeForm-SMS");
                        return ret;
                    case "IeForceSetNewPasswordForm":
                        ret.Status = 6;
                        ret.Response = "Bloqueio: Troca de senha";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "IeForceSetNewPasswordForm-SMS");
                        return ret;
                    case "SubmitPhoneNumberForm":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: SMS";
                        await cliquedin.SendAccountBlock(conta.conta.Username, "SubmitPhoneNumberForm-SMS");
                        return ret;
                    case "EscalationChallengeInformationalForm":
                        var res = await conta.insta.SelectChoiceChallenge(challenge, "0", challenge.Url);
                        if (res.Status == 1)
                            return await conta.Login(cliquedin);
                        else
                        {
                            ret.Status = 8;
                            ret.Response = "Bloqueio: Foto";
                            await cliquedin.SendAccountBlock(conta.conta.Username, "EscalationChallengeInformationalForm-FOTO");
                        }
                        return ret;
                    case "SelectVerificationMethodForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: Email/SMS";
                        if (conta.conta.Verificar_Email)
                        {
                            Console.WriteLine("Verificação de email ou sms requerida...");
                            var resReply = await conta.insta.SelectChoiceChallenge(challenge, "1", challenge.Url);
                            Console.WriteLine(resReply.Response);
                            if (resReply.Response == "VerifyEmailCodeForm")
                            {
                                await Task.Delay(986);
                                string code = GetInstagramCodeFromEmail(conta.conta.Imap_ID, Convert.ToInt32(conta.conta.Imap_Port), conta.conta.Imap_SSL, conta.conta.Email, conta.conta.Email_Password);
                                if (code != "not")
                                {
                                    if (code == "")
                                    {
                                        int max = 0;
                                        while (max < 4 && code == "")
                                        {
                                            await Task.Delay(4895);
                                            code = GetInstagramCodeFromEmail(conta.conta.Imap_ID, Convert.ToInt32(conta.conta.Imap_Port), conta.conta.Imap_SSL, conta.conta.Email, conta.conta.Email_Password);
                                            max++;
                                        }
                                    }
                                    if (code != "" && code != "not")
                                    {
                                        var codeChallenge = await conta.insta.SendCode(code, resReply, resReply.Url);
                                        if (codeChallenge.Status == 1)
                                        {
                                            Console.WriteLine("Conta verificada com sucesso..");
                                            Console.WriteLine("Verificando se a conta está conectada..");
                                            var isLoged = await conta.IsLogged();
                                            if (isLoged)
                                            {
                                                ret.Status = 1;
                                                ret.Response = "Login realizado com sucesso";
                                                return ret;
                                            }
                                            else
                                            {
                                                ret.Status = 7;
                                                ret.Response = "Não foi possivel realizar login na conta...";
                                                await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                                                return ret;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Não foi possivel verificar a conta...");
                                            await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                                            return ret;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Não foi possivel cerificar a conta do instagram..");
                                        await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                                        return ret;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Não foi possivel conectar no email...");
                                    await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                                    return ret;
                                }
                            }
                            else
                            {
                                await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                                return ret;
                            }
                        }
                        else
                        {
                            await cliquedin.SendAccountBlock(conta.conta.Username, "SelectVerificationMethodForm-SMSEMAIL");
                            return ret;
                        }
                    default:
                        ret.Status = 9;
                        ret.Response = "Bloqueio: " + challenge.Response;
                        await cliquedin.SendAccountBlock(conta.conta.Username, $"{challenge.Response}-SMS");
                        return ret;
                }
            }
            else
            {
                ret.Status = -2;
                ret.Response = "Conta com bloqueio de SMS";
                await cliquedin.SendAccountBlock(conta.conta.Username, "ErroIdentificar");
                return ret;
            }
        }

        public static async Task<string> LastPostDate(this BotAccounts conta, Cliquedin cliquedin)
        {
            var last = await conta.insta.GetLastMediaByUsername(conta.conta.Username.ToLower());
            if (last.Status == 1)
            {
                return last.Response;
            }
            else
            {
                return DateTime.Now.ToString("yyyy/MM/dd");
            }
        }

        static private string GetInstagramCodeFromEmail(string IMAP_IP, int IMAP_PORT, bool SSL, string EMAIL, string PASSWORD)
        {
            ImapClient imap = new ImapClient(IMAP_IP, IMAP_PORT, SSL);
            imap.Login(EMAIL, PASSWORD, AuthMethod.Login);
            if (imap.Authed)
            {
                string codigo = GetInstagramCodeFromEmail(imap).Result;
                if (codigo != "")
                {
                    Console.WriteLine("Checando se o código foi enviado...");
                    Console.WriteLine("Código localizado...");
                    return codigo;
                }
                else
                {
                    Console.WriteLine("Código ainda não chegou..");
                    return "";
                }
            }
            Console.WriteLine("Não foi possivel realizar o login no email...");
            return ("not");
        }

        static async private Task<string> GetInstagramCodeFromEmail(ImapClient imap)
        {
            for (int i = 0; i < 5; i++)
            {
                var uids = imap.Search(SearchCondition.All());
                int uids2 = uids.Count();
                int lp = uids2 - 2;
                var email = imap.GetMessage(uids.Last());
                Console.WriteLine("Ultimo Email: " + email.Subject);
                if (email.Subject == "Verify your account" || email.Subject == "Verifique sua conta" || email.Subject == "Hesabınızı Dogrulayın" || CheckBodyForCode(email.Body))
                {
                    var index = email.Body.IndexOf("<font size=\"6\">");
                    int id = index + 15;
                    return email.Body.Substring(id, 6);
                }
                await Task.Delay(TimeSpan.FromSeconds(15));
            }
            return "";
        }

        static private bool CheckBodyForCode(string body)
        {
            if (body.IndexOf("<font size=\"6\">") > -1)
            {
                var index = body.IndexOf("<font size=\"6\">");
                int id = index + 15 + 6;
                if (body[id] == '<')
                    return true;
            }
            return false;
        }

    }
}
