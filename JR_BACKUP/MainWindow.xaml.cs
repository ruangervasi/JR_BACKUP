using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Timers;
using System.Security.AccessControl;
using System.Diagnostics;
using NMDD_EnviaEmail;
using System.Data;
using System.Management;
using System.Text;
using JR_BACKUP.Models;

namespace JR_BACKUP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private DBConn dbMetodos = new DBConn();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        ConfigBackup _configBackup = new ConfigBackup();
        /*public string backuplocal = string.Empty;
        public string backuplocal2 = string.Empty;
        
        public string[] valores = new string[3];
        public string mensagemSucesso;
        public string mensagemErro;
        public string ErroBkpDescricao;
        public string Origem;
        public string Destino1;
        public string Destino2;*/

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(@"c:"))
                {
                    if (!File.Exists(@"C:\gsn\ConfigBackup.ini"))
                    {
                        StreamWriter vWriter = new StreamWriter(@"c:\gsn\ConfigBackup.ini", true);
                        vWriter.WriteLine(@"C:\");
                        vWriter.WriteLine("1");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("18:00");
                        vWriter.WriteLine(":");
                        vWriter.WriteLine(":");
                        vWriter.WriteLine(":");
                        vWriter.WriteLine(":");
                        vWriter.WriteLine(":");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine("1");
                        vWriter.WriteLine(" ");
                        vWriter.WriteLine(" ");
                        vWriter.WriteLine(" ");
                        vWriter.WriteLine("0");
                        vWriter.WriteLine(@"C:\");
                        vWriter.WriteLine("-");
                        vWriter.Flush();
                        vWriter.Close();
                    }

                    //lê arquivo de configurações:
                    if (File.Exists(@"C:\gsn\ConfigBackup.ini"))
                    {
                        string[] ConfigBackup = File.ReadAllLines(@"C:\gsn\ConfigBackup.ini");

                        LocalBackup.Text = ConfigBackup[0];
                        _configBackup.LocalBackup1 = ConfigBackup[0];
                        if (ConfigBackup[4] == ":") { Hora1.Text = string.Empty; } else { Hora1.Text = ConfigBackup[4]; _configBackup.Hora1 = ConfigBackup[4]; }
                        if (ConfigBackup[5] == ":") { Hora2.Text = string.Empty; } else { Hora2.Text = ConfigBackup[5]; _configBackup.Hora2 = ConfigBackup[5]; }
                        if (ConfigBackup[6] == ":") { Hora3.Text = string.Empty; } else { Hora3.Text = ConfigBackup[6]; _configBackup.Hora3 = ConfigBackup[6]; }
                        if (ConfigBackup[7] == ":") { Hora4.Text = string.Empty; } else { Hora4.Text = ConfigBackup[7]; _configBackup.Hora4 = ConfigBackup[7]; }
                        if (ConfigBackup[8] == ":") { Hora5.Text = string.Empty; } else { Hora5.Text = ConfigBackup[8]; _configBackup.Hora5 = ConfigBackup[8]; }
                        if (ConfigBackup[9] == ":") { Hora6.Text = string.Empty; } else { Hora6.Text = ConfigBackup[9]; _configBackup.Hora6 = ConfigBackup[9]; }
                        if (ConfigBackup[18] == "1") { Ativo.IsChecked = true; _configBackup.Ativo = true; }
                        if (ConfigBackup[22] == "0") { LocalBackup2.Text = ""; } else { LocalBackup2.Text = ConfigBackup[22]; _configBackup.LocalBackup2 = ConfigBackup[22]; }
                        if (ConfigBackup[23] == "0") { CbxOrigem.Text = ""; } else { CbxOrigem.Text = ConfigBackup[23]; _configBackup.Origem = ConfigBackup[23]; }
                    }
                    dadosUnidade(LocalBackup.Text.PadLeft(2));
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void UseThread1()
        {
            lblFazendoAguarde.Visibility = Visibility.Visible;
            TestarBackup.IsEnabled = false;
            Cancelar.IsEnabled = false;
            SalvarAlteracoes.IsEnabled = false;
            Ativo.IsEnabled = false;
            LocalBackup.IsEnabled = false;
            LocalBackup2.IsEnabled = false;
            CbxOrigem.IsEnabled = false;
            Hora1.IsEnabled = false;
            Hora2.IsEnabled = false;
            Hora3.IsEnabled = false;
            Hora4.IsEnabled = false;
            Hora5.IsEnabled = false;
            Hora6.IsEnabled = false;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public void btnTestar_Click(object sender, RoutedEventArgs e)
        {

            UseThread1();
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

        public static string ExecutarCMD(string comando)
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

        public void CopiaOrigemDestino(string origem, string destino)
        {
            ExecutarCMD("cacls " + origem  + " /E /P Todos:F");
            ExecutarCMD("cacls " + origem + " /E /P Usuários:F");
            ExecutarCMD("cacls " + origem + " /E /P SISTEMA:F");
            ExecutarCMD("cacls " + origem + " /E /P Users:F");

            if (Directory.Exists(origem))
            {
                CopiaArquivos(origem, destino);
            }
        }

        public void FazerBackup()
        {

            try
            {
                if (Directory.Exists(_configBackup.Origem + "tempBackup"))
                {
                    Directory.Delete(_configBackup.Origem + "tempBackup", true);
                }

                CopiaOrigemDestino(_configBackup.Origem + @"gsn", _configBackup.Origem + @"tempBackup\gsn");
                CopiaOrigemDestino(_configBackup.Origem + @"ecf", _configBackup.Origem + @"tempBackup\ecf");
                CopiaOrigemDestino(_configBackup.Origem + @"financeiro", _configBackup.Origem + @"tempBackup\financeiro");
                CopiaOrigemDestino(_configBackup.Origem + @"SisFinanceiro", _configBackup.Origem + @"tempBackup\SisFinanceiro");
                CopiaOrigemDestino(_configBackup.Origem + @"SisFinanc", _configBackup.Origem + @"tempBackup\SisFinanc");
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

        public void CopiaArquivos(string Origem, string Destino)
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

        private string MascaraHora(string valorCampo)
        {
            if (!valorCampo.Contains(":"))
            {
                if (valorCampo.Length == 3)
                {                                                                              //0123
                    valorCampo = valorCampo.Substring(0, 2) + ":" + valorCampo.Replace(valorCampo.Substring(0, 2), ""); //1230
                }
            }
            return valorCampo;
        }

        private void Hora1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora1.Text = MascaraHora(Hora1.Text);
            Hora1.Select(Hora1.Text.Length, 0);
        }

        private void Hora2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora2.Text = MascaraHora(Hora2.Text);
            Hora2.Select(Hora2.Text.Length, 0);
        }

        private void Hora3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora3.Text = MascaraHora(Hora3.Text);
            Hora3.Select(Hora3.Text.Length, 0);
        }

        private void Hora4_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora4.Text = MascaraHora(Hora4.Text);
            Hora4.Select(Hora4.Text.Length, 0);
        }

        private void Hora5_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora5.Text = MascaraHora(Hora5.Text);
            Hora5.Select(Hora5.Text.Length, 0);
        }

        private void Hora6_TextChanged(object sender, TextChangedEventArgs e)
        {
            Hora6.Text = MascaraHora(Hora6.Text);
            Hora6.Select(Hora6.Text.Length, 0);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            FazerBackup();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TestarBackup.IsEnabled = true;
            Cancelar.IsEnabled = true;
            SalvarAlteracoes.IsEnabled = true;
            Ativo.IsEnabled = true;
            LocalBackup.IsEnabled = true;
            LocalBackup2.IsEnabled = true;
            CbxOrigem.IsEnabled = true;
            Hora1.IsEnabled = true;
            Hora2.IsEnabled = true;
            Hora3.IsEnabled = true;
            Hora4.IsEnabled = true;
            Hora5.IsEnabled = true;
            Hora6.IsEnabled = true;
            lblFazendoAguarde.Visibility = Visibility.Hidden;
            System.Windows.MessageBox.Show("Backup Realizado com Sucesso!","Sucesso",MessageBoxButton.OK,MessageBoxImage.Information);
        }

        private void SalvarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var linha0 = string.Empty; if (LocalBackup.Text != string.Empty) { linha0 = LocalBackup.Text; } else { linha0 = @"c:\"; }
                var linha1 = string.Empty;
                var linha2 = string.Empty;
                var linha3 = string.Empty;
                var linha4 = string.Empty; if (Hora1.Text != "") { linha4 = Hora1.Text; } else { linha4 = "12:00"; }
                var linha5 = string.Empty; if (Hora2.Text != "") { linha5 = Hora2.Text; } else { linha5 = ":"; }
                var linha6 = string.Empty; if (Hora3.Text != "") { linha6 = Hora3.Text; } else { linha6 = ":"; }
                var linha7 = string.Empty; if (Hora4.Text != "") { linha7 = Hora4.Text; } else { linha7 = ":"; }
                var linha8 = string.Empty; if (Hora5.Text != "") { linha8 = Hora5.Text; } else { linha8 = ":"; }
                var linha9 = string.Empty; if (Hora6.Text != "") { linha9 = Hora6.Text; } else { linha9 = ":"; }
                var linha10 = string.Empty;
                var linha11 = string.Empty;
                var linha12 = string.Empty;
                var linha13 = string.Empty;
                var linha14 = string.Empty;
                var linha15 = string.Empty;
                var linha16 = string.Empty;
                var linha17 = string.Empty;
                var linha18 = string.Empty; if (Ativo.IsChecked == true) { linha18 = "1"; } else { linha18 = "0"; }
                var linha19 = string.Empty; 
                var linha20 = string.Empty; 
                var linha21 = string.Empty; 
                var linha22 = string.Empty; if (LocalBackup2.Text != string.Empty) { linha22 = LocalBackup2.Text; } else { linha22 = "0"; }
                var linha23 = @"C:\"; if (CbxOrigem.Text != string.Empty) { linha23 = CbxOrigem.Text; }
                var linha24 = string.Empty;
                var linha25 = string.Empty;

                if (File.Exists(@"c:\gsn\ConfigBackup.ini"))
                {
                    File.Delete(@"c:\gsn\ConfigBackup.ini");
                }

                StreamWriter vWriter = new StreamWriter(@"c:\gsn\ConfigBackup.ini", true);
                vWriter.WriteLine(linha0);
                vWriter.WriteLine(linha1);
                vWriter.WriteLine(linha2);
                vWriter.WriteLine(linha3);
                vWriter.WriteLine(linha4);
                vWriter.WriteLine(linha5);
                vWriter.WriteLine(linha6);
                vWriter.WriteLine(linha7);
                vWriter.WriteLine(linha8);
                vWriter.WriteLine(linha9);
                vWriter.WriteLine(linha10);
                vWriter.WriteLine(linha11);
                vWriter.WriteLine(linha12);
                vWriter.WriteLine(linha13);
                vWriter.WriteLine(linha14);
                vWriter.WriteLine(linha15);
                vWriter.WriteLine(linha16);
                vWriter.WriteLine(linha17);
                vWriter.WriteLine(linha18);
                vWriter.WriteLine(linha19);
                vWriter.WriteLine(linha20);
                vWriter.WriteLine(linha21);
                vWriter.WriteLine(linha22);
                vWriter.WriteLine(linha23);
                vWriter.WriteLine(linha24);
                vWriter.WriteLine(linha25);
                vWriter.Flush();
                vWriter.Close();

                ExecutarCMD("net stop backupjr");
                ExecutarCMD("net start backupjr");

                LabelInfoOther.Content = "Configurações Salvas!";
                dadosUnidade(LocalBackup.Text.PadLeft(2));
            }
            catch
            {
                LabelInfoOther.Content = "Há algo errado, reveja as configurações!!!";
            }


        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void EnviaEmail(string Mensagem, string Assunto)
        {
            EnviaEmail Email = new EnviaEmail();
            Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", Assunto, "logbackupjr@gmail.com", Mensagem, "");
        }

        public void ChooseFolder1()
        {

            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.ShowDialog();
            if (openFolder.SelectedPath != "") LocalBackup.Text = openFolder.SelectedPath;
            dadosUnidade(LocalBackup.Text.PadLeft(2));
        }

        private void dir_Click(object sender, RoutedEventArgs e)
        {
            ChooseFolder1();
        }

        private void dadosUnidade(string unidade)
        {
            try
            {
                StringBuilder varInformacoesVolume = new StringBuilder();
                decimal livre = 0;
                decimal totalEspaco = 0;

                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.Name.Replace("\\", "") == unidade.Replace("\\", ""))
                    {
                        string label = drive.IsReady ? String.Format(" - {0}", drive.VolumeLabel) : null;

                        unidade = drive.Name.Replace("\\", "");

                        ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"" + unidade + "\"");
                        disk.Get();
                        livre = Convert.ToDecimal(disk["FreeSpace"]);
                        livre = livre / 1024 / 1024 / 1024;

                        totalEspaco = Convert.ToDecimal(disk["Size"]);
                        totalEspaco = totalEspaco / 1024 / 1024 / 1024;

                        PGBar.Maximum = Convert.ToDouble(totalEspaco);
                        PGBar.Value = Convert.ToDouble(totalEspaco) - Convert.ToDouble(livre);

                        LivreHD.Content = string.Format("{0:0.##}", livre) + " GB";
                        UsadoHD.Content = string.Format("{0:0.##}", totalEspaco - livre) + " GB";
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
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
            catch (Exception ex)
            {

            }
        }

        private void dir2_Click(object sender, RoutedEventArgs e)
        {
            ChooseFolder2();
        }

        public void ChooseFolder2()
        {

            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.ShowDialog();
            if (openFolder.SelectedPath != "") LocalBackup2.Text = openFolder.SelectedPath;
            dadosUnidade(LocalBackup2.Text.PadLeft(2));
        }

        private void LocalBackup_TextChanged(object sender, TextChangedEventArgs e)
        {
            _configBackup.LocalBackup1 = LocalBackup.Text;
        }

        private void LocalBackup2_TextChanged(object sender, TextChangedEventArgs e)
        {
            _configBackup.LocalBackup2 = LocalBackup2.Text;
        }
    }
}