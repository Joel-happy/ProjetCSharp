using ApiWebApp.Models;
using System;
using System.Threading.Tasks;

namespace ApiWebApp.UserInterface
{
    public class HelperUserInterface
    {
        public static async Task ShowArticleList(InventoryManager inventoryManager)
        {
            var articles = inventoryManager.GetArticles();

            Console.WriteLine("Liste des articles :");
            foreach (var article in articles)
            {
                Console.WriteLine($"ID: {article.Id}, Nom: {article.Name}, Quantité: {article.Quantity}, Prix: {article.Price:C}");
            }

            await Task.CompletedTask;
        }

        public static async Task AddNewArticle(InventoryManager inventoryManager)
        {
            Console.Write("Nom de l'article : ");
            string name = Console.ReadLine();

            Console.Write("Quantité : ");
            int quantity = int.Parse(Console.ReadLine());

            Console.Write("Prix : ");
            decimal price = decimal.Parse(Console.ReadLine());

            inventoryManager.AddArticle(new Article { Name = name, Quantity = quantity, Price = price });
            Console.WriteLine("Article ajouté avec succès.");

            await Task.CompletedTask;
        }

        public static async Task UpdateArticle(InventoryManager inventoryManager)
        {
            Console.Write("ID de l'article à mettre à jour : ");
            int articleId = int.Parse(Console.ReadLine());

            Article articleToUpdate = inventoryManager.GetArticleById(articleId);

            if (articleToUpdate != null)
            {
                Console.Write("Nouveau nom de l'article : ");
                articleToUpdate.Name = Console.ReadLine();

                Console.Write("Nouvelle quantité : ");
                articleToUpdate.Quantity = int.Parse(Console.ReadLine());

                Console.Write("Nouveau prix : ");
                articleToUpdate.Price = decimal.Parse(Console.ReadLine());

                inventoryManager.UpdateArticle(articleToUpdate);
                Console.WriteLine("Article mis à jour avec succès.");
            }
            else
            {
                Console.WriteLine("Article non trouvé.");
            }

            await Task.CompletedTask;
        }
        
        public static async Task DeleteArticle(InventoryManager inventoryManager)
        {
            Console.Write("ID de l'article à supprimer : ");
            int articleId = int.Parse(Console.ReadLine());

            Article articleToDelete = inventoryManager.GetArticleById(articleId);

            if (articleToDelete != null)
            {
                inventoryManager.DeleteArticle(articleToDelete);
                Console.WriteLine("Article supprimé avec succès.");
            }
            else
            {
                Console.WriteLine("Article non trouvé.");
            }

            await Task.CompletedTask;
        }
                public static async Task ShearchArticleList(InventoryManager inventoryManager)
        {
            var flagName = false;
            var flagPrice= false;
            var articles = inventoryManager.GetArticles();

            Console.WriteLine("Recherche par Prix ou par Nom 1 pour le prix 2 pour le nom :");
            if (console.ReadLine() == "1");{
                flagName= true;
            }
            else if (console.ReadLine() == "2")
            {
                flagPrice = true;
            }
            foreach (var article in articles)
            {
                if(article.name == Console.ReadLine() && flagname== true){ # ici je met la recherche par slice 
                Console.WriteLine($"ID: {article.Id}, Nom: {article.Name}, Quantité: {article.Quantity}, Prix: {article.Price:C}");
                }
                if(article.Price == Console.ReadLine() && flagPrice== true){ # ici je met la recherche par tranche de vals
                Console.WriteLine($"ID: {article.Id}, Nom: {article.Name}, Quantité: {article.Quantity}, Prix: {article.Price:C}");
                }
            }

            await Task.CompletedTask;
        } 
    }
}