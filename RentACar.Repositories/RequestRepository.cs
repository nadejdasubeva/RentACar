using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Data.Models;
using RentACar.Repositories.Helpers;
using RentACar.Repositories.Interfaces;

namespace RentACar.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingPeriodRepository _bookingPeriodRepository;

        public RequestRepository(ApplicationDbContext context, IBookingPeriodRepository bookingPeriodRepository)
        {
            _context = context;
            _bookingPeriodRepository = bookingPeriodRepository;
        }

        public async Task<IEnumerable<Request>> GetAllRequestsAsync()
        {
            return await _context.Requests.ToListAsync();
        }
        public async Task<IEnumerable<Request>> GetAllByUserId(string userId)
        {
            return await _context.Requests
                .Where(x => x.UserId == userId)
                .Include(x => x.Auto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetAllRequestsAnswered()
        {
            return await _context.Requests
                .Where(x => x.IsApproved == true || x.IsDeclined == true)
                .ToListAsync();
        }

        public async Task<IEnumerable<Request>> GetAllRequestsUnanswered()
        {
            return await _context.Requests
                .Where(x => x.IsApproved == false && x.IsDeclined == false)
                .ToListAsync();
        }
        public async Task<Request> GetRequestByIdAsync(int id)
        {
            return await _context.Requests.Where(x => x.Id == id)
                                          .Include(x => x.User)
                                          .Include(x => x.Auto)
                                          .FirstOrDefaultAsync();
        }
        public async Task<bool> ApproveRequest(int requestId)
        {
            Request request = await GetRequestByIdAsync(requestId);
            BookingResult book = await _bookingPeriodRepository
                                       .BookAutoAsync(request.AutoId, request.PickUpDate, request.ReturnDate);
            if (!book.Success)
            {
                return false;
            }
            else
            {
                int updatedCount = await _context.Requests
                    .Where(r => r.Id == requestId)
                    .ExecuteUpdateAsync(r => r.SetProperty(x => x.IsApproved, true));
                return updatedCount > 0;
            }
        }

        public async Task<bool> DeclineRequest(int requestId)
        {
            return await _context.Requests.Where(r => r.Id == requestId)
                .ExecuteUpdateAsync(r => r.SetProperty(x => x.IsDeclined, true)) > 0;
        }

        public async Task<bool> AddAsync(Request request)
        {
            _context.Add(request);
            return await SaveAsync();
        }

        public async Task<bool> DeleteAsync(Request request)
        {
            _context.Remove(request);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(Request request)
        {
            _context.Update(request);
            return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
