using MinimalApi.Models.Entidade;
using MinimalApi.Models.Enum;


namespace Test;

[TestClass]
public class AdministadorTest
{
    [TestMethod]
    public void TestarGetESet()
    {   //arrange 
        var adm = new Administrador(); 
    
        //act

        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = Perfil.Editor;


        //assert


        Assert.AreEqual(1,adm.Id);
        Assert.AreEqual("teste@teste.com",adm.Email);
        Assert.AreEqual("teste",adm.Senha);
        Assert.AreEqual(Perfil.Editor,adm.Perfil);
    }
}