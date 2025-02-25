using AutenticacaoJWT.Dominio.Entidade;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutenticacaoJWT.Infra.Mapeamento
{
    public class UsuarioMapeamento : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuario");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("VARCHAR(36)").IsRequired();
            builder.Property(u => u.Nome).HasColumnType("VARCHAR(150)").IsRequired();
            builder.Property(u => u.Email).HasColumnType("VARCHAR(150)").IsRequired();
            builder.Property(u => u.Senha).HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(u => u.Token).HasColumnType("VARCHAR(512)").IsRequired(false);
            builder.Property(u => u.Refresh).HasColumnType("VARCHAR(512)").IsRequired(false);
            builder.Property(u => u.Aplicativo).HasColumnType("VARCHAR(100)").IsRequired(false);
            builder.Property(u => u.UltimoAcesso).HasColumnType("DATETIME").IsRequired();
        }


        /*
         * 
         * 
            create database AutenticacaoJWT

            use AutenticacaoJWT

            CREATE TABLE Usuario (
                Id VARCHAR(36) PRIMARY KEY,
                Nome VARCHAR(150) NOT NULL,
                Email VARCHAR(150) NOT NULL,
                Senha VARCHAR(255) NOT NULL,
                Token VARCHAR(512) NULL,
                Refresh VARCHAR(512) NULL,
                Aplicativo VARCHAR(100) NULL,
                UltimoAcesso DATETIME NOT NULL
            );

        INSERT INTO Usuario (Id, Nome, Email, Senha, Token, Refresh, Aplicativo, UltimoAcesso)
        VALUES 
            (NEWID(), 'Amauri1', 'amauri1@amauri.com', '123456', '', '', '', GETDATE()),
            (NEWID(), 'Amauri2', 'amauri2@amauri.com', '123456', '', '', '', GETDATE());


                     * 
         * 
         */
    }
}
