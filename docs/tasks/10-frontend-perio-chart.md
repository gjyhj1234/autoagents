# Task 10 — Frontend: Periodontal Chart

**GitHub Issue Title**: `[Task-10] Frontend: Periodontal Chart Grid and Trend Visualization`  
**Labels**: `agent-task`, `frontend`  
**Priority**: 🟠 High  
**Depends On**: #5, #7

---

## Objective

Build the periodontal chart data-entry grid (6 measurements per tooth × 32 teeth), trend comparison chart, and BOP/furcation/mobility indicators.

---

## Component Structure

```
PerioChartView.vue
  ├── PerioChartHeader.vue      ← Exam date, provider, notes; chart selector
  ├── PerioGrid.vue             ← Main data entry grid
  │   ├── UpperArchPerio.vue    ← Teeth 1-16
  │   └── LowerArchPerio.vue   ← Teeth 17-32
  ├── PerioSummary.vue          ← BOP%, mean PD, risk assessment
  └── PerioTrendChart.vue       ← ECharts line graph: current vs previous
```

---

## PerioGrid.vue — The Data Entry Table

The grid is a complex table. Layout for UPPER arch (teeth 1–16):

```
Tooth #:     1    2    3    4    5    6    7    8    9   10   11   12   13   14   15   16
             ───────────────────────────────────────────────────────────────────────────
Furcation: [  ] [  ] [B/L] [B/L] [  ] [  ] [  ] [  ] [  ] [  ] [  ] [  ] [  ] [B/L] [B/L] [  ]
             ───────────────────────────────────────────────────────────────────────────
Buccal:    DB B MB | DB B MB | ...  (3 cells per tooth, read right-to-left from patient view)
PD (mm):    2 3 2  |  2 2 2  | ...
BOP:       ○ ● ○  |  ○ ○ ○  | ...  (● = bleeding, ○ = not bleeding)
GingivalMargin: 0 0 0 | ...
AttachLoss:  2 3 2  |  2 2 2  | ...  (auto-calculated = PD + recession)
             ───────────────────────────────────────────────────────────────────────────
Lingual:   ML L DL | ML L DL | ...  (reversed: distal→lingual→mesial from patient view)
PD (mm):    2 2 3  |  2 2 2  | ...
BOP:       ○ ○ ●  |  ○ ○ ○  | ...
GingivalMargin: 0 0 0 | ...
AttachLoss:  2 2 3  | ...
             ───────────────────────────────────────────────────────────────────────────
Mobility:  [0] [0] [0] ...
```

Lower arch is mirrored (teeth 32→17 from left to right on screen, since patient is facing you).

### Cell Interaction
- Click pocket depth cell → select it, show input
- Type 1–9 (single digit) → move to next cell automatically
- Tab/Shift+Tab: move between cells
- Enter: save row and move down
- Color coding applied in real-time:
  - `≤3` → green background
  - `4–5` → yellow background
  - `≥6` → red background
- BOP cell: click to toggle (○/●)

### Furcation Input
For multi-rooted teeth (premolars 4,5,12,13,20,21,28,29 and molars 2,3,14,15,18,19,30,31):
- Show furcation cell with dropdown: — / I / II / III
- Separate values for buccal, lingual (and mesial for lower molars)

---

## PerioSummary.vue

After data entry, show real-time summary:

```
┌──────────────────────────────────────────────────────────┐
│ PERIODONTAL SUMMARY                                       │
│                                                          │
│  BOP:           15.2%  (18/118 bleeding points)         │
│  Mean PD:       2.8 mm                                   │
│  Pockets 4-5mm: 8 sites                                  │
│  Pockets ≥6mm:  2 sites                                  │
│  Mean AL:       2.9 mm                                   │
│                                                          │
│  Risk Level: 🟡 MILD PERIODONTITIS                        │
│  Recommendation: Periodontal maintenance every 3 months  │
└──────────────────────────────────────────────────────────┘
```

Risk levels with colors:
- 🟢 Healthy — all ≤3mm, BOP <10%
- 🟡 Gingivitis — BOP ≥10%, depths ≤3mm
- 🟡 Mild Periodontitis — some 4-5mm sites
- 🟠 Moderate Periodontitis — ≥6mm sites 1-2 teeth
- 🔴 Severe Periodontitis — ≥6mm at multiple sites

---

## PerioTrendChart.vue (ECharts)

Shows comparison between current exam and the selected previous exam:

- X-axis: tooth numbers (1–16 upper / 17–32 lower), switchable between arches
- Y-axis: pocket depth in mm (inverted — larger is worse, so high values go DOWN for visual intuition, or use standard orientation with red zone)
- Two line series: "2024-01-15 (current)" vs "2023-07-10 (previous)"
- Color: green = current, gray = previous
- Shaded area where pockets got worse (deeper) → red shading
- Shaded area where pockets improved → green shading
- Toggle: show all 6 points per tooth, or show max per tooth

---

## Chart Date Selector

```
PERIODONTAL EXAM

[ ← Prev Exam ] [ 2024-01-15 ▼ ] [ Next Exam → ]   [ + New Exam ]   [ Compare with: 2023-07-10 ▼ ]
```

- Dropdown shows all available exam dates
- [+ New Exam] creates a new perio chart and switches to it
- [Compare with] dropdown for trend comparison

---

## Print Layout

Print/export perio chart:
- Full grid displayed (all teeth, all measurements)
- Color coding preserved
- Summary section at bottom
- Patient info header
- Provider and date
- Formatted for A4 landscape

---

## Perio Store (`src/stores/perioChart.ts`)

```typescript
export const usePerioChartStore = defineStore('perioChart', () => {
  const charts = ref<PerioChartSummary[]>([])
  const activeChart = ref<PerioChartDetail | null>(null)
  const compareChart = ref<PerioChartDetail | null>(null)
  const isDirty = ref(false)
  
  async function loadCharts(patientId: string): Promise<void> { ... }
  async function loadChart(patientId: string, chartId: string): Promise<void> { ... }
  async function createChart(patientId: string, examDate: string, providerId: string): Promise<void> { ... }
  async function updateMeasurement(toothNumber: string, measurement: PerioMeasurement): Promise<void> { ... }
  async function saveAll(): Promise<void> { 
    // Bulk save all dirty measurements
  }
  
  const summary = computed(() => {
    // Calculate BOP%, mean PD, risk level from activeChart
  })
  
  return { charts, activeChart, compareChart, isDirty, summary,
           loadCharts, loadChart, createChart, updateMeasurement, saveAll }
})
```

---

## Acceptance Criteria

- [ ] Grid displays all 32 teeth (16 upper + 16 lower) with 6 cells per tooth (buccal + lingual)
- [ ] Single-digit input auto-advances to next cell
- [ ] Pocket depth cells color-coded in real-time (green/yellow/red)
- [ ] BOP toggle works (click to bleeding/not bleeding)
- [ ] Attachment loss auto-calculated and displayed
- [ ] Furcation dropdowns show only for multi-rooted teeth
- [ ] Mobility input works (0–3)
- [ ] Summary section updates in real-time as data entered
- [ ] Risk level assessment displayed with color
- [ ] Trend chart shows two exams on same chart with ECharts
- [ ] Date selector switches between exam dates
- [ ] New exam creation works
- [ ] Print layout correct
- [ ] Tests for: PerioGrid, perio store calculations
