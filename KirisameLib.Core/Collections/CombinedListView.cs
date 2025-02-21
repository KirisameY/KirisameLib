using System.Collections;
using System.Collections.Immutable;

using JetBrains.Annotations;

namespace KirisameLib.Collections;

public class CombinedListView<T>(params IEnumerable<IReadOnlyList<T>> lists) : IReadOnlyList<T>
{
    private ImmutableArray<IReadOnlyList<T>> _lists = lists.ToImmutableArray();

    [MustDisposeResource]
    public IEnumerator<T> GetEnumerator() => _lists.SelectMany(list => list).GetEnumerator();

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _lists.Sum(list => list.Count);

    public T this[int index]
    {
        get
        {
            foreach (var list in _lists)
            {
                if (index >= list.Count) index -= list.Count;
                else return list[index];
            }

            throw new IndexOutOfRangeException();
        }
    }
}