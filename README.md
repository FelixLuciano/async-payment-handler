[Raul Ikeda Gomes da Silva](http://lattes.cnpq.br/5935139039430914). Functional Programming. [Insper](https://github.com/Insper), 2025.

# Async Payment Handler

## Features
Receive and process payment webhooks securely and modular MVC architecture with clear separation of concerns.

### Project Structure
- **PaymentServiceConnector**: C# library for connecting to the external payment service;
- **PaymentWebhookController**: F# library for handling the webhook business logic;
- **PaymentWebhookModel**: F# models representing webhook event schema;
- **PaymentWebhookView**: C# ASP.NET Core project hosting the webhook endpoint and authentication;
- **python**: Python script with some integration tests.

## Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0)
- [Python 3.10](https://www.python.org/downloads/release/python-3100/)

### Running the server

Start the web application:
```sh
dotnet run --project PaymentWebhookView --urls=http://localhost:5000
```

Now the application is currently running at [http://localhost:5000]. For more about the API, access the Swagger application at [/swagger](http://localhost:5000/swagger).

### Testing

Set up the testing environment with Python:
```sh
python3.10 -m venv .venv
source .venv/bin/activate
pip install -r python/requirements.txt
```

Then run the testbench:
```sh
python3 python/test_webhook.py
```
### Configuration

The `appsettings.Development.json` and `appsettings.json` files at the `PaymentWebhookView` module, configures the application behaviour:
- `Webhook.SecretToken`: Webhook authentication token received from `X-Webhook-Token` requests headers;
- `PaymentService. BaseUrl`: Base URL to the integrating payment service, the same as in the testbench.

## License
This project is MIT Licensed!
