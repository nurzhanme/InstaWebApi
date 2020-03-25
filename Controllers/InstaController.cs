using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaWebApi.Data;
using InstaWebApi.Models;
using InstaWebApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InstaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly InstaService _service;

        public InstaController(ApplicationDbContext context, InstaService service)
        {
            _context = context;
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Register(InstaAccount account)
        {
            var res = await _service.CheckUsername(account.Username);

            if (!res)
            {
                throw new ArgumentException(
                    $"Usrname in use {account.Username}.", nameof(account.Username));
            }

            res = await _service.CheckEmail(account.Email);
            
            if (!res)
            {
                throw new ArgumentException(
                    $"Usrname in use {account.Username}.", nameof(account.Username));
            }
            res = await _service.Register(account);

            if (!res)
            {
                throw new ArgumentException(
                    $"Unable to register {account.Username}.", nameof(account.Username));
            }

            _context.InstaAccounts.Add(account);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(account), new { id = account.Id }, account);
        }
    }
}