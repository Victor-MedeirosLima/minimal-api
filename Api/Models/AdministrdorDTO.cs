using MinimalApi.Models.Enum;

namespace MinimalApi.Models.Entidade;

public class AdministrdorDTO
{
    public string Email {get;set;} = default!;
    
    public string Senha {get;set;} = default!;
    
    public Perfil Perfil {get;set;} = default!;
}