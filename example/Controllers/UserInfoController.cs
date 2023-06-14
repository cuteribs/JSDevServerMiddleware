using Microsoft.AspNetCore.Mvc;

namespace AuthenticatedSPAExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
	[HttpGet]
	public IDictionary<string, string> Get()
	{
		return this.User.Claims.ToDictionary(c => c.Type, c => c.Value);
	}
}