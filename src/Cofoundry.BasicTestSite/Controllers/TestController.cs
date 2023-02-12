﻿using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.BasicTestSite;

public class TestController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;
    private readonly IDomainRepository _domainRepository;

    public TestController(
        IAdvancedContentRepository contentRepository,
        IDomainRepository domainRepository
        )
    {
        _contentRepository = contentRepository;
        _domainRepository = domainRepository;
    }

    [Route("/test-admin/api/pets")]
    public IActionResult Pets()
    {
        var result = new object[]
        {
            new { Id = 1, Title = "Dog" },
            new { Id = 2, Title = "Cat" },
            new { Id = 3, Title = "Llama" }
        };

        return Json(result);
    }

    [Route("test/test")]
    public async Task<IActionResult> Test()
    {
        var entity = await _contentRepository
            .CustomEntities()
            .GetByUrlSlug<ExampleCustomEntityDefinition>("test")
            .AsRenderSummaries()
            .ExecuteAsync();

        //var user = await _contentRepository
        //    .Users()
        //    .Current()
        //    .Get()
        //    .AsDetails()
        //    .ExecuteAsync();

        //var preferences = user.Extensions.Get<UserDetails.UserPreferencesModel>();

        return Json(entity);
    }
}
