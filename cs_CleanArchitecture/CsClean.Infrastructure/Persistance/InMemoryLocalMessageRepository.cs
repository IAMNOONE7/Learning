using CsClean.Application.Abstraction;
using CsClean.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClean.Infrastructure.Persistance
{
    // NOTE TO SELF: This is my first *adapter* that talks to the real data source.
    // In real life, this could be a SQL database, an Excel file, or an API.
    // Right now it’s just an in-memory list to prove wiring works.
    public sealed class InMemoryLocalMessageRepository : ILocalMessageRepository
    {
        // Pretend this is my database table with one “System” record.
        private readonly List<LocalMessages> _store =
        [
            new LocalMessages("System", new[]
        {
            new LocalMessageItem(0, "Hello Clean Architecture!"),
            new LocalMessageItem(1, "This message comes from InMemory repo.")
        })
        ];

        // === “SELECT * FROM LocalMessages” equivalent ===
        public Task<IReadOnlyList<LocalMessages>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult((IReadOnlyList<LocalMessages>)_store.ToList());

        // === “INSERT or UPDATE LocalMessage” equivalent ===
        public Task SaveAsync(LocalMessages message, CancellationToken ct = default)
        {
            var existing = _store.FirstOrDefault(m =>
                m.Name.Equals(message.Name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                _store.Remove(existing);

            _store.Add(message);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string name, CancellationToken ct = default)
        {
            var existing = _store.FirstOrDefault(m =>
                m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existing != null) _store.Remove(existing);
            return Task.CompletedTask;
        }
    }
}
