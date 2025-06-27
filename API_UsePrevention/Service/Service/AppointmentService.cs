using AutoMapper;
using Common.helper;
using Dto.Request;
using Dto.Response;
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
                var consultants = await _unitOfWork.Consultant.FindAsync(c => true);
                return _mapper.Map<IEnumerable<ConsultantResponseDto>>(consultants);
            }

            public async Task<IEnumerable<AvailableSlotResponseDto>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request)
            {
                var consultant = await _unitOfWork.Consultant.GetByIdAsync(request.ConsultantId);
                if (consultant == null)
                    throw new KeyNotFoundException("Consultant not found");

                var slots = await _unitOfWork.Appointment.FindAsync(a =>
                    a.ConsultantId == request.ConsultantId &&
                    a.DateTime.Date == request.Date.Date &&
                    a.Status == "Available" &&
                    a.UserId == null);

                return slots.Select(a => new AvailableSlotResponseDto
                {
                    SlotId = a.Id, // Appointment.Id
                    Start = a.DateTime,
                    End = a.DateTime.AddHours(1) // Mỗi slot kéo dài 1 tiếng
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
                var appointments = await _unitOfWork.Appointment.FindAsync(a => a.UserId == userId && a.Status != "Available");
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

                if (appointment.Status != "Pending")
                    throw new InvalidOperationException("Only pending appointments can be cancelled");

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
                var user = await _unitOfWork.User.GetByIdAsync(result.UserId?? 0);
                var consultant = await _unitOfWork.Consultant.GetByIdAsync(result.ConsultantId);
                result.UserFullName = user?.FullName ?? "Unknown";
                result.ConsultantFullName = consultant?.User?.FullName ?? "Unknown";
                result.EndTime = appointment.DateTime.AddHours(1);
                return result;
            }

            public async Task CreateSlotAsync(int userId, CreateSlotRequestDto request)
            {
                var consultant = await _unitOfWork.Consultant.FindAsync(c => c.UserId == userId);
                if (!consultant.Any())
                    throw new UnauthorizedAccessException("Only consultants can create slots");

                var consultantId = consultant.First().Id;

                var availableSlots = ScheduleHelper.ParseWorkSchedule(consultant.First().WorkSchedule, request.StartTime.Date);
                if (!availableSlots.Any(slot => request.StartTime >= slot.Start && request.StartTime.AddHours(1) <= slot.End))
                    throw new InvalidOperationException("Requested slot is not within consultant's work schedule");

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
        }
    }
