using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities;

public class User
{
    public int IdUser { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; } 
    public string Password { get; set; }
    public DateTime CreationDate { get; set; }
    public Roles role { get; set; }
    public int IdCompany { get; set; }
    public Company Company{  get; set; }
    
    public List<DetailLiquidation> detailLiquidations { get; set; } = new List<DetailLiquidation>();
    public List<Workday> workdays { get; set; } = new List<Workday>();

    public User() { }
}
