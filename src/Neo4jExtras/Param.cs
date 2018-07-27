namespace Neo4jExtras
{
    public static class Params
    {
        public static Params<T0> Create<T0>(T0 p0)
        {
            return new Params<T0>(p0);
        }

        public static Params<T0, T1> Create<T0, T1>(T0 p0, T1 p1)
        {
            return new Params<T0, T1>(p0, p1);
        }

        public static Params<T0, T1, T2> Create<T0, T1, T2>(T0 p0, T1 p1, T2 p2)
        {
            return new Params<T0, T1, T2>(p0, p1, p2);
        }
    }

    public class Params<T0>
    {
        public Params(T0 p0)
        {
            this.p0 = p0;
        }

        public T0 p0 { get; }
    }

    public class Params<T0, T1>
    {
        public Params(T0 p0, T1 p1)
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        public T0 p0 { get; }
        public T1 p1 { get; }
    }

    public class Params<T0, T1, T2>
    {
        public Params(T0 p0, T1 p1, T2 p2)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
        }

        public T0 p0 { get; }
        public T1 p1 { get; }
        public T2 p2 { get; }
    }
}
