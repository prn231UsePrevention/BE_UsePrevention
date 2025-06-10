using Dto.Request;
using Dto.Response;
using Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IAppointmentService
    {
        Task<IEnumerable<ConsultantResponseDto>> GetConsultantsAsync();
        Task<IEnumerable<AvailableSlotResponseDto>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request);
        Task<AppointmentResponseDto> CreateAppointmentAsync(int userId, CreateAppointmentRequestDto request);
        Task<IEnumerable<AppointmentResponseDto>> GetUserAppointmentsAsync(int userId);
        Task<IEnumerable<AppointmentResponseDto>> GetConsultantAppointmentsAsync(int consultantId);
        Task UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequestDto request);
        Task CancelAppointmentAsync(int appointmentId, int userId, string userRole);
        Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId);
    }
}
