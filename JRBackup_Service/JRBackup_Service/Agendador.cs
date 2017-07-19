using NMDD_EnviaEmail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler;

namespace JRBackup_Service
{
    public class Agendador
    {
        TaskScheduler.TaskScheduler oAgendador;
        //Para tratar a definição da tarefa
        ITaskDefinition oDefinicaoTarefa;
        //Para tratar a informação do Trigger
        ITimeTrigger oTrigger;
        //Para tratar a informação da Ação
        IExecAction oAcao;

        public void CriarTarefa()
        {
            try
            {
                oAgendador = new TaskScheduler.TaskScheduler();
                oAgendador.Connect();

                //Atribuindo Definição de tarefa
                AtribuiDefinicaoTarefa();
                //Definindo a informação do gatilho da tarefa
                DefineInformacaoGatilho();
                //Definindo a informção da ação da tarefa
                DefineInformacaoAcao();

                //Obtendo a pasta raiz
                ITaskFolder root = oAgendador.GetFolder("\\");
                //Registrando a tarefa , se a tarefa ja estiver registrada então ela será atualizada
                IRegisteredTask regTask = root.RegisterTaskDefinition("BK_JR_Sucesso", oDefinicaoTarefa, (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE, System.Security.Principal.WindowsIdentity.GetCurrent().Name, null, _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN, "");

                //Para executar a tarefa imediatamenteo chamamos o método Run()
                //IRunningTask runtask = regTask.Run(null);
                //exibe mensagem
            }
            catch (Exception ex)
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", "Erro ao criar tarefa", "logbackupjr@gmail.com", "Erro ao criar tarefa: \n" + ex.ToString(), "");
            }
        }

        private void AtribuiDefinicaoTarefa()
        {
            try
            {
                oDefinicaoTarefa = oAgendador.NewTask(0);
                //Registra informação para a tarefa
                //nome do autor da tarefa
                oDefinicaoTarefa.RegistrationInfo.Author = "JR Sistemas";
                //descrição da tarefa
                oDefinicaoTarefa.RegistrationInfo.Description = "BK_JR_Sucesso";
                //Registro da data da tarefa
                oDefinicaoTarefa.RegistrationInfo.Date = DateTime.Now.AddSeconds(60).ToString("yyyy-MM-ddTHH:mm:ss"); //formatacao

                //Definição da tarefa
                //Prioridade da Thread
                oDefinicaoTarefa.Settings.Priority = 7;
                //Habilita a tarefa
                oDefinicaoTarefa.Settings.Enabled = true;
                //Para ocultar/exibir a tarefa
                oDefinicaoTarefa.Settings.Hidden = false;
                //Tempo de execução limite para a tarefa
                oDefinicaoTarefa.Settings.ExecutionTimeLimit = "PT10M"; //10 minutos
                //Define que não precisa de conexão de rede
                oDefinicaoTarefa.Settings.RunOnlyIfNetworkAvailable = false;

            }
            catch (Exception ex)
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", "Erro ao criar tarefa", "logbackupjr@gmail.com", "Erro ao criar tarefa: \n" + ex.ToString(), "");

            }
        }

        //Definindo a informação do Gatilho (Trigger)
        private void DefineInformacaoGatilho()
        {
            try
            {
                //informação do gatilho baseada no tempo - TASK_TRIGGER_TIME
                oTrigger = (ITimeTrigger)oDefinicaoTarefa.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
                //ID do Trigger
                oTrigger.Id = "Trigger_Da_Tarefa";
                //hora de inicio
                oTrigger.StartBoundary = DateTime.Now.AddSeconds(5).ToString("yyyy-MM-ddTHH:mm:ss");  //yyyy-MM-ddTHH:mm:ss

                //hora de encerramento
                oTrigger.EndBoundary = DateTime.Now.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss");  //yyyy-MM-ddTHH:mm:ss
            }
            catch (Exception ex)
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", "Erro ao criar tarefa", "logbackupjr@gmail.com", "Erro ao criar tarefa: \n" + ex.ToString(), "");
            }
        }

        //Define a informação da Ação da tarefa
        private void DefineInformacaoAcao()
        {
            try
            {
                //Informação da Ação baseada no exe- TASK_ACTION_EXEC
                oAcao = (IExecAction)oDefinicaoTarefa.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
                //ID da Ação
                oAcao.Id = "testeAcao1";
                //Define o caminho do arquivo EXE a executar (Vamos abrir o Paint)

                if (File.Exists(@"C:\Program Files (x86)\JR Sistemas\JR Backup\Notificador.exe"))
                {
                    oAcao.Path = @"C:\Program Files (x86)\JR Sistemas\JR Backup\Notificador.exe";
                }

                if (File.Exists(@"C:\Program Files\JR Sistemas\JR Backup\Notificador.exe"))
                {
                    oAcao.Path = @"C:\Program Files\JR Sistemas\JR Backup\Notificador.exe";
                }

                if (File.Exists(@"C:\Arquivos de Programas\JR Sistemas\JR Backup\Notificador.exe"))
                {
                    oAcao.Path = @"C:\Arquivos de Programas\JR Sistemas\JR Backup\Notificador.exe";
                }
            }
            catch (Exception ex)
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", "Erro ao criar tarefa", "logbackupjr@gmail.com", "Erro ao criar tarefa: \n" + ex.ToString(), "");
            }
        }

        public void DeletarTarefa()
        {
            try
            {
                //cria instância do agendador
                TaskScheduler.TaskScheduler oAgendador = new TaskScheduler.TaskScheduler();
                oAgendador.Connect();

                ITaskFolder containingFolder = oAgendador.GetFolder("\\");
                //Deleta a tarefa
                containingFolder.DeleteTask("BK_JR_Sucesso", 0);  //da o nome da tarefa que foi criada
            }
            catch (Exception ex)
            {
                EnviaEmail Email = new EnviaEmail();
                Email.SendEmail("smtp.gmail.com", true, "BackupJR LOG", "logbackupjr@gmail.com", "joel0307", "Erro ao criar tarefa", "logbackupjr@gmail.com", "Erro ao criar tarefa: \n" + ex.ToString(), "");
            }
        }
    }
}
