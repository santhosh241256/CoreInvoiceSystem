CoreInvoiceSystem:
CoreInvoiceSystem is an ASP.NET Core Web API project for managing invoices. It supports creating, retrieving, paying, and processing overdue invoices. This project demonstrates the use of in-memory data storage, dependency injection, custom exception handling, and the integration of Swagger for API documentation.

Table of Contents
  Getting Started
    Prerequisites
    Installation
    Running the Application
  API Endpoints
    Create Invoice
    Get All Invoices
    Pay Invoice
    Process Overdue Invoices
Project Structure
Testing
Technologies Used

1. Getting Started
   a. Prerequisites
      .NET SDK 6.0
       Docker (optional, for containerization)
  
   b. Installation
       Clone the repository:
          git clone https://github.com/yourusername/CoreInvoiceSystem.git
          cd CoreInvoiceSystem
       Restore the project dependencies:
          dotnet restore
   c. Running the Application
       You can run the application using the following command:
          dotnet run
By default, the API will be accessible at http://localhost:5000.

To view the Swagger UI for the API documentation, navigate to http://localhost:5000/Index.html.

2. API Endpoints
  a. Create Invoice
      URL: POST /invoices
      Request Body:
      {
        "amount": 100.0,
        "dueDate": "2024-08-05T00:00:00"
      }
      Response:
      {
        "message": "Invoice created successfully.",
        "invoiceId": 1
      }
  b. Get All Invoices
      URL: GET /invoices
      Response:
      {
        "invoiceId": 1,
        "amount": 100.0,
        "paidAmount": 0.0,
        "dueDate": "2024-08-05",
        "status": "Pending"
      }

  c. Pay Invoice
      URL: POST /invoices/{id}/pay
      Request Body:
      {
        "paymentAmount": 50.0
      }
      Response: 204 No Content
  d. Process Overdue Invoices
      URL: POST /invoices/process-overdue
      Request Body:
      {
        "lateFee": 10.0,
        "overdueDays": 5
      }
      Response: 204 No Content
3. Project Structure
      
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
      
4. Testing
The project uses NUnit for unit testing. To run the tests, use the following command:
      dotnet test

5. Technologies Used
    ASP.NET Core
    In-Memory Data Storage
    Dependency Injection
    Custom Exception Handling
    Swagger for API Documentation
    NUnit for Unit Testing
