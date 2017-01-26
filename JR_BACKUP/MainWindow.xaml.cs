using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Compression; 

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

        public int count = 0;

        public void btnTestar_Click(object sender, RoutedEventArgs e)
        {            
            string cliente = "JR PDV SISTEMAS E AUTOMAÇÃO LTDA ME";

            if (Directory.Exists(@"c:\gsn"))
            {
                FazerBackup(@"c:\gsn", @"c:\tempBackup\gsn");
            }

            if (Directory.Exists(@"c:\ecf"))
            {
                FazerBackup(@"c:\ecf", @"c:\tempBackup\ecf");
            }

            if (Directory.Exists(@"c:\jrsystem"))
            {
                FazerBackup(@"c:\jrsystem", @"c:\tempBackup\jrsystem");
            }

            if (Directory.Exists(@"c:\mysql\data"))
            {
                FazerBackup(@"c:\mysql\data", @"c:\tempBackup\banco");
            }
            
            Compress();

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

            System.Diagnostics.Process.Start(@"c:\gsn\email.exe");

            if (File.Exists(@"c:\gsn\ConfigEmail.txt"))
            {
                 File.Delete(@"c:\gsn\ConfigEmail.txt");
            }

            //if (Directory.Exists(@"c:\tempBackup"))
            //{
            //     Directory.Delete(@"c:\tempBackup", true);
            // }
        }

        public static void Compress()
        {
            string diretorio = @"c:\tempBackup";//Caminho do diretório
            string arquivo = @"c:\BACKUPJR_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + "_" + DateTime.Now.Hour + DateTime.Now.Minute + ".zip";//Caminho do arquivo zip a ser criado
            ZipFile.CreateFromDirectory(diretorio, arquivo); 
        }

        public void FazerBackup(string Origem, string Destino)
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
                    FazerBackup(diretorio, Destino + @"\" + diretorio.Split(Convert.ToChar(@"\"))[diretorio.Split(Convert.ToChar(@"\")).Length - 1]);
                }

            }

        }

        private string MascaraHora(string valorCampo)
        {
            if (count == 2)
            {
                if (!valorCampo.Contains(":"))
                {
                    if (valorCampo.Length == 2)
                    {
                        valorCampo += ":";
                        count++;
                    }
                }
            }
            else { count = 0; };
            return valorCampo;
        }     

        private void Hora1_TextChanged(object sender, TextChangedEventArgs e)
        {
                Hora1.Text = MascaraHora(Hora1.Text);
                Hora1.Select(Hora1.Text.Length, 0);
        }
    }
}
