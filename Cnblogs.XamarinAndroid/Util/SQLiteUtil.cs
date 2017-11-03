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
    }
}