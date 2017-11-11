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
            if (instance == null)
            {
                string DBPATH = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), DBNAME);
                instance = new SQLiteAsyncConnection(DBPATH);
                InitDataBase();
            }
            return instance;
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
        }
        #region  文章相关
        public static async Task<ApiModel.Article> SelectArticle(int id)
        {
            return await Instance().Table<ApiModel.Article>().Where(s => s.Id == id).FirstOrDefaultAsync();
        }
        public static async Task<List<ApiModel.Article>> SelectArticleList(int pageSize)
        {
            return await Instance().Table<ApiModel.Article>().OrderByDescending(a => a.PostDate).Skip(0).Take(pageSize).ToListAsync();
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
        public static async Task<List<ApiModel.StatusCommentsModel>> SelectStatusCommentList(int pageSize)
        {
            return await Instance().Table<ApiModel.StatusCommentsModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateStatusComment(Cnblogs.ApiModel.StatusCommentsModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateStatusCommentList(List<ApiModel.StatusCommentsModel> list)
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
        public static async Task<List<ApiModel.QuestionModel>> SelectListQuestion(int pageSize)
        {
            return await Instance().Table<ApiModel.QuestionModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
        }
        public static async Task UpdateQuestion(Cnblogs.ApiModel.QuestionModel model)
        {
            await Instance().UpdateAsync(model);
        }
        public static async Task UpdateListQuestion(List<ApiModel.QuestionModel> list)
        {
            foreach (var item in list)
            {
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