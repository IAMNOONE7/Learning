using CsClean.Application.Abstraction;
using CsClean.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CsClean.Infrastructure
{
    // NOTE TO SELF: Think of this as “Infrastructure startup”.
    // In real life, this would register DB contexts, API clients, etc.
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register the InMemory repo as the implementation of the ILocalMessageRepository interface.
            // Real life equivalent: telling DI “whenever someone asks for ILocalMessageRepository,
            // give them a SQL/Excel/File/Cloud repo”.
            services.AddSingleton<ILocalMessageRepository, InMemoryLocalMessageRepository>();

            return services;
        }

    }
}
