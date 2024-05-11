using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Data;
using System.Text.RegularExpressions;
using static EcommerceApp.OperationResult;
using static EcommerceApp.SearchBar;


namespace EcommerceApp {
    public partial class Produits : Page {
		
		private OracleConnection connection;
		
		// constructor, starts db connection and populates the datagrid from db table
		// it also checks if it was passed the catid parameter after being called from another page with it
		// and makes a search with it instead of loading the default table
		public Produits(string ecatid) {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			if (ecatid == "") {
				LoadDataIntoDataGridView();
			}
			else {
				searchtb.Text = ecatid;
				searchtb_Search();
			}
			
		}
		
		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
            try {
	            string query = "SELECT * FROM Produit";
                
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
			        prodnomtb.Text = selectedRow.Row[1].ToString();
		            proddesctb.Text = selectedRow.Row[2].ToString();
		            prodidcattb.Text = selectedRow.Row[3].ToString();
		            prodstocktb.Text = selectedRow.Row[4].ToString();
		            prodprixtb.Text = selectedRow.Row[5].ToString();
		            
		        }
		    }
		}
		
		
		// input control
		private bool ControleSaisie() {
		    string prodnom = prodnomtb.Text;
			string proddesc = proddesctb.Text;
			string prodidcat = prodidcattb.Text;
			string prodstock = prodstocktb.Text;
			string prodprix = prodprixtb.Text;
		    
		    if (!(prodidcat.Length >= 1 && prodidcat.Length <= 50)) {
			    resultatop.Text="Verifiez l'id du categorie.";
		        return false;
		    }
		    
		    if (!Regex.IsMatch(prodidcat, @"^\d+$")) {
			    resultatop.Text="Verifiez l'id du categorie.";
		        return false;
		    }

			if (!(prodstock.Length >= 1 && prodstock.Length <= 10)) {
				resultatop.Text="Verifiez le stock.";
		        return false;
		    }
		    
		    if (!Regex.IsMatch(prodstock, @"^\d+$")) {
			    resultatop.Text="Verifiez le stock.";
		        return false;
		    }
		    
		    if (!(prodprix.Length >= 1 && prodprix.Length <= 25)) {
			    resultatop.Text="Verifiez le prix.";
		        return false;
		    }
		    
		    if (!(prodnom.Length >= 1 && prodnom.Length <= 100)) {
			    resultatop.Text="Verifiez le nom du produit.";
		        return false;
		    }
		    
		    if (!(proddesc.Length >= 1 && proddesc.Length <= 255)) {
			    resultatop.Text="Verifiez la description du produit.";
		        return false;
		    }
		    
		    return true;
		}
		
		
		// add new row in database with data in the input fields
		void ajtprodbtn_Click(object sender, RoutedEventArgs e) {
			if (!ControleSaisie()) {
				error_result(resultatop, lefttick, righttick);
				return;
			}
			
			string prodnom = prodnomtb.Text;
			string proddesc = proddesctb.Text;
			string prodidcat = prodidcattb.Text;
			string prodstock = prodstocktb.Text;
			string prodprix = prodprixtb.Text;
			
			
			
		    using (OracleCommand command = new OracleCommand()) {
		        command.Connection = connection;
		        command.CommandText = "INSERT INTO Produit (name, description, categorie_id, stock, prix) VALUES (:name, :description, :categorie_id, :stock, :prix)";
		        
		        command.Parameters.Add(":name", OracleType.VarChar).Value = prodnom;
				command.Parameters.Add(":description", OracleType.VarChar).Value = proddesc;
				command.Parameters.Add(":categorie_id", OracleType.Int32).Value = prodidcat;
				command.Parameters.Add(":stock", OracleType.Int32).Value = prodstock;
				command.Parameters.Add(":prix", OracleType.Number).Value = prodprix;
		
		        try {
		            command.ExecuteNonQuery();
		            success_result(resultatop, lefttick, righttick);
		            resultatop.Text="Les données ont été ajoutées avec succès.";
		            LoadDataIntoDataGridView();
		            prodnom = prodnomtb.Text="";
					proddesctb.Text="";
					prodidcattb.Text="";
					prodstocktb.Text="";
					prodprixtb.Text="";
		        }
		        catch (Exception ex) {
			        error_result(resultatop, lefttick, righttick);
			        resultatop.Text="Error adding data: " + ex.Message;
		        }
		    }
			
		}
		
		
		// delete selected row in the database
		void suprprodbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {

		        int prodid = Convert.ToInt32(selectedRow.Row[0]);
		        
		        try {
		            string query = "DELETE FROM Produit WHERE id = :id";

		            using (OracleCommand command = new OracleCommand(query, connection)) {
		                command.Parameters.Add(":id", OracleType.Int32).Value = prodid;
		                command.ExecuteNonQuery();
		                success_result(resultatop, lefttick, righttick);
		                resultatop.Text="Les données ont été supprimées avec succès.";
		                LoadDataIntoDataGridView();
		                prodnomtb.Text="";
						proddesctb.Text="";
						prodidcattb.Text="";
						prodstocktb.Text="";
						prodprixtb.Text="";
		            }

			    }
			    catch (Exception ex) {
				    error_result(resultatop, lefttick, righttick);
				    resultatop.Text="Error deleting row from database: " + ex.Message;
			    }
		    }
		}
		
		
		// modify selected row in database with new data given in the text box input fields
		void modfprodbtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;
			
			if (selectedRow != null) {
		        int prodid = Convert.ToInt32(selectedRow.Row[0]);
		        
		        if (!ControleSaisie())
				{
					return;
				}
		
				string prodnom = prodnomtb.Text;
				string proddesc = proddesctb.Text;
				string prodidcat = prodidcattb.Text;
				string prodstock = prodstocktb.Text;
				string prodprix = prodprixtb.Text;
		    	

			    using (OracleCommand command = new OracleCommand()) {
			        command.Connection = connection;
			        command.CommandText = "UPDATE Produit SET name = :name, description = :description, categorie_id = :categorie_id, stock = :stock, prix = :prix WHERE id = :id";
			        command.Parameters.Add(":name", OracleType.VarChar).Value = prodnom;
					command.Parameters.Add(":description", OracleType.VarChar).Value = proddesc;
					command.Parameters.Add(":categorie_id", OracleType.Int32).Value = prodidcat;
					command.Parameters.Add(":stock", OracleType.Int32).Value = prodstock;
					command.Parameters.Add(":prix", OracleType.Number).Value = prodprix;
					command.Parameters.Add(":id", OracleType.Number).Value = prodid;
			
			        try {
			            command.ExecuteNonQuery();
			            success_result(resultatop, lefttick, righttick);
			            resultatop.Text="Les données ont été modifiées avec succès.";
			            LoadDataIntoDataGridView();
		                prodnomtb.Text="";
						proddesctb.Text="";
						prodidcattb.Text="";
						prodstocktb.Text="";
						prodprixtb.Text="";
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
	            string query = "SELECT * FROM Produit WHERE id LIKE '%\" + searchTerm + \"%' OR name LIKE '%" + searchTerm + "%' OR description LIKE '%" + searchTerm + "%' OR categorie_id LIKE '%" + searchTerm + "%' OR stock LIKE '%" + searchTerm + "%' OR prix LIKE '%" + searchTerm + "%'";
	            
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
			
			if (e.Column is DataGridTextColumn) {
		        DataGridTextColumn textColumn = e.Column as DataGridTextColumn;

		        if (e.PropertyName == "DESCRIPTION") {
		            textColumn.Width = new DataGridLength(200);
		        }
		    }
		}
		
	}
}