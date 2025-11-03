using CsClean.Application.Abstraction;
using CsClean.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsClean.Application.UseCases
{
    // NOTE TO SELF: A use case = thin orchestrator that the UI can call.
    // It hides "how" data is obtained (infra) behind an interface (port).
    public sealed class GetAllLocalMessages
    {
        private readonly ILocalMessageRepository _repository;
        // NOTE: DIP — depend on abstraction, not concrete infra class.

        public GetAllLocalMessages(ILocalMessageRepository repository)
        {
            // Fail fast if DI is misconfigured.
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // NOTE: Keep naming consistent -> ExecuteAsync() for all use cases.
        public Task<IReadOnlyList<LocalMessages>> ExecuteAsync(CancellationToken ct = default)
            => _repository.GetAllAsync(ct);
    }
}
