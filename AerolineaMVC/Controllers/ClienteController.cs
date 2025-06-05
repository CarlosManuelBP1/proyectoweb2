using AerolineaMVC.Data;
using AerolineaMVC.Models;
using AerolineaMVC.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QRCoder;
using System.Text;

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
                return NotFound();

            return View(vuelo);
        }

        public async Task<IActionResult> SeleccionarTarifa(int id)
        {
            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tarifa == null)
                return NotFound();

            return View(tarifa);
        }

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
                Pasajeros = new List<PasajeroViewModel> { new PasajeroViewModel() }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(ReservaViewModel model)
        {
            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == model.TarifaId);

            if (tarifa == null || tarifa.Vuelo == null || !tarifa.Vuelo.Disponibilidad || tarifa.Vuelo.CantidadAsientos < model.Pasajeros.Count)
            {
                TempData["ErrorMessage"] = "La tarifa o el vuelo ya no está disponible.";
                return RedirectToAction("VuelosDisponibles");
            }

            model.Tarifa = tarifa;
            model.Vuelo = tarifa.Vuelo;

            // descuento
            decimal precioBase = tarifa.Precio;
            decimal subtotal = 0;

            foreach (var pasajero in model.Pasajeros)
            {
                decimal extras = 0;
                if (pasajero.Maleta) extras += 150000;
                if (pasajero.Comida) extras += 100000;
                if (pasajero.Mascota) extras += 200000;

                subtotal += precioBase + extras;
            }

            decimal porcentajeDescuento = 0;
            int cantidadPasajeros = model.Pasajeros.Count;

            if (cantidadPasajeros == 2)
                porcentajeDescuento = 0.10m;
            else if (cantidadPasajeros == 3)
                porcentajeDescuento = 0.15m;
            else if (cantidadPasajeros >= 4)
                porcentajeDescuento = 0.20m;

            decimal descuento = subtotal * porcentajeDescuento;
            decimal totalFinal = subtotal - descuento;

            model.Subtotal = subtotal;
            model.Descuento = descuento;
            model.TotalFinal = totalFinal;
            model.TextoDescuento = descuento > 0 ? $"{porcentajeDescuento * 100}% (-{descuento.ToString("C")})" : "No aplica";


            var tempModel = new ReservaTempModel
            {
                TarifaId = model.TarifaId,
                Pasajeros = model.Pasajeros
            };

            TempData["ReservaViewModel"] = JsonConvert.SerializeObject(tempModel);
            TempData["MetodoPago"] = null;

            return View("PasarelaPago", model);
        }
        [HttpPost]
        public async Task<IActionResult> ProcesarPago(ReservaViewModel model, string metodoPago, string numeroTarjeta, string tipoTarjeta, string cvv, int mesVencimiento, int anioVencimiento)
        {
            if (metodoPago == "Tarjeta")
            {
                if (string.IsNullOrWhiteSpace(numeroTarjeta))
                {
                    ModelState.AddModelError("", "Debe ingresar un número de tarjeta.");
                    return View("PasarelaPago", model);
                }

                if (tipoTarjeta == "Visa" && !numeroTarjeta.StartsWith("4"))
                {
                    ModelState.AddModelError("", "El número de tarjeta no corresponde a una tarjeta Visa (debe comenzar con 4).");
                    return View("PasarelaPago", model);
                }
                else if (tipoTarjeta == "MasterCard" && !numeroTarjeta.StartsWith("5"))
                {
                    ModelState.AddModelError("", "El número de tarjeta no corresponde a una tarjeta MasterCard (debe comenzar con 5).");
                    return View("PasarelaPago", model);
                }
            }

            if (!TempData.ContainsKey("ReservaViewModel"))
                return RedirectToAction("VuelosDisponibles");

            var json = TempData["ReservaViewModel"].ToString();
            var tempModel = JsonConvert.DeserializeObject<ReservaTempModel>(json);

            var tarifa = await _context.Tarifas
                .Include(t => t.Vuelo)
                .FirstOrDefaultAsync(t => t.Id == tempModel.TarifaId);

            if (tarifa == null || tarifa.Vuelo == null || !tarifa.Vuelo.Disponibilidad || tarifa.Vuelo.CantidadAsientos < tempModel.Pasajeros.Count)
            {
                TempData["ErrorMessage"] = "Error al procesar la reserva.";
                return RedirectToAction("VuelosDisponibles");
            }


            //  descuento 

            decimal precioBase = tarifa.Precio;
            decimal subtotal = 0;

            foreach (var pasajero in model.Pasajeros)
            {
                decimal extras = 0;
                if (pasajero.Maleta) extras += 150000;
                if (pasajero.Comida) extras += 100000;
                if (pasajero.Mascota) extras += 200000;

                subtotal += precioBase + extras;
            }

            decimal porcentajeDescuento = 0;
            int cantidadPasajeros = model.Pasajeros.Count;

            if (cantidadPasajeros == 2)
                porcentajeDescuento = 0.10m;
            else if (cantidadPasajeros == 3)
                porcentajeDescuento = 0.15m;
            else if (cantidadPasajeros >= 4)
                porcentajeDescuento = 0.20m;

            decimal descuento = subtotal * porcentajeDescuento;
            decimal totalFinal = subtotal - descuento;

            model.Subtotal = subtotal;
            model.Descuento = descuento;
            model.TotalFinal = totalFinal;
            model.TextoDescuento = descuento > 0 ? $"{porcentajeDescuento * 100}% (-{descuento.ToString("C")})" : "No aplica";




            TempData["TotalFinal"] = totalFinal.ToString("C");
            TempData["DescuentoAplicado"] = descuento > 0 ? $"{porcentajeDescuento * 100}% (-{descuento.ToString("C")})" : "No aplica";






            // hasta aqui va el descuento

            // CREACIÓN DE RESERVAS

            var reservasCreadas = new List<Reserva>();

            foreach (var pasajeroVM in tempModel.Pasajeros)
            {
                var reserva = new Reserva
                {
                    TarifaId = tempModel.TarifaId,
                    NombreCliente = pasajeroVM.Nombre,
                    CorreoCliente = pasajeroVM.ContactoEmergenciaCorreo
                };
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                var pasajero = new Pasajero
                {
                    Nombre = pasajeroVM.Nombre,
                    Documento = pasajeroVM.Documento,
                    Asiento = pasajeroVM.Asiento,
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
                reservasCreadas.Add(reserva);
            }

            tarifa.Vuelo.CantidadAsientos -= tempModel.Pasajeros.Count;
            if (tarifa.Vuelo.CantidadAsientos <= 0)
                tarifa.Vuelo.Disponibilidad = false;

            await _context.SaveChangesAsync();

            string qrContent = $"Reserva: {string.Join(", ", tempModel.Pasajeros.Select(p => $"{p.Nombre} ({p.Asiento})"))} - Vuelo: {tarifa.Vuelo.Origen} → {tarifa.Vuelo.Destino} - Fecha: {tarifa.Vuelo.Fecha.ToShortDateString()}";

            TempData["QrCodeBase64"] = GenerarCodigoQR(qrContent);
            TempData["SuccessMessage"] = "¡Reserva confirmada y pagada correctamente!";
            return RedirectToAction("MostrarQR");
        }


        public IActionResult MostrarQR()
        {
            return View();
        }

        private string GenerarCodigoQR(string contenido)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(contenido, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }

        // Clase auxiliar para evitar errores de tipo dinámico con expresiones lambda
        public class ReservaTempModel
        {
            public int TarifaId { get; set; }
            public List<PasajeroViewModel> Pasajeros { get; set; }
        }
    }
}
