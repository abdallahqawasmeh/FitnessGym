# Fitness Center Management System (ASP.NET Core MVC)

A full-stack web application built with **ASP.NET Core MVC** and **Entity Framework Core** to manage gym operations such as memberships, subscriptions, trainers, workout plans, and administrative reports. The system supports multiple user roles with secure authentication and role-based access control.

## Features

### Admin
- Secure login & session management
- Dashboard overview (members, subscriptions, revenue, etc.)
- Manage Members (Create / Read / Update / Delete)
- Manage Trainers (Create / Read / Update / Delete)
- Manage Membership Plans & Subscriptions
- Approve/Reject testimonials (if implemented)
- Reports (monthly/annual summaries with charts if implemented)
- PDF invoices / export (if implemented)

### Member
- Register & login
- View plans and subscribe
- Profile management
- View subscription history
- Download invoices (if implemented)
- Submit testimonials/feedback (if implemented)

### Trainer
- Login
- View assigned members (if implemented)
- Create/update workout plans for members
- Profile management

## Tech Stack

- **Backend:** ASP.NET Core MVC, C#
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Frontend:** Razor Views, HTML, CSS, Bootstrap
- **Tools:** Visual Studio, Git/GitHub

## Architecture Notes

- Clean separation between Controllers, Services (if used), Models, and Views
- Relational database design with proper entity relationships
- Role-based access control to protect admin/trainer/member functionality

## Getting Started

### Prerequisites
- .NET SDK (8 or 7)
- SQL Server (LocalDB / SQL Server Express / SQL Server)
- Visual Studio 2022 (recommended)

### Setup
1. Clone the repository:
   ```bash
   git clone https://github.com/<abdallahqawasmeh>/<FitnessGym>.git
   cd <FitnessGym>
