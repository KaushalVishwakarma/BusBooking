-- ============================================================
-- Online Bus Ticket Booking System - Database Schema
-- Database: SQL Server
-- ============================================================

CREATE DATABASE BusBookingDB;
GO

USE BusBookingDB;
GO

-- ============================================================
-- 1. USERS
-- ============================================================
CREATE TABLE Users (
    UserId      INT IDENTITY(1,1) PRIMARY KEY,
    FullName    NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(150)  NOT NULL UNIQUE,
    PhoneNumber NVARCHAR(15)   NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role        NVARCHAR(20)   NOT NULL DEFAULT 'Customer',  -- Customer | Admin
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- 2. BUSES
-- ============================================================
CREATE TABLE Buses (
    BusId        INT IDENTITY(1,1) PRIMARY KEY,
    BusNumber    NVARCHAR(20)  NOT NULL UNIQUE,
    BusName      NVARCHAR(100) NOT NULL,
    BusType      NVARCHAR(50)  NOT NULL,  -- Sleeper | Semi-Sleeper | Seater
    TotalSeats   INT           NOT NULL,
    IsActive     BIT           NOT NULL DEFAULT 1
);

-- ============================================================
-- 3. ROUTES
-- ============================================================
CREATE TABLE Routes (
    RouteId     INT IDENTITY(1,1) PRIMARY KEY,
    Origin      NVARCHAR(100) NOT NULL,
    Destination NVARCHAR(100) NOT NULL,
    DistanceKm  DECIMAL(8,2)  NOT NULL,
    IsActive    BIT           NOT NULL DEFAULT 1
);

-- ============================================================
-- 4. SCHEDULES
-- ============================================================
CREATE TABLE Schedules (
    ScheduleId      INT IDENTITY(1,1) PRIMARY KEY,
    BusId           INT            NOT NULL REFERENCES Buses(BusId),
    RouteId         INT            NOT NULL REFERENCES Routes(RouteId),
    DepartureTime   DATETIME2      NOT NULL,
    ArrivalTime     DATETIME2      NOT NULL,
    PricePerSeat    DECIMAL(10,2)  NOT NULL,
    Status          NVARCHAR(20)   NOT NULL DEFAULT 'Scheduled',  -- Scheduled | Cancelled | Completed
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- 5. SEATS
-- ============================================================
CREATE TABLE Seats (
    SeatId      INT IDENTITY(1,1) PRIMARY KEY,
    BusId       INT          NOT NULL REFERENCES Buses(BusId),
    SeatNumber  NVARCHAR(10) NOT NULL,   -- e.g. A1, A2, B1
    SeatType    NVARCHAR(20) NOT NULL,   -- Window | Aisle | Middle
    Deck        NVARCHAR(10) NOT NULL DEFAULT 'Lower',  -- Lower | Upper
    CONSTRAINT UQ_Bus_Seat UNIQUE (BusId, SeatNumber)
);

-- ============================================================
-- 6. BOOKINGS
-- ============================================================
CREATE TABLE Bookings (
    BookingId       INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT            NOT NULL REFERENCES Users(UserId),
    ScheduleId      INT            NOT NULL REFERENCES Schedules(ScheduleId),
    BookingDate     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
    TotalAmount     DECIMAL(10,2)  NOT NULL,
    BookingStatus   NVARCHAR(20)   NOT NULL DEFAULT 'Pending',  -- Pending | Confirmed | Cancelled
    PNRNumber       NVARCHAR(20)   NOT NULL UNIQUE
);

-- ============================================================
-- 7. BOOKING SEATS  (which seats are booked per booking)
-- ============================================================
CREATE TABLE BookingSeats (
    BookingSeatId   INT IDENTITY(1,1) PRIMARY KEY,
    BookingId       INT          NOT NULL REFERENCES Bookings(BookingId),
    SeatId          INT          NOT NULL REFERENCES Seats(SeatId),
    PassengerName   NVARCHAR(100) NOT NULL,
    PassengerAge    INT           NOT NULL,
    PassengerGender NVARCHAR(10)  NOT NULL,  -- Male | Female | Other
    CONSTRAINT UQ_Booking_Seat UNIQUE (BookingId, SeatId)
);

-- ============================================================
-- 8. PAYMENTS
-- ============================================================
CREATE TABLE Payments (
    PaymentId           INT IDENTITY(1,1) PRIMARY KEY,
    BookingId           INT            NOT NULL REFERENCES Bookings(BookingId),
    Amount              DECIMAL(10,2)  NOT NULL,
    PaymentMethod       NVARCHAR(50)   NOT NULL,  -- Stripe | Razorpay | Card | UPI
    TransactionId       NVARCHAR(200)  NOT NULL UNIQUE,
    PaymentStatus       NVARCHAR(20)   NOT NULL DEFAULT 'Pending',  -- Pending | Success | Failed | Refunded
    PaymentDate         DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);

-- ============================================================
-- INDEXES for performance
-- ============================================================
CREATE INDEX IX_Schedules_RouteId       ON Schedules(RouteId);
CREATE INDEX IX_Schedules_DepartureTime ON Schedules(DepartureTime);
CREATE INDEX IX_Bookings_UserId         ON Bookings(UserId);
CREATE INDEX IX_Bookings_ScheduleId     ON Bookings(ScheduleId);
CREATE INDEX IX_BookingSeats_SeatId     ON BookingSeats(SeatId);
CREATE INDEX IX_Payments_BookingId      ON Payments(BookingId);
