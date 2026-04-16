# Task 03 — Backend: Authentication & Patient API

**GitHub Issue Title**: `[Task-03] Backend: JWT Authentication and Patient CRUD API`  
**Labels**: `agent-task`, `backend`  
**Priority**: 🔴 Critical  
**Depends On**: #1, #2

---

## Objective

Implement JWT authentication endpoints and the complete Patient CRUD API with search, filtering, and medical history management.

---

## Authentication Endpoints

### POST /api/auth/login
```json
// Request
{ "username": "admin", "password": "Secret123!" }

// Response 200
{
  "accessToken": "eyJ...",
  "refreshToken": "abc123...",
  "expiresIn": 3600,
  "user": { "id": "...", "username": "admin", "role": "admin", "name": "Admin User" }
}

// Response 401 — invalid credentials
{ "error": "InvalidCredentials", "message": "Username or password is incorrect" }

// Response 423 — account locked
{ "error": "AccountLocked", "message": "Account locked until 2024-01-01T10:00:00Z" }
```

### POST /api/auth/refresh
```json
// Request
{ "refreshToken": "abc123..." }

// Response 200 — new access token issued
{ "accessToken": "eyJ...", "expiresIn": 3600 }

// Response 401 — invalid/expired refresh token
{ "error": "InvalidRefreshToken" }
```

### POST /api/auth/logout
```
Authorization: Bearer <token>
// Invalidates the refresh token
// Response 204 No Content
```

### POST /api/auth/change-password (authenticated)
```json
// Request
{ "currentPassword": "...", "newPassword": "..." }
// Response 200 or 400
```

### JWT Configuration

```json
// appsettings.json
{
  "Jwt": {
    "SecretKey": "USE_ENV_VAR_IN_PRODUCTION",
    "Issuer": "DentalChart.Api",
    "Audience": "DentalChart.Frontend",
    "AccessTokenExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

- Use `Microsoft.AspNetCore.Authentication.JwtBearer`
- Store refresh tokens in a `refresh_tokens` DB table with user_id, token_hash, expires_at, is_revoked
- Implement bcrypt password hashing (`BCrypt.Net-Next`)
- Security: account lockout after 5 failures for 30 minutes; rate limiting on auth routes

---

## Patient API Endpoints

### GET /api/patients
```
?page=1&pageSize=20&search=smith&status=active&provider=uuid&sortBy=lastName&sortDir=asc

// Response 200
{
  "data": [ { patient objects... } ],
  "pagination": { "page": 1, "pageSize": 20, "totalCount": 150, "totalPages": 8 }
}
```

Search matches: first_name, last_name, preferred_name, phone numbers, email (case-insensitive).

### GET /api/patients/:id
```json
{
  "id": "uuid",
  "patientNumber": 1001,
  "firstName": "John",
  "lastName": "Smith",
  "preferredName": "Johnny",
  "dateOfBirth": "1985-03-15",
  "age": 39,               // computed
  "gender": "male",
  "phoneHome": "555-0100",
  "phoneMobile": "555-0101",
  "phoneWork": null,
  "email": "john@example.com",
  "address": { "line1": "123 Main St", "city": "Beijing", "state": "Beijing", "postalCode": "100001" },
  "status": "active",
  "preferredProvider": { "id": "...", "name": "Dr. Zhang Wei" },
  "medicalHistory": { ... },
  "insurancePlans": [ {...} ],
  "alertFlags": [ { "text": "Warfarin — bleeding risk", "severity": "high" } ],
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### POST /api/patients
```json
// Request body — all fields optional except firstName, lastName, dateOfBirth
{
  "firstName": "Jane",
  "lastName": "Doe",
  "dateOfBirth": "1990-05-20",
  "gender": "female",
  "phoneMobile": "555-0200",
  "email": "jane@example.com"
}
// Response 201 with created patient
```

### PUT /api/patients/:id
Update any patient fields. Returns 200 with updated patient.

### DELETE /api/patients/:id
Soft delete (sets `deleted_at`). Returns 204.

---

## Medical History Endpoints

### GET /api/patients/:id/medical-history
Returns the patient's medical history record.

### PUT /api/patients/:id/medical-history
Upsert (create or update) medical history.
```json
{
  "hasDiabetes": true,
  "hasBloodThinners": true,
  "currentMedications": "Warfarin 5mg daily, Metformin 500mg",
  "allergies": [
    { "name": "Penicillin", "severity": "severe", "reaction": "anaphylaxis" },
    { "name": "Latex", "severity": "moderate", "reaction": "contact dermatitis" }
  ],
  "alertFlags": [
    { "text": "Warfarin — bleeding risk, consult physician before surgery", "severity": "high" }
  ]
}
```

---

## Insurance Endpoints

### GET /api/patients/:id/insurance
Returns list of insurance plans.

### POST /api/patients/:id/insurance
Create insurance plan.

### PUT /api/patients/:id/insurance/:planId
Update insurance plan.

### DELETE /api/patients/:id/insurance/:planId
Soft delete.

---

## Implementation Details

### AOT-Compatible Response Serialization

```csharp
// In DentalChart.Api/Serialization/AppJsonContext.cs
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(LoginResponse))]
[JsonSerializable(typeof(PagedResponse<PatientDto>))]
[JsonSerializable(typeof(PatientDto))]
[JsonSerializable(typeof(CreatePatientRequest))]
// ... all DTOs
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class AppJsonContext : JsonSerializerContext { }
```

### Error Response Standard
All errors must use this format:
```json
{ "error": "ErrorCode", "message": "Human-readable message", "details": {} }
```

### Validation
Use `FluentValidation` for request validation (AOT-compatible setup).

---

## Tests Required

```csharp
// DentalChart.Tests/Auth/
AuthEndpointTests.cs      // login success, invalid password, lockout, refresh, logout
PatientApiTests.cs        // CRUD operations, search, pagination
MedicalHistoryTests.cs    // upsert, alert flags
```

Use `WebApplicationFactory<Program>` + Testcontainers PostgreSQL for integration tests.

---

## Acceptance Criteria

- [ ] Login endpoint returns JWT + refresh token
- [ ] Refresh token rotation works
- [ ] Account lockout after 5 failed attempts
- [ ] `GET /api/patients?search=smith` returns filtered results
- [ ] Pagination metadata correct
- [ ] Medical history `PUT` creates on first call, updates on subsequent
- [ ] Alert flags appear in patient detail response
- [ ] All tests pass
- [ ] All AOT-incompatible patterns removed (no `Newtonsoft.Json`, no reflection-based serialization)
