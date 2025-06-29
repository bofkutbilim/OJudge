namespace OJudge.Services
{
    public class MyHashService
    {
        public static string CreateHash(string password)
        {
            const int p = 31;
            const int m = 1_000_000_009;

            long hash = 0;
            long pow = 1;

            foreach (char c in password)
            {
                hash = (hash + (c * pow) % m) % m;
                pow = (pow * p) % m;
            }

            return hash.ToString();
        }
    }
}
