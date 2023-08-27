# Minimal TODO API with ASP.NET Core

This repository contains a simple minimal TODO API using ASP.NET Core and .NET 7. Minimal APIs are architected to create HTTP APIs with minimal dependencies. They are ideal for microservices and apps that want to include only the minimum files, features, and dependencies in ASP.NET Core.


## Overview

The following table lists all endpoints of our TODO API.

| METHOD | ENDPOINT | DESCRIPTION | REQUEST BODY | RESPONSE BODY |
| - | - | - | - | - |
| `GET` | `/todoitems` | Get all todo items | - | Array of `TodoItem` |
| `GET` | `/todoitems/complete` | Get completed todo items | - | Array of `TodoItem` |
| `GET` | `/todoitems/incomplete` | Get incompleted todo items | - | Array of `TodoItem` |
| `GET` | `/todoitems/{id}` | Get a todo item by ID | - | `TodoItem` |
| `POST` | `/todoitems` | Create a new todo item | `TodoItemDto` | `TodoItem` |
| `PUT` | `/todoitems/{id}` | Update an existing todo item | `TodoItemDto` | - |
| `DELETE` | `/todoitems/{id}` | Delete an existing todo item | - | - |


## Database

The TODO API uses a [Sqlite database](https://www.sqlite.org/index.html). You will find the database on Windows in the folder `C:\Users\<YOUR USERNAME>\AppData\Local` and the file is called `TodoApi.db`. If you are running the integration tests, you will find a test database in the same folder called `TodoApi-Tests.db`.


## Tests

The project contains unit tests and integration tests. You can find the corresponding projects in the [tests](/tests/) folder.


## Sample

You will find a [Postman](https://www.postman.com/downloads/) in the [samples](/samples/) folder.