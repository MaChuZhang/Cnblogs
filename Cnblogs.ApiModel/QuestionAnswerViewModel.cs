using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class QuestionAnswerViewModel
    {
            [PrimaryKey, Indexed]
            public int Qid { get; set; }
            public int AnswerID { get; set; }
            public string Answer { get; set; }
            public string ConvertedContent { get; set; }
            public int FormatType { get; set; }
            public string UserName { get; set; }
            public bool IsBest { get; set; }
            public QuestionUserInfoModel AnswerUserInfo { get; set; }
            public string AnswerComments { get; set; }
            public DateTime DateAdded { get; set; }
            public int UserID { get; set; }
            public int DiggCount { get; set; }
            public int Score { get; set; }
            public int BuryCount { get; set; }
            public int CommentCounts { get; set; }
        }
}
