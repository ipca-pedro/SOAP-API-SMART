using System;
using System.Collections.Generic;
using Npgsql;
using SmartFactory.Models;

namespace SmartFactory.Data
{
    /// <summary>
    /// Gestor de acesso à base de dados PostgreSQL.
    /// Responsável por todas as operações CRUD e consultas ao PostgreSQL.
    /// Utiliza ADO.NET puro com Npgsql (sem Entity Framework).
    /// </summary>
    public class DbManager
    {
        /// <summary>
        /// String de conexão para aceder à base de dados PostgreSQL
        /// Contém: Host, Porta, Nome da BD, Utilizador e Senha.
        /// </summary>
        /// <param name="_connString">Connection string PostgreSQL - Host=localhost;Port=5432;Database=iotdb;Username=admin;Password=changeme</param>
        private static readonly string _connString = "Host=localhost;Port=5432;Database=iotdb;Username=admin;Password=changeme";

        // ==========================================
        // SEÇÃO 1: MONITORIZAÇÃO (REST - GET)
        // Métodos para obter dados de sensores da tabela 'dados_sensores_limpos'
        // ==========================================
        
        /// <summary>
        /// Obtém todas as leituras de sensores de temperatura da base de dados.
        /// Filtra apenas sensores do tipo "temperatura" da tabela 'dados_sensores_limpos'.
        /// Retorna TODAS as leituras (não apenas a mais recente).
        /// </summary>
        /// <returns>Lista de objetos SensorData com todas as leituras dos sensores</returns>
        /// <remarks>
        /// A query ordena por sensor e depois por DataHora descendente,
        /// permitindo ver o histórico completo de cada sensor.
        /// </remarks>
        public List<SensorData> GetLatestReadings()
        {
            var list = new List<SensorData>();
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                // SQL com nomes de colunas case-sensitive (entre aspas duplas)
                // Filtra apenas sensores de tipo "temperatura"
                string sql = "SELECT sensor, \"Polo\", \"Valor\", \"Unidade\", \"DataHora\" " +
                             "FROM public.dados_sensores_limpos " +
                             "WHERE \"Tipo\" = 'temperatura' " +
                             "ORDER BY sensor, \"DataHora\" DESC";

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
        // SEÇÃO 2: CRUD DE REGRAS (REST - POST, PUT, DELETE)
        // Métodos para gerir as regras de monitorização na tabela 'machine_rules'
        // ==========================================
        
        /// <summary>
        /// Obtém todas as regras de monitorização da base de dados.
        /// Lê a tabela 'machine_rules' com todas as regras configuradas.
        /// </summary>
        /// <returns>Lista de objetos MachineRule com todas as regras</returns>
        /// <remarks>
        /// Cada regra define uma condição que dispara uma ação quando violada.
        /// </remarks>
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

        /// <summary>
        /// Cria uma nova regra de monitorização na base de dados.
        /// Insere um novo registo na tabela 'machine_rules'.
        /// </summary>
        /// <param name="rule">Objeto MachineRule com os dados da nova regra a criar</param>
        /// <returns>true se a inserção foi bem-sucedida, false caso contrário</returns>
        /// <remarks>
        /// Utiliza Parametrized Queries para prevenir SQL Injection.
        /// </remarks>
        public bool CreateRule(MachineRule rule)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "INSERT INTO public.machine_rules (target_sensor_id, rule_name, threshold_value, condition_type, action_command, is_active) " +
                             "VALUES (@tid, @name, @val, @type, @cmd, @active)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tid", rule.TargetSensorId);
                    cmd.Parameters.AddWithValue("@name", rule.RuleName);
                    cmd.Parameters.AddWithValue("@val", rule.ThresholdValue);
                    cmd.Parameters.AddWithValue("@type", rule.ConditionType);
                    cmd.Parameters.AddWithValue("@cmd", rule.ActionCommand);
                    cmd.Parameters.AddWithValue("@active", rule.IsActive);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Atualiza uma regra de monitorização existente.
        /// Modifica os dados de um registo na tabela 'machine_rules' pelo ID.
        /// </summary>
        /// <param name="rule">Objeto MachineRule com os dados atualizados (deve incluir o ID)</param>
        /// <returns>true se a atualização foi bem-sucedida, false caso contrário</returns>
        /// <remarks>
        /// O ID da regra deve ser preenchido no objeto MachineRule.
        /// </remarks>
        public bool UpdateRule(MachineRule rule)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "UPDATE public.machine_rules SET target_sensor_id = @tid, rule_name = @name, " +
                             "threshold_value = @val, condition_type = @type, action_command = @cmd, is_active = @active WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@tid", rule.TargetSensorId);
                    cmd.Parameters.AddWithValue("@name", rule.RuleName);
                    cmd.Parameters.AddWithValue("@val", rule.ThresholdValue);
                    cmd.Parameters.AddWithValue("@type", rule.ConditionType);
                    cmd.Parameters.AddWithValue("@cmd", rule.ActionCommand);
                    cmd.Parameters.AddWithValue("@active", rule.IsActive);
                    cmd.Parameters.AddWithValue("@id", rule.Id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Elimina uma regra de monitorização da base de dados.
        /// Remove um registo da tabela 'machine_rules' pelo ID.
        /// </summary>
        /// <param name="id">Identificador único da regra a eliminar (int)</param>
        /// <returns>true se a eliminação foi bem-sucedida, false caso contrário</returns>
        public bool DeleteRule(int id)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "DELETE FROM public.machine_rules WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ==========================================
        // SEÇÃO 3: INTERVENÇÃO MANUAL (SOAP)
        // Métodos para intervir manualmente no sistema e registar ações
        // ==========================================
        
        /// <summary>
        /// Executa uma intervenção manual no sistema.
        /// Actualiza o limiar de uma regra e registra a intervenção em logs.
        /// Utiliza transações para garantir consistency dos dados.
        /// </summary>
        /// <param name="ruleId">Identificador da regra a modificar (int)</param>
        /// <param name="newThreshold">Novo valor limite para a regra (double)</param>
        /// <param name="machineName">Nome da máquina ou descrição da intervenção (string)</param>
        /// <returns>true se a intervenção foi bem-sucedida, false caso contrário</returns>
        /// <remarks>
        /// Se alguma operação falhar, toda a transação é revertida (ROLLBACK).
        /// Garante que o UPDATE e o INSERT ocorrem atomicamente.
        /// </remarks>
        public bool ExecuteManualIntervention(int ruleId, double newThreshold, string machineName)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        // Actualizar o limiar da regra
                        string sqlUpdate = "UPDATE public.machine_rules SET threshold_value = @val WHERE id = @id";
                        using (var cmd = new NpgsqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@val", newThreshold);
                            cmd.Parameters.AddWithValue("@id", ruleId);
                            cmd.ExecuteNonQuery();
                        }

                        // Registar a intervenção em machine_logs
                        string sqlLog = "INSERT INTO public.machine_logs (rule_applied_id, sensor_reading, command_issued) " +
                                        "VALUES (@id, @val, @desc)";
                        using (var cmd = new NpgsqlCommand(sqlLog, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", ruleId);
                            cmd.Parameters.AddWithValue("@val", newThreshold);
                            cmd.Parameters.AddWithValue("@desc", $"Intervenção manual: {machineName}");
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
        // SEÇÃO 4: AUTENTICAÇÃO (JWT)
        // Métodos para validação de utilizadores e geração de tokens
        // ==========================================
        
        /// <summary>
        /// Valida as credenciais de um utilizador.
        /// Consulta a tabela 'app_users' para verificar username e password.
        /// </summary>
        /// <param name="username">Nome de utilizador (string) - Campo 'username' na tabela 'app_users'</param>
        /// <param name="password">Senha do utilizador (string) - Campo 'password_hash' na tabela 'app_users'</param>
        /// <returns>Objeto User se as credenciais forem válidas, null caso contrário</returns>
        /// <remarks>
        /// Retorna um objeto User com Username e Role se a autenticação for bem-sucedida.
        /// Este objeto é usado para gerar o JWT Token.
        /// </remarks>
        public User ValidateUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT username, role FROM public.app_users WHERE username = @u AND password_hash = @p";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);

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

        // ==========================================
        // SEÇÃO 5: REGRAS (LOOKUP POR ID)
        // Métodos para obter dados específicos de uma regra
        // ==========================================
        
        /// <summary>
        /// Obtém os detalhes completos de uma regra específica pelo ID.
        /// Consulta a tabela 'machine_rules' para um ID dado.
        /// </summary>
        /// <param name="id">Identificador único da regra (int) - Campo 'id' na tabela 'machine_rules'</param>
        /// <returns>Objeto MachineRule com os dados da regra, ou null se não encontrada</returns>
        /// <remarks>
        /// Usado pelo SOAP service para obter informações de uma regra durante execução de ações.
        /// </remarks>
        public MachineRule GetRuleById(int id)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "SELECT id, target_sensor_id, rule_name, threshold_value, condition_type, action_command, is_active FROM public.machine_rules WHERE id = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MachineRule
                            {
                                Id = (int)reader["id"],
                                TargetSensorId = reader["target_sensor_id"].ToString(),
                                RuleName = reader["rule_name"].ToString(),
                                ThresholdValue = Convert.ToDouble(reader["threshold_value"]),
                                ConditionType = reader["condition_type"].ToString(),
                                ActionCommand = reader["action_command"].ToString(),
                                IsActive = Convert.ToBoolean(reader["is_active"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        // ==========================================
        // SEÇÃO 6: LOG DE EXECUÇÃO DE REGRAS
        // Métodos para registar ações executadas no sistema
        // ==========================================
        
        /// <summary>
        /// Registra a execução de uma ação numa tabela de auditoria.
        /// Insere um novo registo na tabela 'machine_logs' com os detalhes da execução.
        /// </summary>
        /// <param name="ruleId">Identificador da regra que foi executada (int) - Campo 'rule_applied_id'</param>
        /// <param name="sensorReading">Valor do sensor no momento da execução (double) - Campo 'sensor_reading'</param>
        /// <param name="commandIssued">Comando/ação que foi executada (string) - Campo 'command_issued'</param>
        /// <returns>true se o log foi registado com sucesso, false caso contrário</returns>
        /// <remarks>
        /// Este método é chamado automaticamente pelo SOAP service quando uma ação é executada.
        /// O timestamp (NOW()) é preenchido automaticamente pelo PostgreSQL.
        /// O status é sempre 'EXECUTED' nesta versão.
        /// Garante auditoria completa de todas as ações do sistema.
        /// </remarks>
        public bool LogRuleExecution(int ruleId, double sensorReading, string commandIssued)
        {
            using (var conn = new NpgsqlConnection(_connString))
            {
                conn.Open();
                string sql = "INSERT INTO public.machine_logs (rule_applied_id, sensor_reading, command_issued, executed_at, status) " +
                             "VALUES (@id, @val, @cmd, NOW(), 'EXECUTED')";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", ruleId);
                    cmd.Parameters.AddWithValue("@val", sensorReading);
                    cmd.Parameters.AddWithValue("@cmd", commandIssued);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}