namespace Cofoundry.Core.Tests;

public class TopologicalSorterTests
{
    public class SortableEntity
    {
        public SortableEntity(int myId, int? dependentOnId)
        {
            MyId = myId;
            DependentOnId = dependentOnId;
        }

        public int MyId { get; set; }

        public int? DependentOnId { get; set; }

        public override string ToString()
        {
            return $"MyId: {MyId}, DependentId: {DependentOnId}";
        }
    }

    [Fact]
    public void Sort_NullSource_ThrowsException()
    {
        string[]? collection = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            TopologicalSorter.Sort(collection!, (item, source) => Array.Empty<string>(), true);
        });
    }

    [Fact]
    public void Sort_NullDependencySelector_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            TopologicalSorter.Sort(Array.Empty<string>(), null!, true);
        });
    }

    [Fact]
    public void Sort_CanSortList()
    {
        var list = new List<SortableEntity>()
        {
            new(1, 2),
            new(2, 10),
            new(3, 10),
            new(4, 10),
            new(5, null),
            new(6, 5),
            new(7, 5),
            new(8, null),
            new(9, 8),
            new(10, 5),
        };

        var result = TopologicalSorter.Sort(
            list,
            (item, source) => source.Where(l => l.MyId == item.DependentOnId),
            true
            );

        var stringResult = string.Join(',', result.Select(r => r.MyId));

        Assert.Equal("5,10,2,1,3,4,6,7,8,9", stringResult);
    }

    [Fact]
    public void Sort_WithCyclicDependency_ThrowsException()
    {
        var list = new List<SortableEntity>()
        {
            new(1, 2),
            new(2, 3),
            new(3, 4),
            new(4, 5),
            new(5, 1),
        };

        Assert.Throws<CyclicDependencyException>(() =>
        {
            var result = TopologicalSorter.Sort(
                list,
                (item, source) => source.Where(l => l.MyId == item.DependentOnId),
                true
                );
        });
    }

    [Fact]
    public void Sort_WithCyclicDependencyAndNoThrowOption_DoesNotThrowsException()
    {
        var list = new List<SortableEntity>()
        {
            new(1, 5),
            new(2, 4),
            new(3, 3),
            new(4, 2),
            new(5, 1),
        };

        var result = TopologicalSorter.Sort(
            list,
            (item, source) => source.Where(l => l.MyId == item.DependentOnId),
            false
            );

        var stringResult = string.Join(',', result.Select(r => r.MyId));

        Assert.Equal("5,1,4,2,3", stringResult);
    }
}
