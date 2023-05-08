using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;

namespace FinalProjectPVD.Controllers;

[ApiController]
[Route("providers")]
public class ProviderController : ControllerBase
{
    [HttpPut]
    [Route("providers/{id}/enable")]
    public Provider Enable([FromRoute] int id)
    {
        return new Provider();
    }

    [HttpPut]
    [Route("providers/{id}/disable")]
    public Provider Disable([FromRoute] int id)
    {
        return new Provider();
    }
}
