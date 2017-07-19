using JR_BACKUP;
using NMDD_EnviaEmail;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using TaskScheduler;

namespace JRBackup_Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        public string LocalBackup = string.Empty;
        public string LocalBackup2 = string.Empty;
        public string Diario = string.Empty;
        public string Semanal = string.Empty;
        public string Mensal = string.Empty;
        public string Hora1 = string.Empty;
        public string Hora2 = string.Empty;
        public string Hora3 = string.Empty;
        public string Hora4 = string.Empty;
        public string Hora5 = string.Empty;
        public string Hora6 = string.Empty;
        public string Segunda = string.Empty;
        public string Terca = string.Empty;
        public string Quarta = string.Empty;
        public string Quinta = string.Empty;
        public string Sexta = string.Empty;
        public string Sabado = string.Empty;
        public string Domingo = string.Empty;
        public string DiaMes = string.Empty;
        public string Ativo = string.Empty;
        private DBConn dbMetodos = new DBConn();
        public string[] valores = new string[3];
        public string mensagemSucesso;
        public string mensagemErro;
        public string ErroBkpDescricao;
        public string Origem;
        public string Destino1;
        public string Destino2;

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
                if (Ativo == "1")
                {
                    if (hora.Hour.ToString().Length == 1)
                    {
                        PegaHora = "0" + hora.Hour.ToString();
                    }
                    else
                    {
                        PegaHora = hora.Hour.ToString();
                    }

                    if (hora.Minute.ToString().Length == 1)
                    {
                        PegaMinuto = "0" + hora.Minute.ToString();
                    }
                    else
                    {
                        PegaMinuto = hora.Minute.ToString();
                    }

                    string HoraAtual = PegaHora + ":" + PegaMinuto;


                    if (Diario == "1")
                    {
                        VerificaHora(HoraAtual);
                    }
                    if (Semanal == "1")
                    {
                        VerificaSemanal(HoraAtual);
                    }
                    if (Mensal == "1")
                    {
                        VerificaMensal(HoraAtual);
                    }
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

                    LocalBackup = ConfigBackup[0];
                    Diario = ConfigBackup[1];
                    Semanal = ConfigBackup[2];
                    Mensal = ConfigBackup[3];
                    Hora1 = ConfigBackup[4];
                    Hora2 = ConfigBackup[5];
                    Hora3 = ConfigBackup[6];
                    Hora4 = ConfigBackup[7];
                    Hora5 = ConfigBackup[8];
                    Hora6 = ConfigBackup[9];
                    if (ConfigBackup[10] == "1") { Segunda = "Monday"; } else { Segunda = "0"; };
                    if (ConfigBackup[10] == "1") { Terca = "Tuesday"; } else { Terca = "0"; };
                    if (ConfigBackup[10] == "1") { Quarta = "Wednesday"; } else { Quarta = "0"; };
                    if (ConfigBackup[10] == "1") { Quinta = "Thursday"; } else { Quinta = "0"; };
                    if (ConfigBackup[10] == "1") { Sexta = "Friday"; } else { Sexta = "0"; };
                    if (ConfigBackup[10] == "1") { Sabado = "Saturday"; } else { Sabado = "0"; };
                    if (ConfigBackup[10] == "1") { Domingo = "Sunday"; } else { Domingo = "0"; };
                    DiaMes = ConfigBackup[17];
                    Ativo = ConfigBackup[18];
                    valores[0] = ConfigBackup[19];
                    valores[1] = ConfigBackup[20];
                    valores[2] = ConfigBackup[21];
                    LocalBackup2 = ConfigBackup[22];
                    Origem = ConfigBackup[23];
                    Destino1 = ConfigBackup[24];
                    Destino2 = ConfigBackup[25];

                }

                mensagemSucesso = "Backup Realizado com Sucesso no cliente: \n\nNome: " + valores[0] + "\nCNPJ: " + valores[1] + "\nTelefone: " + valores[2] + "\n\nBackup JR Sistemas";
                mensagemErro = "\nO erro acima ocorreu no cliente: \n\nNome: " + valores[0] + "\nCNPJ: " + valores[1] + "\nTelefone: " + valores[2] + "\n\nPor favor, verifique!!!\n\nAtt, Backup JR Sistemas";
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
                StreamWriter vWriter = new StreamWriter(@"c:\testeServico.txt", true);

                vWriter.WriteLine("Servico Parado: " + DateTime.Now.ToString());
                vWriter.Flush();
                vWriter.Close();
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
                if (HoraAtual == Hora1)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == Hora2)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == Hora3)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == Hora4)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == Hora5)
                {
                    ExecutaBackup();
                }
                if (HoraAtual == Hora6)
                {
                    ExecutaBackup();
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
            }
        }

        public void VerificaSemanal(string HoraAtual)
        {
            try
            {
                DateTime DiaDaSemana = DateTime.Now;
                string _DiaDaSemana = DiaDaSemana.DayOfWeek.ToString();

                if (_DiaDaSemana == Segunda)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Terca)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Quarta)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Quinta)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Sexta)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Sabado)
                {
                    VerificaHora(HoraAtual);
                }
                if (_DiaDaSemana == Domingo)
                {
                    VerificaHora(HoraAtual);
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
            }
        }

        public void VerificaMensal(string HoraAtual)
        {
            try
            {
                DateTime date = DateTime.Now;
                var TotalDiasMes = DateTime.DaysInMonth(date.Year, date.Month);
                string DiaAtual = date.Date.Day.ToString();

                if (TotalDiasMes < Convert.ToInt32(DiaMes))
                {
                    if (Convert.ToInt32(DiaAtual) == TotalDiasMes)
                    {
                        VerificaHora(HoraAtual);
                    }
                }
                else if (DiaAtual == DiaMes)
                {
                    VerificaHora(HoraAtual);
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString(), "Erro no Backup: ");
                GeraLogException(ex.ToString());
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
            if (Directory.Exists(origem))
            {
                CopiaArquivos(origem, destino);
            }
        }

        public void ExecutaBackup()
        {
            try
            {
                if (Directory.Exists( Origem +"tempBackup"))
                {
                    Directory.Delete(Origem + "tempBackup", true);
                }

                CopiaOrigemDestino(Origem + @"gsn", Origem + @"tempBackup\gsn");
                CopiaOrigemDestino(Origem + @"ecf", Origem + @"tempBackup\ecf");
                CopiaOrigemDestino(Origem + @"financeiro", Origem + @"tempBackup\financeiro");
                CopiaOrigemDestino(Origem + @"SisFinanceiro", Origem + @"tempBackup\SisFinanceiro");
                CopiaOrigemDestino(Origem + @"JRSystem", Origem + @"tempBackup\JRSystem");
                CopiaOrigemDestino(Origem + @"mysql\data", Origem + @"tempBackup\banco");
                CopiaOrigemDestino(Origem + @"ctbrural", Origem + @"tempBackup\ctbrural");
                CopiaOrigemDestino(Origem + @"TEF_DIAL", Origem + @"tempBackup\tef_dial");

                try
                {
                    Compress(Origem + "tempBackup", LocalBackup + "\\BackupJR");
                }
                catch
                {
                    ConverteNomeUnidade();
                    Compress(Origem + "tempBackup", LocalBackup + "\\BackupJR");
                }
                
                if (LocalBackup2 != "0")
                {
                    try
                    {
                        Compress(Origem + "tempBackup", LocalBackup2 + "\\BackupJR");
                    }
                    catch
                    {
                        ConverteNomeUnidade2();
                        Compress(Origem + "tempBackup", LocalBackup2 + "\\BackupJR");
                    }
                }

                if (Directory.Exists(Origem + "tempBackup"))
                {
                    Directory.Delete(Origem + "tempBackup", true);
                }

                string caminhoArquivos = LocalBackup + "\\BackupJR\\";
                ListarArquivosDiretorioEDeleta(caminhoArquivos);

                if (LocalBackup2 != "0")
                {
                    caminhoArquivos = LocalBackup2 + "\\BackupJR\\";
                    ListarArquivosDiretorioEDeleta(caminhoArquivos);
                }
            }
            catch (Exception e)
            {
                EnviaEmail(e.ToString() + mensagemErro, "Erro no Backup cliente " + valores[0]);
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
                EnviaEmail(e.ToString() + mensagemErro, "Erro no Backup cliente " + valores[0]);
            }
        }

        private void GeraNotificacao()
        {
            Agendador Tarefa = new Agendador();
            Tarefa.CriarTarefa();
        }

        public void ConverteNomeUnidade()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Destino1 != "-" || Destino1 != "" || Destino1 != null)
                {
                    if (drive.VolumeLabel == Destino1)
                    {
                        LocalBackup = drive.Name.Replace(@"\", "") + LocalBackup.Replace(LocalBackup.Substring(2), "");
                    }
                }
            }
        }

        public void ConverteNomeUnidade2()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Destino2 != "-" || Destino2 != "" || Destino2 != null)
                {
                    if (drive.VolumeLabel == Destino2)
                    {
                        LocalBackup2 = drive.Name.Replace("\\", "") + LocalBackup2.Replace(LocalBackup2.Substring(2), "");
                    }
                }
            }
        }
    }
}