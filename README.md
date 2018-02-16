# Shuttle.Core.Data.Http

```
PM> Install-Package Shuttle.Core.Data.Http
```

Register, or use, the `ContextDatabaseContextCache` implementation of the `IDatabaseContextCache` interface for use in web/wcf scenarios:

```
registry.Register<IDatabaseContextCache, ContextDatabaseContextCache>();
```
