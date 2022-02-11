# ReactProjectsAuth

## Table of contents
- [General info](#general-info)
- [Technologies](#technologies)
- [Usage](#usage)

## General info
Project that serves as backend API for authentication & authorization and also for SignalR for React Projects (especially: [Czatter](https://github.com/ptakpiotr/czatter-ui))

## Technologies
- ASP .NET 6 Web API MVC
- SignalR
- Ef Core
- Swagger (OpenAPI)

## Usage
Download (make sure you have dotnet installed or use Dockerfile provided with the projects) and fill in the appsettings.json with proper values.
Important!! Because the Czatter UI is pointing to localhost:7222 for https make sure that either you change addresses in cloned/forked Czatter UI project or provide correct mapping with your containers.
