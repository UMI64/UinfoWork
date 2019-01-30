using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Graphics;

namespace Uinfo
{
    public class SearchEditText : EditText
    {
        private BitmapDrawable imgClear;
        private new Context Context;
        public SearchEditText(Context Context, IAttributeSet attrs) : base(Context, attrs)
        {
            this.Context = Context;
            init();
        }
        private void init()
        {
            imgClear = (BitmapDrawable)Context.GetDrawable(Resource.Drawable.del);
            imgClear.SetTargetDensity(Resources.DisplayMetrics);
            imgClear = zoomDrawable(imgClear, 17, 17, Resources.DisplayMetrics,Context);
            AfterTextChanged += (s, e) =>
            {
                SetDrawable();
            };

        }

        //回执删除图片  
        private void SetDrawable()
        {
            if (Length() < 1)
                SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
            else
                SetCompoundDrawablesWithIntrinsicBounds(null, null, imgClear, null);
        }
        //当触摸范围在右侧时，触发删除方法，隐藏叉叉  
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (imgClear != null && e.Action == MotionEventActions.Up)
            {
                int eventX = (int)e.RawX;
                int eventY = (int)e.RawY;
                Rect rect = new Rect();
                GetGlobalVisibleRect(rect);
                rect.Left = rect.Right - 100;
                if (rect.Contains(eventX, eventY))
                {
                    Text = string.Empty;
                }
            }
            return base.OnTouchEvent(e);
        }
        static Bitmap drawableToBitmap(Drawable drawable) // drawable 转换成 bitmap 
        {
            int width = drawable.IntrinsicWidth;   // 取 drawable 的长宽 
            int height = drawable.IntrinsicHeight;
            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);     // 建立对应 bitmap 
            Canvas canvas = new Canvas(bitmap);         // 建立对应 bitmap 的画布 
            drawable.SetBounds(0, 0, width, height);
            drawable.Draw(canvas);      // 把 drawable 内容画到画布中 
            return bitmap;
        }

        static BitmapDrawable zoomDrawable(Drawable drawable, int w, int h, DisplayMetrics displayMetrics,Context context)
        {
            int width = drawable.IntrinsicWidth;
            int height = drawable.IntrinsicHeight;
            Bitmap oldbmp = drawableToBitmap(drawable); // drawable 转换成 bitmap 
            Matrix matrix = new Matrix();// 创建操作图片用的 Matrix 对象 
            float scaleWidth = (float)w * displayMetrics.Density / width;   // 计算缩放比例 
            float scaleHeight = (float)h * displayMetrics.Density / height;
            matrix.PostScale(scaleWidth, scaleHeight);// 设置缩放比例 
            Bitmap newbmp = Bitmap.CreateBitmap(oldbmp, 0, 0,width,height, matrix, true);// 建立新的 bitmap ，其内容是对原 bitmap 的缩放后的图  
            return new BitmapDrawable(context.Resources , newbmp); // 把 bitmap 转换成 drawable 并返回 
        }
    }

}