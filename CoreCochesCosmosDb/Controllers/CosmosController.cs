using CoreCochesCosmosDb.Models;
using CoreCochesCosmosDb.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreCochesCosmosDb.Controllers
{
    public class CosmosController : Controller
    {
        ServiceCosmosDb ServiceCosmos;
        public CosmosController(ServiceCosmosDb serviceCosmos)
        {
            this.ServiceCosmos = serviceCosmos;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(String accion)
        {
            await this.ServiceCosmos.CrearBbddVehiculosAsync();
            await this.ServiceCosmos.CrearColeccionVehiculosAsync();
            List<Vehiculo> coches = this.ServiceCosmos.CrearCoches();
            foreach (Vehiculo car in coches)
            {
                await this.ServiceCosmos.InsertarVehiculo(car);
            }
            ViewData["MENSAJE"] = "TODO INICIADO EN COSMOS";
            return View();
        }

        public IActionResult ListCoches()
        {
            return View(this.ServiceCosmos.GetVehiculos());
        }

        public async Task<IActionResult> Details(String id)
        {
            return View(await this.ServiceCosmos.FindVehiculoAsync(id));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Vehiculo car, String motor)
        {
            if (motor != null)
            {
                car.Motor = new Motor { Tipo = "Diesel", Caballos = 990, Potencia = 90 };
            }
            await this.ServiceCosmos.InsertarVehiculo(car);
            return RedirectToAction("ListCoches");
        }

        public async Task<IActionResult> Delete(String id)
        {
            await this.ServiceCosmos.EliminarVehiculo(id);
            return RedirectToAction("ListCoches");
        }

        public async Task<IActionResult> Edit(String id)
        {
            return View(await this.ServiceCosmos.FindVehiculoAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Vehiculo car, String motor)
        {
            if (motor != null)
            {
                car.Motor = new Motor { Tipo = "GASOIL", Caballos = 230, Potencia = 800 };
            }
            await this.ServiceCosmos.ModificarVehiculo(car);
            return RedirectToAction("ListCoches");
        }
    }
}
