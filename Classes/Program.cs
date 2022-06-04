class Driver
{
    public static void Main(string[] args)
    {
        int[] arr = { 1, 2, 3, 4 };
        foreach (var el in arr)
        {
            Console.WriteLine(el);
        }

        string[,] array2d;

        array2d = new string[,]
        {
            {"a1", "a2"},
            {"b1", "b2"}
        };
        Console.WriteLine(array2d[1,1]);
    }
}