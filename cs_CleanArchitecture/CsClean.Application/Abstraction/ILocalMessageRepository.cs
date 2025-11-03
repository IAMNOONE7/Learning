using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsClean.Domain.Entities;

namespace CsClean.Application.Abstraction
{
    // NOTE TO SELF: This interface lives in Application so higher layers depend on it.
    // Infra will implement it later. I keep methods async-friendly from the start.
    public interface ILocalMessageRepository
    {
        // Read all messages — later I can expand with paging/filters but not now.
        Task<IReadOnlyList<LocalMessages>> GetAllAsync(CancellationToken ct = default);


        // Save a single message — simple for the demo; real app may need bulk ops.
        Task SaveAsync(LocalMessages message, CancellationToken ct = default);

        Task DeleteAsync(string name, CancellationToken ct = default);       // remove by key
    }
}
