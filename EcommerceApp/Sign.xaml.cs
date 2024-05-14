using System;
using System.Windows;
using System.Data.OracleClient;
using System.Text.RegularExpressions;

namespace EcommerceApp {
    public partial class Sign : Window {
		
		private OracleConnection connection;
		
		// window contructor
		public Sign() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
		}
		
		// input control
		private bool ControleSaisie() {
		    string nom = nomtb.Text;
		    string email = emailtb.Text;
		    string adr = adrtb.Text;
		    string pass = passtb.Password;
		    string passrp = passrptb.Password;
		    string tel = teltb.Text;
		    
		    if (!(nom.Length >= 1 && nom.Length <= 50)) {
		        MessageBox.Show("Verifiez le nom.");
		        return false;
		    }
		    
		    if (!(email.Length >= 1 && email.Length <= 50)) {
		        MessageBox.Show("Verifiez l'email.");
		        return false;
		    }
		    
		    if (!Regex.IsMatch(email, @"^.+@.+\..+$")) {
		        MessageBox.Show("Verifiez l'email.");
		        return false;
		    }
		    
		    if (!(adr.Length >= 1 && adr.Length <= 255)) {
		        MessageBox.Show("Verifiez l'adresse.");
		        return false;
		    }
		    
		    if (!(pass.Length >= 1 && pass.Length <= 25)) {
		        MessageBox.Show("Verifiez le mot de passe.");
		        return false;
		    }
		    
		    if (pass != passrp) {
		        MessageBox.Show("Mots de passe ne sont pas identiques.");
		        return false;
		    }
		    
		    if (!(tel.Length >= 1 && tel.Length <= 25)) {
		        MessageBox.Show("Verifiez le numéro de téléphone.");
		        return false;
		    }
		    
		    if (!Regex.IsMatch(tel, @"^\d+$")) {
		        MessageBox.Show("Verifiez le numéro de téléphone.");
		        return false;
		    }
		    
		    return true;
		}
				
		
		// adds new admin to database
		void signbtn_Click(object sender, RoutedEventArgs e) {
			
			if (!ControleSaisie()) {
				return;
			}
			
			string nom = nomtb.Text;
			string email = emailtb.Text;
			string adr = adrtb.Text;
			string pass = passtb.Password;
			pass = HashSHA256.HashPassword(pass);
			string tel = teltb.Text;
			
			string query = String.Format("INSERT INTO Admin (name, email, adresse, password, numero) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",nom,email,adr,pass,tel);
			                             

			using (OracleCommand command = new OracleCommand(query, connection)) {
				try {
					int rowsInserted = command.ExecuteNonQuery();
					MessageBox.Show(String.Format("Admin has been added.\n{0} row(s) inserted successfully.",rowsInserted));
				}
				catch (Exception ex) {
            		MessageBox.Show("Error: " + ex.Message);
        		}
			}
		}
		
	}
    
}