using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string BlogApp { get; set; }
        public string Avatar { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public int Diggcount { get; set; }
    }
}
