using System.Collections.Generic;
using System.Linq;

namespace ApiWebApp.Models
{
    public class InventoryManager
    {
        private List<Article> articles;
        private int nextArticleId = 1;

        public InventoryManager()
        {
            articles = new List<Article>();
        }

        public List<Article> GetArticles()
        { 
            return articles;
        }
        
        public void AddArticle(Article article)
        {
            article.Id = nextArticleId++;
            articles.Add(article);
        }

        public Article GetArticleById(int articleId)
        {
            return articles.FirstOrDefault(a => a.Id == articleId);
        }

        public void UpdateArticle(Article updatedArticle)
        {
            Article existingArticle = articles.FirstOrDefault(a => a.Id == updatedArticle.Id);

            if (existingArticle != null)
            {
                existingArticle.Name = updatedArticle.Name;
                existingArticle.Quantity = updatedArticle.Quantity;
                existingArticle.Price = updatedArticle.Price;
            }
        }

        public void DeleteArticle(Article article)
        {
            articles.Remove(article);
        }
    }

}