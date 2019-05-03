using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Windows;

namespace RegistroDeHorasDevOpsAzure
{
    public class TarefaRepositorio
    {
        private string ConnectionString = Properties.Settings.Default.RegistroDeHorasConnectionString;

        public int InsereDados(string tipo, string descricao, string OrdemServico, string fase, int? naturezaTrabalho, string responsavel, string estado, string stateFase,
                                int idTarefa, DateTime? data, DateTime? qtdHoras, DateTime? inicio, DateTime? fim, string situacao)
        {
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCmd = new SqlCommand())
                {
                    try
                    {
                        sqlConn.Open();
                        sqlCmd.Connection = sqlConn;
                        sqlCmd.CommandText = "[DBO].[INSERIR_WORKITEM_ANDAMENTO]";
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.Clear();
                        sqlCmd.Parameters.AddWithValue("@Tipo ", tipo);
                        sqlCmd.Parameters.AddWithValue("@Descricao", descricao);
                        sqlCmd.Parameters.AddWithValue("@OrdemServico", OrdemServico);
                        sqlCmd.Parameters.AddWithValue("@FaseOS", fase);
                        sqlCmd.Parameters.AddWithValue("@NaturezaTrabalho", naturezaTrabalho);
                        sqlCmd.Parameters.AddWithValue("@Responsavel", responsavel);
                        sqlCmd.Parameters.AddWithValue("@Estado", estado);
                        sqlCmd.Parameters.AddWithValue("@StateFase", stateFase);
                        sqlCmd.Parameters.AddWithValue("@IdTarefa", idTarefa);
                        sqlCmd.Parameters.AddWithValue("@Data", data);
                        sqlCmd.Parameters.AddWithValue("@QtdHoras", qtdHoras);
                        sqlCmd.Parameters.AddWithValue("@Inicio", inicio);
                        sqlCmd.Parameters.AddWithValue("@Fim", DBNull.Value);
                        sqlCmd.Parameters.AddWithValue("@Situacao", situacao);
                        int idTarefaBanco = int.Parse(sqlCmd.ExecuteScalar().ToString());
                        sqlConn.Close();
                        sqlConn.Dispose();
                        return idTarefaBanco;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.ToString());
                    }
                }
                return 0;
            }
        }

        public void AtualizaDados(DateTime dataFim, int idTarefaBanco)
        {
            //Converte a data para o formato yyyy/MM/dd
            string fim = dataFim.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("en-US"));
            string query = "UPDATE dbo.Registro_Horas SET Fim =  CAST('" + fim + "' AS DATETIME) WHERE IDRegistro_Horas = " + idTarefaBanco;

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                {
                    cmd.CommandType = CommandType.Text;
                    try
                    {
                        sqlConn.Open();
                        int exec = cmd.ExecuteNonQuery();
                        sqlConn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.ToString());
                    }
                }
            }
        }

        public DataTable GetDataTableTarefas(DateTime dataInicial, DateTime dataFinal, string naturezaTrabalho)
        {
            string inicio = dataInicial.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("en-US"));
            string fim = dataFinal.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("en-US"));
            string query = "SELECT IdRegistro_Horas, RH.IdTarefa, Data, QtdHoras, inicio, Fim, Descricao FROM Registro_Horas RH INNER JOIN Atividades A ON RH.IdAtividades = A.IdAtividades" 
                            + naturezaTrabalho + " AND inicio >= '"+ inicio + "' AND Fim <= '"+ fim + "'";
            SqlDataReader dataReader = null;

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                {
                    try
                    {
                        sqlConn.Open();
                        dataReader = cmd.ExecuteReader();

                        DataTable dt = new DataTable();
                        dt.Load(dataReader);

                        return dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.ToString());
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
                return null;
            }
        }

        public DataTable GetTarefaAndamento(string responsavel)
        {
            string query = "SELECT TOP 1 IdRegistro_Horas, RH.IdTarefa, Descricao FROM Registro_Horas RH INNER JOIN Atividades A ON RH.IdAtividades = A.IdAtividades WHERE RH.Responsavel LIKE '%" + responsavel + "%' AND Fim is null order by inicio desc";
            SqlDataReader dataReader = null;

            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, sqlConn))
                {
                    try
                    {
                        sqlConn.Open();
                        DataTable dt = new DataTable();
                        dataReader = cmd.ExecuteReader();
                        List<TarefaModel> tarefaModels = new List<TarefaModel>();
                        dt.Load(dataReader);
                        return dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro: " + ex.ToString());
                    }
                    finally
                    {
                        sqlConn.Close();
                    }
                }
                return null;
            }
        }


    }
}