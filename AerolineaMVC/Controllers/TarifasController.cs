using AerolineaMVC.Data;
using AerolineaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AerolineaMVC.Controllers
{
    public class TarifasController : Controller
    {
        private readonly AerolineaContext _context;

        // Constructor que inicializa el contexto de la base de datos
        public TarifasController(AerolineaContext context)
        {
            _context = context;
        }

        // Método para listar todas las tarifas con la información del vuelo asociado
        public async Task<IActionResult> Index()
        {
            var tarifas = await _context.Tarifas
                .Include(t => t.Vuelo) // Incluye información del vuelo relacionado
                .ToListAsync();
            return View(tarifas);
        }

        // Método para mostrar los detalles de una tarifa
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Retorna 404 si el ID es nulo
            }

            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo) // Incluye los datos del vuelo asociado
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarifa == null)
            {
                return NotFound(); // Retorna 404 si no encuentra la tarifa
            }

            return View(tarifa); // Devuelve la tarifa a la vista
        }

        // Método para mostrar el formulario de creación de una nueva tarifa
        public IActionResult Create()
        {
            ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino"); // Lista de vuelos disponibles
            return View();
        }

        // Método para procesar la creación de una nueva tarifa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarifa tarifa)
        {
            try
            {
                string precioStr = tarifa.Precio.ToString();
                if (precioStr.Contains(","))
                {
                    precioStr = precioStr.Replace(",", ".");
                }

                if (!decimal.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioValidado))
                {
                    ModelState.AddModelError("Precio", "Formato de precio inválido.");
                }
                else
                {
                    tarifa.Precio = precioValidado;
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
                    return View(tarifa);
                }

                if (tarifa.Precio <= 0)
                {
                    ModelState.AddModelError("Precio", "El precio debe ser mayor a 0.");
                    ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
                    return View(tarifa);
                }

                _context.Tarifas.Add(tarifa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tarifa creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar la tarifa: {ex.Message}");
                TempData["ErrorMessage"] = "Ocurrió un error al intentar guardar la tarifa.";
                return View(tarifa);
            }
        }
        //mostrar tarifas de vuelo
        public async Task<IActionResult> ListarPorVuelo(int vueloId)
        {
            var tarifas = await _context.Tarifas
                .Where(t => t.VueloId == vueloId)
                .ToListAsync();

            ViewBag.VueloId = vueloId;
            return View(tarifas);
        }


        // Método para mostrar el formulario de edición de una tarifa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var tarifa = await _context.Tarifas.FindAsync(id);
            if (tarifa == null)
                return NotFound();

            ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
            return View(tarifa);
        }

        // Método para procesar la edición de una tarifa existente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Precio,VueloId")] Tarifa tarifa)
        {
            if (id != tarifa.Id)
                return NotFound();

            string precioStr = tarifa.Precio.ToString();
            if (precioStr.Contains(","))
            {
                precioStr = precioStr.Replace(",", ".");
            }

            if (!decimal.TryParse(precioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioValidado))
            {
                ModelState.AddModelError("Precio", "Formato de precio inválido.");
            }
            else
            {
                tarifa.Precio = precioValidado;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
                return View(tarifa);
            }

            try
            {
                var tarifaExistente = await _context.Tarifas.AsNoTracking().FirstOrDefaultAsync(t => t.Id == tarifa.Id);
                if (tarifaExistente == null)
                    return NotFound();

                var vueloExistente = await _context.Vuelos.FindAsync(tarifa.VueloId);
                if (vueloExistente == null)
                {
                    ModelState.AddModelError("VueloId", "El vuelo seleccionado no existe.");
                    ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
                    return View(tarifa);
                }

                _context.Update(tarifa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tarifa actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al actualizar la tarifa: {ex.Message}");
                TempData["ErrorMessage"] = "Error al actualizar la tarifa.";
            }

            ViewBag.VueloId = new SelectList(_context.Vuelos, "Id", "Destino", tarifa.VueloId);
            return View(tarifa);
        }

        // Método para mostrar la vista de confirmación de eliminación de una tarifa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (tarifa == null)
                return NotFound();

            return View(tarifa);
        }

        // Método para procesar la eliminación de una tarifa
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarifa = await _context.Tarifas.FindAsync(id);
            if (tarifa != null)
            {
                _context.Tarifas.Remove(tarifa);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tarifa eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Método para obtener la cantidad de asientos disponibles de un vuelo específico
        public JsonResult GetAsientosDisponibles(int id)
        {
            var vuelo = _context.Vuelos.Find(id);
            if (vuelo == null)
                return Json(new { error = "Vuelo no encontrado" });

            return Json(new { asientosDisponibles = vuelo.Disponibilidad });
        }
    }
}
