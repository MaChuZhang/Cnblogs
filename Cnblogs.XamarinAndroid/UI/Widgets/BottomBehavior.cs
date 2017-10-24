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
using Android.Support.V4.View.Animation;
using Android.Views.Animations;
using Android.Support.Design.Widget;
using Android.Util;
using Java.Lang;

namespace Cnblogs.XamarinAndroid.UI.Widgets
{
    public enum ScrollDirection {
        Down=0,
        Up=1,
        None=2
    }
    public class BottomBehavior: CoordinatorLayout.Behavior
    {
        private ViewPropertyAnimatorCompat translationAnimator;
        private static readonly IInterpolator InInterpolator = new LinearOutSlowInInterpolator();
        private ScrollDirection _scrollDirection = ScrollDirection.None;
        private float targetY = -1;
        public BottomBehavior() : base(){

        }
        public BottomBehavior(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        //public override bool LayoutDependsOn(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        //{
        //    var s = dependency is AppBarLayout;
        //    return base.LayoutDependsOn(parent, child, dependency);
        //}
        //public override bool OnDependentViewChanged(CoordinatorLayout parent, Java.Lang.Object child, View dependency)
        //{
        //    //float scaleY = System.Math.Abs(dependency.GetY());
        //    //View _child = child as View;
        //    ////child.TranslationY
        //    //AnimateOffset(_child);
        //    return true;
        //    // return base.OnDependentViewChanged(parent, child, dependency);
        //}
        void AnimateOffset(View child , ScrollDirection scrollDirection)
        {
            if (translationAnimator == null)
            {
                translationAnimator = ViewCompat.Animate(child);
                translationAnimator.SetDuration(300);
                translationAnimator.SetInterpolator(InInterpolator);
            }
            else
            {
                translationAnimator.Cancel();
            }
            if (scrollDirection == ScrollDirection.Up)
            {
                translationAnimator.TranslationY(child.Height).Start();
            }
            else if (scrollDirection == ScrollDirection.Down)
            {
                translationAnimator.TranslationY(0).Start();
            }
        }
        public override bool OnStartNestedScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View directTargetChild, View target, int nestedScrollAxes)
        {
            //return base.OnStartNestedScroll(coordinatorLayout, child, directTargetChild, target, nestedScrollAxes);
            if (targetY == -1)
            {
                targetY = target.GetY();
            }
            return nestedScrollAxes != 0;
        }
        public override void OnNestedPreScroll(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, int dx, int dy, int[] consumed)
        {
            System.Diagnostics.Debug.Write("dy",dy>0?dy+"方向是上滑":dy+"方向是下滑");
            base.OnNestedPreScroll(coordinatorLayout, child, target, dx, dy, consumed);
            if (dy > 0)
            {
                _scrollDirection = ScrollDirection.Up;
            }
            else if (dy < 0)
            {
                _scrollDirection = ScrollDirection.Down;
            }
            AnimateOffset(child as View, _scrollDirection);
        }
        public override bool OnNestedFling(CoordinatorLayout coordinatorLayout, Java.Lang.Object child, View target, float velocityX, float velocityY, bool consumed)
        {
            //return base.OnNestedFling(coordinatorLayout, child, target, velocityX, velocityY, consumed);
            _scrollDirection = velocityY > 0 ? ScrollDirection.Up : ScrollDirection.Down;
            AnimateOffset(child as View,_scrollDirection);
            return true;
        }

    }
}