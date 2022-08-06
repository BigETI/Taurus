namespace Taurus.Fragmenters
{
    public static class Fragmentation
    {
        /// <summary>
        /// No fragmentation fragmenter
        /// </summary>
        public static IFragmenter NoFragmentationFragmenter { get; } = new NoFragmentationFragmenter();
    }
}
