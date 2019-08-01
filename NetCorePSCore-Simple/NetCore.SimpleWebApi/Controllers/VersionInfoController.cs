using System;
using Microsoft.AspNetCore.Mvc;
using NetCore.RemotePS;

namespace NetCore.SimpleWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionInfoController : ControllerBase
    {
	    [HttpGet]
	    public string Get()
	    {
		    try
		    {
			    var (_, info) = PowerShellVersionTableScenario.OutputVersionTable();
			    return info;
		    }
		    catch (Exception e)
		    {
			    return e.ToString();
		    }
		}
    }
}