using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Data;
using static EcommerceApp.OperationResult;
using static EcommerceApp.SearchBar;



namespace EcommerceApp {
	public partial class Temoignage : Page {

		private OracleConnection connection;

		// constructor, starts db connection and populates the datagrid from db table
		public Temoignage() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			LoadDataIntoDataGridView();
		}

		// loads the table into datagrid
		private void LoadDataIntoDataGridView() {
			try {
				string query = "SELECT * FROM Temoignage";

				using (OracleCommand command = new OracleCommand(query, connection))
				using (OracleDataAdapter adapter = new OracleDataAdapter(command)) {
					System.Data.DataTable dataTable = new System.Data.DataTable();

					adapter.Fill(dataTable);

					dataGrid.ItemsSource = dataTable.DefaultView;
				}

			}
			catch (Exception ex) {
				error_result(resultatop, lefttick, righttick);
				MessageBox.Show("An error occurred: " + ex.Message, "Error");
			}
		}

		
		// delete selected row in the database
		void suprtembtn_Click(object sender, RoutedEventArgs e) {
			var selectedRow = dataGrid.SelectedItem as DataRowView;

			if (selectedRow != null) {
				int temid = Convert.ToInt32(selectedRow.Row[0]);


				try {
					string query = "DELETE FROM Temoignage WHERE id = :id";


					using (OracleCommand command = new OracleCommand(query, connection)) {
						command.Parameters.Add(":id", OracleType.Int32).Value = temid;
						command.ExecuteNonQuery();
						success_result(resultatop, lefttick, righttick);
						resultatop.Text = "Les données ont été supprimées avec succès.";
						LoadDataIntoDataGridView();

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
			string query = "SELECT * FROM Temoignage WHERE id LIKE '%" + searchTerm + "%' OR user_id LIKE '%" + searchTerm + "%' OR email LIKE '%" + searchTerm + "%' OR produit_id LIKE '%" + searchTerm + "%' OR avis LIKE '%" + searchTerm + "%'";
			
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
			searchtb.Text = "";
		}


		// sets width defaults for datagrid columns for cleaner representation
		void dataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
			foreach (DataGridColumn column in dataGrid.Columns) {
				column.MinWidth = 150;
			}

			if (e.Column is DataGridTextColumn) {
				DataGridTextColumn textColumn = e.Column as DataGridTextColumn;

				if (e.PropertyName == "AVIS") {
					textColumn.Width = new DataGridLength(200);
				}

			}
		}


	}
}