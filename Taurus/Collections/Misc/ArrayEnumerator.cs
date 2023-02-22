using System;
using System.Collections;
using System.Collections.Generic;

namespace Taurus.Collections
{
    internal sealed class ArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] array;

        private int index = -1;

        public T Current
        {
            get
            {
                try
                {
                    return array[index];
                }
                catch
                {
                    throw new InvalidOperationException("Call MoveNext() first.");
                }
            }
        }

        object? IEnumerator.Current => Current;

        public ArrayEnumerator(T[] array) => this.array = array;

        public bool MoveNext()
        {
            bool ret = (index + 1) < array.Length;
            index = ret ? (index + 1) : index;
            return ret;
        }

        public void Reset() => index = 0;

        public void Dispose() => index = -1;
    }
}
