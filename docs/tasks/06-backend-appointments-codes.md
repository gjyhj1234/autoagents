# Task 06 — Backend: Appointments & Procedure Codes API

**GitHub Issue Title**: `[Task-06] Backend: Appointment Scheduling and Procedure Codes API`  
**Labels**: `agent-task`, `backend`  
**Priority**: 🟠 High  
**Depends On**: #2, #3

---

## Objective

Implement APIs for appointment scheduling (full calendar management) and CDT procedure code lookup.

---

## Procedure Code API

### GET /api/procedure-codes
```
?search=composite&category=Restorative&page=1&pageSize=20

Response 200:
{
  "data": [
    {
      "id": "uuid",
      "adaCode": "D2392",
      "description": "Resin-based composite - two surfaces, posterior - primary or permanent",
      "abbreviatedDesc": "Comp 2S Post",
      "category": "Restorative",
      "defaultFee": 320.00
    }
  ],
  "pagination": { ... }
}
```

### GET /api/procedure-codes/:id
Single code detail.

### GET /api/procedure-codes/categories
```json
["Diagnostic", "Preventive", "Restorative", "Endodontic", "Periodontic", 
 "Oral Surgery", "Prosthodontic Fixed", "Prosthodontic Removable", "Orthodontic"]
```

---

## Appointment API

### GET /api/appointments
```
?startDate=2024-01-15&endDate=2024-01-21&providerId=uuid&operatoryId=uuid&patientId=uuid&status=scheduled

Response 200: array of appointment objects
```

### GET /api/appointments/:id
```json
{
  "id": "uuid",
  "patient": { "id": "...", "name": "John Smith", "dob": "1985-03-15", "phone": "555-0100" },
  "provider": { "id": "...", "name": "Dr. Zhang Wei", "color": "#3182CE" },
  "operatory": { "id": "...", "name": "Operatory 1", "color": "#718096" },
  "appointmentType": { "id": "...", "name": "Comprehensive Exam", "color": "#38A169" },
  "startTime": "2024-01-15T09:00:00Z",
  "endTime": "2024-01-15T10:00:00Z",
  "durationMinutes": 60,
  "status": "scheduled",
  "notes": "Patient has latex allergy — use latex-free gloves",
  "procedures": [
    { "procedureCode": "D0150", "description": "Comprehensive oral evaluation" }
  ],
  "createdAt": "2024-01-10T14:30:00Z"
}
```

### POST /api/appointments
```json
{
  "patientId": "uuid",
  "providerId": "uuid",
  "operatoryId": "uuid",
  "appointmentTypeId": "uuid",
  "startTime": "2024-01-15T09:00:00",
  "durationMinutes": 60,
  "notes": "",
  "procedureIds": ["procedure-code-uuid1", "procedure-code-uuid2"]
}
```

Validation:
- No double-booking: check that provider + operatory are free at requested time
- Return `409 Conflict` with details if slot is taken:
  ```json
  { "error": "SlotConflict", "conflicts": [{ "type": "provider", "existingAppointmentId": "uuid" }] }
  ```

### PUT /api/appointments/:id
Update appointment. Same validation as POST.

### DELETE /api/appointments/:id
Soft delete (sets `deleted_at` and status = `cancelled`). Returns 204.

### PUT /api/appointments/:id/status
```json
{ "status": "arrived" }
// Valid transitions: scheduled → confirmed → arrived → in_chair → completed
// Also: any → no_show, any → cancelled
```

### GET /api/appointments/availability
Check provider availability:
```
?providerId=uuid&date=2024-01-15&durationMinutes=60

Response 200:
{
  "date": "2024-01-15",
  "provider": {...},
  "availableSlots": [
    { "startTime": "09:00", "endTime": "10:00" },
    { "startTime": "11:00", "endTime": "12:00" }
  ],
  "bookedSlots": [
    { "startTime": "10:00", "endTime": "11:00", "appointmentId": "uuid" }
  ]
}
```

Office hours: 8:00–18:00 Monday–Friday (configurable via `appsettings.json`).

---

## Provider API

### GET /api/providers
Returns all active providers.

### GET /api/providers/:id
Provider detail with their schedule.

### GET /api/providers/:id/schedule
```
?startDate=2024-01-15&endDate=2024-01-21

Returns appointments + availability for the date range.
```

---

## Operatory API

### GET /api/operatories
Returns all active operatories.

---

## Appointment Type API

### GET /api/appointment-types
Returns all active appointment types.

---

## Calendar View Support

### GET /api/calendar/day
```
?date=2024-01-15&view=provider|operatory

Returns a structured grid suitable for calendar rendering:
{
  "date": "2024-01-15",
  "view": "provider",
  "columns": [
    {
      "id": "provider-uuid",
      "name": "Dr. Zhang Wei",
      "color": "#3182CE",
      "appointments": [ { appointment objects } ]
    }
  ],
  "timeRange": { "start": "08:00", "end": "18:00" }
}
```

### GET /api/calendar/week
```
?startDate=2024-01-15&providerId=uuid

Returns 7 days of appointments.
```

---

## Recall / Follow-up System

### POST /api/patients/:id/recalls
```json
{
  "recallType": "hygiene",        // hygiene, exam, perio
  "intervalMonths": 6,
  "lastCompletedDate": "2024-01-15",
  "notes": "6-month cleaning"
}
```

### GET /api/recalls/due
```
?startDate=2024-02-01&endDate=2024-03-01

Returns patients with recalls due in the date range.
```

---

## Tests Required

- `AppointmentApiTests.cs`: CRUD, conflict detection, status transitions
- `AvailabilityTests.cs`: slot calculation, office hours
- `CalendarViewTests.cs`: day/week view responses
- `ProcedureCodeApiTests.cs`: search, category filter

---

## Acceptance Criteria

- [ ] Appointment CRUD works correctly
- [ ] Double-booking prevention returns 409 with conflict details
- [ ] Status transition validation (only valid transitions allowed)
- [ ] Availability endpoint returns correct open slots based on existing bookings
- [ ] Calendar day/week view returns properly structured response
- [ ] Procedure code search by keyword and category works
- [ ] All tests pass
