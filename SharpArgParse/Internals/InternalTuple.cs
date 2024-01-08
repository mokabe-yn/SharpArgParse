namespace SharpArgParse.Internals
{
    /// <summary>ValueTuple</summary>
    internal readonly struct InternalTuple<T1, T2>
    {
        public T1 Item1 { get; }
        public T2 Item2 { get; }
        public InternalTuple(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
        public void Deconstructor(out T1 t1, out T2 t2)
        {
            t1 = Item1;
            t2 = Item2;
        }
    }
}