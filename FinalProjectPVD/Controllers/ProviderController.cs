using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;

namespace UPB.FinalProjectPVD.Controllers;

[ApiController]
[Route("providers")]
public class ProviderController : ControllerBase
{
    private readonly ProviderManager _providerManager;
    private readonly HttpClient _httpClient;

    public ProviderController(ProviderManager providerManager, HttpClient httpClient)
    {
        _providerManager = providerManager;
        _httpClient = httpClient;
    }

    [HttpGet]
    [Route("{id}")]
    public Provider GetById([FromRoute] int id)
    {
        return _providerManager.GetById(id);
    }

    [HttpGet]
    public List<Provider> Get()
    {
        if (Request.Headers.ContainsKey("ListType"))
        {
            string headerValue = Request.Headers["ListType"];
    
            if (string.IsNullOrEmpty(headerValue))
                return _providerManager.Get(null);
            else
                return _providerManager.Get(Request.Headers["ListType"]);
        }
        else
            return _providerManager.Get(null);
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
