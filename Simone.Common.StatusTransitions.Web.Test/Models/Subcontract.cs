using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Simone.Common.StatusTransitions.Web.Test.Models
{
    /// <summary>
    /// The business entity whose lifecycle is driven by the StatusTransitions package.
    /// Its <see cref="Id"/> is the entity id used when creating transitions.
    /// The current status is NOT stored here: it is derived from the transition log
    /// managed by the package.
    /// </summary>
    public class Subcontract
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contratista")]
        public string Contractor { get; set; } = string.Empty;

        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        public decimal Amount { get; set; }

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Creado")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
