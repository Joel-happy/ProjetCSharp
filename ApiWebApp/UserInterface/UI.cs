using ApiWebApp.Models;
using System;
using System.Threading.Tasks;

namespace ApiWebApp.UserInterface
{
    public class UI
    {
        public static async Task StartInterface()
        {
            InventoryManager inventoryManager = new InventoryManager();
            
            while (true)
            {
                Console.WriteLine("1. Afficher la liste des articles");
                Console.WriteLine("2. Ajouter un nouvel article");
                Console.WriteLine("3. Mettre à jour un article");
                Console.WriteLine("4. Supprimer un article");
                Console.WriteLine("5. Quitter");

                Console.Write("Choisissez une option : ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await HelperUserInterface.ShowArticleList(inventoryManager);
                        break;
                    case "2":
                        await HelperUserInterface.AddNewArticle(inventoryManager);
                        break;
                    case "3":
                        await HelperUserInterface.UpdateArticle(inventoryManager);
                        break;
                    case "4":
                        await HelperUserInterface.DeleteArticle(inventoryManager);
                        break;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Option non valide. Veuillez réessayer.");
                        break;
                }
                Console.WriteLine();
            }
        }
    }
}