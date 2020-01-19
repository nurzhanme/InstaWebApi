using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaWebApi.Models;

namespace InstaWebApi.Repository
{
    public interface IInstaAccountRepository<T> : IRepository<T> where T : class, IEntity
    {
        Task<T> Get(string username);

    }
}
