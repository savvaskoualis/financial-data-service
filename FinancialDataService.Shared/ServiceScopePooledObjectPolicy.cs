using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;

namespace FinancialDataService.Shared;

public class ServiceScopePooledObjectPolicy : IPooledObjectPolicy<IServiceScope>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ServiceScopePooledObjectPolicy(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public IServiceScope Create()
    {
        return _scopeFactory.CreateScope();
    }

    public bool Return(IServiceScope obj)
    {
        obj.Dispose();
        return true;
    }
}