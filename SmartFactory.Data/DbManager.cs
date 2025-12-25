using System;
using System.Collections.Generic;
using Npgsql;
using SmartFactory.Models;

namespace SmartFactory.Data
{
    public class DbManager
    {
        // 1. STRING DE CONEXÃO
        private static readonly string _connString = "Host=localhost;Port=5432;Database=iotdb;Username=admin;Password=changeme";

        // ==========================================
        // 2. MONITORIZAÇÃO (REST - GET)
        // ==========================================
        public List<SensorData> GetLatestReadings()
        {
            var list = new List<SensorData>();
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                // SQL com nomes de colunas case-sensitive (com aspas duplas)
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

        // ==========================================
        // 3. CRUD DE REGRAS (REST - POST, PUT, DELETE)
        // ==========================================
        public List<MachineRule> GetMachineRules()
        {
            var rules = new List<MachineRule>();
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, target_sensor_id, rule_name, threshold_value, condition_type, action_command, is_active FROM public.machine_rules";
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        rules.Add(new MachineRule
                        {
                            Id = (int)dr["id"],
                            TargetSensorId = dr["target_sensor_id"].ToString(),
                            RuleName = dr["rule_name"].ToString(),
                            ThresholdValue = Convert.ToDouble(dr["threshold_value"]),
                            ConditionType = dr["condition_type"].ToString(),
                            ActionCommand = dr["action_command"].ToString(),
                            IsActive = Convert.ToBoolean(dr["is_active"])
                        });
                    }
                }
            }
            return rules;
        }

        public bool CreateRule(MachineRule rule)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "INSERT INTO public.machine_rules (target_sensor_id, rule_name, threshold_value, condition_type, action_command, is_active) " +
                             "VALUES (@tid, @name, @val, @type, @cmd, @active)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("tid", rule.TargetSensorId);
                    cmd.Parameters.AddWithValue("name", rule.RuleName);
                    cmd.Parameters.AddWithValue("val", rule.ThresholdValue);
                    cmd.Parameters.AddWithValue("type", rule.ConditionType);
                    cmd.Parameters.AddWithValue("cmd", rule.ActionCommand);
                    cmd.Parameters.AddWithValue("active", rule.IsActive);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateRule(MachineRule rule)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "UPDATE public.machine_rules SET target_sensor_id = @tid, rule_name = @name, " +
                             "threshold_value = @val, condition_type = @type, action_command = @cmd, is_active = @active WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("tid", rule.TargetSensorId);
                    cmd.Parameters.AddWithValue("name", rule.RuleName);
                    cmd.Parameters.AddWithValue("val", rule.ThresholdValue);
                    cmd.Parameters.AddWithValue("type", rule.ConditionType);
                    cmd.Parameters.AddWithValue("cmd", rule.ActionCommand);
                    cmd.Parameters.AddWithValue("active", rule.IsActive);
                    cmd.Parameters.AddWithValue("id", rule.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteRule(int id)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "DELETE FROM public.machine_rules WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // 4. INTERVENÇÃO MANUAL (SOAP)
        // ==========================================
        public bool ExecuteManualIntervention(int ruleId, double newThreshold, string machineName)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlUpdate = "UPDATE public.machine_rules SET threshold_value = @val WHERE id = @id";
                        using (var cmd = new NpgsqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("val", newThreshold);
                            cmd.Parameters.AddWithValue("id", ruleId);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlLog = "INSERT INTO public.machine_logs (rule_applied_id, sensor_reading, command_issued) " +
                                        "VALUES (@id, @val, @desc)";
                        using (var cmd = new NpgsqlCommand(sqlLog, conn))
                        {
                            cmd.Parameters.AddWithValue("id", ruleId);
                            cmd.Parameters.AddWithValue("val", newThreshold);
                            cmd.Parameters.AddWithValue("desc", $"Intervenção manual: {machineName}");
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

        // ==========================================
        // 5. AUTENTICAÇÃO (JWT)
        // ==========================================
        public User ValidateUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
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