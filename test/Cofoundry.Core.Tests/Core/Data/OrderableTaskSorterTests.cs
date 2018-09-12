using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cofoundry.Core.Tests
{
    public class OrderableTaskSorterTests
    {
        #region private test classes

        private interface TestSortableTask { };

        private class FirstTask : TestSortableTask, IRunAfterTask, IOrderedTask
        {
            public ICollection<Type> RunAfter { get; set; }

            public int Ordering  { get; set; }
        }

        private class SecondTask : TestSortableTask, IRunBeforeTask, IOrderedTask
        {
            public ICollection<Type> RunBefore { get; set; }

            public int Ordering { get; set; }
        }

        private class ThirdTask : TestSortableTask, IRunAfterTask, IOrderedTask
        {
            public ICollection<Type> RunAfter { get; set; }

            public int Ordering { get; set; }
        }

        private class FourthTask : TestSortableTask, IOrderedTask
        {
            public int Ordering { get; set; }
        }

        private class FithTask : TestSortableTask { }

        private class SixthTask : TestSortableTask { }

        #endregion

        [Fact]
        public void Sort_NullSource_ThrowsException()
        {
            string[] collection = null;

            Assert.Throws<ArgumentNullException>(() =>
            {
                OrderableTaskSorter.Sort(collection);
            });
        }

        [Fact]
        public void Sort_WithDependencies_CanSort()
        {
            var list = new List<TestSortableTask>()
            {
                new FirstTask() { RunAfter = new Type[] { typeof(SecondTask), typeof(FithTask) } },
                new SecondTask() { RunBefore = new Type[] { typeof(SixthTask) } },
                new ThirdTask() { RunAfter = new Type[] { typeof(FirstTask) } },
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
            var list = new List<TestSortableTask>()
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
            var list = new List<TestSortableTask>()
            {
                new FirstTask() { Ordering = 1, RunAfter = new Type[] { typeof(SecondTask), typeof(FithTask) } },
                new SecondTask() { Ordering = 10, RunBefore = new Type[] { typeof(SixthTask) } },
                new ThirdTask() { Ordering = 100, RunAfter = new Type[] { typeof(FirstTask) } },
                new FourthTask() { Ordering = 1 },
                new FithTask(),
                new SixthTask()
            };

            var result = OrderableTaskSorter.Sort(list);

            var stringResult = string.Join(',', result.Select(r => r.GetType().Name));
            var expected = string.Join(',', nameof(FithTask), nameof(SecondTask), nameof(SixthTask), nameof(FirstTask), nameof(FourthTask), nameof(ThirdTask));

            Assert.Equal(expected, stringResult);
        }
    }
}
