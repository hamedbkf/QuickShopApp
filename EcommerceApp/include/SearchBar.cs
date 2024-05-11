using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.OracleClient;

namespace EcommerceApp {
    public static class SearchBar {
        // used in everypage with a datagrid and search function
        // method to execute search query and update the grid with the resulting rows
        public static void SearchAndUpdateGrid(OracleConnection connection, DataGrid dataGrid, string query) {
            try {
                using (OracleCommand command = new OracleCommand(query, connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter(command)) {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex) {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
    }

}