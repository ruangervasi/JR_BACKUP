using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Resources;

namespace Notificador
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            NotifyIcon Notificacao = new NotifyIcon();
            Notificacao.Icon = new Icon(System.Windows.Forms.Application.StartupPath + "/backupjr.ico");
            Notificacao.BalloonTipIcon = ToolTipIcon.Info;
            Notificacao.Visible = true;
            Notificacao.Text = "This is a test";
            Notificacao.BalloonTipTitle = "Backup JR";
            Notificacao.BalloonTipText = "Backup Realizado com sucesso!!!";
            Notificacao.ShowBalloonTip(10000);

            JRBackup_Service.Agendador Tarefa = new JRBackup_Service.Agendador();
            //Tarefa.DeletarTarefa();

            this.Close();
        }
    }
}
