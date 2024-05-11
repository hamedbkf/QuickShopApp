using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Data;
using static EcommerceApp.OperationResult;
using static EcommerceApp.SearchBar;


namespace EcommerceApp {
    public partial class Contact : Page {
		
		private OracleConnection connection;
		
		// constructor, starts db connection and populates the datagrid from db table
		public Contact() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			LoadDataIntoDataGridView();
		}
		
		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
            try {
            	
                string query = "SELECT * FROM Contact";
                
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

		            conqestb.Text = selectedRow.Row[3].ToString();
		            conreptb.Text = selectedRow.Row[5].ToString();
		            
		        }
		    }
		}
				

				    
	    // specifies search query and calls another method to execute it and update the datagrid
		void searchtb_Search() {
	            string searchTerm = searchtb.Text.Trim();
	            string query = "SELECT * FROM Contact WHERE id LIKE '%" + searchTerm + "%' OR name LIKE '%\" + searchTerm + \"%' OR email LIKE '%" + searchTerm + "%' OR sujet LIKE '%" + searchTerm + "%' OR repondu LIKE '%" + searchTerm + "%' OR reponse LIKE '%" + searchTerm + "%'";
	            
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
		
		
		// sets width defaults for datagrid columns for cleaner representation
		void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			foreach (DataGridColumn column in dataGrid.Columns) {
		        column.MinWidth = 100;
		    }
		}
		
		// respond to question button, if "repondu" is "non" it will update it to "oui" and add your response
		void conrepbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;
			
			if (selectedRow != null) {
				string conqes = conqestb.Text;
				string conrep = conreptb.Text;

			    using (OracleCommand command = new OracleCommand()) {
			        command.Connection = connection;
			        command.CommandText = "UPDATE Contact SET repondu = 'oui', reponse = :reponse WHERE sujet = :sujet";
			        command.Parameters.Add(":reponse", OracleType.VarChar).Value = conrep;
			        command.Parameters.Add(":sujet", OracleType.VarChar).Value = conqes;
			
			        try {
			            command.ExecuteNonQuery();
			            success_result(resultatop, lefttick, righttick);
			            resultatop.Text="Les données ont été modifiées avec succès.";
			            LoadDataIntoDataGridView();
		                conqestb.Text="";
		                conreptb.Text="";
			        }
			        catch (Exception ex) {
				        error_result(resultatop, lefttick, righttick);
				        resultatop.Text="Error modifying data: " + ex.Message;
			        }
			    }
			}
		}
		
	}
}