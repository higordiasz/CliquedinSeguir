using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliquedinSeguir.Helpers.Log
{
    public static class LogHelper
    {
        public static void LogMessage(string message)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (Directory.Exists($@"{dir}\logs"))
                {
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\{data}.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\{data}.txt");
                        var list = linhas.ToList();
                        list.Add($"CLIQUEDINAPI {HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"CLIQUEDINAPI {HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", linhas);
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory($@"{dir}\logs");
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\{data}.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\{data}.txt");
                        var list = linhas.ToList();
                        list.Add($"CLIQUEDINAPI {HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"CLIQUEDINAPI {HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", linhas);
                        return;
                    }
                }
            }
            catch { }
        }
        public static void LogBloqueios(string message)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (Directory.Exists($@"{dir}\logs"))
                {
                    if (File.Exists($@"{dir}\logs\bloqueio.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\bloqueio.txt");
                        var list = linhas.ToList();
                        list.Add($"CLIQUEDINAPI {DateString()}-{HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\bloqueio.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"CLIQUEDINAPI {DateString()}-{HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\bloqueio.txt", linhas);
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory($@"{dir}\logs");
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\bloqueio.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\bloqueio.txt");
                        var list = linhas.ToList();
                        list.Add($"CLIQUEDINAPI {DateString()}-{HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\bloqueio.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"CLIQUEDINAPI {DateString()}-{HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\bloqueio.txt", linhas);
                        return;
                    }
                }
            }
            catch { }
        }
        private static string DateString()
        {
            var data = DateTime.Today;
            var dia = data.Day.ToString();
            var mes = data.Month.ToString();
            var ano = data.Year.ToString();
            return $"{dia}-{mes}-{ano}";
        }
        private static string HorarioString()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
