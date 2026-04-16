# Task 12 — Reporting, PDF Export & Integration Tests

**GitHub Issue Title**: `[Task-12] Reporting, PDF Export, and End-to-End Integration Tests`  
**Labels**: `agent-task`, `testing`  
**Priority**: 🟡 Medium  
**Depends On**: #8, #9, #10, #11

---

## Objective

Implement all print/PDF export functionality, and write end-to-end integration tests covering the full patient workflow.

---

## Reporting Features

### 1. Dental Chart Print (`PrintDentalChart.vue`)

Full-page dental chart printout:

```
                    DENTAL EXAMINATION RECORD
──────────────────────────────────────────────────────────────────
Patient: John Smith (#1001)          Date: 2024-01-15
DOB: 1985-03-15 (Age: 39)           Provider: Dr. Zhang Wei
Address: 123 Main St, Beijing

                    [SVG TOOTH CHART — full color, both arches]

LEGEND:  🔴 Decay  🟡 Crown  🔵 Planned  ⬜ Composite  ⬛ Amalgam
         ✕ Missing  🔩 Implant  — Bridge  ≡ Root Canal

EXISTING CONDITIONS:
Tooth 14 (O,D): Decay — Treatment Planned — D2392 — Dr. Zhang
Tooth  3 (full): Crown PFM — Existing — D2750
Tooth 30 (root): Root Canal — In Progress — D3320

NOTES: _______________________________________________
       _______________________________________________

Provider Signature: ____________________  Date: _______
──────────────────────────────────────────────────────────────────
```

### 2. Treatment Plan Print (`PrintTreatmentPlan.vue`)

See Task 09 specification for layout. Additional fields:
- Office name, address, phone
- Dentist license number
- Fee schedule note
- Insurance disclaimer

### 3. Periodontal Chart Print (`PrintPerioChart.vue`)

Full perio grid on A4 landscape with color coding:
- Both arches on one page
- Summary statistics at bottom
- Previous exam comparison if available

### 4. Patient Summary Report (`PrintPatientSummary.vue`)

One-page patient overview:
- Demographics
- Medical alerts (prominent red box)
- Active insurance
- Active treatment plan (abbreviated)
- Last 5 appointments
- Last modified date + provider

### 5. Insurance Claim Form Layout (`InsuranceClaimForm.vue`)

ADA 2019 Dental Claim Form layout (pre-populated):
- Patient info section
- Subscriber info section
- Procedure table (up to 15 procedures)
- Diagnosis codes (ICD-10)
- Provider signature block
- Print-ready

---

## PDF Export Service (`src/services/pdfExport.ts`)

```typescript
import jsPDF from 'jspdf'
import html2canvas from 'html2canvas'

export async function exportToPdf(
  elementRef: HTMLElement,
  filename: string,
  options?: PdfOptions
): Promise<void> {
  const canvas = await html2canvas(elementRef, {
    scale: 2,        // high resolution
    useCORS: true,
    logging: false,
  })
  
  const pdf = new jsPDF({
    orientation: options?.orientation ?? 'portrait',
    unit: 'mm',
    format: 'a4',
  })
  
  const imgData = canvas.toDataURL('image/png')
  const pdfWidth = pdf.internal.pageSize.getWidth()
  const pdfHeight = (canvas.height * pdfWidth) / canvas.width
  
  pdf.addImage(imgData, 'PNG', 0, 0, pdfWidth, pdfHeight)
  pdf.save(filename)
}
```

---

## Integration Tests

### Backend Integration Tests (`DentalChart.Tests/Integration/`)

**Full Patient Workflow Test** (`PatientWorkflowTests.cs`):
```csharp
[Fact]
public async Task FullPatientWorkflow_CreateTreatmentPlanToCompletion()
{
    // 1. Create patient
    // 2. Add medical history
    // 3. Create dental chart conditions (decay on #14, #19)
    // 4. Create treatment plan with those procedures
    // 5. Schedule appointment
    // 6. Update condition status to "completed"
    // 7. Update treatment plan item to "completed"
    // 8. Verify chart shows completed conditions
    // 9. Create perio chart with measurements
    // 10. Verify perio summary calculations
}
```

**Auth Security Tests** (`AuthSecurityTests.cs`):
```csharp
// Test that protected endpoints return 401 without token
// Test role-based access: front_desk cannot modify chart conditions
// Test account lockout
// Test refresh token rotation
```

### Frontend Integration Tests (Vitest + Vue Test Utils)

**Complete Chart Interaction Test** (`dental-chart.integration.test.ts`):
```typescript
describe('Dental Chart — full workflow', () => {
  it('renders all 32 teeth', async () => { ... })
  it('clicking a surface selects it and opens detail panel', async () => { ... })
  it('adding a condition updates the surface color', async () => { ... })
  it('changing dentition mode re-renders chart', async () => { ... })
  it('bridge rendering connects abutment teeth', async () => { ... })
})
```

**Treatment Plan Integration Test** (`treatment-plan.integration.test.ts`):
```typescript
describe('Treatment Plan — full workflow', () => {
  it('creates new plan', async () => { ... })
  it('adds procedures and calculates totals', async () => { ... })
  it('drag-drop reorders items', async () => { ... })
  it('marking item complete updates totals', async () => { ... })
})
```

---

## Print CSS (`src/assets/print.css`)

```css
@media print {
  /* Hide navigation, sidebar, toolbars */
  .sidebar, .app-toolbar, .nav-tabs, .detail-panel-actions { display: none !important; }
  
  /* Full width for print content */
  .print-content { width: 100%; margin: 0; padding: 0; }
  
  /* Preserve background colors for condition overlays */
  * { -webkit-print-color-adjust: exact; print-color-adjust: exact; }
  
  /* Page breaks */
  .page-break { page-break-after: always; }
}
```

---

## Settings Page (`src/views/SettingsView.vue`)

Basic settings for admin role:
- Office information (name, address, phone, logo)
- Provider management (add/edit/deactivate)
- Operatory management
- Appointment type management (name, duration, color)
- Procedure code fee schedule (override default fees)

---

## Acceptance Criteria

- [ ] Dental chart prints with full-color condition overlays (no white-out)
- [ ] Treatment plan PDF exports correctly with all procedures and totals
- [ ] Perio chart prints on A4 landscape with all measurements
- [ ] Patient summary printout fits one page
- [ ] PDF export downloads with correct filename
- [ ] Backend integration test: full patient workflow passes
- [ ] Backend integration test: auth security checks pass
- [ ] Frontend integration test: 32 teeth render + surface click works
- [ ] Frontend integration test: treatment plan totals calculate correctly
- [ ] All print layouts tested: no content cutoff
- [ ] Settings page allows admin to manage providers and operatories
