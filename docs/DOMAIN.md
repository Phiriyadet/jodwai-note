# Domain Model

## Core Domain Concepts

### Note Management

Notes are the primary domain object. Each note has:
- A unique identifier (`Id`)
- A title (1-200 characters)
- Content/body (0-10000 characters)
- Creation timestamp
- Update timestamp
- Associated links and tags

Notes support relationships through `NoteLink` value objects and categorization through `Tag` value objects.

## Entities

### Note

**Identity**: Aggregate root, identified by `Id` (`NoteId`).

**Attributes**:
| Property | Type | Description |
|---|---|---|
| `Id` | `NoteId` | Note's unique identifier (GUID) |
| `Title` | `NoteTitle` | Note heading (max 200 chars) |
| `Content` | `NoteContent` | Note body (max 10000 chars) |
| `CreatedAt` | `DateTimeOffset` | When note was created |
| `UpdatedAt` | `DateTimeOffset` | When note was last modified |

**Behaviors**:
- `Update(NoteTitle?, NoteContent?)` - Updates title and/or content, requires at least one field to be provided
- `SyncLinks(List<NoteId>)` - Rebuilds internal links list from database to ensure consistency
- `AddTag(Tag tag)` - Adds a tag to this note
- `RemoveTag(Tag tag)` - Removes a tag from this note

**Invariants**:
- `Id` must be non-empty
- `Title` must be 1-200 characters and non-empty
- `Content` must be 0-10000 characters
- At least one of `Title` or `Content` must change on update
- Cannot add self-referential links via `NoteLink`

**Key Relationships**:
- Contains zero or more `NoteLink` instances (targeting other notes)
- Contains zero or more `Tag` instances

**Private Setters**:
- `Title` has private setter to prevent external mutation
- `Content` has private setter to prevent external mutation
- Updates go through `Update()` method

## Value Objects

### NoteId

**Fields**:
| Field | Type | Description |
|---|---|---|
| `Value` | `Guid` | The underlying GUID value |

**Invariants**:
- `Value` must not be empty/default

**Behavior**:
- Equality based on `Value`
- Used as primary key for `Note` entity

### NoteTitle

**Fields**:
| Field | Type | Description |
|---|---|---|
| `Value` | `string` | The title text |

**Invariants**:
- `Value` length: 1-200 characters
- `Value` must not be empty
- `Value` is trimmed on construction
- `Value` must contain at least one character

**Behavior**:
- Equality based on `Value`
- Throws `ArgumentException` for invalid length

**Factory Methods**:
- `NoteTitle.From(string value)` - Creates from string, applies trimming and validation

### NoteContent

**Fields**:
| Field | Type | Description |
|---|---|---|
| `Value` | `string` | The content text |

**Invariants**:
- `Value` length: 0-10000 characters
- `Value` is trimmed on construction
- Null is converted to empty string

**Behavior**:
- Equality based on `Value`
- Throws `ArgumentException` for length exceeding 10000 characters

**Factory Methods**:
- `NoteContent.From(string? value)` - Creates from string (nullable), applies trimming

### Tag

**Fields**:
| Field | Type | Description |
|---|---|---|
| `Value` | `string` | The tag text (lowercase) |

**Invariants**:
- `Value` length: 1-100 characters
- `Value` must not be empty
- `Value` is trimmed on construction
- `Value` is converted to lowercase on construction

**Behavior**:
- Equality based on `Value` (lowercased)
- Throws `ArgumentException` for invalid length

**Factory Methods**:
- `Tag.From(string value)` - Creates from string, applies trimming and lowercasing

### NoteLink

**Fields**:
| Field | Type | Description |
|---|---|---|
| `TargetId` | `NoteId` | The ID of the linked note |

**Invariants**:
- `TargetId` must not be empty
- `TargetId` cannot be the same as the owning `Note`'s ID (no self-links)

**Behavior**:
- Equality based on `TargetId`
- Throws `ArgumentException` for self-reference

**Usage**:
- Represents a relationship between two notes
- Stored in `Note` entity's internal list

## Aggregates

### Note

**Aggregate Root**: `Note` is the only aggregate in the domain model.

**Responsibilities**:
- Manage its own title and content
- Maintain its list of links and tags
- Enforce invariants on title/content length and uniqueness of links
- Synchronize links against valid note IDs

## Relationships Between Domain Objects

```
Note (1) ────────────< NoteLink
  └─ Contains 0..* TargetNoteIds

Note (1) ────────────< Tag
  └─ Contains 0..* Tags (lowercase)
```

## Ubiquitous Language Glossary

| Term | Definition |
|---|---|
| Note | The primary business object representing a user's note |
| NoteId | Value object representing a note's unique identifier |
| NoteTitle | Value object representing a note's title with validation |
| NoteContent | Value object representing a note's content with validation |
| Tag | Value object for categorizing notes (lowercase) |
| NoteLink | Value object representing a link to another note |
| Aggregate | A `Note` instance with all its related links and tags |