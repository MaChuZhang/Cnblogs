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
using Android.Support.V7.Widget;
using Android.Graphics;
using Android.Content.Res;
using Android.Graphics.Drawables;

namespace Cnblogs.XamarinAndroid
{
        public class RecyclerViewDecoration : RecyclerView.ItemDecoration
        {
            private static int[] ATTRS = new int[] { Android.Resource.Attribute.ListDivider };
            public static int HORIZONTAL = LinearLayoutManager.Horizontal;
            public static int VERTICAL = LinearLayoutManager.Vertical;
            private Drawable _divider;
            private int _orientation;
            public RecyclerViewDecoration(Context context, int orientation)
            {
                TypedArray t = context.ObtainStyledAttributes(ATTRS);
                _divider = t.GetDrawable(0);
                t.Recycle();
                SetOrientation(orientation);
            }
            public void SetOrientation(int orientation)
            {
                if (orientation != HORIZONTAL && orientation != VERTICAL)
                    throw new System.Exception("invalid orientation");
                _orientation = orientation;
            }
            public override void OnDraw(Canvas cValue, RecyclerView parent)
            {
                if (_orientation == VERTICAL)
                {
                    DrawVertical(cValue, parent);
                }
                else
                {
                    DrawHorizontal(cValue, parent);
                }
            }
            // ˙∆¡ ±ª≠ ˙œﬂ
            public void DrawVertical(Canvas c, RecyclerView parent)
            {
                int left = parent.PaddingLeft;
                int right = parent.Width - parent.PaddingRight;
                int childCount = parent.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View childView = parent.GetChildAt(i);
                    RecyclerView v = new RecyclerView(parent.Context);
                    RecyclerView.LayoutParams _params = (RecyclerView.LayoutParams)childView.LayoutParameters;
                    int top = childView.Bottom + _params.BottomMargin;
                    int bottom = top + _divider.IntrinsicHeight;
                    _divider.SetBounds(left, top, right, bottom);
                    _divider.Draw(c);
                }
            }
            //∫·∆¡ ±ª≠∫·œﬂ
            public void DrawHorizontal(Canvas c, RecyclerView parent)
            {
                int top = parent.PaddingTop;
                int bottom = parent.Height - parent.PaddingBottom;
                int childCount = parent.ChildCount;
                for (int i = 0; i < childCount; i++)
                {
                    View childView = parent.GetChildAt(i);
                    RecyclerView v = new RecyclerView(parent.Context);
                    RecyclerView.LayoutParams _params = (RecyclerView.LayoutParams)childView.LayoutParameters;
                    int left = childView.Right + _params.RightMargin;
                    int right = left + _divider.IntrinsicHeight;
                    _divider.SetBounds(left, top, right, bottom);
                    _divider.Draw(c);
                }
            }
            public override void GetItemOffsets(Rect outRect, int itemPosition, RecyclerView parent)
            {
                if (_orientation == VERTICAL)
                {
                    outRect.Set(0, 0, 0, _divider.IntrinsicHeight);
                }
                else
                {
                    outRect.Set(0, 0, _divider.IntrinsicWidth, 0);
                }
            }
        }
    }