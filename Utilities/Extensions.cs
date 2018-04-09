namespace Utilities
{
    public static class Extensions
    {
        public static long Mb(this int mb)
        {
            return mb.Kb() * 1024;
        }
        
        public static long Kb(this int kb)
        {
            return kb * 1024;
        }
    }
}