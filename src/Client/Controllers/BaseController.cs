using Microsoft.AspNetCore.Mvc;

namespace MyReliableSite.Client.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/client/[controller]")]
public class BaseController : ControllerBase
{
}