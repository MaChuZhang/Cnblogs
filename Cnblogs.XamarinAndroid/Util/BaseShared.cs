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

namespace Cnblogs.XamarinAndroid
{
    public class BaseShared
    {
        private ISharedPreferences sp;
        private static BaseShared instance;
        private BaseShared(Context context, string fileName)
        {
            sp = context.GetSharedPreferences(fileName, FileCreationMode.Private);
        }
        public static BaseShared Instance(Context context, string fileName)
        {
            if (instance == null)
            {
                return new BaseShared(context, fileName);
            }
            return instance;
        }

        public string Get(string key, string defValue)
        {
            try
            {
                string spValue = sp.GetString(key, defValue);
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

        public void Set(string key, string value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            try
            {
                editor.PutString(key, value);
            }
            catch (Exception e)
            {
                editor.PutString(key, "");
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