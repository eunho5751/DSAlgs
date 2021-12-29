
public struct QuadRegion
{
    public QuadRegion(Point2 position, float width, float height)
    {
        Position = position;
        Width = width;
        Height = height;
    }

    public bool Contains(Point2 position)
    {
        return position.X >= Position.X && position.X <= (Position.X + Width) && position.Y >= Position.Y && position.Y <= (Position.Y + Height);
    }

    public override string ToString()
    {
        return $"X : {Position.X}, Y : {Position.Y}, Width : {Width}, Height : {Height}";
    }

    public Point2 Position { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}
