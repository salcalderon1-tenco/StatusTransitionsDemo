namespace Simone.Common.StatusTransitions.Web.Test.Models
{
    /// <summary>One row in the subcontracts list, with its current workflow status.</summary>
    public class SubcontractListItem
    {
        public Subcontract Subcontract { get; set; } = null!;
    }

    /// <summary>Everything the details view needs to render state + available actions + history.</summary>
    public class SubcontractDetailsViewModel
    {
        public Subcontract Subcontract { get; set; } = null!;
    }
}
