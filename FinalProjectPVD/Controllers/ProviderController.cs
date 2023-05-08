using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;

namespace FinalProjectPVD.Controllers;

[ApiController]
[Route("providers")]
public class ProviderController : ControllerBase
{
    private readonly ProviderManager _providerManager;

    public ProviderController(ProviderManager providerManager)
    {
        _providerManager = providerManager;
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
