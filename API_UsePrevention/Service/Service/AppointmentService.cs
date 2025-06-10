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

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ConsultantResponseDto>> GetConsultantsAsync()
        {
            var consultants = await _unitOfWork.Consultant.FindAsync(c => true);
            return consultants.Select(c => new ConsultantResponseDto
            {
                Id = c.Id,
                FullName = c.User.FullName,
                Degree = c.Degree,
                Specialty = c.Specialty
            });
        }

        public async Task<IEnumerable<AvailableSlotResponseDto>> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto request)
        {
            var consultant = await _unitOfWork.Consultant.GetByIdAsync(request.ConsultantId);
            if (consultant == null)
                throw new KeyNotFoundException("Consultant not found");

            var bookedAppointments = await _unitOfWork.Appointment.FindAsync(a =>
                a.ConsultantId == request.ConsultantId &&
                a.DateTime.Date == request.Date.Date &&
                a.Status != "Cancelled");

            var availableSlots = ScheduleHelper.ParseWorkSchedule(consultant.WorkSchedule, request.Date);
            return availableSlots
                .Where(slot => !bookedAppointments.Any(a => a.DateTime >= slot.Start && a.DateTime < slot.End))
                .Select(slot => new AvailableSlotResponseDto
                {
                    Start = slot.Start,
                    End = slot.End
                });
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(int userId, CreateAppointmentRequestDto request)
        {
            var consultant = await _unitOfWork.Consultant.GetByIdAsync(request.ConsultantId);
            if (consultant == null)
                throw new KeyNotFoundException("Consultant not found");

            var conflict = await _unitOfWork.Appointment.FindAsync(a =>
                a.ConsultantId == request.ConsultantId &&
                a.DateTime == request.DateTime &&
                a.Status != "Cancelled");

            if (conflict.Any())
                throw new InvalidOperationException("Time slot is already booked");

            var appointment = new Appointment
            {
                UserId = userId,
                ConsultantId = request.ConsultantId,
                DateTime = request.DateTime,
                Status = "Pending",
                Note = request.Note
            };

            await _unitOfWork.Appointment.AddAsync(appointment);
            await _unitOfWork.CommitAsync();

            var user = await _unitOfWork.User.GetByIdAsync(userId);
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                UserFullName = user.FullName,
                ConsultantId = appointment.ConsultantId,
                ConsultantFullName = consultant.User.FullName,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                Note = appointment.Note
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetUserAppointmentsAsync(int userId)
        {
            var appointments = await _unitOfWork.Appointment.FindAsync(a => a.UserId == userId);
            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = a.User.FullName,
                ConsultantId = a.ConsultantId,
                ConsultantFullName = a.Consultant.User.FullName,
                DateTime = a.DateTime,
                Status = a.Status,
                Note = a.Note
            });
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetConsultantAppointmentsAsync(int consultantId)
        {
            var appointments = await _unitOfWork.Appointment.FindAsync(a => a.ConsultantId == consultantId);
            return appointments.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                UserId = a.UserId,
                UserFullName = a.User.FullName,
                ConsultantId = a.ConsultantId,
                ConsultantFullName = a.Consultant.User.FullName,
                DateTime = a.DateTime,
                Status = a.Status,
                Note = a.Note
            });
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

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                UserFullName = appointment.User.FullName,
                ConsultantId = appointment.ConsultantId,
                ConsultantFullName = appointment.Consultant.User.FullName,
                DateTime = appointment.DateTime,
                Status = appointment.Status,
                Note = appointment.Note
            };
        }
    }
}
