using System;
using SnackProject;

#nullable disable

//LOGIN
SnackLogique logic = new SnackLogique();
List<Utilisateur> utilisateurs = logic.ChargerUtilisateurs();

Console.WriteLine("===Authentification===");
Console.Write("Nom d'utilisateur: ");
string nom = Console.ReadLine();
Console.Write("Mot de passe: ");
string motDePasse = Console.ReadLine();

Utilisateur currentUser = logic.Authentifier(utilisateurs, nom, motDePasse);
if (currentUser == null)
{
    Console.WriteLine("Authentification échouée. Accès refusé.");
    return;
}
else
{
    Console.WriteLine($"Bienvenue, {currentUser.Nom}! Votre rôle est: {currentUser.Role}");
}


Console.WriteLine("===Gestion sanack bars==="); 

int qteSandwich = 0;
int qteBoisson = 0;

logic.ChargerStock();
logic.ChargerHistorique();

while (true)
{
    Console.Clear();
    Console.WriteLine("---Menu---");
    Console.WriteLine($"1-Sandwich : {logic.prixSandwich} euros (stock : {logic.stockSandwich})");
    Console.WriteLine($"2-Boisson : {logic.prixBoisson} euros (stock : {logic.stockBoisson})");
    Console.WriteLine("3-Historique des ventes");
    Console.WriteLine($"4-Total CA : {logic.TotalCA,8:F2} euros");
    Console.WriteLine("5-Chiffre d'affaire du jour");
    Console.WriteLine("quitter pour sortir");
    Console.WriteLine("valider pour terminer la commande");

    if (currentUser.Role == "admin")
    {
        Console.WriteLine("6-Modifier les stocks");
        Console.WriteLine("7-Gerer les utilisateurs");
    }

    Console.WriteLine("-- Nouvelle commande --");
    string choix = Console.ReadLine();

    if (choix == "1")
    {
        Console.WriteLine("Quantité:");
        qteSandwich += int.Parse(Console.ReadLine());
        Console.WriteLine($"{qteSandwich} sandwich(s) ajouté(s) à votre commande.");
    }
    else if (choix == "2")
    {
        Console.WriteLine("Quantité:");
        qteBoisson += int.Parse(Console.ReadLine());
        Console.WriteLine($"{qteBoisson} boisson(s) ajouté(s) à votre commande.");
    }
    else if (choix == "3")
    {
        Console.Clear();
        Console.WriteLine("===Historique des ventes===");
        if (logic.Historique.Count == 0)
        {
            Console.WriteLine("Aucune vente enregistrée.");
        }
        else{
        foreach (var v in logic.Historique)
        {
            Console.WriteLine($"Date: {v.Date}, Sandwich: {v.qteSandwich}, Boisson: {v.qteBoisson}, Total HT: {v.totalHT} euros, Total TTC: {v.totalTTC} euros");
        }
        }
    }
    else if (choix == "4")
    {
        Console.WriteLine($"Total des chiffres d'affaires accumulés: {logic.TotalCA,8:F2} euros");
    }
    else if (choix == "5")
    {
        double caJour = logic.CalculerCAjour();
        Console.Clear();
        Console.WriteLine("=== Chiffre d'affaire du jour===");
        Console.WriteLine($"Total des ventes aujourd'hui: {caJour:F2} euros");
    }
    else if (choix == "valider")
    {       
        if (qteSandwich == 0 && qteBoisson == 0)
        {
            Console.WriteLine("Aucun article sélectionné. Veuillez choisir au moins un sandwich ou une boisson.");
        }
        else if(!logic.VerificationStock(qteSandwich, qteBoisson))
        {
            Console.WriteLine("Stock insufisant!");
            Console.WriteLine($"sandwich dispo :{logic.stockSandwich}");
            Console.WriteLine($"boisson dispo :{logic.stockBoisson}");
        }
        else if (choix == "5")
        {
            double caJour = logic.CalculerCAjour();
            Console.Clear();
            Console.WriteLine("=== Chiffre d'affaire du jour===");
            Console.WriteLine($"Chiffre d'affaires total: {caJour:F2} euros");
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
        }
        else if (choix == "6" && currentUser.Role == "admin")
        {
            Console.WriteLine("---Modifier les stocks---");
            Console.Write("Nouveau stock de sandwichs: ");
            int nouveau = int.Parse(Console.ReadLine());
            logic.stockSandwich = nouveau;
            Console.Write("Nouveau stock de boissons: ");
            nouveau = int.Parse(Console.ReadLine());
            logic.stockBoisson = nouveau;
            logic.SauvegarderStock();
            Console.WriteLine("Stocks mis à jour avec succès!");
        }
        else{

            double totalHT = logic.CalculTotalHT(qteSandwich, qteBoisson);
            double totalTTC = logic.CalculTotalTTC(qteSandwich, qteBoisson);

            //enregistrement de la vente
            logic.MiseAJourStock(qteSandwich, qteBoisson);
            logic.SauvegarderStock();
            logic.sauvegarderHistorique(); 

            // ticket de caisse
            Console.Clear();
            Console.WriteLine("                                        ===Ticket de caisse===                                           ");
            Console.WriteLine($"                                    bienvenue chez SNACK MASTER!                                          ");
            Console.WriteLine($"                                        Date: {DateTime.Now}                                             ");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
            Console.WriteLine($"                                             SNACK MASTER                                                  ");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
            Console.WriteLine($"| Item         | Quantité              | Prix Unitaire       | Total                                      |");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
            if (qteSandwich > 0 ) Console.WriteLine($"| Sandwich     | {qteSandwich}  | {logic.prixSandwich,8:F2}      | {(qteSandwich) * logic.prixSandwich,8:F2} euros      |");
            if (qteBoisson > 0 )Console.WriteLine($"| Boisson    | {qteBoisson}   | {logic.prixBoisson,8:F2}       | {(qteBoisson) * logic.prixBoisson,8:F2} euros     |");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
            Console.WriteLine($"| Total à payer: {(qteSandwich) * logic.prixSandwich + (qteBoisson) * logic.prixBoisson,8:F2} euros                                          |");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
            Console.WriteLine($"|                                     TVA (20%): {totalHT * logic.tva,8:F2} euros                                         |");
            Console.WriteLine($"|                                     TOTAL TTC:{totalTTC,8:F2} euros                                                    |");
            Console.WriteLine($"|                                      TOTAL HT: {totalHT,8:F2} euros                                                 |");
            Console.WriteLine($"|                                  Merci d'avoir choisi SNACK MASTER!                                     |");
            Console.WriteLine($"|                                             A bientôt!                                                  |");
            Console.WriteLine($"+---------------------------------------------------------------------------------------------------------+");
 
            //REINITIALISATION DES QUANTITES
            qteSandwich = 0;
            qteBoisson = 0;

            }
    }
    
    else if (choix == "quitter")
    {  
        break;
    }

    Console.WriteLine("Appuyez sur une touche pour continuer...");
    Console.ReadKey();
}
Console.WriteLine("Merci d'avoir utilisé notre service. A bentot!");