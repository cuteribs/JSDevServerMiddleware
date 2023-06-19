using Microsoft.AspNetCore.Mvc;

namespace AuthenticatedSPAExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserInfoController : ControllerBase
{
	[HttpGet]
	public IDictionary<string, string> Get()
	{
		var dictionary = new Dictionary<string, string>
		{
			{ "FirstName", "John" },
			{ "LastName", "Doe" },
			{ "Gender", "Faked" }
		};
		return dictionary;
	}
}