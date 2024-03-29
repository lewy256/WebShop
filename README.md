# E-commerce web app
[![Build Status](https://dev.azure.com/lewy64/WebShop/_apis/build/status%2Ftests%2FRun%20Basket%20Api%20tests?branchName=master)](https://dev.azure.com/lewy64/WebShop/_build/latest?definitionId=2&branchName=master)

This is an e-commerce web application project divided into microservices.

## Technologies
Project is created with:
#### .NET 8
* MassTransit, Serilog, Yarp, OneOf, Mapster, FluentValidation, Carter,
  Entity Framework Core, Azure Storage, Azure Key Vault, AspNetCore.HealthChecks, Mediator, Azure App Configuration
* Integrations tests: xUnit, Testcontainers, Respwan, FluentAssertions, Bogus, Stryker Mutator
#### Angular:
* Angular Material
* E2E tests: Cypress
* Unit tests: Jest
#### Databases: 
* Redis, Azure SQL Database, Cosmos DB
#### CI/CD:
* Azure DevOps

### Run this project locally
```
docker compose up
```
#### Or on Azure Kubernetes Service
[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Flewy256%2FWebShop%2Fmaster%2F.azure%2Finfrastructure%2Ftemplate.json)
