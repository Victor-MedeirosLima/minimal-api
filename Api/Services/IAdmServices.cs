using MinimalApi.Models.Entidade;

namespace Services;

public interface IAdmServices
{
    Administrador? Login(LoginDTO loginDTO);

    void Incluir(Administrador adm);

    List<Administrador> Todos(int? pagina = 1);

    Administrador? BuscaPorId(long id);
}