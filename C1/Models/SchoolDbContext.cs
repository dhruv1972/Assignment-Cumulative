
using MySql.Data.MySqlClient;

namespace Cumulative1.Models
{
    /// <summary>
    /// Represents the database context for accessing the school database.
    /// </summary>
    public class SchoolDbContext
    {
        // Credentials for accessing the database
        private static string User { get { return "root"; } }       
        private static string Password { get { return "root"; } }   
        private static string Database { get { return "school"; } } 
        private static string Server { get { return "localhost"; } } 
        private static string Port { get { return "3306"; } }       

        // Connection string used to establish a connection to the database
        protected static string ConnectionString
        {
            get
            {
                return "server=" + Server
                    + ";user=" + User
                    + ";database=" + Database
                    + ";port=" + Port
                    + ";password=" + Password
                    + ";convert zero datetime=True";
            }
        }

        /// <summary>
        /// Returns a connection to the school database.
        /// </summary>
        /// <returns>A MySqlConnection object representing the connection to the database.</returns>
        public MySqlConnection AccessDatabase()
        {
            
            return new MySqlConnection(ConnectionString);
        }
    }
}
