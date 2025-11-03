using CsClean.Application.Abstraction;
using CsClean.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClean.Application.UseCases
{
    // Real-life: “INSERT/UPDATE row in table”
    public sealed class SaveLocalMessage
    {
        private readonly ILocalMessageRepository _repo;
        public SaveLocalMessage(ILocalMessageRepository repo) => _repo = repo;

        public Task ExecuteAsync(LocalMessages message, CancellationToken ct = default)
            => _repo.SaveAsync(message, ct);
    }
}
