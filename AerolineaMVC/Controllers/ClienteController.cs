using AerolineaMVC.Data;
using AerolineaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AerolineaMVC.Models.ViewModels;

namespace AerolineaMVC.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AerolineaContext _context;

        public ClienteController(AerolineaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> VuelosDisponibles()
        {
            var vuelos = await _context.Vuelos
                .Include(v => v.Tarifas)
                .Where(v => v.Fecha >= DateTime.Now)
                .OrderBy(v => v.Fecha)
                .ToListAsync();

            return View(vuelos);
        }

        public async Task<IActionResult> VerTarifas(int id)
        {
            var vuelo = await _context.Vuelos
                .Include(v => v.Tarifas)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vuelo == null)
            {
                return NotFound();
            }

            return View(vuelo);
        }

        public async Task<IActionResult> SeleccionarTarifa(int id)
        {
            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarifa == null)
            {
                return NotFound();
            }

            return View(tarifa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReservarTarifa(int tarifaId, string nombreCliente, string correoCliente)
        {
            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == tarifaId);

            if (tarifa == null || tarifa.Vuelo == null || !tarifa.Vuelo.Disponibilidad || tarifa.Vuelo.CantidadAsientos <= 0)
            {
                TempData["ErrorMessage"] = "La tarifa o el vuelo ya no está disponible.";
                return RedirectToAction("VuelosDisponibles");
            }

            var reserva = new Reserva
            {
                TarifaId = tarifaId,
                NombreCliente = nombreCliente,
                CorreoCliente = correoCliente
            };

            tarifa.Vuelo.CantidadAsientos--;
            if (tarifa.Vuelo.CantidadAsientos == 0)
            {
                tarifa.Vuelo.Disponibilidad = false;
            }

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Reserva realizada con éxito para {reserva.NombreCliente}.";
            return RedirectToAction("VuelosDisponibles");
        }

        // GET ConfirmarReserva
        public async Task<IActionResult> ConfirmarReserva(int id)
        {
            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarifa == null || tarifa.Vuelo == null || !tarifa.Vuelo.Disponibilidad || tarifa.Vuelo.CantidadAsientos <= 0)
            {
                TempData["ErrorMessage"] = "La tarifa no está disponible.";
                return RedirectToAction("VuelosDisponibles");
            }

            var viewModel = new ReservaViewModel

            {
                TarifaId = id,
                Tarifa = tarifa,
                Vuelo = tarifa.Vuelo,
                Pasajeros = new List<PasajeroViewModel>
                {
                new PasajeroViewModel()
                }
            };

            return View(viewModel);
        }

        // POST ConfirmarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(ReservaViewModel model)

        {
            Console.WriteLine("---- INICIO POST ConfirmarReserva ----");
            Console.WriteLine($"Total keys en Request.Form: {Request.Form.Keys.Count}");

            foreach (var key in Request.Form.Keys)
            {
                Console.WriteLine($"Formulario: {key} = {Request.Form[key]}");
            }

            //if (!ModelState.IsValid)
            //{
            //    Console.WriteLine("Modelo inválido");
            //    return View(model);
            //}

            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == model.TarifaId);

            if (model.Pasajeros == null || !model.Pasajeros.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un pasajero.");
                return View(model);
            }

            if (tarifa == null || tarifa.Vuelo == null || !tarifa.Vuelo.Disponibilidad || tarifa.Vuelo.CantidadAsientos < model.Pasajeros.Count)
            {
                TempData["ErrorMessage"] = "La tarifa o el vuelo ya no está disponible para la cantidad de pasajeros.";
                return RedirectToAction("VuelosDisponibles");
            }

            //var reserva = new Reserva
            //{
            //    TarifaId = model.TarifaId,
            //    NombreCliente = model.NombreCliente,
            //    CorreoCliente = model.CorreoCliente
            //};

            foreach (PasajeroViewModel pasajeroVM in model.Pasajeros)
            {
                Console.WriteLine($"Recibido: Nombre={pasajeroVM.Nombre}, Documento={pasajeroVM.Documento}, Edad={pasajeroVM.Edad}");
                Console.WriteLine($"Contacto Emergencia: Nombre={pasajeroVM.ContactoEmergenciaNombre}, Celular={pasajeroVM.ContactoEmergenciaCelular}, Correo={pasajeroVM.ContactoEmergenciaCorreo}");
                Console.WriteLine($"Servicios Extra: Maleta={pasajeroVM.Maleta}, Comida={pasajeroVM.Comida}, Mascota={pasajeroVM.Mascota}");
                var reserva = new Reserva
                {
                    TarifaId = model.TarifaId,
                    NombreCliente = pasajeroVM.Nombre,
                    CorreoCliente = pasajeroVM.ContactoEmergenciaCorreo
                };
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync(); // Guardamos primero para obtener el ID de la reserva


                var pasajero = new Pasajero
                {
                    Nombre = pasajeroVM.Nombre,
                    Documento = pasajeroVM.Documento,
                    Edad = pasajeroVM.Edad,
                    ContactoEmergenciaNombre = pasajeroVM.ContactoEmergenciaNombre,
                    ContactoEmergenciaCorreo = pasajeroVM.ContactoEmergenciaCorreo,
                    ContactoEmergenciaCelular = pasajeroVM.ContactoEmergenciaCelular,
                    Maleta = pasajeroVM.Maleta,
                    Comida = pasajeroVM.Comida,
                    Mascota = pasajeroVM.Mascota,
                    ReservaId = reserva.Id
                };
                _context.Pasajeros.Add(pasajero);

            };

           

            //foreach (PasajeroViewModel pasajeroVM in model.Pasajeros)
            //{
            //    Console.WriteLine($"Recibido: Nombre={pasajeroVM.Nombre}, Documento={pasajeroVM.Documento}, Edad={pasajeroVM.Edad}");
            //    Console.WriteLine($"Contacto Emergencia: Nombre={pasajeroVM.ContactoEmergenciaNombre}, Celular={pasajeroVM.ContactoEmergenciaCelular}, Correo={pasajeroVM.ContactoEmergenciaCorreo}");
            //    Console.WriteLine($"Servicios Extra: Maleta={pasajeroVM.Maleta}, Comida={pasajeroVM.Comida}, Mascota={pasajeroVM.Mascota}");

            //    var pasajero = new Pasajero
            //    {
            //        Nombre = pasajeroVM.Nombre,
            //        Documento = pasajeroVM.Documento,
            //        Edad = pasajeroVM.Edad,
            //        ContactoEmergenciaNombre = pasajeroVM.ContactoEmergenciaNombre,
            //        ContactoEmergenciaCorreo = pasajeroVM.ContactoEmergenciaCorreo,
            //        ContactoEmergenciaCelular = pasajeroVM.ContactoEmergenciaCelular,
            //        Maleta = pasajeroVM.Maleta,
            //        Comida = pasajeroVM.Comida,
            //        Mascota = pasajeroVM.Mascota,
            //        ReservaId = reserva.Id
            //    };

            //    _context.Pasajeros.Add(pasajero);
            //}

            await _context.SaveChangesAsync();

            // Actualizar asientos disponibles del vuelo
            tarifa.Vuelo.CantidadAsientos -= model.Pasajeros.Count;
            if (tarifa.Vuelo.CantidadAsientos <= 0)
            {
                tarifa.Vuelo.Disponibilidad = false;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Reserva realizada con éxito para {model.NombreCliente}.";
            return RedirectToAction("VuelosDisponibles");
        }
    }
}
