# ASP.NET API Project

## Overview
This project is a RESTful API built with ASP.NET Core, following Clean Architecture principles. It provides authentication, role-based access control, and user profile management, leveraging Redis for refresh token storage.

## Features
- **Authentication & Authorization**: Implements JWT-based authentication with refresh token support.
- **Role-Based Access Control**: Manages user permissions based on roles.
- **User Management**: Allows user registration, profile updates, and password management.
- **Redis Integration**: Stores refresh tokens in Redis for enhanced performance.
- **MediatR Integration**: Implements CQRS pattern for better separation of concerns.

## Technologies Used
- **ASP.NET Core**
- **Entity Framework Core**
- **Redis**
- **MediatR**
- **Docker**
- **MySQL**
- **Swagger**

## Project Structure
```
├── API (Presentation Layer)
├── Application (Business Logic Layer)
├── Infrastructure (Data & External Services Layer)
├── Domain (Entities & Core Business Logic)
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker

### Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/william-le1004/dtp-backend.git
   cd dtp-backend
   ```
2. Set up environment variables for local development (in `secrets.json`).
   2.1. In Api project, right click.
   2.2. Choose Tools.
   2.3. Choose .NET User Secrets.
3. Create .env file in folder have dtp-backend.sln.
4. Start the API:
   ```sh
   dotnet run --project API
   ```
5. Open Swagger at `http://localhost:5000/swagger`.

### Docker Setup
To run the API with Redis and MySQL:
```sh
docker-compose up -d
```

## API Endpoints
| Method | Endpoint | Description |
|--------|---------|-------------|
| POST   | /api/authentication/register | User registration |
| POST   | /api/authentication/login | User login |
| POST   | /api/authentication/refresh | Refresh access token |
| POST   | /api/authentication/logout | User logout |
| GET    | /api/users/profile | Get user profile |
| PUT    | /api/users/profile | Update user profile |

## Contributing
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a Pull Request.

## License
This project is licensed under the MIT License.

## Contact
For questions or support, contact [https://github.com/dokkazy] - [https://github.com/kh0abug] - [https://github.com/wnosphan] - [https://github.com/ya3k] - [https://github.com/william-le1004].

