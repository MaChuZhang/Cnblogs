using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System.IO;
using System.Threading.Tasks;

namespace Cnblogs.XamarinAndroid
{
    public class SQLiteUtil
    {
        private const string DBNAME = "cnblogs.db";
        private static SQLiteAsyncConnection instance;
        public static SQLiteAsyncConnection Instance()
        {
            try
            {
                if (instance == null)
                {
                    string DBPATH = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), DBNAME);
                    instance = new SQLiteAsyncConnection(DBPATH);
                    InitDataBase();
                }
                return instance;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private static async void InitDataBase()
        {
            await Instance().CreateTableAsync<ApiModel.Article>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_Article","success");
            });
            await Instance().CreateTableAsync<ApiModel.KbArticles>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_KbArticles", "success");
            });
            await Instance().CreateTableAsync<ApiModel.StatusModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_StatusModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.StatusCommentsModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_StatusCommentsModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.QuestionModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_QuestionModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.UserInfo>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_UserInfo", "success");
            });
            await Instance().CreateTableAsync<ApiModel.BookmarksModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_BookmarksModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.ArticleCommentModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_ArticleCommentModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.NewsViewModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_NewsViewModel", "success");
            });
            await Instance().CreateTableAsync<ApiModel.NewsCommentViewModel>().ContinueWith(r =>
            {
                System.Diagnostics.Debug.Write("CreateTable_NewsCommentViewModel", "success");
            });
        }
        #region  用户相关
        //public static async Task<ApiModel.UserInfo> UserInfo()
        //{
        //    return  await  Instance
        //}
        #endregion
        #region 收藏相关
        public static async Task<ApiModel.BookmarksModel> SelectBookMark(int id)
        {
            return await Instance().Table<ApiModel.BookmarksModel>().Where(s => s.WzLinkId == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.BookmarksModel>> SelectBookMarkList(int pageSize)
        {
            return await Instance().Table<ApiModel.BookmarksModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateBookMark(Cnblogs.ApiModel.BookmarksModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateBookMarkList(List<ApiModel.BookmarksModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectBookMark(item.WzLinkId) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateBookMark(item);
                }
            }
        }
        #endregion
        #region  文章相关
        public static async Task<ApiModel.Article> SelectArticle(int id)
        {
            return await Instance().Table<ApiModel.Article>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.Article>> SelectArticleList(int pageSize)
        {
            try
            {
                return await Instance().Table<ApiModel.Article>().OrderByDescending(a => a.PostDate).Skip(0).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task<List<ApiModel.Article>> SelectArticleList(int pageSize,bool myself)
        {
            try
            {
                return await Instance().Table<ApiModel.Article>().OrderByDescending(a => a.PostDate).Where(p=>p.MySelf==true).Skip(0).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task UpdateArticle(Cnblogs.ApiModel.Article model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateArticleList(List<ApiModel.Article> list)
        {
            foreach (var item in list)
            {
                if (await SelectArticle(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateArticle(item);
                }
            }
        }
        public static async Task<ApiModel.ArticleCommentModel> SelectArticleComment(int id)
        {
            return await Instance().Table<ApiModel.ArticleCommentModel>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.ArticleCommentModel>> SelectArticlecommentList(int pageSize)
        {
            return await Instance().Table<ApiModel.ArticleCommentModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateArticleComment(Cnblogs.ApiModel.ArticleCommentModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateArticlecommentList(List<ApiModel.ArticleCommentModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectArticleComment(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateArticleComment(item);
                }
            }
        }
        #endregion

        #region 知识库相关
        public static async Task<ApiModel.KbArticles> SelectKbArticles(int id)
        {
            return await Instance().Table<ApiModel.KbArticles>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task UpdateKbArticles(Cnblogs.ApiModel.KbArticles model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateKbArticlesList(List<ApiModel.KbArticles> list)
        {
            foreach (var item in list)
            {
                if (await SelectKbArticles(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateKbArticles(item);
                }
            }
        }
        public static async Task<List<ApiModel.KbArticles>> SelectKbArticleList(int pageSize)
        {
            return await Instance().Table<ApiModel.KbArticles>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        #endregion
        #region 新闻相关



        public static async Task<ApiModel.NewsCommentViewModel> SelectNewsComment(int id)
        {
            return await Instance().Table<ApiModel.NewsCommentViewModel>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task UpdateNewsComment(Cnblogs.ApiModel.NewsCommentViewModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateNewscommentList(List<ApiModel.NewsCommentViewModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectNewsComment(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateNewsComment(item);
                }
            }
        }
        public static async Task<List<ApiModel.NewsCommentViewModel>> SelectNewscommentList(int pageSize)
        {
            return await Instance().Table<ApiModel.NewsCommentViewModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }




        public static async Task<ApiModel.NewsViewModel> SelectNews(int id)
        {
            return await Instance().Table<ApiModel.NewsViewModel>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task UpdateNews(Cnblogs.ApiModel.NewsViewModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateNewsList(List<ApiModel.NewsViewModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectNews(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateNews(item);
                }
            }
        }
        public static async Task<List<ApiModel.NewsViewModel>> SelectNewsList(int pageSize)
        {
            return await Instance().Table<ApiModel.NewsViewModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task<List<ApiModel.NewsViewModel>> SelectNewsListByDigg(int pageSize)
        {
            return await Instance().Table<ApiModel.NewsViewModel>().OrderByDescending(a => a.DateAdded).Where(s=>s.IsRecommend).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task<List<ApiModel.NewsViewModel>> SelectNewsListByHotWeek(int pageSize)
        {
            return await Instance().Table<ApiModel.NewsViewModel>().OrderByDescending(a => a.DateAdded).Where(s => s.IsHot).Skip(0).Take(pageSize).ToListAsync();
        }
        #endregion
        #region  闪存相关
        public static async Task<ApiModel.StatusModel> SelectStatus(int id)
        {
            return await Instance().Table<ApiModel.StatusModel>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.StatusModel>> SelectStatusList(int pageSize)
        {
            return await Instance().Table<ApiModel.StatusModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task<List<ApiModel.StatusModel>> SelectStatusList(int pageSize,bool myself)
        {
            return await Instance().Table<ApiModel.StatusModel>().Where(s=>s.MySelf==myself).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateStatus(Cnblogs.ApiModel.StatusModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateStatusList(List<ApiModel.StatusModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectStatus(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateStatus(item);
                }
            }
        }


        public static async Task<ApiModel.StatusCommentsModel> SelectStatusComment(int id)
        {
            return await Instance().Table<ApiModel.StatusCommentsModel>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.StatusCommentsModel>> SelectStatuscommentList(int pageSize)
        {
            return await Instance().Table<ApiModel.StatusCommentsModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateStatusComment(Cnblogs.ApiModel.StatusCommentsModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateStatuscommentList(List<ApiModel.StatusCommentsModel> list)
        {
            foreach (var item in list)
            {
                if (await SelectStatusComment(item.Id) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateStatusComment(item);
                }
            }
        }
        #endregion

        #region 博问相关
        public static async Task<ApiModel.QuestionModel> SelectQuestion(int id)
        {
            return await Instance().Table<ApiModel.QuestionModel>().Where(s => s.Qid == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.QuestionModel>> SelectListQuestion(int pageSize,bool isMy)
        {
            return await Instance().Table<ApiModel.QuestionModel>().OrderByDescending(a => a.DateAdded).Where(p=>p.isMy==isMy).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateQuestion(Cnblogs.ApiModel.QuestionModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateListQuestion(List<ApiModel.QuestionModel> list,bool isMy)
        {
            foreach (var item in list)
            {
                item.isMy = isMy;
                if (await SelectQuestion(item.Qid) == null)
                {
                    await Instance().InsertAsync(item);
                }
                else
                {
                    await UpdateQuestion(item);
                }
            }
        }
       #endregion
    }
}