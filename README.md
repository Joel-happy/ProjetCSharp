# Introduction

## // --- Project Structure Overview --- \\\

### • *Controllers*
→ Handle incoming HTTP requests <br>
→ Extracts data from requests (query parameters, request bodies) <br>
→ Delegate task to services based on the type of request <br>
→ Generate and return HTTP responses (status codes, response bodies)

### • *Models*
→ Define data structure

### • *Services*
→ Implement business logic / rules
→ Interact with data access layer to isolate CRUD operation on the database

### • *DataAccess*
→ Interaction with actual database using ADO.NET
→ Performs operations such as CRUD

## // --- High-level Flow --- \\\

### • *Incoming HTTP Request*
API receives a HTTP request from the client. This could be a request to create a new user, update user information, delete a user or retrieve all users.

### • *Controller Handling*
The request is routed to the appropriate controller based on the endpoint and HTTP method <br> <br> The endpoint is responsible to identify the resource the client wants to interact with <br>
→ http://localhost:8080/users : acts on the **users** <br>
→ http://localhost:8080/products : acts on the **products** <br>

The HTTP method specifies the action performed on that resource <br>
→ **GET** http://localhost:8080/users : retrieve a list of users <br>
→ **POST** http://localhost:8080/users : create a new user <br>
→ **PUT** http://localhost:8080/products/3 : update product with ID 3 <br>
→ **DELETE** http://localhost:8080/products/23  : delete user with ID 23 <br>

The controller extracts all relevant data from the request such as query parameters or the request body

### • *Service Interaction*
The controller delegates the processing of the request to the appropriate service. The service implements all business logic needed to handle the specific CRUD operation and interacts directly with the database using ADO.NET.

### • *Data Access Layer (Repository / DAO (data access object))*
The repository is responsible for database operations such as CRUD.

### • *Database Interaction*
The service interacts with the actual database to perform CRUD operations using ADO.NET

### • *Response Generation*
The service receives the result from the data access layer and prepares a HTTP response. The response may include the data requested (a list of users) or a success / failue status (creating a user / deleting a user).

### • *HTTP Response*
The controller sends the HTTP response back to the client. The response may include the status code (2xx, 4xx, 5xx), headers or a response body if needed.
