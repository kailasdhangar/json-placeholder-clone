# JSONPlaceholder Clone API

A comprehensive .NET 9 Web API that clones all endpoints from [JSONPlaceholder](https://jsonplaceholder.typicode.com/) with enhanced features including security, validation, documentation, and comprehensive testing.

## Features

- Complete JSONPlaceholder API clone with all endpoints
- Entity Framework Core with in-memory database
- AutoMapper for clean object mapping
- FluentValidation for input validation
- Swagger/OpenAPI documentation
- Rate limiting and security features
- Comprehensive unit and integration tests
- Serilog logging
- Global exception handling

## API Endpoints

### Users
- GET /api/users - Get all users
- GET /api/users/{id} - Get user by ID
- POST /api/users - Create new user
- PUT /api/users/{id} - Update user
- DELETE /api/users/{id} - Delete user
- GET /api/users/{id}/posts - Get user's posts
- GET /api/users/{id}/albums - Get user's albums
- GET /api/users/{id}/todos - Get user's todos

### Posts
- GET /api/posts - Get all posts
- GET /api/posts/{id} - Get post by ID
- POST /api/posts - Create new post
- PUT /api/posts/{id} - Update post
- DELETE /api/posts/{id} - Delete post
- GET /api/posts/{id}/comments - Get post's comments
- GET /api/posts/{id}/with-comments - Get post with comments

### Comments
- GET /api/comments - Get all comments
- GET /api/comments/{id} - Get comment by ID
- POST /api/comments - Create new comment
- PUT /api/comments/{id} - Update comment
- DELETE /api/comments/{id} - Delete comment
- GET /api/comments/by-post/{postId} - Get comments by post ID

### Albums
- GET /api/albums - Get all albums
- GET /api/albums/{id} - Get album by ID
- POST /api/albums - Create new album
- PUT /api/albums/{id} - Update album
- DELETE /api/albums/{id} - Delete album
- GET /api/albums/{id}/photos - Get album's photos
- GET /api/albums/{id}/with-photos - Get album with photos

### Photos
- GET /api/photos - Get all photos
- GET /api/photos/{id} - Get photo by ID
- POST /api/photos - Create new photo
- PUT /api/photos/{id} - Update photo
- DELETE /api/photos/{id} - Delete photo
- GET /api/photos/by-album/{albumId} - Get photos by album ID

### Todos
- GET /api/todos - Get all todos
- GET /api/todos/{id} - Get todo by ID
- POST /api/todos - Create new todo
- PUT /api/todos/{id} - Update todo
- DELETE /api/todos/{id} - Delete todo
- GET /api/todos/by-user/{userId} - Get todos by user ID
- GET /api/todos/completed - Get completed todos
- GET /api/todos/pending - Get pending todos

## Getting Started

1. Clone the repository
2. Run `dotnet restore`
3. Run `dotnet run`
4. Access Swagger UI at the root URL
5. Run tests with `dotnet test`

## Technology Stack

- .NET 9
- Entity Framework Core
- AutoMapper
- FluentValidation
- Swagger/OpenAPI
- Serilog
- xUnit
- Moq
- FluentAssertions 