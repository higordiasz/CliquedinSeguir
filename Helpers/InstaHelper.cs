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

namespace CliquedinSeguir.Helpers
{
    public static class InstaHelper
    {
        public async static Task<Retorno> FollowUser (this BotAccounts conta, string username)
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
                    return await conta.CheckChallenge();
                } else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }
    
        public async static Task<Retorno> LikeMediaShortCode (this BotAccounts conta, string shotcode)
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
                    return await conta.CheckChallenge();
                }
                else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }

        public async static Task<Retorno> SeeStoryByUsername (this BotAccounts conta, string username)
        {
            Retorno ret = new()
            {
                Response = "Error",
                Status = -1
            };
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
                    return await conta.CheckChallenge();
                }
                else
                {
                    ret.Status = follow.Status;
                    ret.Response = follow.Response;
                    return ret;
                }
            }
        }

        public async static Task<Retorno> Login (this BotAccounts conta)
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
                    return await conta.CheckChallenge();
                }
                else
                {
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

        public static async Task<RetornoData> GetDataFromPerfil(this BotAccounts conta)
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
                    var d = await conta.CheckChallenge();
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

        public static async Task<Retorno> CheckChallenge(this BotAccounts conta)
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
                        return ret;
                    case "UFACWWWBloksScreen":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: SMS";
                        return ret;
                    case "ReviewLoginForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: SMS";
                        return ret;
                    case "SelfieCaptchaChallengeForm":
                        ret.Status = 11;
                        ret.Response = "Bloqueio: Selfie";
                        return ret;
                    case "SelectContactPointRecoveryForm":
                        ret.Status = 7;
                        ret.Response = "Bloqueio: SMS";
                        return ret;
                    case "RecaptchaChallengeForm":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: Recaptcha e SMS";
                        return ret;
                    case "IeForceSetNewPasswordForm":
                        ret.Status = 6;
                        ret.Response = "Bloqueio: Troca de senha";
                        return ret;
                    case "SubmitPhoneNumberForm":
                        ret.Status = 5;
                        ret.Response = "Bloqueio: SMS";
                        return ret;
                    case "EscalationChallengeInformationalForm":
                        var res = await conta.insta.SelectChoiceChallenge(challenge, "0", challenge.Url);
                        if (res.Status == 1)
                            return await conta.Login();
                        else
                        {
                            ret.Status = 8;
                            ret.Response = "Bloqueio: Foto";
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
                                                return ret;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Não foi possivel verificar a conta...");
                                            return ret;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Não foi possivel cerificar a conta do instagram..");
                                        return ret;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Não foi possivel conectar no email...");
                                    return ret;
                                }
                            }
                            else
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            return ret;
                        }
                    default:
                        ret.Status = 9;
                        ret.Response = "Bloqueio: " + challenge.Response;
                        return ret;
                }
            }
            else
            {
                ret.Status = -2;
                ret.Response = "Conta com bloqueio de SMS";
                return ret;
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
