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
         
        public string backuplocal = string.Empty;
        public string backuplocal2 = string.Empty;
        private DBConn dbMetodos = new DBConn();
        public string[] valores = new string[3];
        public string mensagemSucesso;
        public string mensagemErro;
        public string ErroBkpDescricao;
        public string Origem;
        public string Destino1;
        public string Destino2;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Directory.Exists(@"c:"))
                {

                    if (!File.Exists(@"C:\gsn\ConfigBackup.ini"))
                    {
                        StreamWriter vWriter = new StreamWriter(@"c:\gsn\ConfigBackup.ini", true);
                        vWriter.WriteLine(@"c:\");
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
                        vWriter.WriteLine(@"C:\");
                        vWriter.WriteLine("-");
                        vWriter.WriteLine("-");
                        vWriter.Flush();
                        vWriter.Close();
                    }

                    //lê arquivo de configurações:
                    if (File.Exists(@"C:\gsn\ConfigBackup.ini"))
                    {
                        string[] ConfigBackup = File.ReadAllLines(@"C:\gsn\ConfigBackup.ini");

                        LocalBackup.Text = ConfigBackup[0];
                        if (ConfigBackup[1] == "1") { RadioDiario.IsChecked = true; }
                        if (ConfigBackup[2] == "1") { RadioSemanal.IsChecked = true; GroupSemanal.IsEnabled = true; }
                        if (ConfigBackup[3] == "1") { RadioMensal.IsChecked = true; DiaMes.IsEnabled = true; }
                        if (ConfigBackup[4] == ":") { Hora1.Text = string.Empty; } else { Hora1.Text = ConfigBackup[4]; }
                        if (ConfigBackup[5] == ":") { Hora2.Text = string.Empty; } else { Hora2.Text = ConfigBackup[5]; }
                        if (ConfigBackup[6] == ":") { Hora3.Text = string.Empty; } else { Hora3.Text = ConfigBackup[6]; }
                        if (ConfigBackup[7] == ":") { Hora4.Text = string.Empty; } else { Hora4.Text = ConfigBackup[7]; }
                        if (ConfigBackup[8] == ":") { Hora5.Text = string.Empty; } else { Hora5.Text = ConfigBackup[8]; }
                        if (ConfigBackup[9] == ":") { Hora6.Text = string.Empty; } else { Hora6.Text = ConfigBackup[9]; }
                        if (ConfigBackup[10] == "1") { CheckSegunda.IsChecked = true; }
                        if (ConfigBackup[11] == "1") { CheckTerca.IsChecked = true; }
                        if (ConfigBackup[12] == "1") { CheckQuarta.IsChecked = true; }
                        if (ConfigBackup[13] == "1") { CheckQuinta.IsChecked = true; }
                        if (ConfigBackup[14] == "1") { CheckSexta.IsChecked = true; }
                        if (ConfigBackup[15] == "1") { CheckSabado.IsChecked = true; }
                        if (ConfigBackup[16] == "1") { CheckDomingo.IsChecked = true; }
                        DiaMes.Text = ConfigBackup[17];
                        if (ConfigBackup[18] == "1") { Ativo.IsChecked = true; }
                        valores[0] = ConfigBackup[19];
                        valores[1] = ConfigBackup[20];
                        valores[2] = ConfigBackup[21];
                        if (ConfigBackup[22] == "0") { LocalBackup2.Text = ""; } else { LocalBackup2.Text = ConfigBackup[22]; }
                        backuplocal = ConfigBackup[0];
                        if (ConfigBackup[22] == "0") { backuplocal2 = "0"; } else { backuplocal2 = ConfigBackup[22]; }
                        if (ConfigBackup[23] != string.Empty) { CbxOrigem.Text = ConfigBackup[23]; Origem = ConfigBackup[23]; }
                        if (ConfigBackup[24] != "-") { Destino1 = ConfigBackup[24]; }
                        if (ConfigBackup[25] != "-") { Destino2 = ConfigBackup[25]; }

                    }
                    System.Windows.Controls.DataGrid dgvresult = new System.Windows.Controls.DataGrid();
                    dgvresult.ItemsSource = dbMetodos.RetornaDataView("Select razaosocial,CNPJ_CPF,telefone from empresa LIMIT 1");
                    dgvresult.CanUserAddRows = false;
                    foreach (DataRowView row in dgvresult.Items)
                    {
                        valores[0] = row.Row.ItemArray[0].ToString();
                        valores[1] = row.Row.ItemArray[1].ToString();
                        valores[2] = row.Row.ItemArray[2].ToString();
                    }
                    mensagemSucesso = "Backup Realizado com Sucesso no cliente: \n\nNome: " + valores[0] + "\nCNPJ: " + valores[1] + "\nTelefone: " + valores[2] + "\n\nBackup JR Sistemas";
                    mensagemErro = "\nO erro acima ocorreu no cliente: \n\nNome: " + valores[0] + "\nCNPJ: " + valores[1] + "\nTelefone: " + valores[2] + "\n\nPor favor, verifique!!!\n\nAtt, Backup JR Sistemas";

                    dadosUnidade(LocalBackup.Text.PadLeft(2));
                }
            }
            catch (Exception ex)
            {
                EnviaEmail(ex.ToString() + mensagemErro, "Erro no cliente " + valores[0]);
            }

        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        private void UseThread1()
        {
            TestarBackup.IsEnabled = false;
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
            if (Directory.Exists(origem))
            {
                CopiaArquivos(origem, destino);
            }
        }

        public void ConverteNomeUnidade()
        {

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Destino1 == "-" || Destino1 == "" || Destino1 == null)
                {
                    if (drive.Name.Replace("\\", "") == backuplocal.Replace(backuplocal.Substring(2), ""))
                    {
                        Destino1 = drive.VolumeLabel;
                    }
                }
                else
                {
                    if (drive.VolumeLabel == Destino1)
                    {
                        backuplocal = drive.Name.Replace(@"\", "") + backuplocal.Replace(backuplocal.Substring(2), "");
                    }

                }
            }
        }

        public void ConverteNomeUnidade2()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (Destino2 != "-" || Destino2 == "" || Destino2 == null)
                {
                    if (drive.Name.Replace(@"\", "") == backuplocal2.Replace(backuplocal2.Substring(2), ""))
                    {
                        Destino2 = drive.VolumeLabel;
                    }
                }
                else
                {
                    if (drive.VolumeLabel == Destino2)
                    {
                        backuplocal2 = drive.Name.Replace("\\", "") + backuplocal2.Replace(backuplocal2.Substring(2), "");
                    }

                }
            }
        }

        public void FazerBackup()
        {
            try
            {
                if (Directory.Exists(Origem + "tempBackup"))
                {
                    Directory.Delete(Origem + "tempBackup", true);
                }

                CopiaOrigemDestino(Origem + @"gsn", Origem + @"tempBackup\gsn");
                CopiaOrigemDestino(Origem + @"ecf", Origem + @"tempBackup\ecf");
                CopiaOrigemDestino(Origem + @"financeiro", Origem + @"tempBackup\financeiro");
                CopiaOrigemDestino(Origem + @"SisFinanceiro", Origem + @"tempBackup\SisFinanceiro");
                CopiaOrigemDestino(Origem + @"SisFinanc", Origem + @"tempBackup\SisFinanc");
                CopiaOrigemDestino(Origem + @"JRSystem", Origem + @"tempBackup\JRSystem");
                CopiaOrigemDestino(Origem + @"mysql\data", Origem + @"tempBackup\banco");
                CopiaOrigemDestino(Origem + @"ctbrural", Origem + @"tempBackup\ctbrural");
                CopiaOrigemDestino(Origem + @"TEF_DIAL", Origem + @"tempBackup\tef_dial");

                try
                {
                    Compress(Origem + "tempBackup", backuplocal + "\\BackupJR");
                }
                catch
                {
                    ConverteNomeUnidade();
                    Compress(Origem + "tempBackup", backuplocal + "\\BackupJR");
                }

                if (backuplocal2 != "0")
                {
                    try
                    {
                        Compress(Origem + "tempBackup", backuplocal2 + "\\BackupJR");
                    }
                    catch
                    {
                        ConverteNomeUnidade2();
                        Compress(Origem + "tempBackup", backuplocal2 + "\\BackupJR");
                    }
                }

                if (Directory.Exists(Origem + "tempBackup"))
                {
                    Directory.Delete(Origem + "tempBackup", true);
                }

                string caminhoArquivos = backuplocal + "\\BackupJR\\";
                ListarArquivosDiretorioEDeleta(caminhoArquivos);

                if (backuplocal2 != "0")
                {
                    caminhoArquivos = backuplocal2 + "\\BackupJR\\";
                    ListarArquivosDiretorioEDeleta(caminhoArquivos);
                }

                JRBackup_Service.Agendador Tarefa = new JRBackup_Service.Agendador();
                Tarefa.CriarTarefa();
            }
            catch (Exception e)
            {
                EnviaEmail(e.ToString() + mensagemErro, "Erro no Backup cliente " + valores[0]);
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
        }

        private void RadioDiario_Checked(object sender, RoutedEventArgs e)
        {
            GroupSemanal.IsEnabled = false;
            DiaMes.IsEnabled = false;
        }

        private void RadioSemanal_Checked(object sender, RoutedEventArgs e)
        {
            GroupSemanal.IsEnabled = true;
            DiaMes.IsEnabled = false;
        }

        private void RadioMensal_Checked(object sender, RoutedEventArgs e)
        {
            GroupSemanal.IsEnabled = false;
            DiaMes.IsEnabled = true;
        }

        private void SalvarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            LabelInfoOther.Content = "...";
            try
            {
                var linha0 = string.Empty; if (LocalBackup.Text != string.Empty) { linha0 = LocalBackup.Text; } else { linha0 = @"c:\"; }
                var linha1 = string.Empty; if (RadioDiario.IsChecked == true) { linha1 = "1"; } else if (RadioDiario.IsChecked == false && RadioSemanal.IsChecked == false && RadioMensal.IsChecked == false) { linha1 = "1"; } else { linha1 = "0"; }
                var linha2 = string.Empty; if (RadioSemanal.IsChecked == true) { linha2 = "1"; } else { linha2 = "0"; }
                var linha3 = string.Empty; if (RadioMensal.IsChecked == true) { linha3 = "1"; } else { linha3 = "0"; }
                var linha4 = string.Empty; if (Hora1.Text != "") { linha4 = Hora1.Text; } else { linha4 = "12:00"; }
                var linha5 = string.Empty; if (Hora2.Text != "") { linha5 = Hora2.Text; } else { linha5 = ":"; }
                var linha6 = string.Empty; if (Hora3.Text != "") { linha6 = Hora3.Text; } else { linha6 = ":"; }
                var linha7 = string.Empty; if (Hora4.Text != "") { linha7 = Hora4.Text; } else { linha7 = ":"; }
                var linha8 = string.Empty; if (Hora5.Text != "") { linha8 = Hora5.Text; } else { linha8 = ":"; }
                var linha9 = string.Empty; if (Hora6.Text != "") { linha9 = Hora6.Text; } else { linha9 = ":"; }
                var linha10 = string.Empty; if (CheckSegunda.IsChecked == true && GroupSemanal.IsEnabled == true) { linha10 = "1"; } else { linha10 = "0"; }
                var linha11 = string.Empty; if (CheckTerca.IsChecked == true && GroupSemanal.IsEnabled == true) { linha11 = "1"; } else { linha11 = "0"; }
                var linha12 = string.Empty; if (CheckQuarta.IsChecked == true && GroupSemanal.IsEnabled == true) { linha12 = "1"; } else { linha12 = "0"; }
                var linha13 = string.Empty; if (CheckQuinta.IsChecked == true && GroupSemanal.IsEnabled == true) { linha13 = "1"; } else { linha13 = "0"; }
                var linha14 = string.Empty; if (CheckSexta.IsChecked == true && GroupSemanal.IsEnabled == true) { linha14 = "1"; } else { linha14 = "0"; }
                var linha15 = string.Empty; if (CheckSabado.IsChecked == true && GroupSemanal.IsEnabled == true) { linha15 = "1"; } else { linha15 = "0"; }
                var linha16 = string.Empty; if (CheckDomingo.IsChecked == true && GroupSemanal.IsEnabled == true) { linha16 = "1"; } else { linha16 = "0"; }
                var linha17 = string.Empty; if (DiaMes.Text != "") { linha17 = DiaMes.Text; } else { linha17 = "0"; }
                var linha18 = string.Empty; if (Ativo.IsChecked == true) { linha18 = "1"; } else { linha18 = "0"; }
                var linha19 = string.Empty; linha19 = valores[0];
                var linha20 = string.Empty; linha20 = valores[1];
                var linha21 = string.Empty; linha21 = valores[2];
                var linha22 = string.Empty; if (LocalBackup2.Text != string.Empty) { linha22 = LocalBackup2.Text; } else { linha22 = "0"; }
                var linha23 = @"C:\"; if (CbxOrigem.Text != string.Empty) { linha23 = CbxOrigem.Text; }
                ConverteNomeUnidade();
                ConverteNomeUnidade2();
                var linha24 = Destino1;
                var linha25 = Destino2;

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
                EnviaEmail(ex.ToString() + mensagemErro, "Erro no Backup cliente " + valores[0]);
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
                EnviaEmail(ex.ToString() + mensagemErro, "Erro no Backup cliente " + valores[0]);
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
            backuplocal = LocalBackup.Text;
        }

        private void LocalBackup2_TextChanged(object sender, TextChangedEventArgs e)
        {
            backuplocal2 = LocalBackup2.Text;
        }
    }
}