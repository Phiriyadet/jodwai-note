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
JodWai-Note/
├── AGENT.md
├── README.md
├── backend/
│   └── JodWai/
│       ├── JodWai.AppHost/
│       │   ├── AppHost.cs
│       │   ├── JodWai.AppHost.csproj
│       ├── JodWai.MigrationService/
│       │   ├── JodWai.MigrationService.csproj
│       │   ├── Program.cs
│       ├── JodWai.ServiceDefaults/
│       │   ├── Extensions.cs
│       │   └── JodWai.ServiceDefaults.csproj
│       ├── JodWai.slnx
│       ├── aspire.config.json
│       ├── src/
│       │   ├── JodWai.Api
│       │   │   ├── Controllers/
│       │   │   │   └── NotesController.cs
│       │   │   ├── Dockerfile
│       │   │   ├── JodWai.Api.csproj
│       │   │   ├── Program.cs
│       │   │   ├── appsettings.Development.json
│       │   │   └── appsettings.json
│       │   ├── JodWai.Application
│       │   │   ├── Behaviors/
│       │   │   │   ├── LoggingBehavior.cs
│       │   │   │   └── ValidationBehavior.cs
│       │   │   ├── Common/
│       │   │   │   └── Results/
│       │   │   │       ├── Error.cs
│       │   │   │       ├── Errors/
│       │   │   │       │   └── NoteErrors.cs
│       │   │   │       └── Result.cs
│       │   │   ├── Extensions/
│       │   │   │   └── ApplicationExtensions.cs
│       │   │   ├── Interfaces/
│       │   │   │   ├── INoteLinkParser.cs
│       │   │   │   ├── INoteRepository.cs
│       │   │   ├── JodWai.Application.csproj
│       │   │   ├── Mappers/
│       │   │   │   └── NoteMapper.cs
│       │   │   └── Notes/
│       │   │       ├── Commands/
│       │   │       │   ├── CreateNote/
│       │   │       │   │   ├── CreateNoteCommand.cs
│       │   │       │   │   └── CreateNoteCommandValidator.cs
│       │   │       │   ├── DeleteNote/
│       │   │       │   │   ├── DeleteNoteCommand.cs
│       │   │       │   │   └── DeleteNoteCommandValidator.cs
│       │   │       │   └── UpdateNote/
│       │   │       │       ├── UpdateNoteCommand.cs
│       │   │       │       └── UpdateNoteCommandValidator.cs
│       │   │       ├── Dtos/
│       │   │       │   ├── NoteDto.cs
│       │   │       │   ├── ParsedNoteLink.cs
│       │   │       │   └── Requests/
│       │   │       │       ├── CreateNoteRequest.cs
│       │   │       │       └── UpdateNoteRequest.cs
│       │   │       └── Queries/
│       │   │           ├── GetAllNotesQuery.cs
│       │   │           ├── GetNoteByIdQuery.cs
│       │   │           └── SearchNotesQuery.cs
│       │   ├── JodWai.Domain/
│       │   │   ├── Entities/
│       │   │   │   └── Note.cs
│       │   │   ├── JodWai.Domain.csproj
│       │   │   └── ValueObjects/
│       │   │       ├── NoteContent.cs
│       │   │       ├── NoteId.cs
│       │   │       ├── NoteLink.cs
│       │   │       ├── NoteTitle.cs
│       │   │       └── Tag.cs
│       │   └── JodWai.Infrastructure/
│       │       ├── Extensions/
│       │       │   └── InfrastructureExtensions.cs
│       │       ├── JodWai.Infrastructure.csproj
│       │       ├── Parsing/
│       │       │   └── WikiStyleNoteLinkParser.cs
│       │       └── Persistence/
│       │           ├── AppDbContext.cs
│       │           ├── Configurations/
│       │           │   └── NoteConfiguration.cs
│       │           ├── Migrations/
│       │           ├── Repositories/
│       │           │   └── NoteRepository.cs
│       │           └── Workers/
│       │               └── MigrationWorker.cs
│       └── tests/
│           ├── JodWai.Tests.Integration/
│           │   └── JodWai.Tests.Integration.csproj
│           └── JodWai.Tests.Unit/
│               ├── Application/
│               │   └── Notes/
│               │       └── Commands/
│               │           ├── CreateNoteCommandTests.cs
│               │           ├── DeleteNoteCommandTests.cs
│               │           └── UpdateNoteCommandTests.cs
│               ├── Constants/
│               │   └── NoteTestConstants.cs
│               ├── Domain/
│               │   ├── Entities/
│               │   │   └── NoteTests.cs
│               │   ├── Shared/
│               │   │   └── NoteBuilder.cs
│               │   └── ValueObjects/
│               │       ├── NoteContentTests.cs
│               │       ├── NoteIdTests.cs
│               │       ├── NoteLinkTests.cs
│               │       ├── NoteTitleTests.cs
│               │       └── TagTests.cs
│               └── JodWai.Tests.Unit.csproj
├── docs/
│   ├── ARCHITECTURE.md
│   ├── CONVENTIONS.md
│   ├── DECISIONS.md
│   ├── DOMAIN.md
│   └── TESTING.md
└── frontend/
    └── JodWai-Web/
        ├── eslint.config.js
        ├── index.html
        ├── package-lock.json
        ├── package.json
        ├── public/
        ├── src/
        │   ├── App.css
        │   ├── App.tsx
        │   ├── assets/
        │   ├── index.css
        │   └── main.tsx
        ├── tsconfig.app.json
        ├── tsconfig.json
        ├── tsconfig.node.json
        └── vite.config.ts
```

## Related Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [Domain](docs/DOMAIN.md)
- [Testing](docs/TESTING.md)
