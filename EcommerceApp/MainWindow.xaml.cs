using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;


namespace EcommerceApp {
    public partial class MainWindow : Window {

	    private string superadm;
	    
	    // window constructor, takes admin name from when the login window calls it
		public MainWindow(string username, string superadmin) {
			InitializeComponent();
			loggedtbl.Text= "Connecté: " + username;
			superadm = superadmin;
			MainApp.Content = new Categories();
		}
		
		
		// changes the form content based on button clicked, accordingly with each page
		void catBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Categories();
		}
		
		void prdBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Produits("");
		}
		
		void cmdBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Commandes("");
		}
		
		void cliBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Clients();
		}
		
		void temBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Temoignage();
		}
		
		void conBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Contact();
		}
		
		void admBtn_Click(object sender, RoutedEventArgs e) {
    		ChangeButtonBorder((Button)sender);
			MainApp.Content = new Admins(superadm);
		}
		
		// disconnect from current user button that closes this window and reopens the login window
		void discbtn_Click(object sender, RoutedEventArgs e) {
			Login login = new Login();
            login.Show();
			Close();
		}
		
		
		// changing button border style, this is purely styling and doesnt affect the functions of the buttons
		void ChangeButtonBorder(Button clickedButton) {
			foreach (Button btn in new List<Button> { catBtn, prdBtn, cmdBtn, cliBtn, temBtn, conBtn, admBtn }) {
				Border border = btn.Template.FindName("border", btn) as Border;
				if (border != null) {
					if (btn == clickedButton) {
						border.BorderThickness = new Thickness(3, 3, 3, 0);
						btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")); // White color
					}
					else {
						border.BorderThickness = new Thickness(3);
						btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C4C4")); // Gray color
					}
				}
			}
		}


		
	}
}