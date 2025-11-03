using CsClean.Application.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClean.Application.UseCases
{
    // Real-life: “DELETE FROM LocalMessages WHERE Name = @name”
    public sealed class DeleteLocalMessageByName
    {
        private readonly ILocalMessageRepository _repo;
        public DeleteLocalMessageByName(ILocalMessageRepository repo) => _repo = repo;

        public Task ExecuteAsync(string name, CancellationToken ct = default)
            => _repo.DeleteAsync(name, ct);
    }
}
