using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core.Assist;
using Com.Nostra13.Universalimageloader.Core.Display;
using Com.Nostra13.Universalimageloader.Core.Imageaware;
using Android.Graphics.Drawables;

namespace Cnblogs.XamarinAndroid
{
    public class DisplayerImageCircle:RoundedBitmapDisplayer
    {
        private static int margin;
        public DisplayerImageCircle(int cornerRadiusPixels):base(cornerRadiusPixels)
        {

        }
        public override void Display(Bitmap bitmap, IImageAware imageAware, LoadedFrom p2)
        {
            base.Display(bitmap, imageAware, p2);
            imageAware.SetImageDrawable(new CircleDrawable(bitmap,margin));
        }
        public class CircleDrawable : Drawable
        {
            
            private static RectF rect = new RectF();
            private static BitmapShader bitmapShader;
            private static Paint paint;
            private RectF bitmapRect;
            public CircleDrawable(Bitmap bitmap,int margin)
            {
                margin = 0;
                bitmapShader = new BitmapShader(bitmap,Shader.TileMode.Clamp,Shader.TileMode.Clamp);
                bitmapRect = new RectF(margin,margin,bitmap.Width-margin,bitmap.Height-margin);
                //设置画笔
                paint = new Paint();
                paint.AntiAlias = true;
                paint.SetShader(bitmapShader);
            }

            protected override void OnBoundsChange(Rect bounds)
            {
                base.OnBoundsChange(bounds);
                rect.Set(margin,margin,bounds.Width()-margin,bounds.Height()-margin);
                //调整位图，设置矩阵
                Matrix shaderMatrix = new Matrix();
                shaderMatrix.SetRectToRect(bitmapRect,rect,Matrix.ScaleToFit.Fill);
                //设置矩阵着色器
                bitmapShader.SetLocalMatrix(shaderMatrix);
            }
            public override int Opacity
            {
                get
                {
                    return (int)Format.Translucent;
                }
            }

            public override void Draw(Canvas canvas)
            {
                //throw new NotImplementedException();
                canvas.DrawRoundRect(rect,rect.Width()/2,rect.Height()/2,paint);
            }

            public override void SetAlpha(int alpha)
            {
                //throw new NotImplementedException();
                paint.Alpha=alpha;
            }

            public override void SetColorFilter(ColorFilter colorFilter)
            {
                //throw new NotImplementedException();
                paint.SetColorFilter(colorFilter);
            }
        }
    }
}