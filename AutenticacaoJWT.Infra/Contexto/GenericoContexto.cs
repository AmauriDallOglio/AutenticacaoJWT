using AutenticacaoJWT.Dominio.Entidade;
using AutenticacaoJWT.Infra.Mapeamento;
using Microsoft.EntityFrameworkCore;

namespace AutenticacaoJWT.Infra.Contexto
{
    public class GenericoContexto : DbContext
    {
        public DbSet<Usuario> UsuarioDb { get; set; }
     

        public GenericoContexto(DbContextOptions<GenericoContexto> options) : base(options)
        {


        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UsuarioMapeamento());
        }
    }
}

