namespace SmartFactory.Models
{
    /// <summary>
    /// Modelo que representa um utilizador autenticado no sistema.
    /// Contém informações básicas do utilizador após validação de credenciais.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Nome único do utilizador no sistema (ex: admin, operador1, etc).
        /// Usado para autenticação e identificação.
        /// </summary>
        /// <param name="Username">Nome de utilizador (string) - Obtido da tabela 'app_users' da BD</param>
        public string Username { get; set; }

        /// <summary>
        /// Papel/Permissão do utilizador no sistema (ex: Admin, Operador).
        /// Define quais as funcionalidades disponíveis para este utilizador.
        /// </summary>
        /// <param name="Role">Papel do utilizador (string) - Obtido da tabela 'app_users' da BD</param>
        public string Role { get; set; }
    }
}