using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GestionnaireBDD;
using MetierTrader;

namespace WPFTrader
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GstBdd gstBdd;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gstBdd = new GstBdd();
            lstTraders.ItemsSource = gstBdd.getAllTraders();
        }

        private void lstTraders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTraders.SelectedItem != null)
            {
                lstActions.ItemsSource = gstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                lstActionsNonPossedees.ItemsSource = gstBdd.getAllActionsNonPossedees((lstTraders.SelectedItem as Trader).NumTrader);
                txtTotalPortefeuille.Text = gstBdd.getTotalPortefeuille((lstTraders.SelectedItem as Trader).NumTrader).ToString();
            }
        }

        private void lstActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imgAction.Source = null;
            if(lstActions.SelectedItem!= null)
            {
                double coursReel = gstBdd.getCoursReel((lstActions.SelectedItem as ActionPerso).NumAction);
                if((lstActions.SelectedItem as ActionPerso).PrixAchat > coursReel)
                {
                    imgAction.Source = new BitmapImage(new Uri("Images/Bas.png", UriKind.Relative));
                }
                else if((lstActions.SelectedItem as ActionPerso).PrixAchat == coursReel)
                {
                    imgAction.Source = new BitmapImage(new Uri("Images/Moyen.png", UriKind.Relative));
                }
                else
                {
                    imgAction.Source = new BitmapImage(new Uri("Images/Haut.png", UriKind.Relative));
                }
            }
        }

        private void btnVendre_Click(object sender, RoutedEventArgs e)
        {
            //Verifie si les valeurs entrée sont des nombres
            bool isConvertible = true;
            try
            {
                Convert.ToInt16(txtQuantiteVendue.Text);
            }
            catch (Exception)
            {
                isConvertible = false;
                MessageBox.Show("Merci d'entrer des valeurs valables");
            }
            if (isConvertible)
            {
                if (lstActions.SelectedItem == null)
                {
                    MessageBox.Show("Merci de choisir une action");
                }
                else if (Convert.ToInt16(txtQuantiteVendue.Text) > (lstActions.SelectedItem as ActionPerso).Quantite)
                {
                    MessageBox.Show("Merci de choisir un quantité à vendre valide");
                }
                else
                {

                    if (Convert.ToInt16(txtQuantiteVendue.Text) < (lstActions.SelectedItem as ActionPerso).Quantite)
                    {
                        //Si une partie des actions sont vendues, met a jour la table en fonction de :
                        //l'action selectionné dans la liste action, le trader selectionné dans la liste trader et la quantité entrée dans le champ
                        gstBdd.UpdateQuantite((lstActions.SelectedItem as ActionPerso).NumAction, (lstTraders.SelectedItem as Trader).NumTrader, (lstActions.SelectedItem as ActionPerso).Quantite - Convert.ToInt16(txtQuantiteVendue.Text));
                    }
                    else
                    {
                        //Si toutes les actions sont vendues, supprime de la table en fonction de : l'action selectionné dans la liste action,
                        //le trader selectionné dans la liste trader
                        gstBdd.SupprimerActionAcheter((lstActions.SelectedItem as ActionPerso).NumAction, (lstTraders.SelectedItem as Trader).NumTrader);
                    }
                    lstActions.ItemsSource = null;
                    lstActions.ItemsSource = gstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                    lstActionsNonPossedees.ItemsSource = null;
                    lstActionsNonPossedees.ItemsSource = gstBdd.getAllActionsNonPossedees((lstTraders.SelectedItem as Trader).NumTrader);
                }
            }
        }

        private void btnAcheter_Click(object sender, RoutedEventArgs e)
        {
            if (lstActionsNonPossedees.SelectedItem == null)
            {
                MessageBox.Show("Merci de choisir une action");
            }
            else if (txtPrixAchat.Text == "")
            {
                MessageBox.Show("Merci d'entrer un prix d'achat");
            }
            else if(txtQuantiteAchetee.Text == "")
            {
                MessageBox.Show("Merci d'entrer une quantité valable");
            }
            else
            {   
                //Verifie si les valeurs entrée sont des nombres
                bool isConvertible = true;
                try
                {
                    Convert.ToInt16(txtQuantiteAchetee.Text);
                    Convert.ToDouble(txtPrixAchat.Text);
                }
                catch (Exception)
                {
                    isConvertible = false;
                    MessageBox.Show("Merci d'entrer des valeurs valables");
                }
                if (isConvertible)
                {
                    //Ajoute une action en fonction de : la selection de la liste action non possédé, la select de la liste trader,
                    //le nombre entré dans la case prix achat et le nombre entrée dans la case quantité 
                    gstBdd.AcheterAction((lstActionsNonPossedees.SelectedItem as MetierTrader.Action).NumAction, (lstTraders.SelectedItem as Trader).NumTrader, Convert.ToDouble(txtPrixAchat.Text), Convert.ToInt16(txtQuantiteAchetee.Text));
                    lstActions.ItemsSource = null;
                    lstActions.ItemsSource = gstBdd.getAllActionsByTrader((lstTraders.SelectedItem as Trader).NumTrader);
                    lstActionsNonPossedees.ItemsSource = null;
                    lstActionsNonPossedees.ItemsSource = gstBdd.getAllActionsNonPossedees((lstTraders.SelectedItem as Trader).NumTrader);
                }
            }
        }
    }
}
