using System;
using SQLite;
namespace Cnblogs.ApiModel
{
    public class KbArticles
    {
        [PrimaryKey,Indexed]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public int ViewCount { get; set; }
        public int Diggcount { get; set; }
        public DateTime DateAdded { get; set; }
        public string Content { get; set; }
    }
}
