using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Data;
using static EcommerceApp.OperationResult;
using static EcommerceApp.SearchBar;


namespace EcommerceApp {
    public partial class Clients : Page {
		
		private OracleConnection connection;
		
		// constructor, starts db connection and populates the datagrid from db table
		public Clients() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			LoadDataIntoDataGridView();
			
		}
		
		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
            try {
	            string query = "SELECT id, name, email, adresse, numero FROM Client";
                
                using (OracleCommand command = new OracleCommand(query, connection))
                using (OracleDataAdapter adapter = new OracleDataAdapter(command)) {
	                System.Data.DataTable dataTable = new System.Data.DataTable();
                    
                    adapter.Fill(dataTable);

                    dataGrid.ItemsSource = dataTable.DefaultView;
                }
        	
            }
            catch (Exception ex) {
	            error_result(resultatop, lefttick, righttick);
	            resultatop.Text="An error occurred: " + ex.Message + "Error";
            }
		}
		
		// updates the input fields with the columns of selected row for easier modification
		void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (dataGrid.SelectedItem != null) {
			    var selectedRow = dataGrid.SelectedItem as DataRowView;
		        if (selectedRow != null && selectedRow.Row.ItemArray.Length > 1) {

		            clinomtb.Text = selectedRow.Row[1].ToString();
		            cliemailtb.Text = selectedRow.Row[2].ToString();
		            
		        }
		    }
		}
		
		
		// delete selected row in the database
		void suprclibtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {
			    int cliid = Convert.ToInt32(selectedRow.Row[0]);
		

		        try {
		            string query = "DELETE FROM Client WHERE Id = :id";
		

		            using (OracleCommand command = new OracleCommand(query, connection)) {
		                command.Parameters.Add(":id", OracleType.Int32).Value = cliid;
		                command.ExecuteNonQuery();
		                success_result(resultatop, lefttick, righttick);
		                resultatop.Text="Les données ont été supprimées avec succès.";
		                LoadDataIntoDataGridView();
		                clinomtb.Text = "";
		            	cliemailtb.Text = "";
		            }

			    }
			    catch (Exception ex) {
				    error_result(resultatop, lefttick, righttick);
				    resultatop.Text="Error deleting row from database: " + ex.Message;
			    }
		    }
		}
		
				    
	    // specifies search query and calls another method to execute it and update the datagrid
		void searchtb_Search() {
	            string searchTerm = searchtb.Text.Trim();
	            string query = "SELECT * FROM Client WHERE id LIKE '%\" + searchTerm + \"%' OR name LIKE '%" + searchTerm + "%' OR email LIKE '%" + searchTerm + "%' OR adresse LIKE '%" + searchTerm + "%' OR numero LIKE '%" + searchTerm + "%'";
	            
	            using (OracleCommand command = new OracleCommand()) {
		            command.Connection = connection;
		            SearchAndUpdateGrid(connection, dataGrid, query);
	            }
		}
				
		// search by pressing 'ENTER'
		void searchtb_KeyUp(object sender, KeyEventArgs e) {
			if (e.Key == Key.Enter) {
				searchtb_Search();
			}
		}
		
		// search by clicking button
		void searchbtn_Click(object sender, RoutedEventArgs e) {
			searchtb_Search();
		}
				
		// refresh datagrid and reset search query
		void refreshbtn_Click(object sender, RoutedEventArgs e) {
			LoadDataIntoDataGridView();
			searchtb.Text="";
		}
		
		
		
		// method for viewing all commands made by selected client
		void voircomclibtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {

		    	string cliid = selectedRow.Row[0].ToString();
		        
				Commandes pagecommandes = new Commandes(cliid);
    			NavigationService.Navigate(pagecommandes);
		    }
		}
		
		
		// sets width defaults for datagrid columns for cleaner representation
		void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			foreach (DataGridColumn column in dataGrid.Columns) {
		        column.MinWidth = 150;
		    }
		}
		
	}
}