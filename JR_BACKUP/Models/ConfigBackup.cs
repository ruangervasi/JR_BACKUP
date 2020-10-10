using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JR_BACKUP.Models
{
    class ConfigBackup
    {
        public bool Ativo { get; set; }
        public string Origem { get; set; }
        public string LocalBackup1 { get; set; }
        public string LocalBackup2 { get; set; }
        public string Hora1 { get; set; }
        public string Hora2 { get; set; }
        public string Hora3 { get; set; }
        public string Hora4 { get; set; }
        public string Hora5 { get; set; }
        public string Hora6 { get; set; }

        public ConfigBackup(bool ativo, string origem, string localbackup1, string localbackup2, string hora1, string hora2, string hora3, string hora4, string hora5, string hora6)
        {
            Ativo = ativo;
            Origem = origem;
            LocalBackup1 = localbackup1;
            LocalBackup2 = localbackup2;
            Hora1 = hora1;
            Hora2 = hora2;
            Hora3 = hora3;
            Hora4 = hora4;
            Hora5 = hora5;
            Hora6 = hora6;
        }

        public ConfigBackup()
        {

        }
    }
}
