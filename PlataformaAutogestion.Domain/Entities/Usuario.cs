using static PlataformaAutogestion.Domain.Enums.QuestionState;

namespace PlataformaAutogestion.Domain.Entities;

public class Usuario
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreationDate { get; set; }
    public Roles role { get; set; }
    public int IdEmpresa { get; set; }
    public Empresa Empresa {  get; set; }

    public List<DetalleLiquidacion> detalleLiquidacions { get; set; } = new List<DetalleLiquidacion>();
    public List<JornadaLaboral> jornadaLaborals { get; set; } = new List<JornadaLaboral>();

    public Usuario() { }
}
