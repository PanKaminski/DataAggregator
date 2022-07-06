using DataAggregator.Bll.Contract.Interfaces;
using DataAggregator.Bll.Contract.Models;
using DataAggregator.WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DataAggregator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AggregatorsController : ControllerBase
    {

        public AggregatorsController()
        {
        }

        [HttpGet("/")]
        public async Task<ActionResult> Index()
        {
            return Ok();
        }
    }
}
