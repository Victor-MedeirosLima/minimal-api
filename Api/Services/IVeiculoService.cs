using MinimalApi.Models.Entidade;

namespace Services;

public interface IVeiculoService
{
    List<Veiculo> Todos(int? pagina = 1 , string? nome = null, string? marca =null);
    Veiculo? BuscaPorId(long id );
    void Incluir(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Apagar(Veiculo veiculo);
}