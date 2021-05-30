public struct Voxel
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }
    public float Size { get; private set; }

    public Voxel(float x, float y, float z, float size)
    {
        X = x;
        Y = y;
        Z = z;
        Size = size;
    }
}