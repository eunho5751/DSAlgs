
public struct Point3
{
    public Point3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Point3 operator+(Point3 lhs, float value) => new Point3(lhs.X + value, lhs.Y + value, lhs.Z + value);
    public static Point3 operator-(Point3 lhs, float value) => new Point3(lhs.X - value, lhs.Y - value, lhs.Z - value);
    public static Point3 operator*(Point3 lhs, float value) => new Point3(lhs.X * value, lhs.Y * value, lhs.Z * value);
    public static Point3 operator/(Point3 lhs, float value) => new Point3(lhs.X / value, lhs.Y / value, lhs.Z / value);
    public static Point3 operator+(Point3 lhs, Point3 rhs) => new Point3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
    public static Point3 operator-(Point3 lhs, Point3 rhs) => new Point3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);

    public static float Dot(Point3 lhs, Point3 rhs) => lhs.X * rhs.X + lhs.Y * rhs.Y + lhs.Z * rhs.Z;
    public static Point3 Cross(Point3 lhs, Point3 rhs) => new Point3(lhs.Y * rhs.Z - lhs.Z * rhs.Y, lhs.Z * rhs.X - lhs.X * rhs.Z, lhs.X * rhs.Y - lhs.Y * rhs.X);

    public override string ToString() => $"({X}, {Y}, {Z})";

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}