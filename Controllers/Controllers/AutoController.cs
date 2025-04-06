using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.Data.Models;
using RentACar.Data.ViewModels;
using RentACar.Repositories;
using RentACar.Repositories.Interfaces;
using RentACar.Services.Interfaces;

namespace RentACar.Controllers.Controllers
{
    public class AutoController : Controller
    {
        private readonly IAutoRepository _autoRepository;
        private readonly IPhotoService _photoService;
        public AutoController(IAutoRepository autoRepository, IPhotoService photoService)
        {
           _autoRepository = autoRepository;
           _photoService = photoService;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var autos = await _autoRepository.GetAllAutosAsync();
            return View(autos);
        }

        public async Task<IActionResult> FreeAutos(DateTime startDate, DateTime endDate)
        {
            var autos = await _autoRepository.GetAllAutosFreeAsync(startDate, endDate);
            return View(autos);
        }

        public async Task<IActionResult> Details(int id)
        {
            Auto auto = await _autoRepository.GetAutoByIdAsync(id);
            if (auto == null)
            {
                return NotFound();
            }
            return View(auto);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAutoVM autoVM)
        {
            if (!ModelState.IsValid)
            {
                return View(autoVM);
            }
            var photoResult = await _photoService.AddPhotoAsync(autoVM.Image);
            if (photoResult.Error != null)
            {
                ModelState.AddModelError("Image", "Photo upload failed");
                return View(autoVM);
            }
            Auto auto = new Auto
            {
                Brand = autoVM.Brand,
                Model = autoVM.Model,
                Year = autoVM.Year,
                PassengerSeats = autoVM.PassengerSeats,
                Description = autoVM.Description,
                Image = photoResult.Url.ToString(),
                PricePerDay = autoVM.PricePerDay,
            };

            await _autoRepository.AddAsync(auto);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            Auto auto = await _autoRepository.GetAutoByIdAsync(id);
            if (auto == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            EditAutoVM editAutoVM = new EditAutoVM
            {
                Id = auto.Id,
                Brand = auto.Brand,
                Model = auto.Model,
                Year = auto.Year,
                PassengerSeats = auto.PassengerSeats,
                Description = auto.Description,
                URL = auto.Image,
                PricePerDay = auto.PricePerDay,
            };
            return View(editAutoVM);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditAutoVM editAutoVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit idea.");
                return View(editAutoVM);
            }
            Auto auto = await _autoRepository.GetAutoByIdAsync(editAutoVM.Id);
            if (auto == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }

            try
            {
                await _photoService.DeletePhotoAsync(auto.Image);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Could not delete photo.");
                return View(editAutoVM);
            }

            var photoResult = await _photoService.AddPhotoAsync(editAutoVM.Image);

            auto.Id = editAutoVM.Id;
            auto.Brand = editAutoVM.Brand;
            auto.Model = editAutoVM.Model;
            auto.Year = editAutoVM.Year;
            auto.PassengerSeats = editAutoVM.PassengerSeats;
            auto.Description = editAutoVM.Description;
            auto.Image = photoResult.Url.ToString();
            auto.PricePerDay = editAutoVM.PricePerDay;

            await _autoRepository.UpdateAsync(auto);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            Auto auto = await _autoRepository.GetAutoByIdAsync(id);
            if (auto == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            return View(auto);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteAuto(int id)
        {
            Auto auto = await _autoRepository.GetAutoByIdAsync(id);
            if (auto == null)
            {
                ViewData["ErrorMessage"] = "A problem occurred while proceeding with your request. Access denied.";
                return View("Error");
            }
            await _autoRepository.DeleteAsync(auto);
            return RedirectToAction("Index");
        }

    }
}
