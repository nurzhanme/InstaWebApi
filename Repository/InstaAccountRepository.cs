using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaWebApi.Data;
using InstaWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InstaWebApi.Repository
{
    public class InstaAccountRepository : EfCoreRepository<InstaAccount, ApplicationDbContext>,
        IInstaAccountRepository<InstaAccount>
    {
        public InstaAccountRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<InstaAccount> Get(string username)
        {
            return await context.Set<InstaAccount>().FirstOrDefaultAsync(acc => acc.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
