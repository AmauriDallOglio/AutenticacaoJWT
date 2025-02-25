using AutenticacaoJWT.Dominio.InterfaceRepositorio;
using AutenticacaoJWT.Infra.Contexto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AutenticacaoJWT.Infra.Repositorio
{
    public class GenericoRepositorio<T> : IGenericoRepositorio<T> where T : class
    {
        protected readonly GenericoContexto _context;
        private readonly DbSet<T> _dbSet;

        public GenericoRepositorio(GenericoContexto context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T?> IncluirAsync(T entidade, CancellationToken cancellationToken)
        {
            try
            {
                await _dbSet.AddAsync(entidade);
                await _context.SaveChangesAsync();
                return entidade;
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível incluir o registro, operação cancelada! {ex.Message} / {ex.InnerException.Message} ");
            }
        }

        public async Task<T?> EditarAsync(T entidade, CancellationToken cancellationToken)
        {
            try
            {
                _dbSet.Update(entidade);
                await _context.SaveChangesAsync();
                return entidade;
            }
            catch (Exception ex)
            {
                throw new Exception($"Não foi possível alterar o registro, operação cancelada! {ex.Message}");
            }
        }

        public async Task<bool> ExcluirAsync(T entidade, CancellationToken cancellationToken)
        {
            try
            {
                EntityEntry<T> deletar = _dbSet.Remove(entidade);
                if (deletar.Context == null)
                    throw new Exception("Não foi possível deletar o registro!");

                int gravar = await _context.SaveChangesAsync();
                if (gravar == 0)
                    throw new Exception("Não foi possível excluir o registro, operação cancelada!");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao excluir o registro: {ex.Message}");
            }
        }

        public async Task<T?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<List<T>> ObterTodosAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync();
        }
    }
}
