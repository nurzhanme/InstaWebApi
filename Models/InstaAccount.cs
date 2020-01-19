using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaWebApi.Models
{
    public class InstaAccount : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Fisrtname { get; set; }
        public string SessionData { get; set; }
    }
}
