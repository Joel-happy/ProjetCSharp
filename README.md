## Table of Contents

- [Introduction](#introduction)
- [Trello](#trello)
- [Project Structure Overview](#project-structure-overview)
- [Usage](#usage)
- [High Level Flow](#high-level-flow)
- [About](#about)

##  Introduction
The goal of the project is to create an API using C# that will be deployed on a Linux Apache server. Additionally, the database for this server will be hosted on a Linux MySql server to establish a sturdy and efficient architecture. The API main purpose is handling CRUD operations for **Accounts** and **Products**.

## Trello
**Trello** was employed as our project management tools to efficiently organize and track the various tasks associated with out project.

Link → *https://trello.com/b/IcqIE588/projet-c*

## Project Structure Overview

### • Controllers
→ Handle incoming HTTP requests <br>
→ Extracts data from requests (query parameters, request bodies) <br>
→ Delegate task to services based on the type of request <br>
→ Generate and return HTTP responses (status codes, response bodies)

### • Models
→ Define data structure

### • Services
→ Implement business logic / rules <br>
→ Interact with data access layer to isolate CRUD operation on the database

### • DataAccess
→ Interaction with actual database <br>
→ Performs operations such as CRUD

## Usage
**(1)** Choose your preferred tool to test the API. You can choose between a web browser or an app like **Postman** to test the endpoints. <br>

**(2)** Access the wanted endpoint depending on the action that you want to do. Every request starts with : **http://localhost:8080/** <br>
From here, refer to **C#_API_DOC.pdf** for all available endpoints.

**(3)** Observe the response. This could be an array of **Accounts** or perhaps a message indicating that a product has been successfully created. The response can also contain a message indicating the corresponding error with the appropriate status code attached to it.

## High-level Flow
### • Incoming HTTP Request
API receives a HTTP request from the client. This could be a request to create a new user, update user information, delete a user or retrieve all users.

### • Controller Handling
The request is routed to the appropriate controller based on the endpoint and HTTP method <br> <br> The endpoint is responsible to identify the resource the client wants to interact with <br>
→ http://localhost:8080/users : acts on the **users** <br>
→ http://localhost:8080/products : acts on the **products** <br>

The HTTP method specifies the action performed on that resource <br>
→ **GET** http://localhost:8080/accounts : retrieve a list of users <br>
→ **POST** http://localhost:8080/accounts : create a new user <br>
→ **PUT** http://localhost:8080/products/3 : update product with ID 3 <br>
→ **DELETE** http://localhost:8080/products/23  : delete user with ID 23 <br>

The controller extracts all relevant data from the request such as query parameters or the request body

### • Service Interaction
The controller delegates the processing of the request to the appropriate service. The service implements all business logic needed to handle the specific CRUD operation and interacts directly with the database.

### • Data Access Layer (Repository)
The repository is responsible for database operations such as CRUD.

### • Database Interaction
The service interacts with the actual database to perform CRUD operations.

### • Response Generation
The service receives the result from the data access layer and prepares a HTTP response. The response may include the data requested (a list of users) or a success / failue status (creating a user / deleting a user).

### • HTTP Response
The controller sends the HTTP response back to the client. The response may include the status code (2xx, 4xx, 5xx), headers or a response body if needed.

## About
• While the UI does indeed work, it is not linked to the actual API and database.