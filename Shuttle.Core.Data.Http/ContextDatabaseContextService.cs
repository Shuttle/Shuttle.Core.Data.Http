using System;
using Shuttle.Core.Contract;
using Microsoft.AspNetCore.Http;

namespace Shuttle.Core.Data.Http
{
    public class ContextDatabaseContextService : IDatabaseContextService
    {
        [ThreadStatic] private static DatabaseContextService _databaseContextService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextDatabaseContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = Guard.AgainstNull(httpContextAccessor, nameof(httpContextAccessor));
        }

        public IDatabaseContext Current => Guarded().Current;

        public ActiveDatabaseContext Use(string name)
        {
            return Guarded().Use(name);
        }

        public ActiveDatabaseContext Use(IDatabaseContext context)
        {
            return Guarded().Use(context);
        }

        public IDatabaseContext Find(Predicate<IDatabaseContext> match)
        {
            return Guarded().Find(match);
        }

        public void Add(IDatabaseContext context)
        {
            Guarded().Add(context);
            Guarded().Use(context);
        }

        public void Remove(IDatabaseContext context)
        {
            Guarded().Remove(context);
        }

        public bool HasCurrent => Guarded().HasCurrent;

        public IDatabaseContext Get(string connectionString)
        {
            return Guarded().Get(connectionString);
        }

        private DatabaseContextService Guarded()
        {
            const string key = "__database-context-service-item__";

            var result = (DatabaseContextService) (UseThreadStatic()
                ? _databaseContextService
                : _httpContextAccessor.HttpContext.Items[key]);

            if (result != null)
            {
                return result;
            }

            result = new DatabaseContextService();

            if (UseThreadStatic())
            {
                _databaseContextService = result;
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