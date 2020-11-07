using System.Drawing;
using System.Drawing.Imaging;



namespace HelpTools
{

   public class ImgTool
   {
       /// <summary>
       /// 放大图片
       /// </summary>
       /// <param name="srcbitmap">原始图片</param>
       /// <param name="multiple">放大倍数</param>
       /// <returns>返回处理后的图片</returns>
       public Bitmap Magnifier(Bitmap srcbitmap, int multiple)
       {
           if (multiple <= 0) { multiple = 0; return srcbitmap; }
           Bitmap bitmap = new Bitmap(srcbitmap.Size.Width * multiple, srcbitmap.Size.Height * multiple);
           BitmapData srcbitmapdata = srcbitmap.LockBits(new Rectangle(new Point(0, 0), srcbitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
           BitmapData bitmapdata = bitmap.LockBits(new Rectangle(new Point(0, 0), bitmap.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
           unsafe
           {
               byte* srcbyte = (byte*)(srcbitmapdata.Scan0.ToPointer());
               byte* sourcebyte = (byte*)(bitmapdata.Scan0.ToPointer());
               for (int y = 0; y < bitmapdata.Height; y++)
               {
                   for (int x = 0; x < bitmapdata.Width; x++)
                   {
                       long index = (x / multiple) * 4 + (y / multiple) * srcbitmapdata.Stride;
                       sourcebyte[0] = srcbyte[index];
                       sourcebyte[1] = srcbyte[index + 1];
                       sourcebyte[2] = srcbyte[index + 2];
                       sourcebyte[3] = srcbyte[index + 3];
                       sourcebyte += 4;
                   }
               }
           }
           srcbitmap.UnlockBits(srcbitmapdata);
           bitmap.UnlockBits(bitmapdata);
           return bitmap;
       }

       /// <summary>
       /// 截取图片方法
       /// </summary>
       /// <param name="bitmap">需要裁剪的图</param>
       /// <param name="beginX">开始位置-X</param>
       /// <param name="beginY">开始位置-Y</param>
       /// <param name="getX">截取长度</param>
       /// <param name="getY">截取宽度</param>
       public Bitmap CutImage(Bitmap bitmap, int beginX, int beginY, int getX, int getY)
       {
           Bitmap destBitmap = new Bitmap(getX, getY);//目标图
           Rectangle destRect = new Rectangle(0, 0, getX, getY);//矩形容器
           Rectangle srcRect = new Rectangle(beginX, beginY, getX, getY);
           Graphics g = Graphics.FromImage(destBitmap);
           g.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
           g.Dispose();
           return destBitmap;

       }

    }
}
