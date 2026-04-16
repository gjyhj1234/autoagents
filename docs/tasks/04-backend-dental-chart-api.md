# Task 04 — Backend: Dental Chart API

**GitHub Issue Title**: `[Task-04] Backend: Dental Chart and Tooth Conditions API`  
**Labels**: `agent-task`, `backend`  
**Priority**: 🔴 Critical  
**Depends On**: #2, #3

---

## Objective

Implement the backend API for the dental chart: loading a patient's complete chart, managing tooth conditions, and handling all condition types (caries, crowns, bridges, implants, root canals, etc.).

---

## API Endpoints

### GET /api/patients/:id/chart

Returns the patient's complete dental chart with all tooth conditions.

```json
{
  "id": "chart-uuid",
  "patientId": "patient-uuid",
  "dentitionMode": "permanent",
  "notationSystem": "universal",
  "teeth": {
    "1": {
      "toothNumber": "1",
      "toothName": "Upper Right Third Molar",
      "quadrant": "upper-right",
      "isAnterior": false,
      "status": "present",
      "conditions": [
        {
          "id": "cond-uuid",
          "conditionType": "decay",
          "surfaces": ["O", "M"],
          "status": "treatment_planned",
          "procedureCode": { "code": "D2392", "description": "Resin-based composite - two surfaces" },
          "provider": { "id": "...", "name": "Dr. Zhang Wei" },
          "fee": 280.00,
          "note": "Watch for further spread",
          "dateCompleted": null,
          "createdAt": "2024-01-15T09:30:00Z"
        }
      ]
    },
    "2": { ... },
    ...
    "32": { ... }
  },
  "bridges": [
    {
      "abutmentTeeth": ["3", "5"],
      "ponticTeeth": ["4"],
      "conditionIds": ["uuid1", "uuid2", "uuid3"]
    }
  ],
  "summary": {
    "totalConditions": 12,
    "treatmentPlanned": 5,
    "existing": 7,
    "missingTeeth": ["1", "16", "17", "32"],
    "teethWithConditions": ["3", "4", "5", "14", "19"]
  }
}
```

### PUT /api/patients/:id/chart/settings

Update chart settings:
```json
{ "dentitionMode": "mixed", "notationSystem": "fdi" }
```

### POST /api/patients/:id/chart/conditions

Add a tooth condition:
```json
{
  "toothNumber": "14",
  "surfaces": ["O", "D"],
  "conditionType": "decay",
  "status": "treatment_planned",
  "procedureCodeId": "uuid",
  "providerId": "uuid",
  "fee": 320.00,
  "note": ""
}
// Response 201
```

Validation rules:
- `toothNumber`: must be "1"–"32" (permanent) or "A"–"T" (primary)
- `surfaces`: subset of ["M", "O", "D", "F", "L", "R"] — empty array allowed for whole-tooth conditions
- `conditionType`: one of the defined types (see list below)
- Cannot add "missing" status if tooth already has non-missing conditions — must confirm

### PUT /api/patients/:id/chart/conditions/:conditionId

Update existing condition:
```json
{
  "status": "completed",
  "dateCompleted": "2024-02-10",
  "note": "Completed with composite"
}
```

### DELETE /api/patients/:id/chart/conditions/:conditionId

Soft delete condition. Returns 204.

### POST /api/patients/:id/chart/conditions/bulk

Apply same condition to multiple teeth/surfaces:
```json
{
  "teeth": ["2", "3", "4"],
  "surfaces": [],
  "conditionType": "sealant",
  "status": "completed",
  "procedureCodeId": "uuid-for-D1351",
  "fee": 50.00
}
// Response 201 — array of created conditions
```

### GET /api/patients/:id/chart/conditions/:conditionId/history

Audit trail for a specific condition:
```json
[
  {
    "action": "create",
    "changedBy": { "name": "Dr. Zhang" },
    "timestamp": "2024-01-15T09:30:00Z",
    "changes": { "status": { "from": null, "to": "treatment_planned" } }
  },
  {
    "action": "update",
    "changedBy": { "name": "Dr. Zhang" },
    "timestamp": "2024-02-10T14:00:00Z",
    "changes": { "status": { "from": "treatment_planned", "to": "completed" } }
  }
]
```

---

## Condition Types

The `conditionType` field must be one of:

```csharp
public static class ConditionType
{
    // Pathology
    public const string Decay = "decay";
    public const string Fracture = "fracture";
    public const string Watch = "watch";
    
    // Restorations (existing)
    public const string Amalgam = "amalgam";
    public const string Composite = "composite";
    public const string GlassIonomer = "glass_ionomer";
    
    // Crowns
    public const string CrownFullMetal = "crown_full_metal";
    public const string CrownPorcelainFusedMetal = "crown_pfm";
    public const string CrownAllCeramic = "crown_ceramic";
    public const string CrownStainlessSteel = "crown_stainless";
    public const string CrownTemporary = "crown_temporary";
    
    // Bridge
    public const string BridgeAbutment = "bridge_abutment";
    public const string BridgePontic = "bridge_pontic";
    
    // Endodontic
    public const string RootCanal = "root_canal";
    public const string PostAndCore = "post_and_core";
    public const string Apicectomy = "apicectomy";
    
    // Implant
    public const string Implant = "implant";
    public const string ImplantCrown = "implant_crown";
    
    // Preventive
    public const string Sealant = "sealant";
    
    // Cosmetic
    public const string Veneer = "veneer";
    
    // Prosthetic
    public const string PartialDenture = "partial_denture";
    public const string FullDenture = "full_denture";
    
    // Status
    public const string Missing = "missing";        // tooth is absent
    public const string Unerupted = "unerupted";   // tooth hasn't erupted
    public const string Impacted = "impacted";     // impacted tooth
    public const string RootFragment = "root_fragment";
    
    // Planned (if not using status field)
    public const string PlannedExtraction = "planned_extraction";
    public const string PlannedCrown = "planned_crown";
}
```

---

## Tooth Numbering Logic

Implement a `ToothNumberingService` that can:

1. **Convert between systems**: Universal (1–32) ↔ FDI (11–48) ↔ Palmer
2. **Get tooth metadata**: name, arch (upper/lower), quadrant, is_anterior, root_count
3. **Validate tooth number**: ensure it's valid for the given dentition mode

```csharp
public class ToothInfo
{
    public string UniversalNumber { get; set; }  // "14"
    public string FdiNumber { get; set; }         // "26"
    public string PalmerNotation { get; set; }    // "6|" (with quadrant symbol)
    public string Name { get; set; }              // "Upper Left First Molar"
    public string NameZh { get; set; }            // "左上第一磨牙"
    public string Arch { get; set; }              // "upper" | "lower"
    public string Quadrant { get; set; }          // "upper-right" | "upper-left" | etc.
    public bool IsAnterior { get; set; }
    public int RootCount { get; set; }            // 1, 2, or 3
    public bool IsPrimary { get; set; }
    public string PrimaryEquivalent { get; set; } // primary tooth letter if permanent
}
```

---

## Bridge Auto-Detection

When loading a chart, the API must automatically detect bridge spans:
- A bridge exists when consecutive teeth have `bridge_abutment` → `bridge_pontic`+ → `bridge_abutment` pattern
- Return bridge metadata in the chart response under the `bridges` array

---

## Tests Required

```csharp
ToothNumberingServiceTests.cs   // all conversion cases, edge cases
ChartApiTests.cs                // full CRUD + bulk operations
BridgeDetectionTests.cs         // bridge auto-detection logic
ConditionValidationTests.cs     // invalid surface, invalid tooth number
```

---

## Acceptance Criteria

- [ ] `GET /api/patients/:id/chart` returns all 32 teeth with their conditions
- [ ] Tooth metadata (name, arch, quadrant) included in response
- [ ] Condition CRUD operations work correctly
- [ ] Bulk condition creation applies to all specified teeth
- [ ] Bridge auto-detection identifies consecutive abutment-pontic patterns
- [ ] Condition history/audit trail records all changes
- [ ] All tooth number conversions (Universal, FDI, Palmer) work correctly
- [ ] Tests pass with ≥ 80% coverage on new code
