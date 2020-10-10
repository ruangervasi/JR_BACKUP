using JR_BACKUP;
using NMDD_EnviaEmail;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.ServiceProcess;
using System.Timers;
using JRBackup_Service.Models;

namespace JRBackup_Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        ConfigBackup _configBackup = new ConfigBackup();
        private DBConn dbMetodos = new DBConn();

        System.Timers.Timer timer1 = new System.Timers.Timer();

        public void Timer()
        {
            try
            {
                timer1.Interval = 45000; // 45 seconds
                timer1.Elapsed += new ElapsedEventHandler(OnTimer);
                timer1.Start();
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
            }
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                timer1.Stop();
                string PegaHora = string.Empty;
                string PegaMinuto = string.Empty;

                DateTime hora = DateTime.Now;
                if (_configBackup.Ativo == true)
                {
                    VerificaHora(DateTime.Now.ToString("HH:mm"));

                    timer1.Start();
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Timer();

                //lê arquivo de configurações:
                if (File.Exists(@"C:\gsn\ConfigBackup.ini"))
                {
                    string[] ConfigBackup = File.ReadAllLines(@"C:\gsn\ConfigBackup.ini");

                    _configBackup.LocalBackup1 = ConfigBackup[0];
                    _configBackup.Hora1 = ConfigBackup[4];
                    _configBackup.Hora2 = ConfigBackup[5];
                    _configBackup.Hora3 = ConfigBackup[6];
                    _configBackup.Hora4 = ConfigBackup[7];
                    _configBackup.Hora5 = ConfigBackup[8];
                    _configBackup.Hora6 = ConfigBackup[9];
                    if (ConfigBackup[18] == "1") { _configBackup.Ativo = true; }
                    _configBackup.LocalBackup2 = ConfigBackup[22];
                    _configBackup.Origem = ConfigBackup[23];
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {

            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
            }
        }

        public void VerificaHora(string HoraAtual)
        {
            try
            {
                if (HoraAtual == _configBackup.Hora1)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == _configBackup.Hora2)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == _configBackup.Hora3)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == _configBackup.Hora4)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == _configBackup.Hora5)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == _configBackup.Hora6)
                {
                    ExecutaBackup();
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
            }
        }


        public static string ExecutarCMD(string comando)
        {
            try
            {
                using (Process processo = new Process())
                {
                    processo.StartInfo.FileName = Environment.GetEnvironmentVariable("comspec");

                    // Formata a string para passar como argumento para o cmd.exe
                    processo.StartInfo.Arguments = string.Format("/c {0}", comando);

                    processo.StartInfo.RedirectStandardOutput = true;
                    processo.StartInfo.UseShellExecute = false;
                    processo.StartInfo.CreateNoWindow = true;

                    processo.Start();
                    processo.WaitForExit();

                    string saida = processo.StandardOutput.ReadToEnd();
                    return saida;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public void CopiaArquivos(string Origem, string Destino)
        {
            try
            {
                if (!Directory.Exists(Destino))
                {
                    Directory.CreateDirectory(Destino);
                    ExecutarCMD("icacls " + Destino + " /grant " + Environment.UserName + ":F");
                    ExecutarCMD("attrib -r +s " + Destino);
                }

                if (!string.IsNullOrEmpty(Origem))
                {
                    string[] arquivos = Directory.GetFiles(Origem);
                    foreach (string arquivo in arquivos)
                    {
                        File.Copy(Origem + @"\" + System.IO.Path.GetFileName(arquivo), Destino + @"\" + System.IO.Path.GetFileName(arquivo));
                    }

                    string[] diretorios = Directory.GetDirectories(Origem);
                    foreach (string diretorio in diretorios)
                    {
                        CopiaArquivos(diretorio, Destino + @"\" + diretorio.Split(Convert.ToChar(@"\"))[diretorio.Split(Convert.ToChar(@"\")).Length - 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
            }
        }

        public static void Compress(string Origem, string Destino)
        {
            if (!Directory.Exists(Destino))
            {
                Directory.CreateDirectory(Destino);
            }

            string diretorio = Origem;//Caminho do diretório
            string arquivo = Destino + @"\BACKUPJR_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "_" + DateTime.Now.Hour + DateTime.Now.Minute + ".zip";//Caminho do arquivo zip a ser criado
            ZipFile.CreateFromDirectory(diretorio, arquivo);
        }

        public void CopiaOrigemDestino(string origem, string destino)
        {
            ExecutarCMD("cacls " + origem + " /E /P Todos:F");
            ExecutarCMD("cacls " + origem + " /E /P Usuários:F");
            ExecutarCMD("cacls " + origem + " /E /P SISTEMA:F");
            ExecutarCMD("cacls " + origem + " /E /P Users:F");

            if (Directory.Exists(origem))
            {
                CopiaArquivos(origem, destino);
            }
        }

        public void ExecutaBackup()
        {
            try
            {
                if (Directory.Exists( _configBackup.Origem +"tempBackup"))
                {
                    Directory.Delete(_configBackup.Origem + "tempBackup", true);
                }

                CopiaOrigemDestino(_configBackup.Origem + @"gsn", _configBackup.Origem + @"tempBackup\gsn");
                CopiaOrigemDestino(_configBackup.Origem + @"ecf", _configBackup.Origem + @"tempBackup\ecf");
                CopiaOrigemDestino(_configBackup.Origem + @"financeiro", _configBackup.Origem + @"tempBackup\financeiro");
                CopiaOrigemDestino(_configBackup.Origem + @"SisFinanceiro", _configBackup.Origem + @"tempBackup\SisFinanceiro");
                CopiaOrigemDestino(_configBackup.Origem + @"JRSystem", _configBackup.Origem + @"tempBackup\JRSystem");
                CopiaOrigemDestino(_configBackup.Origem + @"mysql\data", _configBackup.Origem + @"tempBackup\banco");
                CopiaOrigemDestino(_configBackup.Origem + @"ctbrural", _configBackup.Origem + @"tempBackup\ctbrural");
                CopiaOrigemDestino(_configBackup.Origem + @"TEF_DIAL", _configBackup.Origem + @"tempBackup\tef_dial");

                try
                {
                    Compress(_configBackup.Origem + "tempBackup", _configBackup.LocalBackup1 + "\\BackupJR");
                }
                catch
                {
                    Compress(_configBackup.Origem + "tempBackup", _configBackup.LocalBackup1 + "\\BackupJR");
                }
                
                if (_configBackup.LocalBackup2 != "0")
                {
                    try
                    {
                        Compress(_configBackup.Origem + "tempBackup", _configBackup.LocalBackup2 + "\\BackupJR");
                    }
                    catch
                    {
                        Compress(_configBackup.Origem + "tempBackup", _configBackup.LocalBackup2 + "\\BackupJR");
                    }
                }

                if (Directory.Exists(_configBackup.Origem + "tempBackup"))
                {
                    Directory.Delete(_configBackup.Origem + "tempBackup", true);
                }

                string caminhoArquivos = _configBackup.LocalBackup1 + "\\BackupJR\\";
                ListarArquivosDiretorioEDeleta(caminhoArquivos);

                if (_configBackup.LocalBackup2 != "0")
                {
                    caminhoArquivos = _configBackup.LocalBackup2 + "\\BackupJR\\";
                    ListarArquivosDiretorioEDeleta(caminhoArquivos);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public void EnviaEmail(string Mensagem, string Assunto)
        {
            try
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", Assunto, "logbackupjr@gmail.com", Mensagem, "");
            }
            catch (Exception ex)
            {
                GeraLogException(ex.ToString());
            }
        }

        public void GeraLogException(string ex)
        {
            StreamWriter vWriter = new StreamWriter(@"c:\gsn\LogBackupJR-Erro.txt", true);

            vWriter.WriteLine("Erro: " + ex.ToString() + "  -  Ocorrido as: " + DateTime.Now.ToString());
            vWriter.Flush();
            vWriter.Close();
        }

        private void ListarArquivosDiretorioEDeleta(string diretorio)
        {
            try
            {
                DirectoryInfo Dir = new DirectoryInfo(diretorio);
                FileInfo[] Files = Dir.GetFiles("BACKUPJR_*");

                Array.Sort(Files, delegate (FileInfo x, FileInfo y)
                {
                    return DateTime.Compare(x.CreationTime, y.CreationTime);
                });

                if (Files.Length > 10)
                {
                    foreach (FileInfo Filer in Files)
                    {
                        if (Filer.Name.Substring(0, 9) == "BACKUPJR_")
                        {
                            DirectoryInfo Dir2 = new DirectoryInfo(diretorio);
                            FileInfo[] Files2 = Dir2.GetFiles("BACKUPJR_*");

                            Array.Sort(Files2, delegate (FileInfo x, FileInfo y)
                            {
                                return DateTime.Compare(x.CreationTime, y.CreationTime);
                            });

                            if (Files2.Length <= 10)
                            {

                            }
                            else
                            {
                                DateTime DataLimite = Filer.CreationTime.AddDays(10);
                                if (DataLimite <= DateTime.Now)
                                {
                                    File.Delete(diretorio + "\\" + Filer.Name);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        private void GeraNotificacao()
        {
            Agendador Tarefa = new Agendador();
            Tarefa.CriarTarefa();
        }
    }
}