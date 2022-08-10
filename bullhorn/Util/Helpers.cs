using System;
namespace bullhorn.Util
{
    public class Helpers
    {
        public static bool IsNull<T>(T obj)
        {
            if (obj == null) return true;
            return false;
        }
    }
}

