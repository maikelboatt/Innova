namespace Innova.Infrastructure.Persistence.Transactions
{
    /// <summary>
    ///     Ambient, per-async-flow slot holding the single UnitOfWork
    ///     instance for the command pipeline currently being constructed.
    ///     Exists to solve exactly one problem: IUnitOfWork and
    ///     IDbConnectionProvider are resolved from two DIFFERENT places
    ///     within the same synchronous object graph (TransactionCommandHandler's
    ///     constructor, and a repository's constructor, deep inside
    ///     handlerFactory()) during CompositionRoot.BuildPipeline's factory.
    ///     A plain RegisterType(() => new UnitOfWork(...)) creates a NEW
    ///     instance on every resolve, so the two ended up with different
    ///     objects — the repository's connection was never opened, because
    ///     BeginAsync only ever ran on the OTHER instance.
    ///     AsyncLocal isolates the value per logical call/thread flow —
    ///     unlike a plain static field, concurrent command dispatches on
    ///     different threads/async flows never see each other's value.
    ///     BuildPipeline sets this immediately before constructing the
    ///     graph and clears it in a finally block right after — entirely
    ///     within one synchronous call, before any await — so it never
    ///     needs to survive across a command's actual async execution.
    /// </summary>
    internal static class AmbientUnitOfWork
    {
        private static readonly AsyncLocal<UnitOfWork?> _current = new();

        public static UnitOfWork? Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }
}
