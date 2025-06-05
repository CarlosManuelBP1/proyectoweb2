using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System;
using AerolineaMVC.Models;
namespace AerolineaMVC.Controllers
{
    public class PagosController : Controller
    {
        [HttpGet]
        public IActionResult SimularPago(decimal monto)
        {
            var model = new PagoViewModel
            {
                Monto = monto
            };

            return View(model);
        }


        [HttpPost]
        public IActionResult SimularPago(PagoViewModel model)
        {
            if (model.MetodoPago == "Tarjeta")
            {
                if (string.IsNullOrWhiteSpace(model.TipoTarjeta) ||
                    string.IsNullOrWhiteSpace(model.NumeroTarjeta) ||
                    model.NumeroTarjeta.Length != 16 ||
                    string.IsNullOrWhiteSpace(model.FechaExpiracion) ||
                    string.IsNullOrWhiteSpace(model.CVV))
                {
                    ModelState.AddModelError("", "Datos de tarjeta inválidos.");
                    return View(model);
                }
            }
            else if (model.MetodoPago == "PayPal")
            {
                if (string.IsNullOrWhiteSpace(model.CorreoPayPal))
                {
                    ModelState.AddModelError("", "Ingrese su correo de PayPal.");
                    return View(model);
                }
            }

            model.PagoExitoso = true;
            model.PNR = "PNR" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(model.PNR, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);
                model.QrCodeBase64 = Convert.ToBase64String(qrCodeBytes);

            return View("PagoExitoso", model);
        }
    }
}
