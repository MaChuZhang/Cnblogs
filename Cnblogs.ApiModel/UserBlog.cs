using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    /// <summary>
    /// 用户博客信息
    /// </summary>
    public class UserBlog
    {
        /// <summary>
        /// 博客id
        /// </summary>
        public int BlogId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 子标题
        /// </summary>
        public string SubTitle { get; set; }

        /// <summary>
        /// 博客数量
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 页容量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool EnableScript { get; set; }
    }
}
