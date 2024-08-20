using DB;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Models.Entidade;

namespace Services;

public class AdmServices : IAdmServices
{   
    private readonly DBContext _context;

    public AdmServices(DBContext context)
    {
        _context = context;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        var busca = _context.Administradores.Where(b => b.Email == loginDTO.Email && b.Senha == loginDTO.Senha).FirstOrDefault();

        return busca;
    }


    public void Incluir(Administrador adm)
    {
        _context.Administradores.Add(adm);
        _context.SaveChanges();
    }


    public List<Administrador> Todos(int? pagina = 1)
    {
        var busca = _context.Administradores;

        int ItensPorPagina = 10;

        if(pagina != null){
            busca = (DbSet<Administrador>)busca.Skip(((int)pagina -1)*ItensPorPagina).Take(ItensPorPagina);
        }
        return busca.ToList();
    }

    public Administrador? BuscaPorId(long id){

        var busca = _context.Administradores.Find(id);
        return busca;
    }
}