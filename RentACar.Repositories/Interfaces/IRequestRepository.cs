using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentACar.Data.Models;

namespace RentACar.Repositories.Interfaces
{
    public interface IRequestRepository
    {
        //az
        Task<IEnumerable<Request>> GetAllRequestsAsync();
        Task<IEnumerable<Request>> GetAllRequestsAnswered();
        Task<IEnumerable<Request>> GetAllRequestsUnanswered();
        Task<IEnumerable<Request>> GetAllByUserId(string userId);
        Task<Request> GetRequestByIdAsync(int id);
        Task<bool> ApproveRequest(int requestId);
        Task<bool> DeclineRequest(int requestId);
        Task<bool> AddAsync(Request request);
        Task<bool> UpdateAsync(Request request);
        Task<bool> DeleteAsync(Request request);
        Task<bool> SaveAsync();
    }
}
