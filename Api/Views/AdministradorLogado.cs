using MinimalApi.Models.Enum;

namespace Views;

public class AdministradorLogado
{
    
    
    public string Email {get;set;} = default!;
    
    public Perfil Perfil {get;set;} = default!;

    public string Token {get;set;} = default!;
}