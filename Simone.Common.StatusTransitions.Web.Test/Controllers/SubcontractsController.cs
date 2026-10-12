using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simone.Common.Abstractions.Services;
using Simone.Common.Abstractions;
using Simone.Common.StatusTransitions.Web.Test.Data;
using Simone.Common.StatusTransitions.Web.Test.Models;

namespace Simone.Common.StatusTransitions.Web.Test.Controllers
{
    /// <summary>
    /// Demo controller that drives the subcontract workflow end to end, showing how to consume the
    /// Simone.Common.StatusTransitions
    /// </summary>
    /// <param name="context">The application's database context.</param>
    /// <param name="processProvider">
    /// The package's process provider, used to create the transition service for the subcontract process.
    /// </param>
    /// <param name="userIdentity">Resolves the current authenticated user (author of the transitions).</param>
    /// <param name="logger">Logger for this controller.</param>
    [Authorize(AuthenticationSchemes = IdentityConsts.DefaultAuthenticationSchemes)]
    public class SubcontractsController(
        AppDbContext context,
        IUserIdentity userIdentity,
        ILogger<SubcontractsController> logger) : Controller
    {
        /// <summary>
        /// The current authenticated user's GUID, used as the author of the transitions.
        /// </summary>
        private Guid CurrentUserId => userIdentity.GetCurrentUserGuid();

        /// <summary>
        /// GET: /Subcontracts. Lists every subcontract, newest first. The current status badge is
        /// rendered per row by the transition-badge tag helper.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var subcontracts = await context.Subcontracts
                .OrderByDescending(s => s.Id)
                .ToListAsync();
            logger.LogInformation("Listing {Count} subcontracts for user {UserId}", subcontracts.Count, CurrentUserId);
            return View(subcontracts);
        }

        /// <summary>
        /// GET: /Subcontracts/Create. Shows the empty form to create a new subcontract.
        /// </summary>
        public IActionResult Create() => View(new Subcontract());

        /// <summary>
        /// POST: /Subcontracts/Create. Persists the new subcontract and creates its initial
        /// transition, which places it in the process' initial status.
        /// </summary>
        /// <param name="subcontract">The subcontract bound from the form.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subcontract subcontract)
        {
            if (!ModelState.IsValid)
                return View(subcontract);

            subcontract.CreatedAt = DateTime.UtcNow;
            context.Subcontracts.Add(subcontract);
            await context.SaveChangesAsync();
            logger.LogInformation("Subcontract {Id} created by user {UserId}", subcontract.Id, CurrentUserId);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: /Subcontracts/Card/5. Returns the modal content for a single subcontract. The
        /// transition-badge / transition-buttons tag helpers inside the
        /// partial fetch their own status and available transitions.
        /// </summary>
        /// <param name="id">The subcontract id.</param>
        public async Task<IActionResult> Card(int id)
        {
            var subcontract = await context.Subcontracts.FindAsync(id);
            if (subcontract == null)
            {
                logger.LogWarning("Subcontract {Id} not found for card view", id);
                return NotFound();
            }

            return PartialView("_SubcontractCard", new SubcontractDetailsViewModel
            {
                Subcontract = subcontract
            });
        }
    }
}
