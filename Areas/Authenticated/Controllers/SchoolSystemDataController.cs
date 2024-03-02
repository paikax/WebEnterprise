using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEnterprise.Data;
using WebEnterprise.Models;

namespace WebEnterprise.Areas.Authenticated.Controllers;

[Area(Constants.Areas.AuthenticatedArea)]
[Authorize(Roles = Constants.Roles.AdminRole)]
public class SchoolSystemDataController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public SchoolSystemDataController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var systemData = await _context.SchoolSystemDatas.ToListAsync();
        return View(systemData);
    }
    
    // GET: /Authenticated/SchoolSystemData/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: /Authenticated/SchoolSystemData/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SchoolSystemData systemData)
    {
        if (ModelState.IsValid)
        {
            _context.Add(systemData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(systemData);
    }
    
    // GET: /Authenticated/SchoolSystemData/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemData = await _context.SchoolSystemDatas.FindAsync(id);
            if (systemData == null)
            {
                return NotFound();
            }
            return View(systemData);
        }

        // POST: /Authenticated/SchoolSystemData/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SchoolSystemData systemData)
        {
            if (id != systemData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemDataExists(systemData.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(systemData);
        }

        // GET: /Authenticated/SchoolSystemData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemData = await _context.SchoolSystemDatas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (systemData == null)
            {
                return NotFound();
            }

            return View(systemData);
        }

        // POST: /Authenticated/SchoolSystemData/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemData = await _context.SchoolSystemDatas.FindAsync(id);
            _context.SchoolSystemDatas.Remove(systemData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemDataExists(int id)
        {
            return _context.SchoolSystemDatas.Any(e => e.Id == id);
        }
    
    
}