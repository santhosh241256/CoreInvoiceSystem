# CoreInvoiceSystem

CoreInvoiceSystem is an ASP.NET Core Web API project for managing invoices. It supports creating, retrieving, paying, and processing overdue invoices. This project demonstrates the use of in-memory data storage, dependency injection, custom exception handling, and the integration of Swagger for API documentation.

## Table of Contents

- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
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
- [Docker](https://www.docker.com/products/docker-desktop) (optional, for containerization)

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
- NUnit for Unit Testing
