# PostRedisCRUD
🛠️ Project Description: PostgreSQL + Redis CRUD API with Dapper

📄 This documentation was written with the help of AI (ChatGPT by OpenAI).

This ASP.NET Core 9 Web API project demonstrates how to build a clean and efficient CRUD API using:

- PostgreSQL as the relational database
- Redis for high-performance caching
- Dapper as a lightweight data access layer

Key Technologies:
- ASP.NET Core 9 – Fast, modern API framework
- Dapper – Lightweight ORM for performant SQL operations
- PostgreSQL – Open-source relational database
- Redis (StackExchange.Redis) – Distributed in-memory cache
- Swagger / OpenAPI – Auto-generated interactive API docs
- ILogger – Built-in logging support

Features:
- Full CRUD operations on Product entity
- Redis caching for GET endpoints with 30-second TTL
- Auto-creation of the PostgreSQL database if not present
- Smart key structure for Redis (product:{id} and products)
- Fully testable with Swagger UI
- Organized and modular code structure using Repository pattern

API Overview:
ProductController (via /api/Product)

Method  | Endpoint                        | Description
--------|----------------------------------|-------------------------
GET     | /GetListOfProducts              | Fetch all products
GET     | /GetProductById/{id}            | Get product by ID
POST    | /AddNewProduct                  | Add a new product
PUT     | /UpdateProduct                  | Update a product
DELETE  | /DeleteProduct/{id}             | Delete product by ID

Use Redis for fast reads and automatic cache updates.

How to Run:

Prerequisites:
- .NET 9 SDK
- PostgreSQL (default port 5433 used)
- Redis

Configuration:
In appsettings.json, set your connection string:
Host=localhost;Port=5433;Database=CrudProject;Username=postgres;Password=1234

Run the app:
dotnet run

Swagger UI available at: https://localhost:7003/swagger

Project Structure:
PostRedisCRUD/
- Controllers/      API endpoints
- Entity/           Product model
- Repositories/     Business & DB logic
- Data/             DB migration helper
- appsettings.json  Configurations
- Program.cs        Entry point

Use Cases:
- Product Management APIs
- Optimized read-heavy APIs with Redis cache
- Lightweight microservice-style architecture using Dapper
- API testing and documentation with Swagger

Future Enhancements:
- Add Docker + Docker Compose support
- Introduce JWT-based authentication
- Add integration and unit tests
- Implement cache eviction strategy on Update / Delete

License:
MIT License

This project and its documentation were created as a learning exercise.
Parts of the documentation and structure were generated using AI (ChatGPT by OpenAI).
