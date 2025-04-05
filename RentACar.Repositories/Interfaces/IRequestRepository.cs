using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace RentACar.Repositories.Interfaces
{
    internal interface IRequestRepository
    {
        Task<IEnumerable<Request>> GetAllRequestsAsync();
        Task<IEnumerable<Request>> GetAllRequestsAnswered();
        Task<IEnumerable<Request>> GetAllRequestsUnanswered();
        Task<IEnumerable<Request>> GetAllByUserId(string userId);
        Task<Request> GetRequestByIdAsync(int id);
        Task<Request> ApproveRequest(int requestId);
        Task<Request> DeclineRequest(int requestId);
        Task<bool> AddAsync(Request request);
        Task<bool> UpdateAsync(Request request);
        Task<bool> DeleteAsync(Request request);
        Task<bool> SaveAsync();
    }
}
