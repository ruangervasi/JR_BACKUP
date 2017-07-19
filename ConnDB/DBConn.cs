using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Data;

namespace JR_BACKUP
{
    public class DBConn
    {
        public string sqlStrCon = @"Server=localhost;Database=gsn;Uid=root;"; //String sem proteção por senha

        public DataView RetornaDataView(string strQuerySql)
        {
            DataSet sqlDataSet = new DataSet();
            string strCon = sqlStrCon;

            try
            {
#pragma warning disable IDE0017 // Simplificar a inicialização de objeto
                MySqlConnection SqlCon = new MySqlConnection();
#pragma warning restore IDE0017 // Simplificar a inicialização de objeto
                SqlCon.ConnectionString = strCon;

                try
                {
                    try
                    {
                        try
                        {
                            SqlCon.Open();
                        }
                        catch
                        {
                            strCon = @"Server=localhost;Database=gsn;Uid=root;Pwd=joel0307;"; //String com proteção por senha
                            SqlCon.ConnectionString = strCon;
                            SqlCon.Open();
                        }
                    }
                    catch
                    {
                        strCon = @"Server=localhost;Database=gsn;Uid=root;Pwd=0307joel;"; //String com proteção por senha
                        SqlCon.ConnectionString = strCon;
                        SqlCon.Open();
                    }
                }
                catch
                {
                    strCon = @"Server=localhost;Database=gsn;Uid=root;Pwd=joel0307a;"; //String com proteção por senha
                    SqlCon.ConnectionString = strCon;
                    SqlCon.Open();
                }

#pragma warning disable IDE0017 // Simplificar a inicialização de objeto
                MySqlDataAdapter sqlDA = new MySqlDataAdapter();
#pragma warning restore IDE0017 // Simplificar a inicialização de objeto

                sqlDA.SelectCommand = new MySqlCommand(strQuerySql, SqlCon);


                sqlDA.Fill(sqlDataSet, "temp");
            }
            catch (Exception ex)
            {
                
            }
            return sqlDataSet.Tables["temp"].DefaultView;

        }
    }
}
