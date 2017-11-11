using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
namespace Cnblogs.XamarinAndroid
{
    public class StatusQuestionFragment : Fragment
    {
        private Button btn_status, btn_question;
        private FragmentManager _fm;
        internal static StatusFragment _statusFragment;
        internal static QuestionFragment _questionFragment;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _fm = Activity.SupportFragmentManager;
            btn_status = Activity.FindViewById<Button>(Resource.Id.btn_tabStatus);
            btn_question = Activity.FindViewById<Button>(Resource.Id.btn_tabQuestion);

            btn_status.Click += (s, e) =>
            {
                if (btn_status.Tag != null && !(bool)btn_status.Tag)
                    return;
                // btn_status.SetBackgroundColor(Resources.GetColor(Resource.Color.primaryDark));
                btn_status.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_left_selected);
                btn_status.SetTextColor(Resources.GetColor(Resource.Color.white));
                btn_status.SetTypeface(Android.Graphics.Typeface.SansSerif, Android.Graphics.TypefaceStyle.Bold);
                btn_status.SetTextSize(ComplexUnitType.Sp,14);

                btn_question.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_right);
                btn_question.SetTextColor(Resources.GetColor(Resource.Color.black));
                btn_question.SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);
                btn_question.SetTextSize(ComplexUnitType.Sp,12);

                btn_status.Tag = false;
                btn_question.Tag = true;
                FragmentTransaction ft = _fm.BeginTransaction();
                if (_questionFragment != null)
                {
                    ft.Hide(_questionFragment);
                }
                if (_statusFragment == null)
                {
                    _statusFragment = new StatusFragment();
                    ft.Add(Resource.Id.frameContent, _statusFragment);
                }
                else
                {
                    ft.Show(_statusFragment);
                }
                ft.Commit();
            };
            btn_question.Click += (s, e) =>
            {
                if (btn_question.Tag != null && !(bool)btn_question.Tag)
                    return;

                btn_question.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_right_selected);
                btn_question.SetTextColor(Resources.GetColor(Resource.Color.white));
                btn_question.SetTypeface(Android.Graphics.Typeface.SansSerif, Android.Graphics.TypefaceStyle.Bold);
                btn_question.SetTextSize(ComplexUnitType.Sp,14);

                btn_status.Background = Resources.GetDrawable(Resource.Drawable.shape_corner_left);
                btn_status.SetTextColor(Resources.GetColor(Resource.Color.black));
                btn_status.SetTypeface(Android.Graphics.Typeface.Default, Android.Graphics.TypefaceStyle.Normal);
                btn_status.SetTextSize(ComplexUnitType.Sp, 12);

                btn_status.Tag = true;
                btn_question.Tag = false;

                FragmentTransaction ft = _fm.BeginTransaction();
                if (_statusFragment != null)
                {
                    ft.Hide(_statusFragment);
                }
                if (_questionFragment == null)
                {
                    _questionFragment = new QuestionFragment();
                    ft.Add(Resource.Id.frameContent, _questionFragment);
                }
                else
                {
                    ft.Show(_questionFragment);
                }
                ft.Commit(); 
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            btn_status.PerformClick();
            return base.OnCreateView(inflater, container, savedInstanceState);
        }
    }
}