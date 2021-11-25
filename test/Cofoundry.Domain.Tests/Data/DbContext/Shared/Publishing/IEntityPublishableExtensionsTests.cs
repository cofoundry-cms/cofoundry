using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using Xunit;

namespace Cofoundry.Domain.Tests.Data.DbContext.Shared
{
    public class IEntityPublishableExtensionsTests
    {
        private static DateTime _seedDate = new DateTime(2018, 11, 30, 09, 00, 00);

        [Fact]
        public void SetPublished_WhenNotSet_UsesCurrentDate()
        {
            var currentDate = _seedDate;
            var entity = new Page();
            var hasStatsuChanged = entity.SetPublished(currentDate, null);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeTrue();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(currentDate);
                entity.LastPublishDate.Should().Be(currentDate);
            }
        }

        [Fact]
        public void SetPublished_WhenNotSet_CanSetSpecificDate()
        {
            var currentDate = _seedDate;
            var specificPublishDate = _seedDate.AddMinutes(49);
            var entity = new Page();
            var hasStatsuChanged = entity.SetPublished(currentDate, specificPublishDate);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeTrue();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(specificPublishDate);
                entity.LastPublishDate.Should().Be(specificPublishDate);
            }
        }

        [Fact]
        public void SetPublished_WhenPublished_UsesCurrentDate()
        {
            var firstDate = _seedDate;
            var secondDate = _seedDate.AddMinutes(123);
            var entity = new Page();
            entity.SetPublished(firstDate, null);
            var hasStatsuChanged = entity.SetPublished(secondDate, null);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeFalse();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(firstDate);
                entity.LastPublishDate.Should().Be(secondDate);
            }
        }

        [Fact]
        public void SetPublished_WhenPublished_CanSetSpecificDate()
        {
            var firstDate = _seedDate;
            var secondDate = _seedDate.AddMinutes(24);
            var specificPublishDate = _seedDate.AddMinutes(469);
            var entity = new Page();
            entity.SetPublished(firstDate, null);
            var hasStatsuChanged = entity.SetPublished(secondDate, specificPublishDate);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeFalse();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(specificPublishDate);
                entity.LastPublishDate.Should().Be(specificPublishDate);
            }
        }

        [Fact]
        public void SetPublished_WhenPublished_CanUseSpecificDateInPast()
        {
            var firstDate = _seedDate;
            var secondDate = _seedDate.AddMinutes(24);
            var specificPublishDate = secondDate.AddDays(-6);
            var entity = new Page();
            entity.SetPublished(firstDate, null);
            var hasStatsuChanged = entity.SetPublished(secondDate, specificPublishDate);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeFalse();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(specificPublishDate);
                entity.LastPublishDate.Should().Be(secondDate);
            }
        }

        [Fact]
        public void SetPublished_WhenPreviouslyPublishedWithSpecifiedDate_NewerCurrentDateIsSetAsLast()
        {
            var firstDate = _seedDate;
            var specificPublishDate = firstDate.AddDays(4);
            var secondDate = specificPublishDate.AddMinutes(945);
            var entity = new Page();
            entity.SetPublished(firstDate, specificPublishDate);
            var hasStatsuChanged = entity.SetPublished(secondDate);

            using (new AssertionScope())
            {
                hasStatsuChanged.Should().BeFalse();
                entity.PublishStatusCode.Should().Be(PublishStatusCode.Published);
                entity.PublishDate.Should().Be(specificPublishDate);
                entity.LastPublishDate.Should().Be(secondDate);
            }
        }
    }
}
