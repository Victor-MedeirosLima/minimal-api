using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api;
using MinimalApi.Models.Entidade;
using MinimalApi.Models.Enum;
using Services;
using Views;

public class Startup
{   
    public IConfiguration Configuration { get; set; } = default!;
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Key = Configuration.GetSection("Jwt").ToString() ?? "";
    }

    private string Key;

    public void ConfigureServices(IServiceCollection services){

        

        services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;   
            
            
            
        }).AddJwtBearer(opt =>{
            opt.TokenValidationParameters = new TokenValidationParameters{

                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
                ValidateIssuer =false,
                ValidateAudience =false
                

            };

        });
            
        services.AddAuthorization();

        services.AddScoped<IAdmServices, AdmServices>();
        services.AddScoped<IVeiculoService, VeiculoService>();


        

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt => { 
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{

                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o tokenJWT aqui "
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {

                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference{

                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"

                        }
                    },
                new string [] {}
            }});
            
        });

        services.AddDbContext<DBContext>(options => {
            options.UseSqlServer(Configuration.GetConnectionString("banco"));
            
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });


    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env){

        

        


        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        
        app.UseCors();
        
        app.UseEndpoints(endpoint => {

            #region Home

            endpoint.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

            #endregion





            #region Adm

            string GerarJwtToken(Administrador administrador){

                if(string.IsNullOrEmpty(Key)) return string.Empty;

                var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                var Credenciais = new SigningCredentials(SecurityKey,SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>{

                    new Claim(ClaimTypes.Email, administrador.Email),
                    new Claim(ClaimTypes.Role , administrador.Perfil.ToString()),
                };

                var Token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: Credenciais
                );

                return new JwtSecurityTokenHandler().WriteToken(Token);
            };

                ErrosValidacao ValidaAdmDTO(AdministrdorDTO administrdorDTO){

                var validacao = new ErrosValidacao{
                    Mensagens = []
                };

                    if(string.IsNullOrEmpty(administrdorDTO.Email)){

                        validacao.Mensagens.Add("precisa ter um email");
                    }
                    if(string.IsNullOrEmpty(administrdorDTO.Senha)){

                        validacao.Mensagens.Add("precisa ter uma senha");
                    }
                    if(string.IsNullOrEmpty(administrdorDTO.Perfil.ToString())){

                        validacao.Mensagens.Add("Administraador precisa ter um perfil");
                    }

                    return validacao;

            };

            endpoint.MapPost("/administrador/login", ([FromBody]LoginDTO loginDTO, IAdmServices admServices ) => {
                
                var adm = admServices.Login(loginDTO);

                if(adm != null){

                    string token = GerarJwtToken(adm);

                    return Results.Ok(new AdministradorLogado{

                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token

                    });
                }

                    return Results.Unauthorized();

            }).AllowAnonymous().WithTags("Adm");

            endpoint.MapPost("/administrador", ([FromBody]AdministrdorDTO administrdorDTO, IAdmServices admServices ) => {
                
                var validacao = ValidaAdmDTO(administrdorDTO);
                    if(validacao.Mensagens.Count()>0){
                        return Results.BadRequest(validacao);
                    }
                
                var Adm = new Administrador{
                        
                        Email = administrdorDTO.Email,
                        Senha = administrdorDTO.Senha,
                        Perfil = administrdorDTO.Perfil,

                    };
                    
                    admServices.Incluir(Adm);

                    return Results.Created($"/administrador/{Adm.Id}",Adm);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm}"})
            .WithTags("Adm");

            endpoint.MapGet("/administrador", (int? pagina, IAdmServices admServices) => {

                    var adms = new List<AdimistradorModelView>();
                    var busca = admServices.Todos(pagina);

                    foreach(var adm in busca){

                        adms.Add(new AdimistradorModelView{

                            Id = adm.Id,
                            Email = adm.Email,
                            Perfil = adm.Perfil.ToString()

                        });
                    }

                    return Results.Ok(busca);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm}"})
            .WithTags("Adm");

            endpoint.MapGet("/administrador/{id}", ([FromRoute] long id, IAdmServices admServices) => {

                    var busca = admServices.BuscaPorId(id);

                if(busca==null) return Results.NotFound();

                return Results.Ok(busca);

            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm}"})
            .WithTags("Adm");

            #endregion






            #region Veiculos

            ErrosValidacao ValidaVeiculoDTO(VeiculoDTO veiculoDTO){

                var validacao = new ErrosValidacao{
                    Mensagens = new List<string>()
                };

                    if(string.IsNullOrEmpty(veiculoDTO.Nome)){

                        validacao.Mensagens.Add("O nome nao pode ser vazio!");
                    }
                    if(string.IsNullOrEmpty(veiculoDTO.Marca)){

                        validacao.Mensagens.Add("A marca nao pode ficar em branco !");
                    }
                    if(veiculoDTO.Ano<1950){

                        validacao.Mensagens.Add("Veiculo anteriores a 1950 nao podem ser cadastrados!");
                    }

                    return validacao;

            };

            endpoint.MapPost("/veiculos", ([FromBody]VeiculoDTO veiculoDTO, IVeiculoService veiculoService) => 
                {
                    
                    var validacao = ValidaVeiculoDTO(veiculoDTO);
                    if(validacao.Mensagens.Count()>0){
                        return Results.BadRequest(validacao);
                    }
                    
                    var veiculo = new Veiculo{
                        
                        Nome = veiculoDTO.Nome,
                        Marca = veiculoDTO.Marca,
                        Ano = veiculoDTO.Ano,

                    };
                    
                    veiculoService.Incluir(veiculo);

                    return Results.Created($"/veiculo/{veiculo.Id}",veiculo);
                    
                    
                }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm},{Perfil.Editor}"})
            .WithTags("Veiculo");

            endpoint.MapGet("/veiculos", (int? pagina,IVeiculoService veiculoService) => {

                var busca = veiculoService.Todos(pagina);

                return Results.Ok(busca);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm},{Perfil.Editor}"})
            .WithTags("Veiculo");

            endpoint.MapGet("/veiculos/{id}", ([FromRoute]long id,IVeiculoService veiculoService) => {

                var busca = veiculoService.BuscaPorId(id);

                if(busca==null) return Results.NotFound();

                return Results.Ok(busca);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm},{Perfil.Editor}"})
            .WithTags("Veiculo");



            endpoint.MapPut("/veiculos/{id}", ([FromRoute]long id,VeiculoDTO veiculoDTO,IVeiculoService veiculoService) => {

                
                var validacao = ValidaVeiculoDTO(veiculoDTO);
                    if(validacao.Mensagens.Count()>0){
                        return Results.BadRequest(validacao);
                    }
                
                
                var busca = veiculoService.BuscaPorId(id);

                if(busca==null) return Results.NotFound();

                busca.Nome = veiculoDTO.Nome;
                busca.Marca = veiculoDTO.Marca;
                busca.Ano = veiculoDTO.Ano;

                veiculoService.Atualizar(busca);

                return Results.Ok(busca);
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm}"})
            .WithTags("Veiculo");

            endpoint.MapDelete("/veiculos/{id}", ([FromRoute]long id, IVeiculoService veiculoService) => {

                var busca = veiculoService.BuscaPorId(id);

                if(busca==null) return Results.NotFound();

                

                veiculoService.Apagar(busca);

                return Results.NoContent();
            })
            .RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = $"{Perfil.Adm}"})
            .WithTags("Veiculo");

            #endregion  



        });


    }
}