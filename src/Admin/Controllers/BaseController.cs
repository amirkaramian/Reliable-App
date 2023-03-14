using Microsoft.AspNetCore.Mvc;

namespace MyReliableSite.Admin.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/admin/[controller]")]
public class BaseController : ControllerBase
{
}
