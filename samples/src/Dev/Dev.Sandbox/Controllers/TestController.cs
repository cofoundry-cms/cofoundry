using Microsoft.AspNetCore.Mvc;

namespace Dev.Sandbox;

public class TestController : Controller
{
    private readonly IAdvancedContentRepository _contentRepository;

    public TestController(IAdvancedContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
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

        return Json(entity);
    }
}
