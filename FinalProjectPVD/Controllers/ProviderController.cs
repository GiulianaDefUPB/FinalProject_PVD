using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;

namespace UPB.FinalProjectPVD.Controllers;

[ApiController]
[Route("[controller]")]
public class ProviderController : ControllerBase
{
    private readonly ProviderManager _providerManager;

    public ProviderController(ProviderManager providerManager)
    {
        _providerManager = providerManager;
    }

    [HttpGet]
    [Route("{id}")]
    public Provider GetById([FromRoute] int id)
    {
        return _providerManager.GetById(id);
    }

    [HttpPut]
    [Route("{id}")]
    public Provider Put([FromRoute] int id)
    {
        return new Provider();
    }

    [HttpPost]
    public Provider Post([FromBody]Provider providerToCreate)
    {
        return _providerManager.Create(providerToCreate);
    }

    [HttpDelete]
    [Route("{id}")]
    public Provider Delete([FromRoute] int id)
    {
        return _providerManager.Delete(id);
    }
}
