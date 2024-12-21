[![Build Status](https://dev.azure.com/lewy64/WebShop/_apis/build/status%2Ftests%2FRun%20Basket%20Api%20tests?branchName=master)](https://dev.azure.com/lewy64/WebShop/_build/latest?definitionId=2&branchName=master)

# E-commerce web app

This is an e-commerce web application project divided into microservices.

## Features
* Placing orders
* Browsing products and their price history
* Adding items to the wishlist
* Uploading product images
* Adding items to the basket
* Logging in and registering customers
* Post reviews for purchased products

## Technologies
#### .NET 8
* MassTransit, Serilog, Yarp, OneOf, Mapster, FluentValidation, Carter,
  Entity Framework Core, Azure Storage, Azure Key Vault, AspNetCore.HealthChecks, Mediator, Azure App Configuration
* Integration tests: xUnit, Testcontainers, Respwan, FluentAssertions, Bogus, Stryker Mutator
#### Angular:
* Angular Material
#### Databases: 
* Redis, Azure SQL Database, Cosmos DB
#### CI/CD:
* Azure DevOps

## Usage

* Docker
```
docker compose up
```
* Azure Kubernetes Service


[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Flewy256%2FWebShop%2Fmaster%2F.azure%2Finfrastructure%2Ftemplate.json)

<h3 align="center">Screenshots</h3>
<div align="center" style="display: flex; justify-content: space-between;">
  <img src=https://github.com/lewy256/WebShop/blob/master/images/1.png height=300p>
  <img src=https://github.com/lewy256/WebShop/blob/master/images/3.png height=300px>
</div>



