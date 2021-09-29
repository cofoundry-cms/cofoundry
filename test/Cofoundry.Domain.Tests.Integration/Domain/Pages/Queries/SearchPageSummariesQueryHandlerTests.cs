using Cofoundry.Core;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Pages.Queries
{
    [Collection(nameof(DbDependentFixture))]
    public class SearchPageSummariesQueryHandlerTests
    {
        const string UNIQUE_PREFIX = "SearchPageDetailsSumQHT ";

        private readonly DbDependentFixture _dbDependentFixture;
        private readonly TestDataHelper _testDataHelper;

        public SearchPageSummariesQueryHandlerTests(
            DbDependentFixture dbDependantFixture
            )
        {
            _dbDependentFixture = dbDependantFixture;
            _testDataHelper = new TestDataHelper(dbDependantFixture);
        }

        [Fact]
        public async Task DoesNotReturnDeletedPage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(DoesNotReturnDeletedPage);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c => c.Publish = true);

            await contentRepository
                .Pages()
                .DeleteAsync(pageId);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId
                })
                .ExecuteAsync();

            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task DoesNotReturnPageWithArchivedTemplate()
        {
            var uniqueData = UNIQUE_PREFIX + "NotRetPageWithArchivedTmpl";

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var pageTemplateId = await _testDataHelper.PageTemplates().AddMockTemplateAsync(uniqueData);
            var pageId = await _testDataHelper.Pages().AddAsync(uniqueData, directoryId, c =>
            {
                c.Publish = true;
                c.PageTemplateId = pageTemplateId;
            });
            await _testDataHelper.PageTemplates().ArchiveTemplateAsync(pageTemplateId);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task MapsBasicData()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(MapsBasicData);
            var sluggedUniqueData = SlugFormatter.ToSlug(uniqueData);
            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var addPageCommand = _testDataHelper.Pages().CreateAddCommand(uniqueData, directoryId);
            addPageCommand.Tags.Add(_dbDependentFixture.SeededEntities.TestTag.TagText);
            addPageCommand.OpenGraphTitle = uniqueData + "OG Title";
            addPageCommand.OpenGraphDescription = uniqueData + "OG desc";
            addPageCommand.OpenGraphImageId = _dbDependentFixture.SeededEntities.TestImageId;
            addPageCommand.MetaDescription = uniqueData + "Meta";
            addPageCommand.Publish = true;
            addPageCommand.ShowInSiteMap = true;

            var pageId = await contentRepository
                .Pages()
                .AddAsync(addPageCommand);

            var versionId = await _testDataHelper.Pages().AddDraftAsync(pageId);
            await _testDataHelper.Pages().PublishAsync(pageId);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                var page = result.Items.SingleOrDefault(p => p.PageId == pageId);

                page.Should().NotBeNull();
                page.PageId.Should().Be(addPageCommand.OutputPageId);

                page.AuditData.Should().NotBeNull();
                page.AuditData.Creator.Should().NotBeNull();
                page.AuditData.Creator.UserId.Should().BePositive();

                page.FullPath.Should().Be($"/{sluggedUniqueData}/{addPageCommand.UrlPath}");

                page.Should().NotBeNull();
                page.HasDraftVersion.Should().BeFalse();
                page.Locale.Should().BeNull();
                page.PageType.Should().Be(addPageCommand.PageType);
                page.PublishDate.Should().NotBeNull().And.NotBeDefault();
                page.PublishStatus.Should().Be(PublishStatus.Published);
                page.Tags.Should().ContainSingle(t => t == _dbDependentFixture.SeededEntities.TestTag.TagText);
                page.Title.Should().Be(addPageCommand.Title);
                page.UrlPath.Should().Be(addPageCommand.UrlPath);
            }
        }

        [Fact]
        public async Task CanSearch()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearch);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directory1Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "1");
            var directory2Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "2");
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directory1Id, c => c.Publish = true);
            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directory2Id, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery())
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().NotBeNull();
                result.Items.Should().HaveCountGreaterOrEqualTo(2);
            }
        }

        [Fact]
        public async Task CanSearchByDirectory()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByDirectory);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directory1Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "1");
            var directory2Id = await _testDataHelper.PageDirectories().AddAsync(uniqueData + "2");
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directory1Id, c => c.Publish = true);
            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directory2Id, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directory2Id
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(1);
                result.Items.Single().PageId.Should().Be(page2Id);
            }
        }

        [Fact]
        public async Task CanSearchByPageTemplate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByPageTemplate);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
            var page2Id = await _testDataHelper.Pages().AddCustomEntityPageDetailsAsync(uniqueData + "2", directoryId, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    PageTemplateId = _dbDependentFixture.SeededEntities.TestPageTemplate.PageTemplateId
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(1);
                result.Items.Single().PageId.Should().Be(page1Id);
            }
        }

        [Fact]
        public async Task CanSearchByTag()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTag);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var tag1 = UNIQUE_PREFIX.Trim() + "s1";
            var tag2 = UNIQUE_PREFIX.Trim() + "s2";
            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync(uniqueData + "1", directoryId, c =>
            { 
                c.Publish = true; 
                c.Tags = new string[] { tag1 }; 
            });
            var page2Id = await _testDataHelper.Pages().AddAsync(uniqueData + "2", directoryId, c =>
            {
                c.Publish = true;
                c.Tags = new string[] { tag1, tag2 };
            });
            var page3Id = await _testDataHelper.Pages().AddAsync(uniqueData + "3", directoryId, c =>
            {
                c.Publish = true;
                c.Tags = new string[] { _dbDependentFixture.SeededEntities.TestTag.TagText };
            });

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    Tags = $"{tag1}, {tag2}"
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(1);
                result.Items.Should().Contain(p => p.PageId == page2Id);
            }
        }

        [Fact]
        public async Task CanSearchByTextUsingTitle()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTextUsingTitle);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync("Red Pippin", directoryId, c =>
            {
                c.Publish = true;
                c.UrlPath = "1";
            });
            var page2Id = await _testDataHelper.Pages().AddAsync("Ribston Pippin", directoryId, c =>
            {
                c.Publish = true;
                c.UrlPath = "2";
            });
            var page3Id = await _testDataHelper.Pages().AddAsync("Red Prince", directoryId, c =>
            {
                c.Publish = true;
                c.UrlPath = "3";
            });

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    Text = "pip"
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(2);
                result.Items.Should().Contain(p => p.PageId == page1Id);
                result.Items.Should().Contain(p => p.PageId == page2Id);
            }
        }

        [Fact]
        public async Task CanSearchByTextUsingUrlSlug()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSearchByTextUsingUrlSlug);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            var page1Id = await _testDataHelper.Pages().AddAsync("Red Pippin", directoryId, c =>
            {
                c.Publish = true;
                c.Title = "1";
            });
            var page2Id = await _testDataHelper.Pages().AddAsync("Ribston Pippin", directoryId, c =>
            {
                c.Publish = true;
                c.Title = "2";
            });
            var page3Id = await _testDataHelper.Pages().AddAsync("Red Prince", directoryId, c =>
            {
                c.Publish = true;
                c.Title = "3";
            });

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    Text = "-pip"
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(2);
                result.Items.Should().Contain(p => p.PageId == page1Id);
                result.Items.Should().Contain(p => p.PageId == page2Id);
            }
        }

        [Fact]
        public async Task CanPage()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanPage);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData + "1", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "2", directoryId, c => c.Publish = true);
            var page3Id = await _testDataHelper.Pages().AddAsync(uniqueData + "3", directoryId, c => c.Publish = true);
            var page4Id = await _testDataHelper.Pages().AddAsync(uniqueData + "4", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "5", directoryId, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    PageSize = 2,
                    PageNumber = 2
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.PageCount.Should().Be(3);
                result.TotalItems.Should().Be(5);
                result.PageSize.Should().Be(2);
                result.PageCount.Should().Be(3);
                result.Items.Should().HaveCount(2);
                result.Items.Should().Contain(p => p.PageId == page3Id);
                result.Items.Should().Contain(p => p.PageId == page4Id);
            }
        }

        [Fact]
        public async Task CanSortByCreateDate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSortByCreateDate);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData + "T", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "H", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "E", directoryId, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    SortBy = PageQuerySortType.CreateDate,
                    SortDirection = SortDirection.Reversed
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(3);
                result.Items.Should().BeInAscendingOrder(p => p.AuditData.CreateDate);
            }
        }

        [Fact]
        public async Task CanSortByPublishDate()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSortByPublishDate);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData + "M", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2020, 4, 23); });
            await _testDataHelper.Pages().AddAsync(uniqueData + "E", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2021, 2, 18); });
            await _testDataHelper.Pages().AddAsync(uniqueData + "G", directoryId, c => { c.Publish = true; c.PublishDate = new DateTime(2019, 12, 5); });

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    SortBy = PageQuerySortType.PublishDate
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(3);
                result.Items.Should().BeInDescendingOrder(p => p.PublishDate);
            }
        }

        [Fact]
        public async Task CanSortByName()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CanSortByName);

            using var scope = _dbDependentFixture.CreateServiceScope();
            var contentRepository = scope.GetContentRepositoryWithElevatedPermissions();

            var directoryId = await _testDataHelper.PageDirectories().AddAsync(uniqueData);
            await _testDataHelper.Pages().AddAsync(uniqueData + "S", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "P", directoryId, c => c.Publish = true);
            await _testDataHelper.Pages().AddAsync(uniqueData + "Y", directoryId, c => c.Publish = true);

            var result = await contentRepository
                .Pages()
                .Search()
                .AsSummaries(new SearchPageSummariesQuery()
                {
                    PageDirectoryId = directoryId,
                    SortBy = PageQuerySortType.Title,
                    SortDirection = SortDirection.Reversed
                })
                .ExecuteAsync();

            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Items.Should().HaveCount(3);
                result.Items.Should().BeInDescendingOrder(p => p.Title);
            }
        }
    }
}
