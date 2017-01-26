using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Input;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //lê arquivo de configurações:
            string[] ConfigBackup = File.ReadAllLines(@"C:\gsn\ConfigBackup.txt");

            LocalBackup.Text = ConfigBackup[0];
            if (ConfigBackup[1] == "1") { RadioDiario.IsChecked = true; }
            if (ConfigBackup[2] == "1") { RadioDiario.IsChecked = true; }
            if (ConfigBackup[3] == "1") { RadioDiario.IsChecked = true; }
            if (ConfigBackup[4] == "00:00") { Hora1.Text = string.Empty; }
            if (ConfigBackup[5] == "00:00") { Hora2.Text = string.Empty; }
            if (ConfigBackup[6] == "00:00") { Hora3.Text = string.Empty; }
            if (ConfigBackup[7] == "00:00") { Hora4.Text = string.Empty; }
            if (ConfigBackup[8] == "00:00") { Hora5.Text = string.Empty; }
            if (ConfigBackup[9] == "00:00") { Hora6.Text = string.Empty; }
            if (ConfigBackup[10] == "1") { CheckSegunda.IsChecked = true; }
            if (ConfigBackup[11] == "1") { CheckTerca.IsChecked = true; }
            if (ConfigBackup[12] == "1") { CheckQuarta.IsChecked = true; }
            if (ConfigBackup[13] == "1") { CheckQuinta.IsChecked = true; }
            if (ConfigBackup[14] == "1") { CheckSexta.IsChecked = true; }
            if (ConfigBackup[15] == "1") { CheckSabado.IsChecked = true; }
            if (ConfigBackup[16] == "1") { CheckDomingo.IsChecked = true; }
            DiaMes.Text = ConfigBackup[17];
            
            GroupSemanal.IsEnabled = false;
            DiaMes.IsEnabled = false;
        }

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public int count = 0;

        private void UseThread1 ()
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

        public static void Compress()
        {
            string diretorio = @"c:\tempBackup";//Caminho do diretório
            string arquivo = @"c:\BACKUPJR_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "_" + DateTime.Now.Hour + DateTime.Now.Minute + ".zip";//Caminho do arquivo zip a ser criado
            ZipFile.CreateFromDirectory(diretorio, arquivo); 
        }

        public void FazerBackup()
        {
            if (Directory.Exists(@"c:\tempBackup"))
            {
                Directory.Delete(@"c:\tempBackup", true);
            }

            string cliente = "JR PDV SISTEMAS E AUTOMAÇÃO LTDA ME";

            //if (Directory.Exists(@"c:\gsn"))
            //{
            //    CopiaArquivos(@"c:\gsn", @"c:\tempBackup\gsn");
            //}

            if (Directory.Exists(@"c:\ecf"))
            {
                CopiaArquivos(@"c:\ecf", @"c:\tempBackup\ecf");
            }

            //if (Directory.Exists(@"c:\jrsystem"))
            //{
            //    CopiaArquivos(@"c:\jrsystem", @"c:\tempBackup\jrsystem");
            //}

            //if (Directory.Exists(@"c:\mysql1\data"))
            //{
            //    CopiaArquivos(@"c:\mysql\data", @"c:\tempBackup\banco");
            //}

            Compress();

            if (File.Exists(@"c:\gsn\ConfigEmail.txt"))
            {
                File.Delete(@"c:\gsn\ConfigEmail.txt");
            }

            StreamWriter vWriter = new StreamWriter(@"c:\gsn\ConfigEmail.txt", true);
            vWriter.WriteLine("SMTP=smtp.gmail.com");
            vWriter.WriteLine("SSL=True");
            vWriter.WriteLine("REMETENTENOME=" + cliente);
            vWriter.WriteLine("REMETENTEEMAIL=logbackupjr@gmail.com");
            vWriter.WriteLine("REMETENTESENHA=joel0307");
            vWriter.WriteLine("ASSUNTO=REF.: Backup realizado!");
            vWriter.WriteLine("DESTINATARIO=logbackupjr@gmail.com");
            vWriter.WriteLine("CORPOEMAIL=Backup do cliente " + cliente + " Realizado com sucesso!" + DateTime.Now.ToString());
            vWriter.WriteLine("ASSUNTO=REF.: Backup realizado!");
            vWriter.WriteLine("ANEXO=");
            vWriter.Flush();
            vWriter.Close();

            if (IsConnected() == true)
            {
                System.Diagnostics.Process.Start(@"c:\gsn\email.exe");
            }
            else
            {
                MessageBox.Show("sem internet!");
            }

            if (Directory.Exists(@"c:\tempBackup"))
            {
                Directory.Delete(@"c:\tempBackup", true);
            }
        }

        public void CopiaArquivos(string Origem, string Destino)
        {
            if (!Directory.Exists(Destino))
            {
                Directory.CreateDirectory(Destino);
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
                    valorCampo = valorCampo.Substring(0, 2) + ":" + valorCampo.Replace(valorCampo.Substring(0, 2),""); //1230
                }
            }
            return valorCampo;
        }

        [DllImport("wininet.dll")]
        private extern static Boolean InternetGetConnectedState(out int Description, int ReservedValue);

        public static Boolean IsConnected()
        {
            int Description;
            return InternetGetConnectedState(out Description, 0);
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
        }
        
        private void RadioMensal_Checked(object sender, RoutedEventArgs e)
        {
            GroupSemanal.IsEnabled = false;
            DiaMes.IsEnabled = true;
        }
    }
}
