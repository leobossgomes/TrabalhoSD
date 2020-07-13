using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfacexD.Models
{
    public class BancoDeDados
    {
        public static List<Objeto> TodosOsObjetos = new List<Objeto>();
        public static List<Estante> Estantes = new List<Estante>();
        public static List<Robo> Robos = new List<Robo>();
        public static PontoEntrega PontoDeEntrega;

        public static void BuscaObjetosDaTela()
        {
            var listaRobos = Select("Robo", new List<string> { "id", "posicaoInicialX", "posicaoInicialY" });

            TodosOsObjetos = new List<Objeto>();
            Robos = new List<Robo>();
            foreach (object[] robozinho in listaRobos)
            {
                Robo robo = new Robo(int.Parse(robozinho[0].ToString()), int.Parse(robozinho[1].ToString()), int.Parse(robozinho[2].ToString()));
                Robos.Add(robo);
                TodosOsObjetos.Add(robo);
            }

            var listaEstante = Select("Estante", new List<string> { "id", "posicaoInicialX", "posicaoInicialY" });

            Estantes = new List<Estante>();
            foreach (object[] estanteEE in listaEstante)
            {
                Estante estante = new Estante(int.Parse(estanteEE[0].ToString()), int.Parse(estanteEE[1].ToString()), int.Parse(estanteEE[2].ToString()));
                Estantes.Add(estante);
                TodosOsObjetos.Add(estante);
            }

            var listaPde = Select("PontoDeEntrega", new List<string> { "id", "posicaoInicialX", "posicaoInicialY" });

            foreach (object[] pde in listaPde)
            {
                PontoDeEntrega = new PontoEntrega(int.Parse(pde[0].ToString()), int.Parse(pde[1].ToString()), int.Parse(pde[2].ToString()));
            }
            TodosOsObjetos.Add(PontoDeEntrega);
        }



        #region select
        public static List<object> Select(string table, List<string> fieldList)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "automatizacaoderobos.database.windows.net";
                builder.UserID = "natiroots";
                builder.Password = "TrabalhoSistemas1";
                builder.InitialCatalog = "AutomatizacaoDeRobos";

                List<object> listaResultado = new List<object>();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    String sb = $"SELECT {string.Join(',', fieldList)} from [dbo].[{table}]";

                    using (SqlCommand command = new SqlCommand(sb, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object[] objeto = new object[fieldList.Count];
                                for (int i = 0; i < fieldList.Count; i++)
                                {
                                    objeto[i] = reader[i];
                                }

                                listaResultado.Add(objeto);
                            }
                        }
                    }
                }

                return listaResultado;
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }

        public static List<object> Select(string table, List<string> fieldList, string whereCondition)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "automatizacaoderobos.database.windows.net";
                builder.UserID = "natiroots";
                builder.Password = "TrabalhoSistemas1";
                builder.InitialCatalog = "AutomatizacaoDeRobos";

                List<object> listaResultado = new List<object>();

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    String sb = $"SELECT {string.Join(',', fieldList)} from [dbo].[{table}] where {whereCondition}";

                    using (SqlCommand command = new SqlCommand(sb, connection))
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                object[] objeto = new object[fieldList.Count];
                                for (int i = 0; i < fieldList.Count; i++)
                                {
                                    objeto[i] = reader[i];
                                }

                                listaResultado.Add(objeto);
                            }
                        }
                    }
                }

                return listaResultado;
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }

        public static List<object> Select(string table, string field)
        {
            return Select(table, new List<string> { field });
        }
        #endregion

        #region Insert
        public void Insert(string table, Dictionary<string, string> fieldListWithValues)
        {
            try
            {
                MySqlConnection sqlConection = new MySqlConnection("server = automatizacaoderobos.database.windows.net; port = 3307; database = AutomatizacaoDeRobos; User Id = natiroots; password = TrabalhoSistemas1");
                sqlConection.Open();

                string fields = fieldListWithValues != null ? "(" + String.Join(',', fieldListWithValues.Select(r => r.Key)) + ")" : "";
                string values = string.Empty;

                for (int i = 0; i < fieldListWithValues.Count; i++)
                {
                    if (values.Length > 0)
                    {
                        values += ",";
                    }
                    values += "?";
                }

                string command = $"insert into {table} {fields} values ({values});";

                MySqlCommand realizaConsultaBaseadoNoComando = new MySqlCommand(command, sqlConection);

                foreach (var field in fieldListWithValues)
                {
                    realizaConsultaBaseadoNoComando.Parameters.Add($"@{field.Value}");
                }

                realizaConsultaBaseadoNoComando.ExecuteNonQuery();

                sqlConection.Close();
            }
            catch (Exception erro)
            {
                Console.Write(erro.Message);

            }
            Console.ReadLine();
        }

        public void Insert(string table, string fieldName, string fieldValue)
        {
            Insert(table, new Dictionary<string, string> {
                { fieldName, fieldValue }
            });
        }
        #endregion

        #region Delete
        public void Delete(string table, List<long> recordIDs)
        {
            try
            {
                string connectionString = "server=localhost;port=3307;database=AutomatizacaoDeRobos;User Id=root; password=usbw";

                MySqlConnection sqlConection = new MySqlConnection();
                //System.Security.Permissions.PermissionState.Unrestricted

                sqlConection.ConnectionString = connectionString;

                sqlConection.Open();

                string operatorSQL = recordIDs.Count > 1 ? "in" : "=";

                string command = $"delete from {table} where id {operatorSQL} {String.Join(',', recordIDs)}";

                MySqlCommand executeQuerry = new MySqlCommand(command, sqlConection);
                executeQuerry.ExecuteNonQuery();

                sqlConection.Close();
            }
            catch (Exception erro)
            {
                throw new Exception(erro.Message);
            }
        }

        public void Delete(string table, long recordID)
        {
            Delete(table, new List<long> { recordID });
        }
        #endregion 
    }
}
