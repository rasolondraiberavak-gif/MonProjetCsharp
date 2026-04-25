using System;
#nullable disable

namespace SnackProject
{
    public class SnackLogique
    {
        //Les stocks
        public int stockSandwich {get; set;} = 10;
        public int stockBoisson {get; set;} = 20;

        //Les prix
        public double prixSandwich => 5.50;
        public double prixBoisson => 2.50;

        //TVA
        public double tva => 0.20;
        public double TotalCA{get; set; } = 0;

        public List<Vente> Historique{get; private set; } = new List<Vente>();

        //Calcul du total HT
        public double CalculTotalHT(int qteSandwich, int qteBoisson)
        {        
            return (qteSandwich * prixSandwich) + (qteBoisson * prixBoisson);
        }

        //Calcul du total TTC
        public double CalculTotalTTC(int qteSandwich, int qteBoisson)
        {
            double ht = CalculTotalHT(qteSandwich, qteBoisson);
            return ht * (1 + tva);
        }

        //Verification stock
        public bool VerificationStock(int qteSandwich, int qteBoisson)
        {
            return stockSandwich >= qteSandwich && stockBoisson >= qteBoisson;
        }

        //Mise à jour du stock
        public void MiseAJourStock(int qteSandwich, int qteBoisson)
        {
            stockSandwich -= qteSandwich;
            stockBoisson -= qteBoisson;

            double totalTTC = CalculTotalTTC(qteSandwich, qteBoisson);
            double totalHT = CalculTotalHT(qteSandwich, qteBoisson);
            TotalCA += totalTTC;

            string ligneVente = $"{DateTime.Now:yyyy-MM-dd};{DateTime.Now:HH:mm};{totalTTC}";
            File.AppendAllText("ventes.txt", ligneVente + Environment.NewLine);

            //nouveau : enregistree dans l'historique des ventes
            Vente vente = new Vente
            {
                Date = DateTime.Now,
                qteSandwich = qteSandwich,
                qteBoisson = qteBoisson,
                totalHT = totalHT,
                totalTTC = totalTTC
            };
            Historique.Add(vente);
        }

        public void sauvegarderHistorique()
        {
            string json = System.Text.Json.JsonSerializer.Serialize(Historique);
            File.WriteAllText("historique.json", json);
        }
        public void SauvegarderStock()
        {
            string contenu = $"{stockSandwich};{stockBoisson};{TotalCA}";
            File.WriteAllText ("stock.txt", contenu);
        }

        public void ChargerHistorique()
        {
            if (File.Exists("historique.json"))
            {
                string json = File.ReadAllText("historique.json");
                Historique = System.Text.Json.JsonSerializer.Deserialize<List<Vente>>(json) ?? new List<Vente>();
            }
        }

        public void ChargerStock()
        {
            if (File.Exists("stock.txt"))
            {
                string[] donnees = File.ReadAllText("stock.txt").Split(';');
                stockSandwich = int.Parse(donnees[0]);
                stockBoisson = int.Parse(donnees[1]);
                if (donnees.Length > 2)
                {
                    TotalCA = double.Parse(donnees[2]);
                }
            }
        }
        public double CalculerCAjour()
        {
            
            double total = 0;
            string Aujourdhui = DateTime.Now.ToString("yyyy-MM-dd");
            if (File.Exists("ventes.txt"))
            {
                string[] lignes = File.ReadAllLines("ventes.txt");
                foreach (string ligne in lignes)
                {
                    if (ligne.StartsWith(Aujourdhui))
                    {
                        string[] parties = ligne.Split(';');
                        double montant = double.Parse(parties[2]);
                        total += montant;
                    }
                }
            }
            return total;
        }

        public List<Utilisateur> ChargerUtilisateurs()
        {
            List<Utilisateur> utilisateurs = new List<Utilisateur>();
            if (File.Exists("user.txt"))
            {
                string[] lignes = File.ReadAllLines("user.txt");
                foreach (string ligne in lignes)
                {
                    string[] parties = ligne.Split(';');
                    if (parties.Length == 3)
                    {
                        utilisateurs.Add(new Utilisateur
                        {
                            Nom = parties[0],
                            MotDePasse = parties[1],
                            Role = parties[2]
                        });
                    }
                }
            }
            else
            {
                //creation d'un utilisateur par defaut si le fichier n'existe pas
                utilisateurs.Add(new Utilisateur{
                    Nom = "admin",
                    MotDePasse = "1234",
                    Role = "admin"
                });
                utilisateurs.Add(new Utilisateur{
                    Nom = "vendeur",
                    MotDePasse = "5678",
                    Role = "vendeur"
                });
                SauvegarderUtilisateurs(utilisateurs);
            }
            return utilisateurs;
        }

        public void SauvegarderUtilisateurs(List<Utilisateur> utilisateurs)
        {
            List<string> lignes = new List<string>();
            foreach (var u in utilisateurs)
            {
                lignes.Add($"{u.Nom};{u.MotDePasse};{u.Role}");
            }
            File.WriteAllLines("user.txt", lignes);
        }

        public Utilisateur Authentifier(List<Utilisateur> utilisateurs, string nom, string motDePasse)
        {
            foreach (var u in utilisateurs)
            {
                if (u.Nom == nom && u.MotDePasse == motDePasse)
                {
                    return u;
                }
            }
            return null;
        }
    }
    public class Vente
    {
        public DateTime Date {get; set;}
        public int qteSandwich {get; set;}
        public int qteBoisson {get; set;}
        public double totalHT {get; set;}
        public double totalTTC {get; set;}
    }
}