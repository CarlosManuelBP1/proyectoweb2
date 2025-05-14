using AerolineaMVC.Data;
using AerolineaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AerolineaMVC.Controllers
{
    public class VuelosController : Controller
    {
        private readonly AerolineaContext _context;
        private readonly ILogger<VuelosController> _logger;

        public VuelosController(AerolineaContext context, ILogger<VuelosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var vuelos = await _context.Vuelos.AsNoTracking().ToListAsync();
            return View(vuelos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vuelo = await _context.Vuelos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        public IActionResult Create()
        {
            return View(new Vuelo { Fecha = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Origen,Destino,Fecha,Disponibilidad,CantidadAsientos")] Vuelo vuelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(vuelo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vuelo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al crear vuelo: {ex.Message}");
                    TempData["ErrorMessage"] = "Error al crear el vuelo.";
                }
            }

            return View(vuelo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vuelo = await _context.Vuelos.FindAsync(id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Origen,Destino,Fecha,Disponibilidad,CantidadAsientos")] Vuelo vuelo)
        {
            _logger.LogInformation($"Iniciando edición de vuelo. ID recibido: {id}");

            if (id != vuelo.Id)
            {
                _logger.LogError($"Error: ID en URL ({id}) no coincide con ID en formulario ({vuelo.Id})");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState no es válido. Errores:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
                return View(vuelo);
            }

            try
            {
                var vueloExistente = await _context.Vuelos.FindAsync(id);
                if (vueloExistente == null)
                {
                    _logger.LogError("Vuelo no encontrado en la base de datos.");
                    return NotFound();
                }

                _logger.LogInformation("Valores antes de actualizar:");
                _logger.LogInformation($"Origen: {vueloExistente.Origen}, Destino: {vueloExistente.Destino}, Fecha: {vueloExistente.Fecha}, Disponibilidad: {vueloExistente.Disponibilidad}, CantidadAsientos: {vueloExistente.CantidadAsientos}");

                // Se actualizan los valores manualmente
                vueloExistente.Origen = vuelo.Origen;
                vueloExistente.Destino = vuelo.Destino;
                vueloExistente.Fecha = vuelo.Fecha;
                vueloExistente.Disponibilidad = vuelo.Disponibilidad;
                vueloExistente.CantidadAsientos = vuelo.CantidadAsientos;

                _context.Vuelos.Update(vueloExistente);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Vuelo actualizado con éxito.");

                TempData["SuccessMessage"] = "Vuelo actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar vuelo: {ex.Message}");
                TempData["ErrorMessage"] = "Error al actualizar el vuelo.";
                return View(vuelo);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vuelo = await _context.Vuelos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var vuelo = await _context.Vuelos.FindAsync(id);
                if (vuelo != null)
                {
                    _context.Vuelos.Remove(vuelo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vuelo eliminado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "El vuelo ya no existe.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar vuelo: {ex.Message}");
                TempData["ErrorMessage"] = "Error al eliminar el vuelo.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VueloExists(int id)
        {
            return _context.Vuelos.Any(e => e.Id == id);
        }
    }
}
