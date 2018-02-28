/// <summary>
/// Simple tuple-like container for realted pairs of values that should be passed together
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T2"></typeparam>
public class Pair<T, T2>
    {
        public T A { get; set; }
        public T2 B { get; set; }

        public Pair(T a, T2 b)
        {
            A = a;
            B = b;
        }
    }
