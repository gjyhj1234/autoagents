# Task 05 — Backend: Treatment Plans & Periodontal Chart API

**GitHub Issue Title**: `[Task-05] Backend: Treatment Planning and Periodontal Chart API`  
**Labels**: `agent-task`, `backend`  
**Priority**: 🟠 High  
**Depends On**: #2, #3, #4

---

## Objective

Implement APIs for treatment plan management and periodontal chart recording.

---

## Treatment Plan API

### GET /api/patients/:id/treatment-plans
```json
[
  {
    "id": "plan-uuid",
    "name": "Comprehensive Phase 1",
    "status": "active",
    "totalFee": 3850.00,
    "totalInsuranceEstimate": 1925.00,
    "totalPatientPortion": 1925.00,
    "itemCount": 8,
    "completedCount": 2,
    "createdAt": "2024-01-10T00:00:00Z"
  }
]
```

### POST /api/patients/:id/treatment-plans
```json
// Request
{ "name": "Phase 1 - Urgent Care" }
// Response 201
```

### GET /api/patients/:id/treatment-plans/:planId
Full plan with all items:
```json
{
  "id": "plan-uuid",
  "name": "Comprehensive Phase 1",
  "status": "active",
  "acceptedDate": null,
  "items": [
    {
      "id": "item-uuid",
      "toothNumber": "14",
      "surfaces": ["O", "D"],
      "procedureCode": { "code": "D2392", "description": "Resin composite - 2 surfaces" },
      "status": "planned",
      "provider": { "id": "...", "name": "Dr. Zhang Wei" },
      "fee": 320.00,
      "insuranceEstimate": 160.00,
      "patientPortion": 160.00,
      "visitNumber": 1,
      "sortOrder": 0,
      "note": "",
      "appointmentId": null
    }
  ],
  "totals": {
    "totalFee": 3850.00,
    "insuranceEstimate": 1925.00,
    "patientPortion": 1925.00
  }
}
```

### POST /api/patients/:id/treatment-plans/:planId/items
```json
{
  "toothNumber": "19",
  "surfaces": ["M", "O", "D"],
  "procedureCodeId": "uuid",
  "fee": 480.00,
  "insuranceEstimate": 240.00,
  "visitNumber": 2
}
// Response 201 with created item
```

### PUT /api/patients/:id/treatment-plans/:planId/items/:itemId
Update item (status, fee, sort order, etc.)

### DELETE /api/patients/:id/treatment-plans/:planId/items/:itemId
Soft delete item.

### PUT /api/patients/:id/treatment-plans/:planId/items/reorder
```json
// Reorder all items (drag-and-drop result)
{ "itemIds": ["uuid3", "uuid1", "uuid2"] }
// Returns 200 with updated items
```

### POST /api/patients/:id/treatment-plans/:planId/accept
```json
{ "signature": "base64-encoded-signature-image", "acceptedDate": "2024-01-15" }
```

---

## Periodontal Chart API

### GET /api/patients/:id/perio-charts
```json
[
  { "id": "chart-uuid", "examDate": "2024-01-15", "provider": {...}, "createdAt": "..." },
  { "id": "chart-uuid2", "examDate": "2023-07-10", "provider": {...}, "createdAt": "..." }
]
```

### POST /api/patients/:id/perio-charts
```json
{ "examDate": "2024-01-15", "providerId": "uuid", "notes": "" }
// Response 201 with new chart (empty measurements)
```

### GET /api/patients/:id/perio-charts/:chartId
Full chart with all measurements:
```json
{
  "id": "chart-uuid",
  "examDate": "2024-01-15",
  "provider": {...},
  "measurements": {
    "1": {
      "toothNumber": "1",
      "buccal": { "db": 2, "b": 2, "mb": 3 },
      "lingual": { "dl": 2, "l": 2, "ml": 2 },
      "recession": {
        "buccal": { "db": 0, "b": 0, "mb": 0 },
        "lingual": { "dl": 0, "l": 0, "ml": 0 }
      },
      "bop": {
        "buccal": { "db": false, "b": false, "mb": false },
        "lingual": { "dl": false, "l": false, "ml": false }
      },
      "furcation": { "buccal": null, "lingual": null, "mesial": null },
      "mobility": 0,
      "attachmentLoss": { "buccal": { "db": 2, "b": 2, "mb": 3 }, ... }  // computed
    },
    ...
  },
  "summary": {
    "bopPercentage": 15.2,
    "meanPocketDepth": 2.8,
    "pocketDepth4to5": 8,
    "pocketDepth6plus": 2,
    "meanAttachmentLoss": 2.8
  }
}
```

### PUT /api/patients/:id/perio-charts/:chartId/measurements/:toothNumber
Update or create measurements for one tooth:
```json
{
  "buccal": { "db": 3, "b": 4, "mb": 3 },
  "lingual": { "dl": 2, "l": 3, "ml": 2 },
  "recession": { "buccal": { "db": 1, "b": 1, "mb": 0 }, "lingual": { "dl": 0, "l": 0, "ml": 0 } },
  "bop": { "buccal": { "db": true, "b": false, "mb": false }, "lingual": { "dl": false, "l": false, "ml": false } },
  "furcation": { "buccal": "I", "lingual": null, "mesial": null },
  "mobility": 0
}
```

### PUT /api/patients/:id/perio-charts/:chartId/measurements/bulk
Submit all measurements at once (entire chart):
```json
{
  "measurements": {
    "1": { ... },
    "2": { ... },
    ...
    "32": { ... }
  }
}
```

### GET /api/patients/:id/perio-charts/comparison
Compare two perio charts (trend analysis):
```json
// Query: ?chartId1=uuid1&chartId2=uuid2
{
  "chart1": { "examDate": "2023-07-10" },
  "chart2": { "examDate": "2024-01-15" },
  "teeth": {
    "14": {
      "pocketDepth": { "before": [3,5,4,3,4,3], "after": [2,3,3,2,3,2], "change": [-1,-2,-1,-1,-1,-1] },
      "attachmentLoss": { "before": [3,5,4,3,4,3], "after": [2,3,3,2,3,2] }
    }
  },
  "overallBopChange": { "before": 25.0, "after": 15.2, "change": -9.8 }
}
```

---

## Business Logic

### Attachment Loss Calculation
`Attachment Loss = Pocket Depth + Recession` (recession is positive for recession, negative for hyperplasia)

### BOP Percentage
`BOP% = (count of true BOP points / total measured BOP points) × 100`

### Risk Assessment (include in summary)
Based on pocket depths:
- Healthy: all depths ≤ 3mm
- Gingivitis: BOP present, depths ≤ 3mm
- Mild periodontitis: some depths 4–5mm
- Moderate periodontitis: depths ≥ 6mm at 1–2 sites
- Severe periodontitis: depths ≥ 6mm at ≥ 3 sites

---

## Tests Required

- `TreatmentPlanApiTests.cs`: CRUD + reorder + accept
- `PerioChartApiTests.cs`: create chart + bulk measurements + comparison
- `PerioCalculationTests.cs`: attachment loss, BOP%, risk assessment logic

---

## Acceptance Criteria

- [ ] Treatment plan CRUD operations work correctly
- [ ] Item reordering updates sort_order values correctly
- [ ] Plan totals (fee, insurance, patient portion) calculated correctly
- [ ] Perio chart creation and measurement entry work
- [ ] Bulk measurement submission atomically saves all tooth measurements
- [ ] Comparison endpoint returns correct delta values
- [ ] Attachment loss computed from recession + pocket depth
- [ ] BOP percentage calculated correctly
- [ ] All tests pass
