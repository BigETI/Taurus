namespace Taurus.Fragmenters
{
    public static class Fragmentation
    {
        /// <summary>
        /// No fragmenter
        /// </summary>
        public static IFragmenter NoFragmenter { get; } = new NoFragmenter();
    }
}
