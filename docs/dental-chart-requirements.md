# 🦷 Dental Chart System — Full Product Requirements Document

## Project Overview

A production-grade, full-stack **dental practice management platform** centered on a
comprehensive **tooth chart (口腔牙位图)**. This system is inspired by Open Dental and must
match or exceed all of its dental chart functionality.

**Tech Stack**: Vue 3 + TypeScript (frontend) · .NET 10 AOT (backend) · PostgreSQL 16 (database)

---

## 1. Scope

### 1.1 In Scope

| Module | Description |
|--------|-------------|
| Dental Chart | Interactive SVG tooth chart with per-surface condition marking |
| Patient Records | Demographics, medical history, insurance, photos |
| Treatment Planning | Multi-plan, procedure codes, cost breakdown |
| Periodontal Chart | 6-point probing, BOP, furcation, attachment loss |
| Appointment Scheduling | Calendar, chair/provider assignment |
| Procedure Codes | Full CDT/ADA code lookup |
| Reporting | Print, PDF export, insurance claim forms |
| Authentication | JWT-based, multi-role (admin, dentist, assistant, patient portal) |
| Audit Log | Full history of all changes |

### 1.2 Out of Scope (Phase 1)

- Billing / payment processing (Phase 2)
- 3D CBCT viewer (Phase 3)
- AI caries detection (Phase 3)
- Patient portal mobile app (Phase 2)

---

## 2. Functional Requirements

### 2.1 Dental Chart (Core Feature)

#### 2.1.1 Tooth Chart Display

- **FR-001**: Display all 32 permanent teeth in ADA Universal Numbering order
  (1–16 upper right→left, 17–32 lower left→right from patient's perspective)
- **FR-002**: Display primary teeth A–T in Palmer position equivalents
- **FR-003**: Toggle between: Permanent mode / Primary mode / Mixed dentition mode
- **FR-004**: Each tooth rendered as SVG with 5 surface segments:
  Mesial (M), Occlusal/Incisal (O/I), Distal (D), Facial/Buccal (F/B), Lingual (L)
- **FR-005**: Root section displayed below crown (clickable for root conditions)
- **FR-006**: Tooth number displayed above (upper arch) or below (lower arch) each tooth
- **FR-007**: FDI notation display option (toggle ADA ↔ FDI ↔ Palmer)
- **FR-008**: Zoom in/out on chart (keyboard shortcut + mouse wheel)

#### 2.1.2 Tooth Status Indicators

Each tooth must be able to show the following statuses (can be combined):

| Status | Visual |
|--------|--------|
| Missing / Extracted | Large X through tooth, gray fill |
| Unerupted | Dashed border |
| Impacted | Rotated/angled tooth icon |
| Implant | Blue screw icon below root |
| Root fragment | Partial tooth icon |
| Watch | Orange dot |

#### 2.1.3 Surface Conditions (color-coded overlays)

| Condition | Color | Surface |
|-----------|-------|---------|
| Decay (caries) | Red #E53E3E | Per-surface |
| Existing amalgam filling | Silver/Dark gray | Per-surface |
| Existing composite filling | Light yellow | Per-surface |
| Existing crown (full) | Gold #D69E2E | Whole tooth |
| Porcelain crown | Light blue | Whole tooth |
| Stainless steel crown | Gray | Whole tooth |
| Bridge pontic | Purple #805AD5 | Whole tooth |
| Bridge abutment | Gold with line | Whole tooth |
| Root canal (RCT) | Red lines through root | Root |
| Post & core | Brown dot in root | Root |
| Veneer | Pink #FED7E2 | Facial surface |
| Sealant | Teal #81E6D9 | Occlusal only |
| Planned extraction | Red dashed X | Whole tooth |
| Planned crown | Blue crown outline | Whole tooth |
| Treatment planned (generic) | Blue overlay | Per-surface |
| Partial denture | Hatched pattern | Multiple teeth |
| Full denture | Full arch indicator | Arch-level |

#### 2.1.4 Bridge Rendering

- **FR-020**: When 3+ consecutive teeth have bridge status, render horizontal connector lines
  between abutments and pontic(s)
- **FR-021**: Automatically detect bridge span on data load
- **FR-022**: Bridge creation UI: select abutments + pontic teeth, then apply

#### 2.1.5 Interaction

- **FR-025**: Click a tooth → select it (highlight border)
- **FR-026**: Click a surface segment → select that specific surface
- **FR-027**: Multi-select with Shift+Click or Ctrl+Click
- **FR-028**: Right-click context menu on tooth: "Add Condition", "Set Status", "View History",
  "Schedule", "Add Note"
- **FR-029**: Selected tooth/surface → Detail Panel opens on the right
- **FR-030**: Hover tooltip showing tooth number, current conditions, last procedure date

### 2.2 Condition/Procedure Entry

- **FR-040**: Add condition via: select surface → pick procedure code → set status
- **FR-041**: CDT procedure code lookup: type code (e.g., D2391) or description keyword
- **FR-042**: Statuses: Existing (other provider), Existing (this office), Treatment Planned,
  In Progress, Completed, Referred, Watched
- **FR-043**: Per-condition fields: code, status, tooth, surfaces, date, provider, fee, notes
- **FR-044**: Edit and delete conditions with confirmation
- **FR-045**: Condition history log per tooth (who added, when, changes)
- **FR-046**: "Quick Add" toolbar for most common procedures (configurable)

### 2.3 Periodontal Chart

- **FR-060**: Record 6 pocket depth measurements per tooth (3 buccal: DB, B, MB; 3 lingual: DL, L, ML)
- **FR-061**: Recession value per measurement point
- **FR-062**: Attachment level auto-calculated = recession + pocket depth
- **FR-063**: Bleeding on Probing (BOP): toggle per measurement point (red dot = bleeding)
- **FR-064**: Suppuration toggle per measurement point
- **FR-065**: Furcation involvement: Class I (△), II (◈), III (◉) for premolars/molars
- **FR-066**: Mobility score: 0–3 per tooth
- **FR-067**: Color coding: ≤3mm = green, 4–5mm = yellow, ≥6mm = red
- **FR-068**: Bone loss % display (estimated from pocket depth)
- **FR-069**: Line graph overlay showing current vs previous measurement (trend)
- **FR-070**: Multiple perio chart dates (track progression over visits)
- **FR-071**: Print perio chart

### 2.4 Treatment Planning

- **FR-080**: Multiple named treatment plans per patient (e.g., "Comprehensive Plan", "Phase 1")
- **FR-081**: Each plan contains procedure items with: code, tooth, surface, fee, insurance estimate,
  patient portion, status, provider, appointment
- **FR-082**: Drag-and-drop reordering of procedures
- **FR-083**: Group procedures by visit number
- **FR-084**: Fee totals: total fee, estimated insurance, estimated patient portion
- **FR-085**: Insurance fee schedule application (reduce fee to contracted amount)
- **FR-086**: "Presentation mode" — patient-facing view with simplified language
- **FR-087**: Treatment plan status: Active, Inactive, Completed, Rejected
- **FR-088**: Accept/reject plan by patient (with signature field)
- **FR-089**: Print treatment plan as patient-friendly document
- **FR-090**: "Schedule" button on procedure → opens appointment creation

### 2.5 Patient Management

- **FR-100**: Patient demographics: name, DOB, gender, contact info, address, SSN (masked)
- **FR-101**: Emergency contact
- **FR-102**: Preferred provider, preferred language
- **FR-103**: Patient status: Active, Inactive, Deceased, Non-patient
- **FR-104**: Medical history form (checkbox-based for common conditions + free text)
- **FR-105**: Allergies list with severity
- **FR-106**: Current medications list
- **FR-107**: Alert flags shown prominently on chart (e.g., "Anticoagulant!", "Latex allergy!")
- **FR-108**: Insurance: primary + secondary, group number, member ID, employer
- **FR-109**: Referral source tracking

### 2.6 Appointment Scheduling

- **FR-120**: Calendar view: day / week / provider / operatory views
- **FR-121**: Create appointment: patient, type, provider, operatory, date/time, duration, notes
- **FR-122**: Appointment types with default duration and color (configurable)
- **FR-123**: Appointment statuses: Scheduled, Confirmed, Arrived, In Chair, Completed, No-Show, Cancelled
- **FR-124**: Recall/hygiene appointment scheduling
- **FR-125**: Block out time (provider unavailable, office closed)
- **FR-126**: Patient appointment history
- **FR-127**: SMS/email confirmation (configuration for external service — no actual sending required in Phase 1)

### 2.7 Reporting & Export

- **FR-140**: Print dental chart (current state) — full-page layout
- **FR-141**: Export dental chart to PDF
- **FR-142**: Print periodontal chart
- **FR-143**: Treatment plan printout (patient-friendly)
- **FR-144**: Patient record summary printout
- **FR-145**: Procedure history report (by patient, by provider, by date range)
- **FR-146**: Insurance claim form (ADA Form 2019 layout — pre-populated)

### 2.8 Authentication & Authorization

- **FR-160**: Roles: Admin, Dentist (Provider), Hygienist, Front Desk, Patient (portal)
- **FR-161**: JWT access token (1h expiry) + refresh token (7 days)
- **FR-162**: Role-based access: only Admin can add providers; only Dentist/Hygienist can edit chart
- **FR-163**: Per-user audit log: every create/update/delete recorded
- **FR-164**: Password requirements: min 8 chars, 1 uppercase, 1 number, 1 symbol
- **FR-165**: Account lockout after 5 failed attempts

---

## 3. Non-Functional Requirements

### 3.1 Performance
- Dental chart page load: < 2 seconds (cold)
- Chart interaction (click → condition update): < 300ms
- API response time: < 200ms p95 for CRUD operations

### 3.2 Reliability
- Zero-downtime deployment via blue-green or rolling update
- Database: point-in-time recovery (WAL archiving configured in Docker Compose)

### 3.3 Accessibility
- WCAG 2.1 AA compliance
- All interactive SVG elements have `aria-label`
- Keyboard navigation for tooth chart (arrow keys to navigate between teeth)
- High-contrast mode

### 3.4 Security
- All API endpoints authenticated (except /health, /auth/login)
- HTTPS only in production
- Passwords hashed with bcrypt (cost factor 12)
- No secrets in source code — use environment variables
- SQL injection protection via EF Core parameterized queries
- XSS protection via Vue template auto-escaping

### 3.5 Internationalization
- All user-facing strings wrapped in i18n translation keys
- Default language: Chinese (zh-CN) and English (en-US)
- Date formats: locale-aware

---

## 4. Data Model (Summary)

```
patients (id, first_name, last_name, dob, gender, phone, email, address, ...)
  └── dental_charts (id, patient_id, created_at)
       └── tooth_conditions (id, chart_id, tooth_number, notation_system, surfaces[], 
                             condition_type, status, procedure_code_id, ...)
  └── treatment_plans (id, patient_id, name, status, ...)
       └── treatment_plan_items (id, plan_id, tooth_number, surfaces[], procedure_code_id, 
                                  fee, insurance_est, status, visit_number, sort_order, ...)
  └── perio_charts (id, patient_id, exam_date, provider_id, ...)
       └── perio_measurements (id, chart_id, tooth_number, 
                               buccal_db, buccal_b, buccal_mb,
                               lingual_dl, lingual_l, lingual_ml,
                               recession_db, ... [12 recession values],
                               bop_db, ... [12 BOP booleans],
                               furcation_buccal, furcation_lingual,
                               mobility)
  └── appointments (id, patient_id, provider_id, operatory_id, type_id, 
                    start_time, end_time, status, ...)
  └── medical_history (id, patient_id, ...)
  └── insurance_plans (id, patient_id, carrier_name, group_number, member_id, ...)

procedure_codes (id, ada_code, description, abbreviation, category, default_fee)
providers (id, first_name, last_name, license_number, specialty, color)
operatories (id, name, abbreviation, color)
users (id, username, email, password_hash, role, provider_id, ...)
audit_logs (id, user_id, action, entity_type, entity_id, old_value, new_value, timestamp)
```

---

## 5. Task Breakdown

See `docs/tasks/` for individual task specifications:

| # | Task | Description |
|---|------|-------------|
| 01 | Project Infrastructure | Docker, solution structure, CI config |
| 02 | Database Schema | PostgreSQL schema + migrations + seed data |
| 03 | Backend — Auth & Patients | JWT auth + patient CRUD API |
| 04 | Backend — Dental Chart API | Tooth conditions, chart CRUD |
| 05 | Backend — Treatment & Perio | Treatment plan + perio chart APIs |
| 06 | Backend — Appointments & Codes | Appointment scheduling + CDT lookup |
| 07 | Frontend — Project Setup | Vue scaffold, routing, auth, layout |
| 08 | Frontend — Dental Chart SVG | Core SVG tooth chart component |
| 09 | Frontend — Condition Entry | Add/edit/delete conditions panel |
| 10 | Frontend — Treatment Planning | Treatment plan editor |
| 11 | Frontend — Periodontal Chart | Perio grid + trend chart |
| 12 | Frontend — Patient & Appointments | Patient management + scheduler |
| 13 | Reporting & PDF Export | Print/export all documents |
| 14 | Integration Testing & Polish | E2E tests, accessibility, i18n |
