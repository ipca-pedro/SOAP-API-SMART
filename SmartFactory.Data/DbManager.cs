using System;
using System.Collections.Generic;
using Npgsql;
using SmartFactory.Models;

namespace SmartFactory.Data
{
    public class DbManager
    {
        // 1. STRING DE CONEXÃO (Privada e única para a classe)
        private static readonly string _connString = "Host=localhost;Port=5432;Database=iotdb;Username=admin;Password=changeme";

        // 2. SELECT SENSORES (Para o dashboard REST)
        public List<SensorData> GetLatestReadings()
        {
            var list = new List<SensorData>();
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                // SQL para obter a leitura mais recente de cada sensor
                string sql = "SELECT DISTINCT ON (sensor) sensor, \"Polo\", \"Valor\", \"Unidade\", \"DataHora\" " +
                             "FROM public.dados_sensores_limpos ORDER BY sensor, \"DataHora\" DESC";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SensorData
                        {
                            SensorId = reader["sensor"].ToString(),
                            Polo = reader["Polo"].ToString(),
                            Valor = Convert.ToDouble(reader["Valor"]),
                            Unidade = reader["Unidade"].ToString(),
                            DataHora = Convert.ToDateTime(reader["DataHora"])
                        });
                    }
                }
            }
            return list;
        }

        // 3. INTERVENÇÃO MANUAL (Para o serviço SOAP)
        public bool ExecuteManualIntervention(int ruleId, double newThreshold, string machineName)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // SQL 1: Atualizar o limite da regra na tabela machine_rules
                        string sqlUpdate = "UPDATE public.machine_rules SET threshold_value = @val WHERE id = @id";
                        using (var cmd = new NpgsqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("val", newThreshold);
                            cmd.Parameters.AddWithValue("id", ruleId);
                            cmd.ExecuteNonQuery();
                        }

                        // SQL 2: Gravar o Log na tabela machine_logs (Ajustado às colunas do seu SQL)
                        // De acordo com o seu ficheiro ISI-DB.sql, as colunas são: 
                        // rule_applied_id, sensor_reading, command_issued
                        string sqlLog = "INSERT INTO public.machine_logs (rule_applied_id, sensor_reading, command_issued) " +
                                        "VALUES (@id, @val, @desc)";
                        using (var cmd = new NpgsqlCommand(sqlLog, conn))
                        {
                            cmd.Parameters.AddWithValue("id", ruleId);
                            cmd.Parameters.AddWithValue("val", newThreshold);
                            cmd.Parameters.AddWithValue("desc", $"Ajuste manual para {machineName}");
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        // 4. VALIDAÇÃO DE UTILIZADOR (Para o Auth JWT)
        public User ValidateUser(string username, string password)
        {
            // Corrigido: Agora usa _connString que é a variável definida no topo
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                // SQL baseado na sua tabela app_users
                string sql = "SELECT username, role FROM public.app_users WHERE username = @u AND password_hash = @p";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username);
                    cmd.Parameters.AddWithValue("p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Username = reader["username"].ToString(),
                                Role = reader["role"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}