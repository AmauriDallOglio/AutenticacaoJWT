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

            builder.Property(u => u.Id)
                   .IsRequired()
                   .HasColumnName("Id");

            builder.Property(u => u.Nome)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(u => u.Email)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(u => u.Senha)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(u => u.Token)
                   .HasMaxLength(500);

            builder.Property(u => u.Codigo)
                   .HasMaxLength(50);

            builder.Property(u => u.Aplicativo)
                   .HasMaxLength(100);

            builder.Property(u => u.UltimoAcesso);

            builder.Property(u => u.DataCadastro)
                   .IsRequired();

            builder.Property(u => u.DataAlteracao);

            builder.Property(u => u.Refresh)
                   .HasMaxLength(512);

        }


        /*
         * 
         * 
            create database AutenticacaoJWT

            use AutenticacaoJWT

 
        CREATE TABLE Usuario(
            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
           
            Nome NVARCHAR(150) NOT NULL,
            Email NVARCHAR(250) NOT NULL,
            Senha NVARCHAR(255) NOT NULL,
            Token NVARCHAR(500) NULL,
            Codigo NVARCHAR(50) NULL,
            Aplicativo NVARCHAR(100) NULL,
            UltimoAcesso DATETIME NULL,
            DataCadastro DATETIME NOT NULL,
            DataAlteracao DATETIME NULL,
       
        );

 ALTER TABLE Usuario ADD Refresh VARCHAR(512) NULL;

  INSERT INTO Usuario (Id, Nome, Email, Senha, Token, Refresh, Aplicativo, UltimoAcesso, DataCadastro, DataAlteracao)
 VALUES 
     (NEWID(), 'Amauri1', 'usuario1@usuario.com.br', '123456', '', '', '', GETDATE(), GETDATE(), GETDATE()),
     (NEWID(), 'Amauri2', 'usuario2@usuario.com.br', '123456', '', '', '', GETDATE(), GETDATE(), GETDATE());


                     * 
         * 
         */
    }
}
