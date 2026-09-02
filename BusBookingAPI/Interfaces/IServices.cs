using BusBookingAPI.DTOs;

namespace BusBookingAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> Register(RegisterDto dto);
    Task<AuthResponseDto?> Login(LoginDto dto);
}

public interface IBusService
{
    Task<List<BusDto>> GetAll();
    Task<BusDto?> GetById(int id);
    Task<BusDto?> Create(CreateBusDto dto);
    Task<bool> Update(int id, CreateBusDto dto);
    Task<bool> Deactivate(int id);
    Task<List<SeatDto>> GetSeats(int busId, int scheduleId);
    Task AddSeats(int busId, List<CreateSeatDto> seats);
}

public interface IRouteService
{
    Task<List<RouteDto>> GetAll();
    Task<RouteDto?> GetById(int id);
    Task<RouteDto> Create(CreateRouteDto dto);
    Task<bool> Update(int id, CreateRouteDto dto);
    Task<bool> Deactivate(int id);
}

public interface IScheduleService
{
    Task<List<ScheduleDto>> Search(SearchScheduleDto dto);
    Task<ScheduleDto?> GetById(int id);
    Task<List<ScheduleDto>> GetAll();
    Task<ScheduleDto> Create(CreateScheduleDto dto);
    Task<bool> UpdateStatus(int id, string status);
}

public interface IBookingService
{
    Task<BookingResponseDto> Create(int userId, CreateBookingDto dto);
    Task<List<BookingResponseDto>> GetUserBookings(int userId);
    Task<BookingResponseDto?> GetByPNR(string pnr);
    Task<bool> Cancel(int bookingId, int userId);
    Task<List<BookingResponseDto>> GetAll();
}

public interface IPaymentService
{
    Task<PaymentResponseDto> InitiatePayment(CreatePaymentDto dto);
    Task<bool> ConfirmPayment(ConfirmPaymentDto dto);
    Task<bool> RefundPayment(int bookingId);
}
