using System.ComponentModel.DataAnnotations;

namespace PDF_EMAIL.Models
{
    public class NovoColaborador
    {
        [Required(ErrorMessage="O nome do novo colaborador é obrigatório")]
        [Display(Name="Nome do colaborador")]
        public string NomeColaborador { get; set; }

        [Required(ErrorMessage="O cargo do funcionário é obrigatório")]
        [Display(Name="Cargo")]
        public string Cargo { get; set; }

        [Required(ErrorMessage="A área do funcionário é obrigatória")]
        [Display(Name="Área")]
        public string Area { get; set; }

        [Display(Name="AX")]
        public bool AcessoAX { get; set; }
    }
}