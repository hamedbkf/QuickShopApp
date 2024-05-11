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
    public partial class Commandes : Page {
		
		private OracleConnection connection;
		
		// constructor, starts db connection and populates the datagrid from db table
		// it also checks if it was passed the catid parameter after being called from another page with it
		// and makes a search with it instead of loading the default table
		public Commandes(string ecliid) {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			LoadDataIntoDataGridView();
			if (ecliid == "") {
				LoadDataIntoDataGridView();
			}
			else {
				searchtb.Text = ecliid;
				searchtb_Search();
			}
			
		}
		
		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
            try {
	            string query = "SELECT * FROM Command";
                
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
			        comidtb.Text= selectedRow.Row[0].ToString();
		            switch(selectedRow.Row[1].ToString()) {
					  case "en cours":
					    comsttcb.SelectedIndex = 0;
					    break;
					  case "annulee":
					    comsttcb.SelectedIndex = 1;
					    break;
					  case "complet":
					    comsttcb.SelectedIndex = 2;
					    break;
					}
		            comadrtb.Text = selectedRow.Row[2].ToString();
		            comcliidtb.Text = selectedRow.Row[3].ToString();
                    comnumtb.Text = selectedRow.Row[5].ToString();
		            
		            
		        }
		    }
		}
		
		
		// input control
		private bool ControleSaisie() {
		    string comid = comidtb.Text;
		    string comadr = comadrtb.Text;
			string comcliid = comcliidtb.Text;
			string comnum = comnumtb.Text;

		    if (!(comid.Length >= 1 && comid.Length <= 50)) {
			    resultatop.Text="Verifiez l'id du commande.";
		        return false;
		    }
		    
		    if (!Regex.IsMatch(comid, @"^\d+$")) {
			    resultatop.Text="Verifiez l'id du commande.";
		        return false;
		    }

			if (!(comcliid.Length >= 1 && comcliid.Length <= 50)) {
				resultatop.Text="Verifiez l'id du client.";
		        return false;
		    }
		    
		    if (!Regex.IsMatch(comcliid, @"^\d+$")) {
			    resultatop.Text="Verifiez l'id du client.";
		        return false;
		    }
		    
		    if (!(comadr.Length >= 1 && comadr.Length <= 255)) {
			    resultatop.Text="Verifiez l'adresse.";
		        return false;
		    }
		    
		    if (!(comnum.Length >= 1 && comnum.Length <= 50)) {
			    resultatop.Text="Verifiez l'id du commande.";
			    return false;
		    }
		    
		    if (!Regex.IsMatch(comnum, @"^\d+$")) {
			    resultatop.Text="Verifiez l'id du commande.";
			    return false;
		    }

		    if (!(comnum.Length >= 1 && comnum.Length <= 50)) {
			    resultatop.Text="Verifiez l'id du client.";
			    return false;
		    }
		    
		    return true;
		}
		
		
		// modify selected row in database with new data given in the text box input fields
		void modfcombtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;
			
			if (selectedRow != null) {
				
				if (!ControleSaisie()) {
					error_result(resultatop, lefttick, righttick);
					return;
				}

				int comidc = Convert.ToInt32(selectedRow.Row[0]);
				string comid = comidtb.Text;
				string comstt = "";
				switch(comsttcb.SelectedIndex) {
					case 0:
						comstt = "en cours";
						break;
					case 1:
						comstt = "annulee";
						break;
					case 2:
						comstt = "complet";
						break;
				}
				string comadr = comadrtb.Text;
				string comcliid = comcliidtb.Text;
				string comnum = comnumtb.Text;
			    	

				using (OracleCommand command = new OracleCommand()) {
					command.Connection = connection;
					command.CommandText = "UPDATE Command SET id = :id, statut = :statut, adresse = :adresse, user_id = :user_id, numero = :numero WHERE id = :idc";
					command.Parameters.Add(":id", OracleType.Int32).Value = comid;
					command.Parameters.Add(":statut", OracleType.VarChar).Value = comstt;
					command.Parameters.Add(":adresse", OracleType.VarChar).Value = comadr;
					command.Parameters.Add(":user_id", OracleType.Int32).Value = comcliid;
					command.Parameters.Add(":idc", OracleType.Number).Value = comidc;
					command.Parameters.Add(":numero", OracleType.VarChar).Value = comnum;

			
					try {
						command.ExecuteNonQuery();
						success_result(resultatop, lefttick, righttick);
						resultatop.Text="Les données ont été modifiées avec succès.";
						LoadDataIntoDataGridView();
						comidtb.Text= "";
						comsttcb.SelectedIndex=0;
						comadrtb.Text="";
						comcliidtb.Text= "";
						comnumtb.Text= "";
					}
					catch (Exception ex) {
						error_result(resultatop, lefttick, righttick);
						resultatop.Text="Error modifying data: " + ex.Message;
					}
				}
			}
		}

		// delete selected row in the database
		void suprcombtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

		    if (selectedRow != null) {
		        int comid = Convert.ToInt32(selectedRow.Row[0]);
		        
		        try {
		            string query = "DELETE FROM Command WHERE id = :id";
		

		            using (OracleCommand command = new OracleCommand(query, connection)) {
		                command.Parameters.Add(":id", OracleType.Int32).Value = comid;
		                command.ExecuteNonQuery();
		                success_result(resultatop, lefttick, righttick);
		                resultatop.Text="Les données ont été supprimées avec succès.";
		                LoadDataIntoDataGridView();
		                comidtb.Text="";
						comsttcb.SelectedIndex=0;
						comadrtb.Text="";
						comcliidtb.Text="";
						comnumtb.Text = "";
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
	            string query = "SELECT * FROM Command WHERE id LIKE '%" + searchTerm + "%' OR statut LIKE '%" + searchTerm + "%' OR adresse LIKE '%" + searchTerm + "%' OR user_id LIKE '%" + searchTerm + "%' OR numero LIKE '%\" + searchTerm + \"%'";
	            
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

				if (e.PropertyName == "CONTENU") {
					textColumn.Width = new DataGridLength(200);
				}
				
				if (e.PropertyName == "ADRESSE") {
					textColumn.Width = new DataGridLength(200);
				}
			}
		}
		
	}
}