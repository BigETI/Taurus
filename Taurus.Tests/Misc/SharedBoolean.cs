namespace Taurus.Tests
{
    internal class SharedBoolean
    {
        private volatile bool value;

        public bool Value
        {
            get => value;
            set => this.value = value;
        }

        public SharedBoolean(bool value) => this.value = value;

        public static implicit operator bool(SharedBoolean sharedBoolean) => sharedBoolean.value;
    }
}
