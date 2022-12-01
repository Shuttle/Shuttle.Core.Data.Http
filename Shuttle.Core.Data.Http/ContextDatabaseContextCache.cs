using System;
using Shuttle.Core.Contract;
using Microsoft.AspNetCore.Http;

namespace Shuttle.Core.Data.Http
{
    public class ContextDatabaseContextCache : IDatabaseContextCache
    {
        [ThreadStatic] private static DatabaseContextCache _cache;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextDatabaseContextCache(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = Guard.AgainstNull(httpContextAccessor, nameof(httpContextAccessor));
        }

        public IDatabaseContext Current => GuardedCache().Current;

        public ActiveDatabaseContext Use(string name)
        {
            return GuardedCache().Use(name);
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            return GuardedCache().Use(context);
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            return GuardedCache().Find(match);
        }

        public bool Contains(string connectionString)
        {
            return GuardedCache().Contains(connectionString);
        }

        public bool ContainsConnectionString(string connectionString)
        {
            return GuardedCache().ContainsConnectionString(connectionString);
        }

        public IDatabaseContext GetConnectionString(string connectionString)
        {
            return GuardedCache().GetConnectionString(connectionString);
        }

        public void Add(IDatabaseContext context)
        {
            GuardedCache().Add(context);
            GuardedCache().Use(context);
        }

        public void Remove(IDatabaseContext context)
        {
            GuardedCache().Remove(context);
        }

        public bool HasCurrent => GuardedCache().HasCurrent;

        public IDatabaseContext Get(string connectionString)
        {
            return GuardedCache().Get(connectionString);
        }

        private DatabaseContextCache GuardedCache()
        {
            const string key = "__database-context-cache-item__";

            var result = (DatabaseContextCache) (UseThreadStatic()
                ? _cache
                : _httpContextAccessor.HttpContext.Items[key]);

            if (result != null)
            {
                return result;
            }

            result = new DatabaseContextCache();

            if (UseThreadStatic())
            {
                _cache = result;
            }
            else
            {
                _httpContextAccessor.HttpContext.Items[key] = result;
            }

            return result;
        }

        private bool UseThreadStatic()
        {
            return _httpContextAccessor.HttpContext == null;
        }
    }
}