# Task 11 — Frontend: Patient Management & Appointment Scheduling

**GitHub Issue Title**: `[Task-11] Frontend: Patient Detail, Medical History, and Appointment Scheduling`  
**Labels**: `agent-task`, `frontend`  
**Priority**: 🟠 High  
**Depends On**: #3, #6, #7

---

## Objective

Build the complete patient management UI (demographics, medical history, insurance) and the appointment calendar with scheduling from the treatment plan.

---

## Patient Detail View

### Demographics Tab (`PatientOverview.vue`)

Left column — patient info card:
```
┌─────────────────────────────┐
│ 👤 John Smith (Johnny)      │
│ DOB: 1985-03-15 (Age: 39)  │
│ Male | Patient #1001        │
│ Status: 🟢 Active           │
│                             │
│ 📱 Mobile: 555-0101         │
│ 🏠 Home: 555-0100           │
│ ✉️  john@example.com        │
│ 📍 123 Main St, Beijing     │
│                             │
│ Provider: Dr. Zhang Wei     │
│ Language: 中文              │
│                             │
│ [Edit Patient Info]         │
└─────────────────────────────┘
```

Right column — alerts and insurance:
```
┌─────────────────────────────────────────────┐
│ ⚠️  MEDICAL ALERTS                           │
│ 🔴 Warfarin — bleeding risk                  │
│ 🟠 Latex allergy — use latex-free gloves    │
└─────────────────────────────────────────────┘

┌─────────────────────────────────────────────┐
│ 🏥 INSURANCE                                 │
│ Primary: Ping An Health Insurance            │
│   Group: PF-2024-001  Member: J123456       │
│   Remaining benefit: ¥1,500 / ¥2,000       │
│ [Edit Insurance]                            │
└─────────────────────────────────────────────┘
```

Bottom — recent activity:
- Last 5 appointments (date, type, provider, status)
- Last 5 procedures completed
- Next scheduled appointment

### Medical History Tab (`MedicalHistoryForm.vue`)

Two-column checkbox form:
```
SYSTEMIC CONDITIONS          MEDICATIONS & ALLERGIES
□ Diabetes                   Current Medications:
□ Hypertension               [_________________________]
□ Heart Disease              [_________________________]
□ Artificial Heart Valve     [+ Add medication]
□ Pacemaker
□ Blood Thinners (Anticoag)  ALLERGIES
□ Bisphosphonates            [+ Add allergy]
□ Bleeding Disorder          ┌──────────────────────┐
□ HIV/AIDS                   │ Penicillin           │
□ Hepatitis                  │ Severity: Severe     │
□ Epilepsy                   │ Reaction: Anaphylaxis│
□ Asthma                     │ [Remove]             │
□ Pregnant                   └──────────────────────┘
□ Nursing
                             ALERT FLAGS
Other Conditions:            [+ Add Alert]
[_________________________]  ┌──────────────────────┐
                             │ 🔴 Warfarin — check  │
                             │ bleeding risk        │
                             │ [Remove]             │
                             └──────────────────────┘

[Save Medical History]
```

Auto-generate alert flags based on checked conditions:
- Blood thinners checked → suggest "Anticoagulant therapy — pre-surgical consultation required"
- Bisphosphonates checked → suggest "Bisphosphonate use — MRONJ risk, consult physician before extractions"
- Pacemaker → suggest "Pacemaker — avoid electrosurgery"

### Insurance Tab (`InsurancePlans.vue`)

```
INSURANCE PLANS

PRIMARY INSURANCE
Carrier: [ Ping An Health ____________ ]
Plan Name: [ Comprehensive Dental Plan ]
Group #: [ PF-2024-001 ]    Member ID: [ J123456 ]
Subscriber: [Self ▼]
Effective: [2024-01-01]  Termination: [2026-12-31]
Annual Max: [¥ 2,000 ]   Deductible: [¥ 200 ]

[+ Add Secondary Insurance]
[Save Insurance]
```

---

## Appointment Calendar (`CalendarView.vue`)

### Views

**Day View** (`CalendarDayView.vue`):
```
                    Monday, January 15, 2024
         Dr. Zhang         Dr. Li          Hygienist Chen
8:00  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐
      │ John Smith  │  │             │  │ Mary Wang   │
      │ Comp. Exam  │  │             │  │ Prophylaxis │
9:00  ├─────────────┤  │             │  ├─────────────┤
      │             │  │ Jane Doe    │  │             │
      │             │  │ Crown Prep  │  │             │
10:00 └─────────────┘  │             │  └─────────────┘
      ┌─────────────┐  │             │
      │ Bob Lee     │  └─────────────┘
      │ Extraction  │
11:00 └─────────────┘
```

- Each appointment card: patient name, procedure type, colored border by appointment type
- Click appointment card → opens appointment detail popover
- Click empty time slot → opens new appointment modal

**Week View** (`CalendarWeekView.vue`):
5-day grid with one provider, or all providers in columns.

### Appointment Detail Popover
```
📅 John Smith — Comprehensive Exam
──────────────────────────────────
Time: 9:00 AM – 10:00 AM (60 min)
Provider: Dr. Zhang Wei
Operatory: Op. 1
Status: Scheduled

[Check In] [Edit] [Cancel]
```

### New Appointment Modal

```
Schedule Appointment
────────────────────────────────────────
Patient: [Search patient... ] or current patient
Date: [ 2024-01-15 📅 ]   Time: [ 09:00 ▼ ]
Duration: [ 60 ] minutes
Provider: [ Dr. Zhang Wei ▼ ]
Operatory: [ Operatory 1 ▼ ]
Type: [ Comprehensive Exam ▼ ]
Procedures: [ + Add from treatment plan ]
Notes: [________________________________]

Availability check:
✅ Dr. Zhang Wei is free at 09:00-10:00 on Jan 15

                            [Cancel] [Schedule]
```

After scheduling, update linked treatment plan items' `appointmentId`.

---

## Stores

### Patient Store (`src/stores/patient.ts`)
```typescript
export const usePatientStore = defineStore('patient', () => {
  const patients = ref<Patient[]>([])
  const currentPatient = ref<PatientDetail | null>(null)
  const pagination = ref<Pagination>({ page: 1, pageSize: 20, totalCount: 0, totalPages: 0 })
  
  async function fetchPatients(params: PatientSearchParams): Promise<void> { ... }
  async function fetchPatient(id: string): Promise<void> { ... }
  async function createPatient(data: CreatePatientRequest): Promise<Patient> { ... }
  async function updatePatient(id: string, data: UpdatePatientRequest): Promise<void> { ... }
  async function updateMedicalHistory(patientId: string, data: MedicalHistoryRequest): Promise<void> { ... }
  async function updateInsurance(patientId: string, data: InsurancePlanRequest): Promise<void> { ... }
  
  return { patients, currentPatient, pagination, fetchPatients, fetchPatient, 
           createPatient, updatePatient, updateMedicalHistory, updateInsurance }
})
```

### Calendar Store (`src/stores/calendar.ts`)
```typescript
export const useCalendarStore = defineStore('calendar', () => {
  const appointments = ref<Appointment[]>([])
  const selectedDate = ref(new Date())
  const viewMode = ref<'day' | 'week'>('day')
  const selectedProviderId = ref<string | null>(null)
  
  async function fetchAppointments(startDate: Date, endDate: Date): Promise<void> { ... }
  async function createAppointment(data: CreateAppointmentRequest): Promise<Appointment> { ... }
  async function updateAppointmentStatus(id: string, status: AppointmentStatus): Promise<void> { ... }
  async function cancelAppointment(id: string): Promise<void> { ... }
  async function checkAvailability(providerId: string, date: Date, duration: number): Promise<TimeSlot[]> { ... }
  
  return { appointments, selectedDate, viewMode, selectedProviderId,
           fetchAppointments, createAppointment, updateAppointmentStatus, cancelAppointment, checkAvailability }
})
```

---

## Acceptance Criteria

- [ ] Patient detail page shows all tabs (Overview, Medical History, Insurance, Appointments)
- [ ] Medical alerts displayed prominently with correct severity colors
- [ ] Medical history form saves correctly (all checkbox and text fields)
- [ ] Allergy and alert flag add/remove works
- [ ] Auto-suggested alerts based on medical conditions work
- [ ] Insurance form saves primary and secondary insurance
- [ ] Calendar day view renders appointments in time grid
- [ ] Appointment cards show patient name, type, and colored border
- [ ] Click empty slot → new appointment modal opens
- [ ] Double-booking prevention: shows error if slot is taken
- [ ] Availability check shows free slots before confirming
- [ ] Appointment status update (check in, complete) works
- [ ] All component tests pass
