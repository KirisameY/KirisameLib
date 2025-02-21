using System.Collections;
using System.Collections.Immutable;

using JetBrains.Annotations;

namespace KirisameLib.Collections;

public class CombinedCollectionView<T>(params IEnumerable<IReadOnlyCollection<T>> collections) : IReadOnlyCollection<T>, ICollection<T>
{
    private readonly ImmutableArray<IReadOnlyCollection<T>> _collections = collections.ToImmutableArray();

    [MustDisposeResource]
    public IEnumerator<T> GetEnumerator() => _collections.SelectMany(collection => collection).GetEnumerator();

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _collections.Sum(c => c.Count);

    public bool Contains(T item) => _collections.Any(collection => collection.Contains(item));


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