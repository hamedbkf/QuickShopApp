using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.OracleClient;
using System.Text.RegularExpressions;


namespace EcommerceApp {
    public partial class Login : Window {
	    
		private OracleConnection connection;
		
		// class constructor
		public Login() {
			InitializeComponent();
			connection = DatabaseManager.GetConnection();
			
			this.KeyDown += Login_KeyDown;
		}
		
		// on login button click it makes user authentication with the database and closes this window and launches the main app window
		void loginbtnclick(object sender, RoutedEventArgs e) {
			
			if (!ControleSaisie()) {
				return;
			}
			
			string username = usernametb.Text;
			string password = passwordtb.Password;
			password = HashSHA256.HashPassword(password);
            
			// no need to select the entire row, just checking if it exists
			string query = String.Format("SELECT COUNT(*) FROM Admin WHERE email = '{0}' AND password = '{1}'",username,password);
			
			using (OracleCommand command = new OracleCommand(query, connection)) {
                try {
	                // executescalar is the fastest and easiest method to return just 1 variable
                    int userCount = Convert.ToInt32(command.ExecuteScalar());
                    if (userCount > 0) { 
	                    // admin name and superadmin status for use in the main app
	                    string namequery = String.Format("SELECT name, superadmin FROM Admin WHERE email = '{0}'",username);
	                    string loggeduser = "";
	                    string superadmin = "";
                        using (OracleCommand nameCommand = new OracleCommand(namequery, connection)) {
	                        using (var reader = nameCommand.ExecuteReader()) {
		                        if (reader.Read()) {
			                        loggeduser = reader.GetString(0); 
			                        superadmin = reader.GetString(1); 
		                        }
		                        else {
			                        MessageBox.Show("No rows found.");
		                        }
	                        }
	                        
                        	MainWindow win1 = new MainWindow(loggeduser, superadmin);
                        	win1.Show();
                        	Close();
                        }
                        usernametb.Text = "";
                        passwordtb.Password = "";
                    }
                    else {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Error logging in: " + ex.Message);
                }
            }
		}
		
		// input control
		private bool ControleSaisie() {
			string username = usernametb.Text;
		    
			if (!(username.Length >= 1 && username.Length <= 50)) {
				MessageBox.Show("Verifiez l'email.");
				return false;
			}
		    
			if (!Regex.IsMatch(username, @"^.+@.+\..+$")) {
				MessageBox.Show("Verifiez l'email.");
				return false;
			}
		    
			return true;
		}
		
		// executes the login button event when pressing 'ENTER', for convenience
		private void Login_KeyDown(object sender, KeyEventArgs e) {
	        if (e.Key == Key.Enter) {
	            loginbtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
	        }
	    }
		
	}
}