using System.Data.OracleClient;

namespace EcommerceApp {
    public static class DatabaseManager {
		
        private static string connectionString = "Data Source=xe;User Id=ecomm;Password=123456;";
        
        private static OracleConnection connection;
        
        // database connection method
        // used in every window and page that connects to the database
        public static OracleConnection GetConnection() {
            if (connection == null) {
                connection = new OracleConnection(connectionString);
                connection.Open();
            }
            else if (connection.State != System.Data.ConnectionState.Open) {
                connection.Open();
            }

            return connection;
        }
		
        public static void CloseConnection() {
            if (connection != null && connection.State == System.Data.ConnectionState.Open) {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }
    }
}