
public struct QuadRegion
{
    public QuadRegion(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Contains(float x, float y)
    {
        return x >= X && x <= (X + Width) && y >= Y && y <= (Y + Height);
    }

    public override string ToString()
    {
        return $"X : {X}, Y : {Y}, Width : {Width}, Height : {Height}";
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}
