using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Models.Entidade;
using MinimalApi.Models.Enum;

namespace DB;

public class DBContext : DbContext
{   
    private readonly IConfiguration _Configuration;

    public DBContext(IConfiguration Configuration)
    {
        _Configuration = Configuration;
    }

    public DbSet<Administrador> Administradores {get;set;} = default!;
    public DbSet<Veiculo> Veiculos {get;set;} = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador{
                Id = 1,
                Email = "emailteste@.com",
                Senha = "12345",
                Perfil = 0
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_Configuration.GetConnectionString("banco"));
       
    }

}