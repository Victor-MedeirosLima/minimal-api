using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MinimalApi.Models.Enum;

namespace MinimalApi.Models.Entidade;

public class Administrador
{   
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id {get;set;}

    [Required]
    [StringLength(255)]
    public string Email {get;set;} = default!;
    [Required]
    [StringLength(50)]
    public string Senha {get;set;} = default!;
    [Required]
    [StringLength(10)]
    public Perfil Perfil {get;set;} = default!;
}