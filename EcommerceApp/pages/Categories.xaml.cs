using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Data;
using System.Windows.Media;
using static EcommerceApp.OperationResult;
using static EcommerceApp.SearchBar;

namespace EcommerceApp {
    public partial class Categories : Page {
		
		private OracleConnection connection;
		
		// constructor, starts db connection and populates the datagrid from db table
		public Categories() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			LoadDataIntoDataGridView();
			
		}
		
		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
            try {
                string query = "SELECT * FROM Categorie";
                
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

		            catnomtb.Text = selectedRow.Row[1].ToString();
		        }
		    }
		}
		
		// add new row in database with data in the input fields
		void ajtcatbtn_Click(object sender, RoutedEventArgs e) {
			if (!ControleSaisie()) {
				error_result(resultatop, lefttick, righttick);
				return;
			}
			
			string catnom = catnomtb.Text;
			
	
		    using (OracleCommand command = new OracleCommand()) {
		        command.Connection = connection;
		        command.CommandText = "INSERT INTO Categorie (name) VALUES (:name)";
		        command.Parameters.Add(":name", OracleType.VarChar).Value = catnom;
		
		        try {
		            command.ExecuteNonQuery();
		            success_result(resultatop, lefttick, righttick);
		            resultatop.Text="Les données ont été ajoutées avec succès.";
		            lefttick.Visibility = Visibility.Visible;
		            righttick.Visibility = Visibility.Visible;
		            LoadDataIntoDataGridView();
		            catnomtb.Text="";
		        }
		        catch (Exception ex) {
			        error_result(resultatop, lefttick, righttick);
			        resultatop.Text="Error adding data: " + ex.Message;
		        }
		    }
			
		}
		
		// delete selected row in the database
		void suprcatbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {
		        int catid = Convert.ToInt32(selectedRow.Row[0]);
		        
		        try {
		            string query = "DELETE FROM Categorie WHERE id = :id";
		            
		            using (OracleCommand command = new OracleCommand(query, connection)) {
		                command.Parameters.Add(":id", OracleType.Int32).Value = catid;
		                command.ExecuteNonQuery();
		                success_result(resultatop, lefttick, righttick);
		                resultatop.Text="Les données ont été supprimées avec succès.";
		                LoadDataIntoDataGridView();
		                catnomtb.Text="";
		            }

			    }
			    catch (Exception ex) {
				    error_result(resultatop, lefttick, righttick);
				    resultatop.Text="Error deleting row from database: " + ex.Message;
			    }
		    }
		}
		
		// modify selected row in database with new data given in the text box input fields
		void modfcatbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;
			
			if (selectedRow != null) {

		        int catid = Convert.ToInt32(selectedRow.Row[0]);
		        
		        if (!ControleSaisie()) {
                    error_result(resultatop, lefttick, righttick);
					return;
				}
		
				string catnom = catnomtb.Text;

			    using (OracleCommand command = new OracleCommand()) {
			        command.Connection = connection;
			        command.CommandText = "UPDATE Categorie SET name = :name WHERE id = :id";
			        command.Parameters.Add(":name", OracleType.VarChar).Value = catnom;
			        command.Parameters.Add(":id", OracleType.Int32).Value = catid;
			
			        try {
			            command.ExecuteNonQuery();
			            success_result(resultatop, lefttick, righttick);
			            resultatop.Text="Les données ont été modifiées avec succès.";
			            LoadDataIntoDataGridView();
		                catnomtb.Text="";
			        }
			        catch (Exception ex) {
				        error_result(resultatop, lefttick, righttick);
				        resultatop.Text="Error modifying data: " + ex.Message;
			        }
			    }
			}
		}
		
	    
		// specifies search query and calls another method to execute it and update the datagrid
		void searchtb_Search() {
	            string searchTerm = searchtb.Text.Trim();
	            string query = "SELECT * FROM Categorie WHERE id LIKE '%" + searchTerm + "%' OR name LIKE '%" + searchTerm + "%'";
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
		
		
		// method for viewing products under selected category
		void voirprodcatbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {

		    	string catid = selectedRow.Row[0].ToString();
		        
				Produits pageproduits = new Produits(catid);
    			NavigationService.Navigate(pageproduits);
		    }
		}
		
		// input control
		private bool ControleSaisie() {
			string catnom = catnomtb.Text;
			
			if (!(catnom.Length >= 1 && catnom.Length <= 255)) {
				resultatop.Text="Verifiez le nom du categorie.";
				return false;
			}
			
			return true;
		}
		
		// sets width defaults for datagrid columns for cleaner representation
		void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			foreach (DataGridColumn column in dataGrid.Columns) {
		        column.MinWidth = 150;
		    }
		}
		
	}
}