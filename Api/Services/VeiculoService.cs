using DB;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Models.Entidade;

namespace Services;

public class VeiculoService : IVeiculoService
{   

    private readonly DBContext _context;

    public VeiculoService (DBContext context)
    {
        _context = context;
    }
    public void Apagar(Veiculo veiculo)
    {
        _context.Veiculos.Remove(veiculo);
        _context.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _context.Veiculos.Update(veiculo);
        _context.SaveChanges();
    }

    public Veiculo? BuscaPorId(long id)
    {
        
        var busca = _context.Veiculos.Find(id);
        return busca;

    }

    public void Incluir(Veiculo veiculo)
    {
        _context.Veiculos.Add(veiculo);
        _context.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var busca = _context.Veiculos;

        if(!string.IsNullOrEmpty(nome)){

            busca = (DbSet<Veiculo>)busca.Where(a => a.Nome.ToLower().Contains(nome.ToLower()));
        }

        int ItensPorPagina = 10;

        if(pagina != null){
            busca = (DbSet<Veiculo>)busca.Skip(((int)pagina -1)*ItensPorPagina).Take(ItensPorPagina);
        }
        return busca.ToList();
    }
}
