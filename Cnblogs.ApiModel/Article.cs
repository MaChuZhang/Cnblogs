using System;
using SQLite;
namespace Cnblogs.ApiModel
{
    public class Article
    {
        [PrimaryKey,Indexed]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string BlogApp { get; set; }
        [Ignore]
        public string Content { get; set; }
        public string Avatar { get; set; }
        public int ViewCount { get; set; }
        
        public int CommentCount { get; set; }
        public int Diggcount { get; set; }
        public DateTime PostDate { get; set; }
        public bool MySelf { get; set; }
    }
}
