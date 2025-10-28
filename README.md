#  Bus Ticket Reservation System  
A full-stack web application built using **.NET Core (C#)** and **Angular**, following **Clean Architecture** and **Domain-Driven Design (DDD)** principles.

---

##  Objective  
Design and develop a scalable and maintainable Bus Ticket Reservation System that includes:

- Clean business logic implementation  
- Layered architecture with proper separation  
- RESTful APIs with Angular UI integration  
- Database-driven seat booking  
- Unit test coverage for core use cases  

---

##  Technologies Used  

###  Backend
- .NET 9 (C#)
- Entity Framework Core (PostgreSQL)
- Clean Architecture + Domain Driven Design


###  Frontend
- Angular (Latest Stable Version)
- TypeScript
- Bootstrap / TailwindCSS (Optional)

---

##  Architecture Overview  

Project structure follows **Clean Architecture**:
--src
- Domain → Entities, Value Objects, Domain Services
- Application → Business logic, Application Services, Use cases
-Application.Contracts → Interfaces, DTOs
-Infrastructure → EF Core, Repository Implementations
- WebApi → REST API Controllers / Endpoints
-ClientApp → Angular UI


✅ No direct DbContext access outside Infrastructure  
✅ Strong encapsulation & boundaries  

---

##  System Description  

A simple Bus Ticket Reservation System enabling users to:

| Feature | Description |
|--------|-------------|
| 🔍 Search Buses | By From, To & Journey Date |
| 🪑 View Seats | Live seat layout with statuses |
| 🎫 Book Tickets | Passenger info + seat reservation |
---
<img width="1366" height="768" alt="font-page" src="https://github.com/user-attachments/assets/21752049-c76a-41f6-82db-4967cf1a605f" />
<img width="1366" height="768" alt="search-page" src="https://github.com/user-attachments/assets/0711423a-2e54-4189-825f-dd40ba29e78c" />
<img width="1366" height="768" alt="seat-layout" src="https://github.com/user-attachments/assets/1eedee64-d1e8-4987-975f-90e625d23cfb" />
<img width="1366" height="768" alt="successfully-booked" src="https://github.com/user-attachments/assets/9c201f6d-2f3d-4964-ba76-9679fadb8e00" />


---

## ✅ Functional Requirements  

###  Search Available Buses  

Users search by:
- From
- To
- Journey Date

Results show:
- Company Name  
- Bus Name  
- Start Time  
- Arrival Time  
- Seats Left  
- Price  

<img width="1366" height="768" alt="api-search-date" src="https://github.com/user-attachments/assets/6d424f5c-871b-40d1-95f0-c7d4d034432a" />

<img width="1366" height="768" alt="api-seatplan" src="https://github.com/user-attachments/assets/af893445-ac08-4001-9521-edc442b6e708" />

<img width="1366" height="768" alt="book-api" src="https://github.com/user-attachments/assets/980b3e2d-0959-4675-8af2-122dfbee904a" />

<img width="1366" height="768" alt="search-witout-date" src="https://github.com/user-attachments/assets/95df8af3-040b-4052-af03-2ce47f6f103f" />


