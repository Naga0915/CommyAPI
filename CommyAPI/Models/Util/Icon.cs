using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System.Text;
using System.Security.Cryptography;
using CommyAPI.Setting.Media;
using CommyAPI.Setting;

namespace CommyAPI.Models.Util
{
    public static class Icon
    {
        public static async Task<MemoryStream> GetIconAsync(string id, CommyServices service)
        {
            int size = (int)service;
            bool frame = (service == CommyServices.BBSTrip);
            return await GenerateIcon(id, size, (int)(((size * size) / 2) * GlobalSetting.iconPointRatio), 10, frame);
        }

        private static async Task<MemoryStream> GenerateIcon(string id, int imageSize, int pointNum, int threshold, bool frame)
        {
            if (imageSize < 2) throw new IconGenerationException("アイコンサイズは2以上");
            if (imageSize % 2 == 1) throw new IconGenerationException("アイコンサイズは偶数");
            if (pointNum > (imageSize * imageSize) / 2) throw new IconGenerationException($"模様の数は{(imageSize * imageSize) / 2}以下");
            Color colorBG;
            Color colorPoint;
            Color colorFrame = Color.Purple;
            int seed;
            //idからハッシュ生成
            byte[] raw = Encoding.UTF8.GetBytes(id);
            byte[] hashed;
            using (SHA256 sha256 = SHA256.Create())
            {
                hashed = sha256.ComputeHash(raw);
            }

            //ハッシュを乱数のシードに
            seed = BitConverter.ToInt32(hashed, 0);
            var rnd = new Random(seed);
            var pointPos = new List<PointF>();
            //模様生成
            for (int i = 0; i < pointNum; i++)
            {
                int x = rnd.Next(0, imageSize / 2);
                int y = rnd.Next(0, imageSize);
                var point = new PointF(x, y);
                if (pointPos.Contains(point))
                {
                    i--;
                    continue;
                }
                var mirrorPoint = new PointF(imageSize - 1 - x, y);
                pointPos.Add(point);
                pointPos.Add(mirrorPoint);
            }

            byte[] colorBGBytes = new byte[3];
            byte[] colorPointBytes = new byte[3];
            rnd.NextBytes(colorBGBytes);
            //背景の色生成
            colorBG = Color.FromRgb(colorBGBytes[0], colorBGBytes[1], colorBGBytes[2]);
            double diff;
            do
            {
                //色ベクトルのノルム計算　数値が低いと同じような色で、高いと違う色
                rnd.NextBytes(colorPointBytes);
                int diffX = Math.Abs(colorBGBytes[0] - colorPointBytes[0]);
                int diffY = Math.Abs(colorBGBytes[1] - colorPointBytes[1]);
                int diffZ = Math.Abs(colorBGBytes[2] - colorPointBytes[2]);

                diff = Math.Sqrt(diffX + diffY + diffZ);
                //ノルムがしきい値以下であれば繰り返す
            } while (diff < threshold);
            //模様の色生成
            colorPoint = Color.FromRgb(colorPointBytes[0], colorPointBytes[1], colorPointBytes[2]);

            //枠生成
            byte[] colorFrameBytes = new byte[3];
            if (frame)
            {
                double diff2;
                do
                {
                    //色ベクトルのノルム計算　数値が低いと同じような色で、高いと違う色
                    rnd.NextBytes(colorFrameBytes);
                    int diffX = Math.Abs(colorFrameBytes[0] - colorBGBytes[0]);
                    int diffY = Math.Abs(colorFrameBytes[1] - colorBGBytes[1]);
                    int diffZ = Math.Abs(colorFrameBytes[2] - colorBGBytes[2]);

                    diff = Math.Sqrt(diffX + diffY + diffZ);

                    int diff2X = Math.Abs(colorFrameBytes[0] - colorPointBytes[0]);
                    int diff2Y = Math.Abs(colorFrameBytes[1] - colorPointBytes[1]);
                    int diff2Z = Math.Abs(colorFrameBytes[2] - colorPointBytes[2]);

                    diff2 = Math.Sqrt(diff2X + diff2Y + diff2Z);

                    //ノルムがしきい値以下であれば繰り返す
                } while (diff < threshold || diff2 < threshold);

                colorFrame = Color.FromRgb(colorFrameBytes[0], colorFrameBytes[1], colorFrameBytes[2]);
            }

            //画像生成
            Pen penPoint = Pens.Solid(colorPoint, 1);
            Image<Rgba32> img = new(imageSize, imageSize);
            img.Mutate(x => x.Fill(colorBG));
            foreach (var point in pointPos)
            {
                img.Mutate(x => x.DrawLine(penPoint, point, point));
            }
            //フレーム生成
            if (frame)
            {
                Pen penFrame = Pens.Solid(colorFrame, 1);
                img.Mutate(x => x.DrawLine(penFrame, new PointF(0, 0), new PointF(0, imageSize - 1)));
                img.Mutate(x => x.DrawLine(penFrame, new PointF(0, 0), new PointF(imageSize - 1, 0)));
                img.Mutate(x => x.DrawLine(penFrame, new PointF(imageSize - 1, imageSize - 1), new PointF(0, imageSize - 1)));
                img.Mutate(x => x.DrawLine(penFrame, new PointF(imageSize - 1, imageSize - 1), new PointF(imageSize - 1, 0)));
            }

            //画像のリサイズ
            ResizeOptions options = new ResizeOptions();
            options.Size = new Size(256, 256);
            options.Sampler = KnownResamplers.NearestNeighbor;
            img.Mutate(x => x.Resize(options));
            //ストリームに保存
            var stream = new MemoryStream();
            await img.SaveAsPngAsync(stream);
            switch (MIME.mimeBBSIcon)
            {
                case "image/png":
                    await img.SaveAsPngAsync(stream);
                    break;
                default:
                    throw new IconGenerationException("無効なMIMEタイプ：" + MIME.mimeBBSIcon);
            }
            img.Dispose();
            stream.Position = 0;
            return stream;
        }
    }

    public enum CommyServices
    {
        None = 0,
        BBS = 8,
        BBSTrip = 12
    }


    internal class IconGenerationException : Exception
    {
        public IconGenerationException(string? message) : base(message)
        {
        }
    }
}
