using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VMManager.Core.Entities;
using VMManager.Services.Interfaces; // ✅ Fixed namespace

namespace VMManager.Infrastructure.Persistence
{
    /// <summary>
    /// LiteDB-backed implementation of IHostRepository.
    /// </summary>
    public sealed class LiteDbHostRepository : IHostRepository
    {
        private const string DbFile = "VMManager.db";
        private const string CollectionName = "hosts";

        public Task<IEnumerable<Host>> GetAllAsync()
        {
            using var db = new LiteDatabase(DbFile);
            var col = db.GetCollection<Host>(CollectionName);
            var result = col.FindAll().ToList();
            return Task.FromResult<IEnumerable<Host>>(result);
        }

        public Task AddAsync(Host host)
        {
            using var db = new LiteDatabase(DbFile);
            var col = db.GetCollection<Host>(CollectionName);
            col.Insert(host);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Host host)
        {
            using var db = new LiteDatabase(DbFile);
            var col = db.GetCollection<Host>(CollectionName);
            var existing = col.FindOne(x => x.HostName == host.HostName);
            if (existing != null)
            {
                host.Id = existing.Id;
                col.Update(host);
            }
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string hostName)
        {
            using var db = new LiteDatabase(DbFile);
            var col = db.GetCollection<Host>(CollectionName);
            col.DeleteMany(h => h.HostName == hostName);
            return Task.CompletedTask;
        }
    }
}
