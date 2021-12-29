
public struct Point2
{
    public Point2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static Point2 operator+(Point2 lhs, float value) => new Point2(lhs.X + value, lhs.Y + value);
    public static Point2 operator-(Point2 lhs, float value) => new Point2(lhs.X - value, lhs.Y - value);
    public static Point2 operator*(Point2 lhs, float value) => new Point2(lhs.X * value, lhs.Y * value);
    public static Point2 operator/(Point2 lhs, float value) => new Point2(lhs.X / value, lhs.Y / value);
    public static Point2 operator+(Point2 lhs, Point2 rhs) => new Point2(lhs.X + rhs.X, lhs.Y + rhs.Y);
    public static Point2 operator-(Point2 lhs, Point2 rhs) => new Point2(lhs.X - rhs.X, lhs.Y - rhs.Y);

    public override string ToString() => $"({X}, {Y})";

    public float X { get; set; }
    public float Y { get; set; }
}
