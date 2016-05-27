using System;
using System.ServiceModel;
using System.Web;

namespace Shuttle.Core.Data.Http
{
	public class ContextDatabaseContextCache : IDatabaseContextCache
	{
		[ThreadStatic] private static DatabaseContextCache _cache;

		public IDatabaseContext Current
		{
			get { return GuardedCache().Current; }
		}

		public void Use(string name)
		{
			GuardedCache().Use(name);
		}

		public void Use(IDatabaseContext context)
		{
			GuardedCache().Use(context);
		}

		public bool Contains(string connectionString)
		{
			return GuardedCache().Contains(connectionString);
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

		private static DatabaseContextCache GuardedCache()
		{
			const string key = "__database-context-cache-item__";

			var result = (DatabaseContextCache) (UseThreadStatic()
				? _cache
				: (OperationContext.Current != null
					? ItemOperationContext.Current.Items[key]
					: HttpContext.Current.Items[key]));

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
				if (OperationContext.Current != null)
				{
					ItemOperationContext.Current.Items[key] = result;
				}
				else
				{
					HttpContext.Current.Items[key] = result;
				}
			}

			return result;
		}

		private static bool UseThreadStatic()
		{
			return OperationContext.Current == null && HttpContext.Current == null;
		}
	}
}