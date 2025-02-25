namespace AutenticacaoJWT.Dominio.InterfaceRepositorio
{
    public interface IGenericoRepositorio<T> where T : class
    {
        Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken);
        Task<T> ObterPorIdAsync(Guid id, CancellationToken cancellationToken);
        Task<T> IncluirAsync(T entidade, CancellationToken cancellationToken);
        Task<T> EditarAsync(T entidade, CancellationToken cancellationToken);
        Task<bool> ExcluirAsync(T entidade, CancellationToken cancellationToken);
    }
}

