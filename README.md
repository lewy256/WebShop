# E-commerce web app
[![Build Status](https://dev.azure.com/lewy256/WebShop/_apis/build/status%2FRun%20Basket%20Api%20tests?branchName=master)](https://dev.azure.com/lewy256/WebShop/_build/latest?definitionId=36&branchName=master)

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
#### Continuous integration:
* Azure DevOps

### Run this project locally
```
docker compose up
```
#### Or on Azure
![Deploy to Azure](https://aka.ms/deploytoazurebutton)

