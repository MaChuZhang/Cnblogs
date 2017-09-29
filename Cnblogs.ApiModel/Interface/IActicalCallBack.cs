using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cnblogs.ApiModel;
namespace Cnblogs.ApiModel
{
    public interface IArticleListCallBack
    {
        void GetArticalListSuccess(List<Article> articleList);
        void GetArticalListError(List<Article> articleList);
    }
}
