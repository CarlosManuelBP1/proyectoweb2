using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AerolineaMVC.Data;
using AerolineaMVC.Models;

namespace AerolineaMVC.Controllers
{
        public class PasajerosController : Controller
        {
            private readonly AerolineaContext _context;

            public PasajerosController(AerolineaContext context)
            {
                _context = context;
            }

            // GET: Pasajeros
            public async Task<IActionResult> Index()
            {
                var pasajeros = await _context.Pasajeros
                    .Include(p => p.Reserva)
                    .ToListAsync();
                return View(pasajeros);
            }

            // GET: Pasajeros/Create
            public IActionResult Create()
            {
                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
                return View();
            }

        // POST: Pasajeros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pasajero pasajero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pasajero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ⚠️ Aquí agregamos este código para depurar:
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Error en {modelState.Key}: {error.ErrorMessage}");
                }
            }

            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pasajero.ReservaId);
            return View(pasajero);
        }


        // GET: Pasajeros/Edit/5
        public async Task<IActionResult> Edit(int id)
            {
                var pasajero = await _context.Pasajeros.FindAsync(id);
                if (pasajero == null)
                {
                    return NotFound();
                }

                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pasajero.ReservaId);
                return View(pasajero);
            }

            // POST: Pasajeros/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Documento,Edad,ReservaId")] Pasajero pasajero)
            {
                if (id != pasajero.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(pasajero);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.Pasajeros.Any(e => e.Id == pasajero.Id))
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

                ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pasajero.ReservaId);
                return View(pasajero);
            }

            // GET: Pasajeros/Delete/5
            public async Task<IActionResult> Delete(int id)
            {
                var pasajero = await _context.Pasajeros
                    .Include(p => p.Reserva)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (pasajero == null)
                {
                    return NotFound();
                }

                return View(pasajero);
            }

            // POST: Pasajeros/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var pasajero = await _context.Pasajeros.FindAsync(id);
                if (pasajero != null)
                {
                    _context.Pasajeros.Remove(pasajero);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }

