using SQLite;
using System;

namespace Cnblogs.ApiModel
{
    public class NewsCommentViewModel
    {
        [PrimaryKey, Indexed]
        public int Id => CommentID;
        public int CommentID { get; set; }
        public int ContentID { get; set; }
        public string CommentContent { get; set; }
        public Guid UserGuid { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FaceUrl { get; set; }
        public int Floor { get; set; }
        public DateTime DateAdded { get; set; }
        public int AgreeCount { get; set; }
        public int AntiCount { get; set; }
        public int ParentCommentID { get; set; }
        public NewsCommentViewModel ParentComment { get; set; }
    }
}
