# Project Guidelines

## Libraries and frameworks

### Database access
For database operations, use [*EntityFramework core*](https://learn.microsoft.com/en-us/ef/core/) *LINQ to Entities* functionality 
during development and testing of the project components. When necessary
after development phase, switch to using raw SQL statements inside EntityFramework
for improved performance.

### Logging
For logging purposes, the [*Serilog*](https://serilog.net/) library is to be used.

### JSON operations
For serialization, deserialization and manipulation of JSON the [*System.Text.Json*](https://www.nuget.org/packages/System.Text.Json) is to be used 

## Code structure

### Controllers
Each controller represents a single resource

## Architecture

The [REST-full](https://en.wikipedia.org/wiki/REST) API scheme will be used across entire application with some exceptions

## Routing
Routing is to be all lower case, and all the route parameters have to be lower case too