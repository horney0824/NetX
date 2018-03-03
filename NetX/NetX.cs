namespace NetX
{
    public enum SaveFlags : byte
    {
        None = 0,
        Compressed = 1 << 0,
        JSON = 1 << 1
    };
}
