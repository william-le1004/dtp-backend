# ASP.NET API Project

## Overview

This project is a RESTful API built with ASP.NET Core, following Clean Architecture principles. It provides authentication, role-based access control, and user profile management, leveraging Redis for refresh token storage and RabbitMQ for message queuing.

## Features

- **Authentication & Authorization**: Implements JWT-based authentication with refresh token support.
- **Role-Based Access Control**: Manages user permissions based on roles.
- **User Management**: Allows user registration, profile updates, and password management.
- **Redis Integration**: Stores refresh tokens in Redis for enhanced performance.
- **MediatR Integration**: Implements CQRS pattern for better separation of concerns.
- **RabbitMQ Integration**: Message queuing for asynchronous operations.
- **Output Caching**: Implements caching policies for improved performance.
- **Firebase Integration**: Firebase Admin SDK integration for additional services.

## Technologies Used

- **ASP.NET Core 8**
- **Entity Framework Core**
- **Redis**
- **RabbitMQ**
- **MediatR**
- **Docker**
- **MySQL 8.0**
- **Swagger**
- **Firebase Admin SDK**

## Project Structure

```
├── Api/                           # Presentation Layer
│   ├── Controllers/              # API Controllers
│   ├── Filters/                  # Action Filters
│   ├── Middlewares/             # Custom Middlewares
│   ├── OutputCachingPolicy/     # Caching Policies
│   └── Extensions/              # API-specific Extensions
├── Application/                  # Business Logic Layer
│   ├── Common/                  # Shared Application Logic
│   ├── Features/               # CQRS Commands and Queries
│   └── Interfaces/             # Application Interfaces
├── Infrastructure/              # Data & External Services Layer
│   ├── Persistence/           # Database Context & Migrations
│   ├── Services/              # External Service Implementations
│   └── Identity/              # Identity & Authentication
└── Domain/                     # Core Business Logic
    ├── Entities/              # Domain Entities
    ├── Enums/                # Domain Enums
    └── Interfaces/           # Domain Interfaces
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- MySQL 8.0 (if running locally)
- Redis (if running locally)
- RabbitMQ (if running locally)

### Environment Setup

1. Create a `.env` file in the root directory with the following variables:

```env
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_DATABASE=your_database_name
REDIS_PASSWORD=your_redis_password
RABBITMQ_USER=your_rabbitmq_user
RABBITMQ_PASSWORD=your_rabbitmq_password
```

### Running with Docker

1. **Clone the repository:**

   ```sh
   git clone https://github.com/william-le1004/dtp-backend.git
   cd dtp-backend
   ```

2. **Start the services:**

   ```sh
   docker-compose up -d
   ```

   This will start:

   - MySQL on port 3306
   - Redis on port 6379
   - RabbitMQ on port 5672 (Management UI on 15672)

3. **Run the API:**

   ```sh
   dotnet run --project Api
   ```

4. **Access Swagger UI** at `http://localhost:5000/swagger`

### Running Locally

1. **Set up environment variables:**

   - In the `Api` project, right-click and select **Manage User Secrets**
   - Add your configuration values

2. **Update connection strings** in `appsettings.json`:

   - Update MySQL connection string
   - Update Redis connection string
   - Update RabbitMQ connection details

3. **Run the API:**
   ```sh
   dotnet run --project Api
   ```

## API Documentation

The API documentation is available through Swagger UI at `http://localhost:5000/swagger` when the application is running.

### Authentication Endpoints

| Method | Endpoint                       | Description          |
| ------ | ------------------------------ | -------------------- |
| `POST` | `/api/authentication/register` | User registration    |
| `POST` | `/api/authentication/login`    | User login           |
| `POST` | `/api/authentication/refresh`  | Refresh access token |
| `POST` | `/api/authentication/logout`   | User logout          |

### User Management

| Method | Endpoint             | Description         |
| ------ | -------------------- | ------------------- |
| `GET`  | `/api/users/profile` | Get user profile    |
| `PUT`  | `/api/users/profile` | Update user profile |

## Development

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Write unit tests for new features

### Git Workflow

1. Create a feature branch from `main`
2. Make your changes
3. Write/update tests
4. Submit a pull request

## Contributing

1. **Fork the repository**
2. **Create a feature branch**:
   ```sh
   git checkout -b feature-branch
   ```
3. **Commit your changes**:
   ```sh
   git commit -m "Add new feature"
   ```
4. **Push to the branch**:
   ```sh
   git push origin feature-branch
   ```
5. **Open a Pull Request**

## License

This project is licensed under the **MIT License**.

## Contact

For questions or support, contact:

- [dokkazy](https://github.com/dokkazy)
- [kh0abug](https://github.com/kh0abug)
- [wnosphan](https://github.com/wnosphan)
- [ya3k](https://github.com/ya3k)
- [william-le1004](https://github.com/william-le1004)
