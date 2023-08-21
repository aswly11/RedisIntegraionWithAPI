using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RedisIntegraionWithAPI.Data;
using RedisIntegraionWithAPI.Models;
using RedisIntegraionWithAPI.Services;

namespace RedisIntegraionWithAPI.Controllers
{
    [Route("[controller]")]
    public class DriversController : Controller
    {
        private readonly ILogger<DriversController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDBContext _context;



        public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDBContext appDbContext)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = appDbContext;
        }

        [HttpGet("Drivers")]
        public async Task<IActionResult> Get()
        {

            var cacheData = await _cacheService.GetData<IEnumerable<Driver>>("drivers");

            if (cacheData != null && cacheData.Count() > 0)
            {
                return Ok(cacheData);
            }
            cacheData = await _context.Drivers.ToListAsync();
            var expiryTime = DateTime.Now.AddSeconds(30);
            await _cacheService.SetData("drivers", cacheData, expiryTime);
            return Ok(cacheData);

        }
        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post([FromBody] Driver driver)
        {
            if (ModelState.IsValid)
            {
                var addedDriver = await _context.Drivers.AddAsync(driver);
                var expiryTime = DateTime.Now.AddSeconds(30);
                await _cacheService.SetData($"driver{driver.Id}", driver, expiryTime);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int Id)
        {
            var driver = await _context.Drivers.FindAsync(Id);
            if (driver == null)
            {
                return NotFound();
            }
            _context.Drivers.Remove(driver);
            await _cacheService.RemoveData($"driver{driver.Id}");
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
}