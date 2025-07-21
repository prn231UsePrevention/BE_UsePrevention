using AutoMapper;
using Common.helper;
using Dto.Request;
using Dto.Response;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using Repository.UWO;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{

    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ConsultantResponseDto>> GetConsultantsAsync()
        {
            var consultants = await _unitOfWork.Consultant
                .Query()
                .Include(c => c.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ConsultantResponseDto>>(consultants);
        }

        public async Task<IEnumerable<AvailableSlotResponseDto>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request)
        {
            var consultant = await _unitOfWork.Consultant.GetByIdAsync(request.ConsultantId);
            if (consultant == null)
                throw new KeyNotFoundException("Consultant not found");

            var slots = await _unitOfWork.Appointment.FindAsync(a =>
                (a.ConsultantId == request.ConsultantId ||
                a.DateTime.Date == request.Date.Date) &&
                a.Status == "Available" &&
                a.UserId == null);

            return slots.Select(a => new AvailableSlotResponseDto
            {
                SlotId = a.Id, // Appointment.Id
                Start = a.DateTime,
                End = a.DateTime.AddHours(1), // Mỗi slot kéo dài 1 tiếng
                ConsultantId = a.ConsultantId,
            });
        }

        public async Task<IEnumerable<AvailableSlotResponseDto>> GetAllAvailableSlotsAsync()
        {
            var slots = await _unitOfWork.Appointment
                .Query()
                .Include(a => a.Consultant)
                .ThenInclude(c => c.User)
                .Where(a => a.Status == "Available" && a.UserId == null)
                .OrderBy(a => a.DateTime)
                .ToListAsync();

            return slots.Select(a => new AvailableSlotResponseDto
            {
                SlotId = a.Id,
                Start = a.DateTime,
                End = a.DateTime.AddHours(1),
                ConsultantId = a.ConsultantId,
            });
        }

        public async Task<AppointmentResponseDto> BookSlotAsync(int userId, BookSlotRequestDto request)
        {
            var slot = await _unitOfWork.Appointment.GetByIdAsync(request.SlotId);
            if (slot == null || slot.Status != "Available" || slot.UserId != null)
                throw new InvalidOperationException("Invalid or unavailable slot");

            var consultant = await _unitOfWork.Consultant.GetByIdAsync(slot.ConsultantId);
            if (consultant == null)
                throw new KeyNotFoundException("Consultant not found");

            var user = await _unitOfWork.User.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            slot.UserId = userId;
            slot.Status = "Pending";
            slot.Note = request.Note;
            _unitOfWork.Appointment.UpdateAsync(slot);

            await _unitOfWork.CommitAsync();

            var result = _mapper.Map<AppointmentResponseDto>(slot);
            result.UserFullName = user?.FullName ?? "Unknown";
            result.ConsultantFullName = consultant.User?.FullName ?? "Unknown";
            result.StartTime = slot.DateTime;
            result.EndTime = slot.DateTime.AddHours(1);
            return result;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetUserAppointmentsAsync(int userId)
        {
            var appointments = await _unitOfWork.Appointment
                .Query()
                .Include(a => a.Consultant)
                .ThenInclude(c => c.User) // Nạp thông tin User của Consultant
                .Include(a => a.User) // Nạp thông tin User của Appointment
                .Where(a => a.UserId == userId && a.Status != "Available")
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
            foreach (var appointmentDto in result)
            {
                appointmentDto.UserFullName = appointments.First(a => a.Id == appointmentDto.Id).User?.FullName ?? "Unknown";
                appointmentDto.ConsultantFullName = appointments.First(a => a.Id == appointmentDto.Id).Consultant?.User?.FullName ?? "Unknown";
                appointmentDto.EndTime = appointmentDto.StartTime.AddHours(1);
            }
            return result;
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetConsultantAppointmentsAsync(int consultantId)
        {
            var appointments = await _unitOfWork.Appointment.FindAsync(a => a.ConsultantId == consultantId && a.Status != "Available");
            var result = _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
            foreach (var appointmentDto in result)
            {
                var user = await _unitOfWork.User.GetByIdAsync(appointmentDto.UserId ?? 0);
                var consultant = await _unitOfWork.Consultant.GetByIdAsync(appointmentDto.ConsultantId);
                appointmentDto.UserFullName = user?.FullName ?? "Unknown";
                appointmentDto.ConsultantFullName = consultant?.User?.FullName ?? "Unknown";
                appointmentDto.EndTime = appointmentDto.StartTime.AddHours(1);
            }
            return result;
        }

        public async Task UpdateAppointmentStatusAsync(int appointmentId, UpdateAppointmentStatusRequestDto request)
        {
            var appointment = await _unitOfWork.Appointment.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            if (!new[] { "Pending", "Confirmed", "Cancelled", "Completed" }.Contains(request.Status))
                throw new ArgumentException("Invalid status");

            appointment.Status = request.Status;
            _unitOfWork.Appointment.UpdateAsync(appointment);
            await _unitOfWork.CommitAsync();
        }

        public async Task CancelAppointmentAsync(int appointmentId, int userId, string userRole)
        {
            var appointment = await _unitOfWork.Appointment.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            var isConsultant = await _unitOfWork.Consultant.GetByIdAsync(userId) != null;
            if (userRole == "Member" && appointment.UserId != userId)
                throw new UnauthorizedAccessException("User not authorized to cancel this appointment");

            appointment.Status = "Cancelled";
            _unitOfWork.Appointment.UpdateAsync(appointment);
            await _unitOfWork.CommitAsync();
        }

        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointment.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found");

            var result = _mapper.Map<AppointmentResponseDto>(appointment);
            var user = await _unitOfWork.User.GetByIdAsync(result.UserId ?? 0);
            var consultant = await _unitOfWork.Consultant.GetByIdAsync(result.ConsultantId);
            result.UserFullName = user?.FullName ?? "Unknown";
            result.ConsultantFullName = consultant?.User?.FullName ?? "Unknown";
            result.EndTime = appointment.DateTime.AddHours(1);
            return result;
        }

        public async Task CreateSlotAsync(int userId, CreateSlotRequestDto request)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (user.RoleId != 5) // Chỉ Consultant (RoleId = 5) được tạo slot
                throw new UnauthorizedAccessException("Only consultants (RoleId = 5) can create slots");

            var consultant = await _unitOfWork.Consultant.FindAsync(c => c.UserId == userId);
            if (!consultant.Any())
                throw new KeyNotFoundException("Consultant not found");

            var consultantId = consultant.First().Id;

            if (request.StartTime < DateTime.Now)
                throw new ArgumentException("Start time must be in the future");

            var conflict = await _unitOfWork.Appointment.FindAsync(a =>
                a.ConsultantId == consultantId &&
                a.DateTime == request.StartTime &&
                a.Status != "Cancelled");

            if (conflict.Any())
                throw new InvalidOperationException("Slot already exists");

            var slot = new Appointment
            {
                ConsultantId = consultantId,
                DateTime = request.StartTime,
                Status = "Available",
                UserId = null,
                Note = "Slot created by consultant"
            };

            await _unitOfWork.Appointment.AddAsync(slot);
            await _unitOfWork.CommitAsync();
        }

        public async Task<RevisitAppointmentResponseDto> ScheduleRevisitAsync(int consultantUserId, int previousAppointmentId, DateTime newTime)
        {

            var consultant = await _unitOfWork.Consultant
     .Query()
     .Include(c => c.User)
     .FirstOrDefaultAsync(c => c.UserId == consultantUserId);

            if (consultant == null)
                throw new UnauthorizedAccessException("Consultant not found");



            var previousAppointment = await _unitOfWork.Appointment.GetByIdAsync(previousAppointmentId);
            if (previousAppointment == null || previousAppointment.ConsultantId != consultant.Id)
                throw new ArgumentException("Invalid previous appointment");

            if (previousAppointment.Status != "Completed")
                throw new InvalidOperationException("Only completed appointments can have revisits");


            var conflict = await _unitOfWork.Appointment.FindAsync(a =>
                a.ConsultantId == consultant.Id &&
                a.DateTime == newTime &&
                a.Status != "Cancelled");

            if (conflict.Any())
                throw new InvalidOperationException("A slot already exists at the given time");


            var revisitSlot = new Appointment
            {
                ConsultantId = consultant.Id,
                UserId = previousAppointment.UserId,
                DateTime = newTime,
                Status = "Pending",
                Note = "Revisit appointment",
                IsRevisit = true,
                ParentAppointmentId = previousAppointment.Id
            };

            await _unitOfWork.Appointment.AddAsync(revisitSlot);
            await _unitOfWork.CommitAsync();

            var user = await _unitOfWork.User.GetByIdAsync(revisitSlot.UserId ?? 0);
            var response = _mapper.Map<RevisitAppointmentResponseDto>(revisitSlot);
            response.UserFullName = user?.FullName ?? "Unknown";
            response.ConsultantFullName = consultant.User?.FullName ?? "Unknown";
            response.StartTime = revisitSlot.DateTime;
            response.EndTime = revisitSlot.DateTime.AddHours(1);
            return response;
        }

    }
}
