using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public class UserInfo
    {
        [PrimaryKey, Indexed]
        public Guid UserId { get; set; }

        public int SpaceUserId { get; set; }

        public int BlogId { get; set; }

        public string DisplayName { get; set; }

        public string Face { get; set; }

        public string Avatar { get; set; }

        public string Seniority { get; set; }

        public string BlogApp { get; set; }
        public int Score { get; set; }
    }
}
