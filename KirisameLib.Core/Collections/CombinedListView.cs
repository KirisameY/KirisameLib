using System.Collections;

using JetBrains.Annotations;

namespace KirisameLib.Collections;

public class CombinedListView<T>(params IReadOnlyList<T>[] lists) : IReadOnlyList<T>
{
    [MustDisposeResource]
    public IEnumerator<T> GetEnumerator() => lists.SelectMany(list => list).GetEnumerator();

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => lists.Sum(list => list.Count);

    public T this[int index]
    {
        get
        {
            foreach (var list in lists)
            {
                if (index >= list.Count) index -= list.Count;
                else return list[index];
            }

            throw new IndexOutOfRangeException();
        }
    }
}