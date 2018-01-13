using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cnblogs.ApiModel
{
    public enum MyStatusType
    {
        /// <summary>
        /// 我的
        /// </summary>
        my,

        /// <summary>
        /// 我回复的
        /// </summary>
        mycomment,
        /// <summary>
        /// 提到我的
        /// </summary>
        mention,
        /// <summary>
        /// 回复我的
        /// </summary>
        comment
    }
    public enum StatusType {
        /// <summary>
        /// 全站
        /// </summary>
        all,
        /// <summary>
        /// 新回应
        /// </summary>
        recentcomment,
        /// <summary>
        /// 我关注的用户
        /// </summary>
        following,
    }
    public class StatusModel
    {
            [PrimaryKey, Indexed]
            public int Id { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        /// <summary>
        /// 是否私有
        /// </summary>
            public bool IsPrivate { get; set; }
            public bool IsLucky { get; set; }
            /// <summary>
            /// 评论总数
        /// </summary>
            public int CommentCount { get; set; }
            public string UserAlias { get; set; }
            /// <summary>
            /// 用户显示名
            ///</summary>
            public string UserDisplayName { get; set; }
           /// <summary>
           /// 发布日期
           /// </summary>
            public DateTime DateAdded { get; set; }
            /// <summary>
            /// 用户头像url
            /// </summary>
            public string UserIconUrl { get; set; }
            public int UserId { get; set; }
            public Guid UserGuid { get; set; }
            [Ignore]
            public int ParentCommentId { get; set; }
            [Ignore]
            public string ParentCommentContent { get; set; }
            [Ignore]
            public int StatusId { get; set; }
            [Ignore]
            public string StatusUserAlias { get; set; }
            [Ignore]
            public string StatusContent { get; set; }
            [Ignore]
            public List<StatusCommentsModel> Comments { get; set; }
            public bool MySelf { get; set; }
        }
}
