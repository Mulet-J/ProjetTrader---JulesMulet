using MySql.Data.MySqlClient;
using System;
using MetierTrader;
using System.Collections.Generic;

namespace GestionnaireBDD
{
    public class GstBdd
    {
        private MySqlConnection cnx;
        private MySqlCommand cmd;
        private MySqlDataReader dr;

        // Constructeur
        public GstBdd()
        {
            string chaine = "Server=localhost;Database=bourse;Uid=root;Pwd=";
            cnx = new MySqlConnection(chaine);
            cnx.Open();
        }

        public List<Trader> getAllTraders()
        {
            List<Trader> lesTraders = new List<Trader>();
            cmd = new MySqlCommand("select idTrader, nomTrader from trader", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Trader unTrader = new Trader(Convert.ToInt16(dr[0]), dr[1].ToString());
                lesTraders.Add(unTrader);
            }
            dr.Close();
            return lesTraders;
        }
        public List<ActionPerso> getAllActionsByTrader(int numTrader)
        {
            List<ActionPerso> lesActionsPerso = new List<ActionPerso>();
            cmd = new MySqlCommand("select numAction, nomAction, prixAchat, quantite from acheter ar join action an on ar.numAction = an.idAction where numTrader=" + numTrader+";", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                ActionPerso uneActionPerso = new ActionPerso(Convert.ToInt16(dr[0]), dr[1].ToString(), Convert.ToDouble(dr[2]), Convert.ToInt16(dr[3]));
                lesActionsPerso.Add(uneActionPerso);
            }
            dr.Close();
            return lesActionsPerso;
        }

        public List<MetierTrader.Action> getAllActionsNonPossedees(int numTrader)
        {
            List<MetierTrader.Action> lesActions = new List<MetierTrader.Action>();
            cmd = new MySqlCommand("select idAction, nomAction, coursReel from action where idAction NOT IN (select numAction from acheter where numTrader=" + numTrader + ");", cnx);
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                MetierTrader.Action uneAction = new MetierTrader.Action(Convert.ToInt16(dr[0]), dr[1].ToString(), Convert.ToDouble(dr[2]));
                lesActions.Add(uneAction);
            }
            dr.Close();
            return lesActions;
        }

        public void SupprimerActionAcheter(int numAction, int numTrader)
        {
            cmd = new MySqlCommand("DELETE FROM `acheter` WHERE numAction=" + numAction + " AND numTrader =" + numTrader + ";", cnx);
            cmd.ExecuteNonQuery();
        }

        public void UpdateQuantite(int numAction, int numTrader, int quantite)
        {
            cmd = new MySqlCommand("UPDATE `acheter` SET `quantite`=" + quantite + " WHERE numAction=" + numAction + " AND numTrader=" + numTrader + ";", cnx);
            cmd.ExecuteNonQuery();
        }

        public double getCoursReel(int numAction)
        {
            double coursReel;
            cmd = new MySqlCommand("select coursReel from action where idAction =" + numAction + ";", cnx);
            dr = cmd.ExecuteReader();
            dr.Read();
            coursReel =Convert.ToDouble(dr[0]);
            dr.Close();
            return coursReel;
        }
        public void AcheterAction(int numAction, int numTrader, double prix, int quantite)
        {
            cmd = new MySqlCommand("INSERT INTO `acheter` (`numAction`, `numTrader`, `prixAchat`, `quantite`) VALUES ('" + numAction + "', '" + numTrader + "', '" + prix + "', '" + quantite + "');", cnx);
            cmd.ExecuteNonQuery();
        }
        public double getTotalPortefeuille(int numTrader)
        {
            double total = 0;
            cmd = new MySqlCommand("select sum(prixAchat*quantite) from acheter where numTrader="+ numTrader + "; ",cnx);
            dr = cmd.ExecuteReader();
            dr.Read();
            total = Convert.ToDouble(dr[0]);
            dr.Close();
            return total;
        }
    }
}
