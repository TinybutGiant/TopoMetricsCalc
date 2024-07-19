/* using SkiaSharp;
using System;
using System.IO;

public class CircleOverlap
{
    public static void Main()
    {
        float radius1 = 50;
        float radius2 = 50;
        float distance = 30; // 距离两个圆心的距离

        // 计算重叠面积
        float overlapArea = CalculateOverlapArea(radius1, radius2, distance);
        Console.WriteLine($"Overlap Area: {overlapArea}");

        // 绘制两个圆
        DrawCircles(radius1, radius2, distance);
    }

    public static float CalculateOverlapArea(float r1, float r2, float d)
    {
        if (d >= r1 + r2) return 0; // 没有重叠
        if (d <= Math.Abs(r1 - r2)) return (float)(Math.PI * Math.Min(r1, r2) * Math.Min(r1, r2)); // 一个圆完全包含在另一个圆内

        float part1 = r1 * r1 * (float)Math.Acos((d * d + r1 * r1 - r2 * r2) / (2 * d * r1));
        float part2 = r2 * r2 * (float)Math.Acos((d * d + r2 * r2 - r1 * r1) / (2 * d * r2));
        float part3 = 0.5f * (float)Math.Sqrt((-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2));

        return part1 + part2 - part3;
    }

    public static void DrawCircles(float r1, float r2, float d)
    {
        var info = new SKImageInfo(300, 150);
        using (var surface = SKSurface.Create(info))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var paint = new SKPaint
            {
                Color = SKColors.Blue,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            canvas.DrawCircle(100, 75, r1, paint);
            canvas.DrawCircle(100 + d, 75, r2, paint);

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite("circles.png"))
            {
                data.SaveTo(stream);
            }
        }

        Console.WriteLine("Circles drawn and saved as circles.png");
    }
}
*/