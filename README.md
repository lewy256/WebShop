# E-commerce web app

This is an e-commerce web application project divided into microservices.

| Tests  | [![Build Status](https://dev.azure.com/lewy64/WebShop/_apis/build/status%2Ftests%2FRun%20Basket%20Api%20tests?branchName=master)](https://dev.azure.com/lewy64/WebShop/_build/latest?definitionId=2&branchName=master) |
| --------------- | --------------- |
| Deployment  | [![Build Status](https://dev.azure.com/lewy64/WebShop/_apis/build/status%2Fdeployment%2FDeploy%20WebShop%20app?branchName=master)](https://dev.azure.com/lewy64/WebShop/_build/latest?definitionId=4&branchName=master) |

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

<h3 align="center">Screenshots</h3>
<p align="center">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/1.png width="80%">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/2.png width="80%">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/3.png width="20%">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/4.png width="80%">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/5.png width="80%">
</p>
