using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;


namespace UPB.FinalProjectPVD.Controllers;

[ApiController]
[Route("providers")]
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
    public Provider Put([FromRoute] int id, [FromBody]Provider providerToUpdate)
    {
        return _providerManager.Update(id, providerToUpdate);
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
    
    [HttpPut]
    [Route("providers/{id}/enable")]
    public Provider Enable([FromRoute] int id)
    {
        return _providerManager.Enable(id);
    }

    [HttpPut]
    [Route("providers/{id}/disable")]
    public Provider Disable([FromRoute] int id)
    {
        return _providerManager.Disable(id);
    }
}
