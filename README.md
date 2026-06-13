# JodWai Note

## Overview
JodWai Note is a note-taking REST API built with ASP.NET Core Web API. The system manages notes with title, content, tags, and note-to-note relationships (links). Users can create, read, update, and delete notes, organize them with tags, and link related notes together.

## Tech Stack
- **Backend:** .NET Core 10.0.7
- **Frontend:** Not yet defined.
- **Database:** PostgreSQL
- **Testing Framework:** xUnit + Moq

## Prerequisites
- .NET SDK 6.0 or higher
- PostgreSQL 14 or higher
- Node.js for frontend (if applicable)

## Local Setup

### Backend
1. Clone the repository:
   ```bash
   git clone https://github.com/Phiriyadet/jodwai-note.git
   cd jodwai-note/backend
   ```
2. Update `appsettings.Development.json` with your PostgreSQL connection string.
3. Run migrations:
   ```bash
   dotnet ef database update -c AppDbContext
   ```
4. Build and run the application:
   ```bash
   dotnet build
   dotnet run
   ```

### Frontend (if applicable)
1. Navigate to the frontend directory:
   ```bash
   cd ../frontend/JodWai-Web/
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Build and serve:
   ```bash
   npm start
   ```

## Running Application
Run the application as described in the "Local Setup" section.

## Running Tests
To run all tests, use:
```bash
dotnet test
```
To run specific test project:
```bash
dotnet test backend/JodWai/src/JodWai.Tests.Unit/JodWai.Tests.Unit.csproj
```

## Database Migration
Migrations are managed in the `Migrations` folder. To apply migrations, run:
```bash
dotnet ef database update -c AppDbContext
```

## Environment Variables
Set environment variables as required in your project's configuration files.

## Folder Structure
```
JodWai.Note/
├── backend/
│   ├── JodWai/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Handlers/
│   │   ├── Repositories/
│   │   └── ...
│   ├── Program.cs
│   └── appsettings.json
├── frontend/
│   └── JodWai-Web/
│       ├── src/
│       ├── public/
│       └── package.json
└── docs/
    ├── ARCHITECTURE.md
    ├── CONVENTIONS.md
    ├── DECISIONS.md
    ├── DOMAIN.md
    └── TESTING.md
```

## Related Documentation
- [Architecture](docs/ARCHITECTURE.md)
- [Domain](docs/DOMAIN.md)
- [Testing](docs/TESTING.md)