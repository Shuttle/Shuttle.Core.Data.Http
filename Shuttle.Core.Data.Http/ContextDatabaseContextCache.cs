using System;
using System.Web;
#if (!NETCOREAPP2_1 && !NETSTANDARD2_0)
using System.ServiceModel;
#endif
#if (NETCOREAPP2_1 || NETSTANDARD2_0)
using Shuttle.Core.Contract;
using Microsoft.AspNetCore.Http;
#endif

namespace Shuttle.Core.Data.Http
{
    public class ContextDatabaseContextCache : IDatabaseContextCache
    {
        [ThreadStatic] private static DatabaseContextCache _cache;

#if (NETCOREAPP2_1 || NETSTANDARD2_0)
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextDatabaseContextCache(IHttpContextAccessor httpContextAccessor)
        {
            Guard.AgainstNull(httpContextAccessor, nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
        }
#endif

        public IDatabaseContext Current => GuardedCache().Current;

        public ActiveDatabaseContext Use(string name)
        {
            return GuardedCache().Use(name);
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            return GuardedCache().Use(context);
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

        public IDatabaseContext Get(string connectionString)
        {
            return GuardedCache().Get(connectionString);
        }

        private DatabaseContextCache GuardedCache()
        {
            const string key = "__database-context-cache-item__";

#if (!NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0)
            var result = (DatabaseContextCache) (UseThreadStatic()
                ? _cache
                : (OperationContext.Current != null
                    ? ItemOperationContext.Current.Items[key]
                    : HttpContext.Current.Items[key]));
#else
            var result = (DatabaseContextCache) (UseThreadStatic()
                ? _cache
                : _httpContextAccessor.HttpContext.Items[key]);
#endif

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
#if (!NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0)
                if (OperationContext.Current != null)
                {
                    ItemOperationContext.Current.Items[key] = result;
                }
                else
                {
                    HttpContext.Current.Items[key] = result;
                }
#else
                _httpContextAccessor.HttpContext.Items[key] = result;
#endif
            }

            return result;
        }

        private bool UseThreadStatic()
        {
#if (!NETCOREAPP2_0 && !NETCOREAPP2_1 && !NETSTANDARD2_0)
            return OperationContext.Current == null && HttpContext.Current == null;
#else
            return _httpContextAccessor.HttpContext == null;
#endif
        }
    }
}