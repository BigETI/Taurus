using Taurus.Collections;

namespace Taurus.Tests
{
    [TestClass]
    public sealed class FastEnumerableUnitTests
    {
        [TestMethod]
        public void TestFastEnumerableConcurrentBag()
        {
            FastEnumerableConcurrentBag<int> ints =
                new()
                {
                    1,
                    2,
                    3
                };
            Assert.IsTrue(ints.Contains(1));
            Assert.IsTrue(ints.Contains(2));
            Assert.IsTrue(ints.Contains(3));
            Assert.IsFalse(ints.Contains(4));
            ints.TryTake(out int value);
            Assert.IsFalse(ints.Contains(value));
            ints.Clear();
        }

        [TestMethod]
        public void TestFastEnumerableConcurrentDictionary()
        {
            FastEnumerableConcurrentDictionary<int, int> ints = new();
            ints.TryAdd(1, 1);
            ints.TryAdd(2, 2);
            ints.TryAdd(3, 3);
            Assert.IsTrue(ints.ContainsKey(1));
            Assert.IsTrue(ints.ContainsKey(2));
            Assert.IsTrue(ints.ContainsKey(3));
            Assert.IsFalse(ints.ContainsKey(4));
            ints.TryRemove(2, out int value);
            Assert.AreEqual(2, value);
            Assert.IsFalse(ints.ContainsKey(2));
            ints.Clear();
        }

        [TestMethod]
        public void TestFastEnumerableConcurrentQueue()
        {
            FastEnumerableConcurrentQueue<int> ints = new();
            ints.Enqueue(1);
            ints.Enqueue(2);
            ints.Enqueue(3);
            Assert.IsTrue(ints.Contains(1));
            Assert.IsTrue(ints.Contains(2));
            Assert.IsTrue(ints.Contains(3));
            Assert.IsFalse(ints.Contains(4));
            ints.TryDequeue(out int value);
            Assert.IsFalse(ints.Contains(value));
            ints.Clear();
        }

        [TestMethod]
        public void TestFastEnumerableDictionary()
        {
            FastEnumerableDictionary<int, int> ints = new();
            ints.TryAdd(1, 1);
            ints.TryAdd(2, 2);
            ints.TryAdd(3, 3);
            Assert.IsTrue(ints.ContainsKey(1));
            Assert.IsTrue(ints.ContainsKey(2));
            Assert.IsTrue(ints.ContainsKey(3));
            Assert.IsFalse(ints.ContainsKey(4));
            ints.Remove(2, out int value);
            Assert.AreEqual(2, value);
            Assert.IsFalse(ints.ContainsKey(2));
            ints.Clear();
        }

        [TestMethod]
        public void TestFastEnumerableHashSet()
        {
            FastEnumerableHashSet<int> ints =
                new()
                {
                    1,
                    2,
                    3
                };
            Assert.IsTrue(ints.Contains(1));
            Assert.IsTrue(ints.Contains(2));
            Assert.IsTrue(ints.Contains(3));
            Assert.IsFalse(ints.Contains(4));
            ints.Remove(2);
            Assert.IsFalse(ints.Contains(2));
            ints.Clear();
        }
    }
}
