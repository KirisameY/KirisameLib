using System.Collections;

using JetBrains.Annotations;

namespace KirisameLib.Core.Collections;

public class CombinedCollectionView<T>(params IReadOnlyCollection<T>[] collections) : IReadOnlyCollection<T>, ICollection<T>
{
    [MustDisposeResource]
    public IEnumerator<T> GetEnumerator() => collections.SelectMany(collection => collection).GetEnumerator();

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => collections.Sum(c => c.Count);

    public bool Contains(T item) => collections.Any(collection => collection.Contains(item));


    //ICollection
    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex <= 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, "Param arrayIndex must greater than 0");

        using var enumerator = GetEnumerator();
        for (int i = arrayIndex; i < array.Length && enumerator.MoveNext(); i++)
        {
            array[i] = enumerator.Current;
        }
        if (enumerator.MoveNext())
            throw new ArgumentException("The number of elements in the source collection is greater than"
                                      + " the available space from arrayIndex to the end of the destination array {array}.");
    }

    //Readonly
    bool ICollection<T>.IsReadOnly => true;

    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    void ICollection<T>.Clear() => throw new NotSupportedException();

    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();
}