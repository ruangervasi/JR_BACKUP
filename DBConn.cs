using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Data;
using System.Net.Mail;

namespace DBConn
{
    public class DBConnMYSQL
    {

        /* Instruções de como usar os metodos
         
         * Para usar o metodo de inserção de registro sem retorno do ID veja o Exemplo abaixo:
         * string[] colunas = { "'" + txtNome.Text + "'", "'" + txtCPF.Text + "'", "'" + txtRG.Text + "'", "'" + txtEmail.Text + "'", "'" + txtTelefone.Text + "'", "'" + txtEndereco.Text + "'", "'" + txtNum.Text + "'", "'" + txtComplemento.Text + "'", "'" + txtBairro.Text + "'", "'" + txtCidade.Text + "'", "'" + txtEstado.Text + "'", "'" + txtCep.Text + "'", "'" + DateTime.Now.ToShortDateString() + "'" };
         * lblMsg.Text = metodos.AddRegistro("sys_contatos(nome,cpf,rg,email,telefone,endereco,num,complemento,bairro,cidade,estado,cep,dataCadastro) VALUES(", colunas);

         * Para usar o metodo de inserção de registro com retorno de ID veja o Exemplo abaixo:
         * string[] colunas = {txtNome.Text, txtCPF.Text,txtRG.Text,txtEmail.Text,txtTelefone.Text,txtEndereco.Text,txtNum.Text,
         *                      txtComplemento.Text,txtBairro.Text,txtCidade.Text,txtEstado.Text,txtCep.Text,txtEmpresa.Text,DateTime.Now.ToShortDateString(),txtAniversario.Text};
         * lblMsg.Text = metodos.AddRegistroRetornoID("sys_contatos(nome,cpf,rg,email,telefone,endereco,num,complemento,bairro,cidade,estado,cep,empresa,dataCadastro,dataAniversario) VALUES(@nome,@cpf,@rg,@email,@telefone,@endereco,@num,@complemento,@bairro,@cidade,@estado,@cep,@empresa,@dataCadastro,@dataAniversario", colunas);
         
         * Para usar o metodo de RetornoDataView (Retornar valores em um DataView) veja o Exemplo abaixo:
         * objeto.DataSource = metodos.RetornaDataView("select * from sys_departamentos");
         
         * Para usar o metodo de UpRegistro (Atualizar registros) veja o Exemplo abaixo:
         * string_recebendoRetorno = metodos.UpRegistro("sys_users set ", "password", txtSenha2.Text, Session["userID"].ToString());
         
         * Para usar o metodo de DelRegistro (Remover registros) veja o Exemplo abaixo:
         * string_recebendoRetorno = metodos.DelRegistro("sol_atividades", gvAllAtividades.SelectedRow.Cells[0].Text);

         */

        //String de conexão ao arquivo de banco de dados local (SQL Server Compact Edition 3.5)
        private string sqlStrCon = @"Data Source= gsn=dsn=gsn; server=localhost; database=gsn;"; //String com proteção por senha
        //private string sqlStrCon = @"Data Source= C:\\Users\\Djoni Carvalho\\Desktop\\IAPP_DBDietModel.sdf"; //String com proteção por senha
        //private string sqlStrCon = @"Data Source= .\IAPP_DBDietModel.sdf"; //String sem proteção por senha
        //private string sqlStrCon = @"Data Source= C:\\Users\\ruan.gervasi\\Desktop\\IAPP_DBDietModel.sdf"; //String sem proteção por senha

        //Metodo para inserir registro sem retorno do ID
        public string AddRegistro(string strComplementoQuerySql, string[] strValores)
        {
            string msg = null;

            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;
            SqlCeCommand addRegistro = null;

            try
            {
                int count = 0;
                int totalItens = strValores.Length;

                string valores = null;

                while (count < totalItens)
                {
                    valores += strValores[count];

                    if (count != (totalItens - 1))
                    {
                        valores += ",";
                    }

                    count++;
                }

                valores += ")";

                addRegistro = new SqlCeCommand("INSERT INTO " + strComplementoQuerySql + valores, SqlCon);
                SqlCon.Open();
                addRegistro.ExecuteNonQuery();

                msg = "Registro adicionado com sucesso!";

            }
            catch (Exception ex)
            {
                msg = "Erro: " + ex.Message;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return msg;


        }

        //Metodo para inserir registro com retorno do ID
        public string AddRegistroRetornoID(string strComplementoQuerySql, string[] strValores)
        {
            string msg = null;
            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;
            SqlCeCommand addRegistro = null;

            try
            {
                int count = 0;
                int totalItens = strValores.Length;

                string valores = null;

                while (count < totalItens)
                {
                    valores += strValores[count];

                    if (count != (totalItens - 1))
                    {
                        valores += ",";
                    }

                    count++;
                }

                valores += ")";

                SqlCon.Open();
                addRegistro = SqlCon.CreateCommand();
                addRegistro.CommandText = "INSERT INTO " + strComplementoQuerySql + valores;

                addRegistro.ExecuteNonQuery();
                addRegistro.CommandText = "SELECT @@IDENTITY";

                var retorno = addRegistro.ExecuteScalar();
                msg = retorno.ToString();

            }
            catch (Exception ex)
            {
                msg = "Erro: " + ex.Message;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }

            }

            return msg;

        }

        //Metodo para retornar DataView para popular Grids e Combo
        public DataView RetornaDataView(string strQuerySql)
        {
            DataSet sqlDataSet = new DataSet();
            string strCon = sqlStrCon;

            try
            {
                SqlCeConnection SqlCon = new SqlCeConnection();
                SqlCon.ConnectionString = strCon;

                SqlCon.Open();

                SqlCeDataAdapter sqlDA = new SqlCeDataAdapter();

                sqlDA.SelectCommand = new SqlCeCommand(strQuerySql, SqlCon);


                sqlDA.Fill(sqlDataSet, "temp");

            }
            catch (Exception ex)
            {

            }

            return sqlDataSet.Tables["temp"].DefaultView;

        }

        //Metodo para atualizar registro passando ID
        public string UpRegistro(string tabela, string colunas, string valores, string id)
        {
            string msg = null;
            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;
            SqlCeCommand upRegistro = null;

            try
            {
                //Colunas (quais as colunas que serão atualizadas?)
                string[] arrColunas = colunas.Split(',');

                //Valores das colunas que serão atualizadas (valores provenientes dos Textbox).
                string[] arrValores = valores.Split(',');
                string[] arrValoresNNulos = null;

                //Separando valores vazios de arrValores e adicionando em arrValoresNNulos
                int count = 0;
                int totalValores = arrValores.Length;
                Hashtable linhaTotal = new Hashtable();
                ArrayList indices = new ArrayList();


                while (count < totalValores)
                {
                    if (arrValores[count] != "")
                    {
                        linhaTotal.Add(count, arrValores[count]);
                        indices.Add(count);
                    }

                    count++;
                }

                string linhaUpdate = null;
                int totalLinhaTotal = linhaTotal.Count;
                int totalIndices = indices.Count;
                count = 0;

                foreach (int item in indices)
                {
                    if (count == (totalIndices - 1))
                    {
                        linhaUpdate += arrColunas[Convert.ToInt32(item)] + "='" + linhaTotal[item].ToString() + "'";
                    }
                    else
                    {
                        linhaUpdate += arrColunas[Convert.ToInt32(item)] + "='" + linhaTotal[item].ToString() + "',";
                    }

                    count++;
                }

                upRegistro = new SqlCeCommand("UPDATE " + tabela + linhaUpdate + " WHERE id=" + id, SqlCon);
                SqlCon.Open();
                upRegistro.ExecuteNonQuery();

                msg = "Registro atualizado com sucesso!";

            }
            catch (Exception ex)
            {
                msg = "Erro: " + ex.Message;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return msg;
        }

        //Metodo para remover registro passando ID
        public string DelRegistro(string tabela, string id)
        {
            string msg = null;

            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;
            SqlCeCommand delRegistro = null;

            try
            {
                delRegistro = new SqlCeCommand("DELETE FROM " + tabela + " WHERE id=" + id, SqlCon);
                SqlCon.Open();
                delRegistro.ExecuteNonQuery();

                msg = "Registro removido com sucesso!";

            }
            catch (Exception ex)
            {
                msg = "Erro: " + ex.Message;
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return msg;


        }

        //Metodo para adicionar valores em colunas do tipo DateTime no banco SQL
        //public void updateDate(string tabela, string campo, DateTime data, string id)

        public void updateDate(string tabela, string campo, string id, DateTime data)
        {
            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;

            try
            {
                //SqlCeCommand addRegistro = new SqlCeCommand("UPDATE tb_Pacientes SET dataNascimento=@value WHERE id=35", SqlCon);
                SqlCeCommand addRegistro = new SqlCeCommand("UPDATE " + tabela + " SET " + campo + "=@value WHERE id=" + id + "", SqlCon);
                SqlCeParameter valor = new SqlCeParameter("@value", SqlDbType.DateTime);
                valor.Value = data;
                //valor.Value = "30/03/2000";
                addRegistro.Parameters.Add(valor);

                SqlCon.Open();
                addRegistro.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na atualização da data: " + ex.Message);
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }
        }

        //Metodo para adicionar registros float/decimal
        public string AddRegistroFloat(string tabela, string campo, decimal floatValor)
        {
            string msg = null;
            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;

            try
            {
                SqlCeCommand addRegistro = new SqlCeCommand("INSERT INTO " + tabela + "(" + campo + ") VALUES(@value)", SqlCon);
                SqlCeParameter valor = new SqlCeParameter("@value", SqlDbType.Decimal);
                valor.Value = floatValor;
                //valor.Value = "30/03/2000";
                addRegistro.Parameters.Add(valor);

                SqlCon.Open();
                addRegistro.ExecuteNonQuery();

                msg = "Registro adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na atualização da data: " + ex.Message);
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return msg;

        }

        //Metodo para atualizar registro Decimal 
        public string upRegistroDec(string tabela, string campo, string doubleValor, string id)
        {
            string msg = null;
            string strCon = sqlStrCon;
            SqlCeConnection SqlCon = new SqlCeConnection();
            SqlCon.ConnectionString = strCon;

            try
            {
                //SqlCeCommand addRegistro = new SqlCeCommand("INSERT INTO " + tabela + "(" + campo + ") VALUES(@value)", SqlCon);
                SqlCeCommand addRegistro = new SqlCeCommand("UPDATE " + tabela + " SET " + campo + "=@value WHERE id=" + id + "", SqlCon);
                SqlCeParameter valor = new SqlCeParameter("@value", SqlDbType.Decimal);
                valor.Value = doubleValor;
                //valor.Value = "30/03/2000";
                addRegistro.Parameters.Add(valor);

                SqlCon.Open();
                addRegistro.ExecuteNonQuery();

                msg = "Registro adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro na atualização: " + ex.Message);
            }
            finally
            {
                if (SqlCon.State == ConnectionState.Open)
                {
                    SqlCon.Close();
                }
            }

            return msg;

        }

    }

    public class IAPPTelas
    {
        public void abreTela(string tela, Form parent)
        {
            switch (tela)
            {
                case "frmCadastrarAlimento":
                    frmCadastrarAlimento formCadastrarAlimento = null;
                    if ((formCadastrarAlimento = (frmCadastrarAlimento)IsFormAlreadyOpen(typeof(frmCadastrarAlimento))) == null)
                    {
                        formCadastrarAlimento = new frmCadastrarAlimento();
                        formCadastrarAlimento.MdiParent = parent;
                        formCadastrarAlimento.Show();
                    }
                    else
                    {
                        formCadastrarAlimento.WindowState = FormWindowState.Maximized;
                        formCadastrarAlimento.BringToFront();
                    }
                    break;

                case "frmCaloriasPercentuais":
                    frmCaloriasPercentuais formCaloriasPercentuais = null;
                    if ((formCaloriasPercentuais = (frmCaloriasPercentuais)IsFormAlreadyOpen(typeof(frmCaloriasPercentuais))) == null)
                    {
                        formCaloriasPercentuais = new frmCaloriasPercentuais();
                        formCaloriasPercentuais.MdiParent = parent;
                        formCaloriasPercentuais.Show();
                    }
                    else
                    {
                        formCaloriasPercentuais.WindowState = FormWindowState.Maximized;
                        formCaloriasPercentuais.BringToFront();
                    }
                    break;

                case "frmCadastrarGrupoAlimentos":
                    frmCadastrarGrupoAlimentos formCadastrarGrupoAlimentos = null;
                    if ((formCadastrarGrupoAlimentos = (frmCadastrarGrupoAlimentos)IsFormAlreadyOpen(typeof(frmCadastrarGrupoAlimentos))) == null)
                    {
                        formCadastrarGrupoAlimentos = new frmCadastrarGrupoAlimentos();
                        formCadastrarGrupoAlimentos.MdiParent = parent;
                        formCadastrarGrupoAlimentos.Show();
                    }
                    else
                    {
                        formCadastrarGrupoAlimentos.WindowState = FormWindowState.Maximized;
                        formCadastrarGrupoAlimentos.BringToFront();
                    }
                    break;

                case "frmCadastrarPaciente":
                    frmPrincipal formPrincipal = new frmPrincipal();
                    frmCadastrarPaciente formCadastrarPaciente = null;
                    if ((formCadastrarPaciente = (frmCadastrarPaciente)IsFormAlreadyOpen(typeof(frmCadastrarPaciente))) == null)
                    {
                        formCadastrarPaciente = new frmCadastrarPaciente();
                        formCadastrarPaciente.MdiParent = parent;
                        formCadastrarPaciente.WindowState = FormWindowState.Maximized;

                        if (formPrincipal.intTelaModo == 0)
                        {
                            formCadastrarPaciente.tabPaciente.TabPages.RemoveAt(2);
                        }

                        formCadastrarPaciente.Show();
                    }
                    else
                    {
                        formCadastrarPaciente.WindowState = FormWindowState.Maximized;
                        formCadastrarPaciente.BringToFront();

                    }
                    break;

                case "frmConfiguracoes":
                    frmConfiguracoes formConfiguracoes = null;

                    if ((formConfiguracoes = (frmConfiguracoes)IsFormAlreadyOpen(typeof(frmConfiguracoes))) == null)
                    {
                        formConfiguracoes = new frmConfiguracoes();
                        formConfiguracoes.MdiParent = parent;
                        formConfiguracoes.Show();
                    }
                    else
                    {
                        formConfiguracoes.WindowState = FormWindowState.Maximized; ;
                        formConfiguracoes.BringToFront();
                    }


                    /*   if (menuSuperiorConfiguracoesSistema.Pressed == true)
                       {
                           formConfiguracoes.tabConfiguracoes.SelectTab(1);
                           formConfiguracoes.picProfissional.Visible = false;
                       }
                       else
                       {
                           formConfiguracoes.tabConfiguracoes.SelectTab(0);
                           formConfiguracoes.picProfissional.Visible = true;
                       }*/

                    break;

                case "frmConsultarAlimento":
                    frmConsultarAlimento formConsultarAlimento = null;

                    if ((formConsultarAlimento = (frmConsultarAlimento)IsFormAlreadyOpen(typeof(frmConsultarAlimento))) == null)
                    {
                        formConsultarAlimento = new frmConsultarAlimento();
                        formConsultarAlimento.MdiParent = parent;
                        formConsultarAlimento.Show();
                    }
                    else
                    {
                        formConsultarAlimento.WindowState = FormWindowState.Maximized;
                        formConsultarAlimento.BringToFront();
                    }
                    break;

                case "frmConsultarPaciente":
                    frmConsultarPaciente formConsultarPaciente = null;
                    if ((formConsultarPaciente = (frmConsultarPaciente)IsFormAlreadyOpen(typeof(frmConsultarPaciente))) == null)
                    {
                        formConsultarPaciente = new frmConsultarPaciente();
                        formConsultarPaciente.MdiParent = parent;
                        formConsultarPaciente.Show();

                    }
                    else
                    {
                        formConsultarPaciente.WindowState = FormWindowState.Maximized;
                        formConsultarPaciente.BringToFront();
                    }
                    break;

                case "frmPorcoesMedidas":
                    frmPorcoesMedidas formPorcoesMedidas = null;
                    if ((formPorcoesMedidas = (frmPorcoesMedidas)IsFormAlreadyOpen(typeof(frmPorcoesMedidas))) == null)
                    {
                        formPorcoesMedidas = new frmPorcoesMedidas();
                        formPorcoesMedidas.MdiParent = parent;
                        formPorcoesMedidas.Show();
                    }
                    else
                    {
                        formPorcoesMedidas.WindowState = FormWindowState.Maximized;
                        formPorcoesMedidas.BringToFront();
                    }
                    break;

                case "frmCopiaSeguranca":
                    frmCopiaSeguranca formCopiaSeguranca = null;
                    if ((formCopiaSeguranca = (frmCopiaSeguranca)IsFormAlreadyOpen(typeof(frmCopiaSeguranca))) == null)
                    {
                        formCopiaSeguranca = new frmCopiaSeguranca();
                        formCopiaSeguranca.MdiParent = parent;
                        formCopiaSeguranca.Show();

                    }
                    else
                    {
                        formCopiaSeguranca.WindowState = FormWindowState.Maximized;
                        formCopiaSeguranca.BringToFront();
                    }

                    break;

                case "frmEmailPadrao":
                    frmEmailPadrao formEmailPadrao = null;
                    if ((formEmailPadrao = (frmEmailPadrao)IsFormAlreadyOpen(typeof(frmEmailPadrao))) == null)
                    {
                        formEmailPadrao = new frmEmailPadrao();
                        formEmailPadrao.MdiParent = parent;
                        formEmailPadrao.Show();
                    }
                    else
                    {
                        formEmailPadrao.WindowState = FormWindowState.Maximized;
                        formEmailPadrao.BringToFront();
                    }

                    break;

                case "frmManualIAPPDiet":
                    frmManualIAPPDiet formManualIAPPDiet = null;
                    if ((formManualIAPPDiet = (frmManualIAPPDiet)IsFormAlreadyOpen(typeof(frmManualIAPPDiet))) == null)
                    {
                        formManualIAPPDiet = new frmManualIAPPDiet();
                        formManualIAPPDiet.MdiParent = parent;
                        formManualIAPPDiet.Show();
                    }
                    else
                    {
                        formManualIAPPDiet.WindowState = FormWindowState.Maximized;
                        formManualIAPPDiet.BringToFront();
                    }

                    break;

                case "frmPrescreverDieta":
                    frmPrescreverDieta formPrescreverDieta = null;
                    if ((formPrescreverDieta = (frmPrescreverDieta)IsFormAlreadyOpen(typeof(frmPrescreverDieta))) == null)
                    {
                        formPrescreverDieta = new frmPrescreverDieta();
                        formPrescreverDieta.MdiParent = parent;
                        formPrescreverDieta.Show();
                    }
                    else
                    {
                        formPrescreverDieta.WindowState = FormWindowState.Maximized;
                        formPrescreverDieta.BringToFront();
                    }
                    break;

                case "frmPrescricaoDieta":
                    frmPrescricaoDieta formPrescricaoDieta = null;
                    if ((formPrescricaoDieta = (frmPrescricaoDieta)IsFormAlreadyOpen(typeof(frmPrescricaoDieta))) == null)
                    {
                        formPrescricaoDieta = new frmPrescricaoDieta();
                        formPrescricaoDieta.MdiParent = parent;
                        formPrescricaoDieta.Show();
                    }
                    else
                    {
                        formPrescricaoDieta.WindowState = FormWindowState.Maximized;
                        formPrescricaoDieta.BringToFront();
                    }
                    break;

                case "frmRefeicoesDieta":
                    frmRefeicoesDieta formPrescreverRefeicoesDieta = null;
                    if ((formPrescreverRefeicoesDieta = (frmRefeicoesDieta)IsFormAlreadyOpen(typeof(frmRefeicoesDieta))) == null)
                    {
                        formPrescreverRefeicoesDieta = new frmRefeicoesDieta();
                        formPrescreverRefeicoesDieta.MdiParent = parent;
                        formPrescreverRefeicoesDieta.Show();
                    }
                    else
                    {
                        formPrescreverRefeicoesDieta.WindowState = FormWindowState.Maximized;
                        formPrescreverRefeicoesDieta.BringToFront();
                    }

                    break;

                case "frmSobreSistema":
                    frmSobreSistema formSobreSistema = null;
                    if ((formSobreSistema = (frmSobreSistema)IsFormAlreadyOpen(typeof(frmSobreSistema))) == null)
                    {
                        formSobreSistema = new frmSobreSistema();
                        formSobreSistema.MdiParent = parent;
                        formSobreSistema.Show();
                    }
                    else
                    {
                        formSobreSistema.WindowState = FormWindowState.Maximized;
                        formSobreSistema.BringToFront();
                    }

                    break;
                case "frmContinuarDieta":
                    frmContinuarDieta formContinuarDieta = null;
                    if ((formContinuarDieta = (frmContinuarDieta)IsFormAlreadyOpen(typeof(frmContinuarDieta))) == null)
                    {
                        formContinuarDieta = new frmContinuarDieta();
                        formContinuarDieta.MdiParent = parent;
                        formContinuarDieta.Show();
                    }
                    else
                    {
                        formContinuarDieta.BringToFront();
                    }

                    break;

                default:
                    break;
            }
        }

        public static Form IsFormAlreadyOpen(Type FormType)
        {
            foreach (Form OpenForm in Application.OpenForms)
            {
                if (OpenForm.GetType() == FormType)
                    return OpenForm;
            }

            return null;
        }
    }
}