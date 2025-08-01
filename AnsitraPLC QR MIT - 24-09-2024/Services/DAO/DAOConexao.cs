using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;



namespace AnsitraPLC_QR_MIT.Services.DAO
{
    public class DAOConexao
    {
        public SqlConnection conexaoSQL = null;

        public static SqlConnection ConectarSQL(string servicoBD, string nomeBanco, string usuarioBD, string senhaBD)
        {
            try
            {
                //Cria a conexao como o banco.

                SqlConnection conexaoSQL = new SqlConnection(
                            "Data Source=" + servicoBD +
                            ";Initial Catalog=" + nomeBanco +
                            ";User Id=" + usuarioBD +
                            ";Password=" + senhaBD + ";" +
                            "TrustServerCertificate = True;");

                conexaoSQL.Open();
                Debug.WriteLine("Conexão com o banco de dados realizada com sucesso!");

                return conexaoSQL;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Erro ao conectar ao banco de dados:\n" + ex.Message, "Erro");
                return null;
            }
        }        

        ////Executa uma consulta SQL e retorna um conjunto de resultados
        ////Usa SqlDataReader para percorrer os dados
        ////Ideal para consultas que retornam múltiplas linhas e colunas
        //public SqlDataReader PesquisarSQL(string pSQL)
        //{
        //    if (ConexaoSQL == null) ConectarSQL();

        //    SqlCommand comandoSQL = new SqlCommand();
        //    comandoSQL.Connection = ConexaoSQL;
        //    comandoSQL.CommandType = CommandType.Text;
        //    comandoSQL.CommandText = pSQL;
        //    return comandoSQL.ExecuteReader();
        //}

        ////Executa uma consulta SQL e retorna um único valor
        ////Ideal para contagens, somas e buscas individuais
        ////Diferente de PesquisarSQL, que retorna múltiplos registros
        ////Libera recursos corretamente após a execução
        //public object SQLScalar(string pSQL)
        //{
        //    object obj;
        //    SqlCommand comandoSql = null;
        //    try
        //    {
        //        if (ConexaoSQL == null) ConectarSQL();

        //        comandoSql = new SqlCommand();
        //        comandoSql.Connection = ConexaoSQL;
        //        comandoSql.CommandType = CommandType.Text;
        //        comandoSql.CommandText = pSQL;
        //        obj = comandoSql.ExecuteScalar();
        //    }
        //    finally
        //    {
        //        if (comandoSql != null)
        //            comandoSql.Dispose();

        //        FecharConexao();
        //    }
        //    return obj;
        //}

        //// Fechar conexão com o banco.
        //public void FecharConexao()
        //{
        //    try
        //    {
        //        if (conexaoSQL != null)
        //        {
        //            conexaoSQL.Close();
        //            conexaoSQL = null;
        //        }
        //    }
        //    catch { }
        //}
    }
}
