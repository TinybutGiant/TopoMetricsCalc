using SkiaSharp;
using System;
using System.IO;

public class ConcentricCirclesOverlap
{
    public static void Main()
    {
        float RA_1 = 50; // 大圆半径
        float RA_2 = 30; // 小圆半径
        float RB_1 = 50; // 大圆半径
        float RB_2 = 30; // 小圆半径
        float d = 25;    // 两圆心距离

        // 计算重叠面积
        float overlapArea = CalculateOverlapArea(RA_1, RA_2, RB_1, RB_2, d);
        Console.WriteLine($"Overlap Area: {overlapArea}");

        // 绘制同心圆
        DrawConcentricCircles(RA_1, RA_2, RB_1, RB_2, d);
    }

    public static float CalculateOverlapArea(float RA_1, float RA_2, float RB_1, float RB_2, float d)
    {
        // 情况1：两个圆不相交
        if (d >= RA_1 + RB_1) return 0;

        // 情况2：一个圆完全包含在另一个圆内
        if (d <= Math.Abs(RA_1 - RB_1)) return (float)(Math.PI * Math.Min(RA_2, RB_2) * Math.Min(RA_2, RB_2));

        // 情况3：部分相交，需要计算重叠面积
        float part1 = SegmentArea(RA_1, RA_2, d);
        float part2 = SegmentArea(RB_1, RB_2, d);

        return part1 + part2;
    }

    public static float SegmentArea(float R1, float R2, float d)
    {
        float r1 = Math.Min(R1, R2);
        float r2 = Math.Max(R1, R2);

        float part1 = r1 * r1 * (float)Math.Acos((d * d + r1 * r1 - r2 * r2) / (2 * d * r1));
        float part2 = r2 * r2 * (float)Math.Acos((d * d + r2 * r2 - r1 * r1) / (2 * d * r2));
        float part3 = 0.5f * (float)Math.Sqrt((-d + r1 + r2) * (d + r1 - r2) * (d - r1 + r2) * (d + r1 + r2));

        return part1 + part2 - part3;
    }

    public static void DrawConcentricCircles(float RA_1, float RA_2, float RB_1, float RB_2, float d)
    {
        var info = new SKImageInfo(400, 200);
        using (var surface = SKSurface.Create(info))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            var paint = new SKPaint
            {
                Color = SKColors.Green,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            // 绘制第一个同心圆
            canvas.DrawCircle(100, 100, RA_1, paint);
            canvas.DrawCircle(100, 100, RA_2, paint);

            // 绘制第二个同心圆
            canvas.DrawCircle(100 + d, 100, RB_1, paint);
            canvas.DrawCircle(100 + d, 100, RB_2, paint);

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite("concentric_circles.png"))
            {
                data.SaveTo(stream);
            }
        }

        Console.WriteLine("Concentric circles drawn and saved as concentric_circles.png");
    }
}
