using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class ZzkDocumentViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string UserAlias { get; set; }
        public int VoteTimes { get; set; }
        public int ViewTimes { get; set; }
       public int CommentTimes { get; set; }
        public string Uri { get; set; }
        public DateTime PublishTime { get; set; }
    }
}
