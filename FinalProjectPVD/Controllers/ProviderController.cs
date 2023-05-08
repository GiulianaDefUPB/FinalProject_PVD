using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;

namespace UPB.FinalProjectPVD.Controllers;

[ApiController]
[Route("[controller]")]
public class ProviderController : ControllerBase
{
    public ProviderController()
    {
    }

    [HttpGet]
    [Route("{id}")]
    public Provider GetById([FromRoute] int id)
    {
        return new Provider();
    }

    [HttpPut]
    [Route("{id}")]
    public Provider Put([FromRoute] int id)
    {
        return new Provider();
    }

    [HttpPost]
    public Provider Post()
    {
        return new Provider();
    }

    [HttpDelete]
    [Route("{id}")]
    public Provider Delete([FromRoute] int id)
    {
        return new Provider();
    }
}
