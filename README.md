# 🚍 SwiftBus — Full Stack Bus Ticket Booking System

SwiftBus is a modern **full-stack bus ticket booking system** built using **.NET 10 Clean Architecture** and **Angular 20**. The system is designed to simulate a real-world bus reservation platform with secure authentication, seat booking, ticket management, and administrative control.

This project demonstrates enterprise-level architecture, scalable backend design, modern frontend practices, and secure authentication workflows.

---

#  Project Overview

SwiftBus allows users to search bus routes, view schedules, select seats, and book tickets through an interactive interface. Admin users can manage buses, routes, schedules, and bookings.

The system follows **Clean Architecture principles**, ensuring maintainability, modularity, and separation of concerns.

---
#  Screenshots

##  Home Page

<img width="1258" height="628" alt="image" src="https://github.com/user-attachments/assets/4ae23cf5-e6b7-49dc-bcf5-0ddce8d90fb6" />

<img width="1256" height="622" alt="image" src="https://github.com/user-attachments/assets/5f2b4c82-8127-4f7f-aeee-b1b4b4d9ee51" />
<img width="1157" height="628" alt="image" src="https://github.com/user-attachments/assets/0ac68ff0-3316-43d0-8a17-5cbac476308e" />

<img width="1262" height="632" alt="image" src="https://github.com/user-attachments/assets/762cf881-a424-4f9d-9489-a88cd26d6eb9" />


---

##  Login Page

<img width="1200" height="592" alt="image" src="https://github.com/user-attachments/assets/6d62a90d-fbe1-4f49-baa2-2976b4fe57d9" />
<img width="1172" height="623" alt="image" src="https://github.com/user-attachments/assets/aaf6eb29-c6ca-4d8e-a860-9adef1e56e88" />
<img width="1229" height="617" alt="image" src="https://github.com/user-attachments/assets/9ccf0210-1dbc-473b-9a15-5a0e110a003c" />


---

##  Bus Search

<img width="1208" height="534" alt="image" src="https://github.com/user-attachments/assets/0dfa7cf6-6507-4525-a747-fcab0170c8b2" />


---

##  Seat Selection

<img width="1092" height="636" alt="image" src="https://github.com/user-attachments/assets/f5814671-1c98-4e5b-a2e0-f5a54e7901bb" />


---

##  My Bookings

<img width="1104" height="521" alt="image" src="https://github.com/user-attachments/assets/661a6d3f-9119-4b9e-8aa8-5bdc920a7221" />


---

##  Admin Dashboard

<img width="1360" height="632" alt="image" src="https://github.com/user-attachments/assets/fc8ffc52-825a-4ae9-905a-090a8b9b00eb" />

<img width="1361" height="615" alt="image" src="https://github.com/user-attachments/assets/f99b8e9e-17b3-4669-8c08-28e3b14834e4" />
<img width="1363" height="599" alt="image" src="https://github.com/user-attachments/assets/9f41c1d2-4f54-4d10-9be2-a7e5204b2b8d" />
<img width="1362" height="603" alt="image" src="https://github.com/user-attachments/assets/c48567cf-4e19-4a70-84cc-2b0e004bea81" />


---

#  System Architecture

The backend follows **Clean Architecture**, structured into four layers:

* **WebAPI Layer (Presentation Layer)** — Handles HTTP requests and responses
* **Application Layer (Business Logic)** — Contains services that implement core functionality
* **Domain Layer (Core Entities)** — Contains business entities and interfaces
* **Infrastructure Layer (Database & External Services)** — Implements repositories and database logic

Dependencies flow inward only, meaning outer layers depend on inner layers, but inner layers do not depend on outer ones.

---

#  Technology Stack

## Backend

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* Clean Architecture
* Repository Pattern
* Unit of Work Pattern
* JWT Authentication
* BCrypt Password Hashing
* Background Services
* Swagger / OpenAPI

## Frontend

* Angular 20
* Standalone Components
* Angular Signals
* Bootstrap 5
* Lazy Loading Routing
* JWT Interceptor
* Route Guards

## Database

* SQL Server
* EF Core Code-First Migrations

---

#  Backend Architecture Details

## Domain Layer

The Domain layer contains core business entities such as:

* User
* Bus
* Route
* BusSchedule
* Seat
* SeatLock
* Ticket
* Passenger
* RefreshToken

All entities inherit from a base entity containing:

* Id
* CreatedAt
* UpdatedAt
* IsDeleted (Soft Delete Support)

This layer also defines key interfaces such as:

* IRepository<T>
* IUnitOfWork

No external dependencies exist in this layer.

---

## Application Layer

The Application layer contains business logic implemented through services such as:

* AuthService
* BookingService
* AdminBusService
* AdminScheduleService
* SeatAvailabilityService

Responsibilities include:

* Authentication and authorization
* Booking validation
* Seat availability checking
* Ticket generation
* Transaction management

---

## Infrastructure Layer

The Infrastructure layer implements domain interfaces and connects the application to the database.

Key components include:

* GenericRepository<T>
* UnitOfWork
* ApplicationDbContext

### Background Service

**SeatLockCleanupService** runs every 60 seconds to remove expired seat locks and prevent stale reservations.

---

## WebAPI Layer

The WebAPI layer exposes REST endpoints.

Key features include:

* JWT authentication
* Role-based authorization
* Swagger/OpenAPI documentation
* Secure admin endpoints

Admin endpoints are protected using role-based authorization, ensuring only authorized users can perform administrative tasks.

---

#  Frontend Architecture

The frontend is built using **Angular 20 standalone components**, removing the need for NgModules and improving modularity.

Key frontend features include:

* Lazy-loaded routes
* JWT interceptor
* Admin dashboard layout
* Responsive UI using Bootstrap
* Modern state management using Angular Signals

---

## State Management

Angular Signals are used for state management:

* `signal()` for reactive state
* `computed()` for derived values

This reduces complexity compared to traditional RxJS-based state handling.

---

## Route Guards

The application includes route protection mechanisms:

* **authGuard** — Allows only authenticated users
* **adminGuard** — Allows only admin users

---

## JWT Interceptor

The JWT interceptor automatically:

* Attaches tokens to requests
* Handles token expiration
* Refreshes tokens
* Retries failed requests

---

#  Booking Flow (End-to-End)

The booking workflow follows a secure transaction-based process:

1. User selects a seat and enters passenger details
2. Booking request is sent with JWT authentication
3. Backend extracts the authenticated user ID
4. A transaction begins
5. Seat availability is verified
6. Ticket is generated
7. Seat status is updated
8. Transaction is committed
9. Booking confirmation is returned
10. User is redirected to the booking history page

This ensures data integrity and prevents duplicate bookings.

---

#  Authentication & Authorization

The system uses secure token-based authentication.

Authentication features include:

* JWT Access Tokens
* Refresh Tokens
* Role-Based Authorization

Supported roles:

* User
* Admin

---

#  Key Features

* User Registration & Login
* Secure JWT Authentication
* Role-Based Authorization
* Bus Management System
* Route Management
* Schedule Management
* Interactive Seat Selection
* Ticket Generation System
* Seat Lock Mechanism
* Background Cleanup Service
* Refresh Token System
* Lazy-Loaded Angular Interface
* Automatic Token Refresh
* Robust DateTime Handling
* Transaction-Based Booking

---

#  Admin Features

Admin users can:

* Add and manage buses
* Create and update routes
* Configure schedules
* Monitor bookings
* Manage seat availability

---

#  User Features

Users can:

* Register and login
* Search bus routes
* View available schedules
* Select seats visually
* Book tickets securely
* View booking history


---


#  Testing Recommendations

Suggested testing scenarios:

* Authentication testing
* Booking validation
* Seat availability verification
* API endpoint testing

---

#  Future Improvements

Planned enhancements include:

* Payment Gateway Integration
* Email Ticket Notifications
* Real-Time Seat Updates (SignalR)
* Mobile Application Support
* Analytics Dashboard

---

# Author
**Masum Ahmed**
-Department of Information and Communication Technology
-Mawlana Bhashani Science and Technology University
-Bangladesh

---



