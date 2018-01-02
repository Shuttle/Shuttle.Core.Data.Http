#if (!NETCOREAPP2_0 && !NETSTANDARD2_0)
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace Shuttle.Core.Data.Http
{
    public class ItemOperationContext : IExtension<OperationContext>
    {
        private ItemOperationContext()
        {
            Items = new Dictionary<string, object>();
        }

        public IDictionary Items { get; }

        public static ItemOperationContext Current
        {
            get
            {
                var context = OperationContext.Current.Extensions.Find<ItemOperationContext>();
                if (context == null)
                {
                    context = new ItemOperationContext();
                    OperationContext.Current.Extensions.Add(context);
                }

                return context;
            }
        }

        public void Attach(OperationContext owner)
        {
        }

        public void Detach(OperationContext owner)
        {
        }
    }
}
#endif