using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cnblogs.ApiModel
{
    public class Token
    {
        public string access_token { get; set; }
        public DateTime expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public DateTime RefreshTime { get; set; }
    }
}
