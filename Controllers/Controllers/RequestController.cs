using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RentACar.Data.Models;
using RentACar.Data.ViewModels;
using RentACar.Repositories;
using RentACar.Repositories.Interfaces;
using System.Security.Claims;

namespace RentACar.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IAutoRepository _autoRepository;
        private readonly IBookingPeriodRepository _bookingPeriodRepository;
        private readonly ILogger<HomeController> _logger;

        public RequestController( IRequestRepository requestRepository, IAutoRepository autoRepository, IBookingPeriodRepository bookingPeriodRepository, ILogger<HomeController> logger)
        {
            _requestRepository = requestRepository;
            _autoRepository = autoRepository;
            _bookingPeriodRepository = bookingPeriodRepository;
            _logger = logger;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index(string filter = "all", string sortOrder = "newest")
        {
            IEnumerable<Request> requests;

            // Apply filter
            switch (filter.ToLower())
            {
                case "pending":
                    requests = await _requestRepository.GetAllRequestsUnanswered();
                    break;
                case "processed":
                    requests = await _requestRepository.GetAllRequestsAnswered();
                    break;
                default:
                    requests = await _requestRepository.GetAllRequestsAsync();
                    break;
            }

            // Apply sorting
            requests = sortOrder.ToLower() switch
            {
                "oldest" => requests.OrderBy(r => r.DateOfRequest),
                "status" => requests.OrderBy(r => r.IsApproved),
                _ => requests.OrderByDescending(r => r.DateOfRequest) // default: newest first
            };

            // Pass filter and sort options to view
            ViewBag.CurrentFilter = filter;
            ViewBag.CurrentSort = sortOrder;

            return View(requests);
        }

        [Authorize(Roles = "BasicUser")]
        public async Task<IActionResult> MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var requests = await _requestRepository.GetAllByUserId(userId);
            return View(requests);
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Administrator"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (request.UserId != userId)
                {
                    return Forbid();
                }
            }

            return View(request);
        }

        [Authorize(Roles = "BasicUser")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateRequestVM
            {
                PickUpDate = DateTime.Today.AddDays(1),
                ReturnDate = DateTime.Today.AddDays(2)
            };
            return View(model);
        }

        [Authorize(Roles = "BasicUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAvailability([FromForm] DateTime pickUpDate, [FromForm] DateTime returnDate)
        {
            try
            {
                // Basic validation
                if (pickUpDate == default || returnDate == default)
                {
                    return BadRequest("Both dates are required");
                }

                if (pickUpDate.Date < DateTime.Today)
                {
                    return BadRequest("Pick-up date cannot be in the past");
                }

                if (pickUpDate >= returnDate)
                {
                    return BadRequest("Return date must be after pick-up date");
                }

                // Get available autos
                var availableAutos = await _autoRepository.GetAllAutosFreeAsync(pickUpDate.Date, returnDate.Date);

                // Return the available autos as a partial view
                var model = new CreateRequestVM
                {
                    AvailableAutos = availableAutos,
                    PickUpDate = pickUpDate,
                    ReturnDate = returnDate
                };

                return PartialView("_AutoSelectionPartial", model); // This should be the partial view that displays available autos
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckAvailability");
                return StatusCode(500, "An error occurred while checking availability");
            }
        }

        [Authorize(Roles = "BasicUser")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Ensure the auto is still available
            var isAvailable = await _bookingPeriodRepository.IsAutoAvailableAsync(
                model.AutoId,
                model.PickUpDate,
                model.ReturnDate);

            if (!isAvailable)
            {
                ModelState.AddModelError("", "Selected vehicle is no longer available.");
                model.AvailableAutos = await _autoRepository.GetAllAutosFreeAsync(model.PickUpDate, model.ReturnDate);
                return View(model);
            }

            if (model.AutoId == 0)
            {
                ModelState.AddModelError("", "Please select a vehicle.");
                model.AvailableAutos = await _autoRepository.GetAllAutosFreeAsync(model.PickUpDate, model.ReturnDate);
                return View(model);
            }

            // Create request
            var request = new Request
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AutoId = model.AutoId,
                PickUpDate = model.PickUpDate,
                ReturnDate = model.ReturnDate,
                DateOfRequest = DateTime.Now
            };

            // Book the vehicle
            var bookingResult = await _bookingPeriodRepository.BookAutoAsync(
                model.AutoId,
                model.PickUpDate,
                model.ReturnDate);

            if (!bookingResult.Success)
            {
                ModelState.AddModelError("", bookingResult.Message);
                model.AvailableAutos = await _autoRepository.GetAllAutosFreeAsync(model.PickUpDate, model.ReturnDate);
                return View(model);
            }

            // Save the request in the database
            await _requestRepository.AddAsync(request);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToAction("MyRequests");
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _requestRepository.ApproveRequest(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to approve request. The auto might be unavailable for the selected period.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Request approved successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id)
        {
            var success = await _requestRepository.DeclineRequest(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to decline request";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Request declined successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (request.UserId != userId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _requestRepository.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var success = await _requestRepository.DeleteAsync(request);
            if (!success)
            {
                TempData["ErrorMessage"] = "Failed to delete request";
                return View(request);
            }

            TempData["SuccessMessage"] = "Request deleted successfully";
            return RedirectToAction(nameof(MyRequests));
        }
    }
}