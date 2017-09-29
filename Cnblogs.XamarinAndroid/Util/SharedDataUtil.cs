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
using Cnblogs.ApiModel;
using Cnblogs.HttpClient;

namespace Cnblogs.XamarinAndroid
{
    public class SharedDataUtil
    {
        private ISharedPreferences sp;
        private static SharedDataUtil instance;
        private SharedDataUtil(Context  context,string  fileName)
        {
            sp = context.GetSharedPreferences(fileName, FileCreationMode.Private);
        }
        public static SharedDataUtil Instance(Context context,string fileName)
        {
            if (instance == null)
            {
                return new SharedDataUtil(context, fileName);
            }
            return instance;
        }
        public static void SaveToken(Token token,Context context)
        {
            var fileName = Constact.Tag_AccessKen;
            Instance(context, fileName).SetString(Constact.KeyAccessToken,token.access_token);
            Instance(context, fileName).SetString(Constact.KeyTokenType, token.token_type);
            Instance(context, fileName).SetInt(Constact.KeyExpiresIn,token.expires_in);
            Instance(context, fileName).SetDateTime(Constact.KeyRefreshTime, token.RefreshTime);
        }
        public static Token GetToken(Context context)
        {
            Token token = new Token();
            token.access_token = Instance(context, Constact.Tag_AccessKen).GetString(Constact.KeyAccessToken, "");
            token.token_type = Instance(context, Constact.Tag_AccessKen).GetString(Constact.KeyTokenType, "");
            token.expires_in = Instance(context, Constact.Tag_AccessKen).GetInt(Constact.KeyExpiresIn, 0);
            token.RefreshTime =Instance(context, Constact.Tag_AccessKen).GetDateTime(Constact.KeyRefreshTime);
            return token;
        }

        public string Get(string  key,string  defValue)
        {
            try {
                string spValue = sp.GetString(key,defValue);
                return string.IsNullOrEmpty(spValue) ? defValue : spValue;
            }
            catch (Exception e)
            {
                return defValue;
            }
        }
        public int GetInt(string key, int defValue)
        {
            return int.Parse(Get(key, defValue.ToString()));
        }
        public string GetString(string key, string defValue)
        {
            return Get(key, defValue);
        }
        public DateTime GetDateTime(string key)
        {
            return DateTime.Parse(Get(key, DateTime.MinValue.ToString()));
        }


        public bool GetBool(string key, bool defValue)
        {
            return Boolean.Parse(Get(key, defValue.ToString()));
        }

        public void Set(string  key,string  value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            try
            {
                editor.PutString(key, value);
            }
            catch (Exception e)
            {
                editor.PutString(key,"");
            }
            editor.Apply();
        }
        public void SetString(string key, string value)
        {
            Set(key, value);
        }
        public void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }
        public void SetDateTime(string key, DateTime value)
        {
            Set(key, value.ToString());
        }
        public void SetBool(string key, bool value)
        {
            Set(key, value.ToString());
        }
    }
}