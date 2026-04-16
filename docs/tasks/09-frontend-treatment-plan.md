# Task 09 — Frontend: Treatment Planning UI

**GitHub Issue Title**: `[Task-09] Frontend: Treatment Planning Editor`  
**Labels**: `agent-task`, `frontend`  
**Priority**: 🟠 High  
**Depends On**: #5, #7, #8

---

## Objective

Build the complete treatment planning UI with drag-and-drop procedure ordering, fee calculations, status tracking, and plan printing.

---

## Component Structure

```
TreatmentPlanView.vue
  ├── TreatmentPlanList.vue        ← List of plans for patient
  │   └── TreatmentPlanCard.vue   ← Summary card per plan
  ├── TreatmentPlanEditor.vue     ← Full editor for one plan
  │   ├── PlanHeader.vue          ← Plan name, status, totals
  │   ├── VisitGroup.vue          ← Group by visit number (×N)
  │   │   └── ProcedureItem.vue   ← One draggable procedure row
  │   ├── AddProcedureModal.vue   ← Add procedure dialog
  │   └── PlanTotals.vue          ← Fee summary footer
  └── PlanPrintView.vue           ← Patient-facing printout layout
```

---

## TreatmentPlanList.vue

Shows all treatment plans for the patient:

```
[ + New Treatment Plan ]

┌─────────────────────────────────────────────────────────┐
│ 🟢 Comprehensive Phase 1                    [Active]    │
│ 8 procedures | ¥3,850 total | 2 completed               │
│ Created: 2024-01-10                                      │
│ [Open Plan] [Duplicate] [Delete]                        │
└─────────────────────────────────────────────────────────┘
┌─────────────────────────────────────────────────────────┐
│ ⚪ Orthodontic Consultation                [Inactive]   │
│ 3 procedures | ¥12,000 total | 0 completed              │
│ [Open Plan] [Activate] [Delete]                         │
└─────────────────────────────────────────────────────────┘
```

---

## TreatmentPlanEditor.vue

Full-screen (or full-panel) editor:

```
Plan: [ Comprehensive Phase 1 ______ ] Status: [Active ▼]  [Print Plan] [Accept Plan]
──────────────────────────────────────────────────────────────────────────────
VISIT 1                                                          [+ Add Procedure]
  ≡ #14 O,D | D2392 Composite 2-Surface | Planned | Dr. Zhang | ¥320 | ¥160 est. | [✎][🗑]
  ≡ #19 M,O | D2393 Composite 3-Surface | Planned | Dr. Zhang | ¥380 | ¥190 est. | [✎][🗑]

VISIT 2                                                          [+ Add Procedure]
  ≡ #3     | D2740 Crown Porcelain       | Planned | Dr. Zhang | ¥2,800| ¥1,400 est.| [✎][🗑]
  ≡ #30    | D3310 RCT Anterior          | In Progress| Dr. Li  | ¥800  | ¥400 est. | [✎][🗑]

[ + Add Visit Group ]
──────────────────────────────────────────────────────────────────────────────
                                    Total Fee:      ¥ 4,300.00
                                    Insurance Est.: ¥ 2,150.00
                                    Patient Portion:¥ 2,150.00
```

### Drag-and-Drop
- `≡` icon is the drag handle
- Items can be dragged within a visit group (reorders) or between visit groups (changes visit number)
- On drop → call `PUT /api/.../items/reorder`

### ProcedureItem.vue Inline Editing
Click [✎] to edit in place:
- Status dropdown
- Provider dropdown  
- Fee input
- Insurance estimate input (auto = 50% of fee by default, editable)
- Note text field
- [Save] [Cancel]

---

## AddProcedureModal.vue

Modal dialog for adding new procedure:

```
Add Procedure
─────────────────────────────────
Procedure Code: [ D2392 Composite ______ ]  ← autocomplete search
                  (type code or keyword)
Tooth: [ 14 ] Surfaces: [M][O][D][F][L]
Status: [ Planned ▼ ]
Provider: [ Dr. Zhang Wei ▼ ]
Visit #: [ 1 ▼ ]
Fee: [ ¥ 320.00 ]   Insurance Est.: [ ¥ 160.00 ]
Note: [_________________________________]
                            [Cancel] [Add Procedure]
```

Procedure code field features:
- Autocomplete: as user types, shows matching CDT codes from `GET /api/procedure-codes?search=`
- After selection, auto-fills fee from `defaultFee`

---

## PlanTotals.vue

Shows real-time totals (update as items change):

| | Total Fee | Insurance Est. | Patient Portion |
|--|--|--|--|
| Planned | ¥3,500 | ¥1,750 | ¥1,750 |
| Completed | ¥800 | ¥400 | ¥400 |
| **Grand Total** | **¥4,300** | **¥2,150** | **¥2,150** |

---

## Accept Plan Flow

[Accept Plan] button:
1. Opens modal: "Patient accepts this treatment plan"
2. Shows plan summary
3. Signature pad (using `signature_pad` npm package)
4. Date field (defaults to today)
5. [Confirm & Sign] → PATCH plan status to `accepted`, save signature

---

## PlanPrintView.vue

Patient-facing printout layout (triggered by browser print / jsPDF):

```
                    DENTAL TREATMENT PLAN
──────────────────────────────────────────────────────
Patient: John Smith                  Date: 2024-01-15
Provider: Dr. Zhang Wei              Plan: Phase 1

Procedure                    Tooth  Fee      Insurance  Your Cost
─────────────────────────────────────────────────────────────────
Composite Restoration 2-Surf.  #14  ¥320.00  ¥160.00   ¥160.00
Composite Restoration 3-Surf.  #19  ¥380.00  ¥190.00   ¥190.00
Porcelain Crown                 #3  ¥2,800   ¥1,400    ¥1,400
Root Canal Treatment           #30  ¥800.00  ¥400.00   ¥400.00
─────────────────────────────────────────────────────────────────
TOTAL                              ¥4,300   ¥2,150    ¥2,150

I have read and understand the above treatment plan.

Patient Signature: ___________________  Date: __________
```

---

## Treatment Plan Store (`src/stores/treatmentPlan.ts`)

```typescript
export const useTreatmentPlanStore = defineStore('treatmentPlan', () => {
  const plans = ref<TreatmentPlan[]>([])
  const activePlan = ref<TreatmentPlanDetail | null>(null)
  
  async function loadPlans(patientId: string): Promise<void> { ... }
  async function loadPlan(patientId: string, planId: string): Promise<void> { ... }
  async function createPlan(patientId: string, name: string): Promise<TreatmentPlan> { ... }
  async function addItem(patientId: string, planId: string, item: CreateItemRequest): Promise<void> { ... }
  async function updateItem(patientId: string, planId: string, itemId: string, update: UpdateItemRequest): Promise<void> { ... }
  async function deleteItem(patientId: string, planId: string, itemId: string): Promise<void> { ... }
  async function reorderItems(patientId: string, planId: string, itemIds: string[]): Promise<void> { ... }
  
  const totals = computed(() => {
    // Calculate totals from activePlan.items
  })
  
  return { plans, activePlan, totals, loadPlans, loadPlan, createPlan, addItem, updateItem, deleteItem, reorderItems }
})
```

---

## Acceptance Criteria

- [ ] Plan list shows all plans with correct status and counts
- [ ] New plan creation works
- [ ] Procedure items display with all fields
- [ ] Drag-and-drop reordering works (updates sort order in API)
- [ ] Items can be moved between visit groups
- [ ] Adding a procedure: autocomplete CDT code search works
- [ ] Fee auto-populates from selected procedure code
- [ ] Insurance estimate defaults to 50% of fee
- [ ] Real-time totals update as items are added/changed
- [ ] Status changes (Planned → Completed) work
- [ ] Accept plan modal with signature pad works
- [ ] Print layout renders correctly
- [ ] Component tests for: ProcedureItem, AddProcedureModal, PlanTotals
