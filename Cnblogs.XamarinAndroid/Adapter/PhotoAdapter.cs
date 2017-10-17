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
using Android.Support.V4.View;
using Java.Lang;
using UK.CO.Senab.Photoview;

namespace Cnblogs.XamarinAndroid.Adapter
{
    public class PhotoAdapter:PagerAdapter
    {
        private Handler handler;
        private List<string> hrefs;
        public Action<PhotoView,int> LoadPhoto;
        public static View CurrentEnterView;
        public PhotoAdapter(List<string> hrefs)
        {
            this.hrefs = hrefs;
            this.handler = new Handler();
        }
        public override int Count => hrefs.Count;

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view == objectValue;
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            container.RemoveView((View)objectValue);
        }
        public override Java.Lang.Object InstantiateItem(ViewGroup container,int position)
        {
            View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.item_photo, container, false);
            handler.Post(() =>
            {
                PhotoView photo = view.FindViewById<PhotoView>(Resource.Id.photoview);
                LoadPhoto(photo,position);
                container.AddView(view);
            });
            return view;
        }
        public override void SetPrimaryItem(View container, int position, Java.Lang.Object objectValue)
        {
            base.SetPrimaryItem(container, position, objectValue);
            CurrentEnterView = (View)objectValue;
        }
        public static View GetPrimaryItem(int index)
        {
            return CurrentEnterView;
        }
    }
}