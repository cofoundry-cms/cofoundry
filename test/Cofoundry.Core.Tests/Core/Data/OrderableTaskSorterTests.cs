namespace Cofoundry.Core.Tests;

public class OrderableTaskSorterTests
{
    [Fact]
    public void Sort_NullSource_ThrowsException()
    {
        string[]? collection = null;

        Assert.Throws<ArgumentNullException>(() =>
        {
            OrderableTaskSorter.Sort(collection!);
        });
    }

    [Fact]
    public void Sort_WithDependencies_CanSort()
    {
        var list = new List<ITestSortableTask>()
        {
            new FirstTask() { RunAfter = [typeof(SecondTask), typeof(FithTask)] },
            new SecondTask() { RunBefore = [typeof(SixthTask)]},
            new ThirdTask() { RunAfter = [typeof(FirstTask)]},
            new FourthTask(),
            new FithTask(),
            new SixthTask()
        };

        var result = OrderableTaskSorter.Sort(list);

        var stringResult = string.Join(',', result.Select(r => r.GetType().Name));
        var expected = string.Join(',', nameof(SecondTask), nameof(FithTask), nameof(FirstTask), nameof(FourthTask), nameof(SixthTask), nameof(ThirdTask));

        Assert.Equal(expected, stringResult);
    }

    [Fact]
    public void Sort_WithOrdering_CanSort()
    {
        var list = new List<ITestSortableTask>()
        {
            new FirstTask() { Ordering = 6 },
            new SecondTask() { Ordering = 5 },
            new ThirdTask() { Ordering = -3 },
            new FourthTask() { Ordering = -8 },
            new FithTask(),
            new SixthTask()
        };

        var result = OrderableTaskSorter.Sort(list);

        var stringResult = string.Join(',', result.Select(r => r.GetType().Name));
        var expected = string.Join(',', nameof(FourthTask), nameof(ThirdTask), nameof(FithTask), nameof(SixthTask), nameof(SecondTask), nameof(FirstTask));

        Assert.Equal(expected, stringResult);
    }

    [Fact]
    public void Sort_WithOrderingAndDependencies_CanSort()
    {
        var list = new List<ITestSortableTask>()
        {
            new FirstTask() { Ordering = 1, RunAfter = [typeof(SecondTask), typeof(FithTask)] },
            new SecondTask() { Ordering = 10, RunBefore = [typeof(SixthTask)] },
            new ThirdTask() { Ordering = 100, RunAfter = [typeof(FirstTask)] },
            new FourthTask() { Ordering = 1 },
            new FithTask(),
            new SixthTask()
        };

        var result = OrderableTaskSorter.Sort(list);

        var stringResult = string.Join(',', result.Select(r => r.GetType().Name));
        var expected = string.Join(',', nameof(FithTask), nameof(SecondTask), nameof(SixthTask), nameof(FirstTask), nameof(FourthTask), nameof(ThirdTask));

        Assert.Equal(expected, stringResult);
    }

    private interface ITestSortableTask { };

    private class FirstTask : ITestSortableTask, IRunAfterTask, IOrderedTask
    {
        public IReadOnlyCollection<Type> RunAfter { get; set; } = Array.Empty<Type>();

        public int Ordering { get; set; }
    }

    private class SecondTask : ITestSortableTask, IRunBeforeTask, IOrderedTask
    {
        public IReadOnlyCollection<Type> RunBefore { get; set; } = Array.Empty<Type>();

        public int Ordering { get; set; }
    }

    private class ThirdTask : ITestSortableTask, IRunAfterTask, IOrderedTask
    {
        public IReadOnlyCollection<Type> RunAfter { get; set; } = Array.Empty<Type>();

        public int Ordering { get; set; }
    }

    private class FourthTask : ITestSortableTask, IOrderedTask
    {
        public int Ordering { get; set; }
    }

    private class FithTask : ITestSortableTask { }

    private class SixthTask : ITestSortableTask { }
}
