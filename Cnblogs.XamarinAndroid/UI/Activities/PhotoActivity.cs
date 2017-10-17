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
using Android.Support.V7.App;
using Android.Support.V4.View;
using UK.CO.Senab.Photoview;
using Com.Nostra13.Universalimageloader.Core;

namespace Cnblogs.XamarinAndroid
{
    [Activity(Label = "PhotoActivity",Theme ="@style/AppTheme")]
    public class PhotoActivity : AppCompatActivity,ViewPager.IOnPageChangeListener
    {
        private ViewPager viewPager_Img;
        private PhotoView photoview;
        private Adapter.PhotoAdapter photoAdapter;
        private DisplayImageOptions options;
        private TextView tv_index;
        private int photoCount;
        private int photoEnterIndex; //¥´»ÎµƒÕº∆¨index
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Photo);
            viewPager_Img = FindViewById<ViewPager>(Resource.Id.viewpager_img);
            photoview = FindViewById<PhotoView>(Resource.Id.photoview);
            tv_index = FindViewById<TextView>(Resource.Id.tv_index);
            ImageLoaderConfiguration configuration = new ImageLoaderConfiguration.Builder(this).Build();//≥ı ºªØÕº∆¨º”‘ÿøÚº‹
            ImageLoader.Instance.Init(configuration);
            //œ‘ æÕº∆¨≈‰÷√
            options = new DisplayImageOptions.Builder()
                  .ShowImageOnFail(Resource.Drawable.Icon)
                  .CacheInMemory(false)
                  .BitmapConfig(Android.Graphics.Bitmap.Config.Rgb565)
                  .ShowImageOnFail(Resource.Drawable.noavatar)
                  .ShowImageOnLoading(Resource.Drawable.icon_loading)
                  .CacheOnDisk(false)
                  .Build();

            List<string> urls = Intent.GetStringArrayExtra("urls").ToList();
            photoEnterIndex  = Intent.GetIntExtra("index",-1);
            urls.RemoveAt(urls.Count - 1);
            photoCount = urls.Count;
           
            viewPager_Img.OffscreenPageLimit = photoCount;
            photoAdapter = new Adapter.PhotoAdapter(urls);
            photoAdapter.LoadPhoto += (photoview,position) =>
            {
                System.Diagnostics.Debug.Write(urls[position]);
                ImageLoader.Instance.DisplayImage(urls[position], photoview, options);
            };
            viewPager_Img.Adapter =photoAdapter;
            viewPager_Img.AddOnPageChangeListener(this);
                viewPager_Img.SetCurrentItem(photoEnterIndex, false);
            tv_index.Text = $"{photoEnterIndex+1}/{photoCount}";
            // Create your application here
        }
        public static void Enter(Context context,string[] urls,int index)
        {
            Intent intent = new Intent(context,typeof(PhotoActivity));
            intent.PutExtra("urls",urls);
            intent.PutExtra("index",index);
            context.StartActivity(intent);
        }

        public void OnPageScrollStateChanged(int state)
        {
           // throw new NotImplementedException();
        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {
            //throw new NotImplementedException();
        }

        public void OnPageSelected(int position)
        {
            tv_index.Text =$"{(position + 1)}/{photoCount}";
        }
    }
}