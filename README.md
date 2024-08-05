Sure! Here's the updated README file with instructions for building and using Docker, including the `Dockerfile` and `docker-compose.yml` details.

---

# CoreInvoiceSystem

CoreInvoiceSystem is an ASP.NET Core Web API project for managing invoices. It supports creating, retrieving, paying, and processing overdue invoices. This project demonstrates the use of in-memory data storage, dependency injection, custom exception handling, and the integration of Swagger for API documentation.

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
  - [Docker](#docker)
- [API Endpoints](#api-endpoints)
  - [Create Invoice](#create-invoice)
  - [Get All Invoices](#get-all-invoices)
  - [Pay Invoice](#pay-invoice)
  - [Process Overdue Invoices](#process-overdue-invoices)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Technologies Used](#technologies-used)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

### Prerequisites

- [.NET SDK 6.0](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/products/docker-desktop)

### Installation

1. Clone the repository:

```bash
git clone https://github.com/yourusername/CoreInvoiceSystem.git
cd CoreInvoiceSystem
```

2. Restore the project dependencies:

```bash
dotnet restore
```

### Running the Application

You can run the application using the following command:

```bash
dotnet run
```

By default, the API will be accessible at `http://localhost:5000`.

To view the Swagger UI for the API documentation, navigate to `http://localhost:5000/swagger`.

### Docker

You can use Docker to containerize the application. Below are the instructions to build and run the Docker container.

#### Dockerfile

Here is the `Dockerfile` used for this project:

```dockerfile
# Use the official .NET 6 SDK image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Verify that dotnet executable is available
RUN ["dotnet", "--info"]

# List files in the working directory
RUN ["ls", "-la"]

ENTRYPOINT ["dotnet", "CoreInvoiceSystem.dll"]
```

#### docker-compose.yml

Here is the `docker-compose.yml` file used for this project:

```yaml
version: '3.8'

services:
  coreinvoicesystem:
    build: .
    ports:
      - "8080:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

#### Building and Running the Docker Container

1. Build the Docker image:

```bash
docker-compose build
```

2. Run the Docker container:

```bash
docker-compose up
```

The API will be accessible at `http://localhost:8080`.

## API Endpoints

### Create Invoice

- **URL:** `POST /invoices`
- **Request Body:**

```json
{
  "amount": 100.0,
  "dueDate": "2024-08-05T00:00:00"
}
```

- **Response:**

```json
{
  "message": "Invoice created successfully.",
  "invoiceId": 1
}
```

### Get All Invoices

- **URL:** `GET /invoices`
- **Response:**

```json
[
  {
    "invoiceId": 1,
    "amount": 100.0,
    "paidAmount": 0.0,
    "dueDate": "2024-08-05",
    "status": "Pending"
  }
]
```

### Pay Invoice

- **URL:** `POST /invoices/{id}/pay`
- **Request Body:**

```json
{
  "paymentAmount": 50.0
}
```

- **Response:** `204 No Content`

### Process Overdue Invoices

- **URL:** `POST /invoices/process-overdue`
- **Request Body:**

```json
{
  "lateFee": 10.0,
  "overdueDays": 5
}
```

- **Response:** `204 No Content`

## Project Structure

```
CoreInvoiceSystem/
│
├── Controllers/
│   └── InvoiceController.cs
│
├── Models/
│   ├── InvoiceModel.cs
│   ├── InvoiceInputModel.cs
│   ├── InvoiceResponseModel.cs
│   ├── PaymentInputModel.cs
│   ├── OverdueProcessingInputModel.cs
│   └── ErrorResponse.cs
│
├── Services/
│   ├── IInvoiceService.cs
│   └── InvoiceService.cs
│
├── Data/
│   ├── IDataStore.cs
│   └── InMemoryDataStore.cs
│
├── Exceptions/
│   ├── InvalidInputException.cs
│   ├── InvoiceNotFoundException.cs
│   └── PaymentAmountMismatchException.cs
│
├── Tests/
│   ├── InvoiceServiceTests.cs
│   └── InvoiceControllerTests.cs
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json
├── Program.cs
├── CoreInvoiceSystem.csproj
└── README.md
```

## Testing

The project uses NUnit for unit testing. To run the tests, use the following command:

```bash
dotnet test
```

## Technologies Used

- ASP.NET Core
- In-Memory Data Storage
- Dependency Injection
- Custom Exception Handling
- Swagger for API Documentation
- Docker for Containerization
- NUnit for Unit Testing

  
