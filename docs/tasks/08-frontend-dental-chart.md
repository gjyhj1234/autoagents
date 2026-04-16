# Task 08 — Frontend: Dental Chart SVG Component

**GitHub Issue Title**: `[Task-08] Frontend: Interactive SVG Dental Chart Component`  
**Labels**: `agent-task`, `frontend`  
**Priority**: 🔴 Critical  
**Depends On**: #4, #7

---

## Objective

Build the core SVG-based interactive dental chart — the heart of the application. All 32 permanent teeth displayed anatomically with per-surface clickable segments, condition color overlays, and status indicators.

---

## Component Architecture

```
DentalChartView.vue                 ← Page: loads patient + chart data
  ├── ChartToolbar.vue              ← Mode toggle, zoom, print button
  ├── ToothChart.vue                ← Main SVG container (both arches)
  │   ├── UpperArch.vue             ← Teeth 1-16 (left to right: 1,2,...,16)
  │   │   └── ToothSVG.vue (×16)   ← Individual tooth
  │   └── LowerArch.vue            ← Teeth 17-32 (left to right: 32,31,...,17)
  │       └── ToothSVG.vue (×16)
  └── ToothDetailPanel.vue         ← Right panel: selected tooth conditions
      ├── ConditionList.vue         ← Existing conditions
      └── AddConditionForm.vue      ← Add new condition
```

---

## ToothSVG.vue — Detailed Specification

Each tooth is rendered as an inline SVG, approximately 40×65px:

```
  [Number label]  ← above for upper arch, below for lower arch
  ┌───────┐
  │   F   │  ← Facial/Buccal surface (top segment)
  ├─M─┼─D─┤  ← Mesial (left) | Occlusal (center) | Distal (right)
  │   L   │  ← Lingual surface (bottom segment)
  └───────┘
  [Root section — clickable for root canal, implant icons]
```

For anterior teeth (1-6, 11-16, 17-22, 27-32), the center surface is "Incisal" (I) not "Occlusal".

### SVG Structure Per Tooth

```vue
<template>
  <g class="tooth" :class="toothClasses" @contextmenu.prevent="onRightClick">
    <!-- Tooth number -->
    <text class="tooth-number">{{ displayNumber }}</text>
    
    <!-- Facial surface (top) -->
    <path class="surface surface-F" :fill="getSurfaceColor('F')" 
          @click="onSurfaceClick('F')" :aria-label="`Tooth ${number} Facial surface`" />
    
    <!-- Mesial surface (left) -->
    <path class="surface surface-M" :fill="getSurfaceColor('M')"
          @click="onSurfaceClick('M')" />
    
    <!-- Occlusal/Incisal (center) -->
    <rect class="surface surface-O" :fill="getSurfaceColor('O')"
          @click="onSurfaceClick('O')" />
    
    <!-- Distal surface (right) -->
    <path class="surface surface-D" :fill="getSurfaceColor('D')"
          @click="onSurfaceClick('D')" />
    
    <!-- Lingual surface (bottom) -->
    <path class="surface surface-L" :fill="getSurfaceColor('L')"
          @click="onSurfaceClick('L')" />
    
    <!-- Status overlays -->
    <g v-if="isMissing" class="status-missing">
      <line class="missing-x-1" />
      <line class="missing-x-2" />
    </g>
    
    <g v-if="hasImplant" class="status-implant">
      <!-- Screw icon in root area -->
    </g>
    
    <!-- Root canal lines (red lines through root) -->
    <g v-if="hasRootCanal" class="status-rct">
      <line class="rct-line" stroke="#E53E3E" />
    </g>
    
    <!-- Bridge connector line (rendered from parent UpperArch/LowerArch) -->
    
    <!-- Selection highlight -->
    <rect v-if="isSelected" class="selection-highlight" 
          fill="none" stroke="#3182CE" stroke-width="2" />
    
    <!-- Hover tooltip trigger -->
    <title>{{ tooltipText }}</title>
  </g>
</template>
```

### Props

```typescript
interface ToothSVGProps {
  toothNumber: string          // "1" to "32"
  conditions: ToothCondition[] // conditions on this tooth
  isSelected: boolean
  selectedSurfaces: ToothSurface[]
  dentitionMode: 'permanent' | 'primary' | 'mixed'
  notationSystem: 'universal' | 'fdi' | 'palmer'
  arch: 'upper' | 'lower'
  disabled?: boolean           // for non-existent primary teeth in mixed mode
}
```

### Events

```typescript
interface ToothSVGEmits {
  'surface-click': [toothNumber: string, surface: ToothSurface]
  'tooth-click': [toothNumber: string]
  'context-menu': [toothNumber: string, event: MouseEvent]
}
```

### Surface Color Calculation

```typescript
function getSurfaceColor(surface: ToothSurface): string {
  // Priority: if surface has conditions, use the highest-priority condition color
  // Priority order: decay > crown > planned > existing > sealant > watch > default
  
  const conditionsOnSurface = conditions.filter(c => 
    c.surfaces.includes(surface) || c.surfaces.length === 0 // whole-tooth
  )
  
  if (conditionsOnSurface.length === 0) return '#FFFFFF' // white = healthy
  
  // Use CONDITION_COLORS from copilot-instructions.md
  const topCondition = prioritize(conditionsOnSurface)
  return CONDITION_COLORS[topCondition.conditionType] ?? '#FFFFFF'
}
```

---

## Bridge Rendering

In `UpperArch.vue` and `LowerArch.vue`, detect bridge spans and render connector lines:

```typescript
// After rendering all tooth SVGs, render bridge connectors
bridgeSpans.forEach(bridge => {
  // Draw horizontal line connecting abutment teeth over pontic(s)
  // Position: middle-height of tooth, above the arch gap
})
```

---

## ChartToolbar.vue

```
[Permanent ▼]  [Universal ▼]  ──────  [-] [100%] [+]  ──  [🖨 Print]  [📤 Export PDF]
    ↑               ↑
  Dentition     Notation System
```

### Controls:
- Dentition mode selector: Permanent / Primary / Mixed
- Notation system: Universal (ADA) / FDI / Palmer  
- Zoom: -, percentage display, + (range 50%–200%)
- Print button → browser print with chart-only print CSS
- Export PDF button → calls PDF export utility

---

## ToothDetailPanel.vue

Right side panel, appears when a tooth is selected:

```
┌─────────────────────────────────┐
│ Tooth #14 — Upper Left 1st Molar│
│ FDI: 26 | Palmer: 6|            │
├─────────────────────────────────┤
│ EXISTING CONDITIONS             │
│ ┌─────────────────────────────┐ │
│ │ 🔴 Decay — Surfaces: O, D   │ │
│ │ Status: Treatment Planned   │ │
│ │ Code: D2392 | Fee: ¥320     │ │
│ │ Provider: Dr. Zhang         │ │
│ │ [Edit] [Delete]             │ │
│ └─────────────────────────────┘ │
├─────────────────────────────────┤
│ ADD NEW CONDITION               │
│ Condition type: [_________ ▼]   │
│ Surfaces: [M][O][D][F][L]       │
│ Procedure code: [Search... ]    │
│ Status: [Planned ▼]             │
│ Fee: [¥ _____ ]                 │
│ Note: [________________ ]       │
│ [Save Condition]                │
└─────────────────────────────────┘
```

---

## Context Menu

Right-click on tooth:
```
Tooth #14 — Upper Left 1st Molar
─────────────────────────────────
➕ Add Condition
📋 View History
📅 Schedule Appointment
──
⚠️ Set as Missing
🔲 Set as Impacted
──
📝 Add Note
```

---

## Dental Chart Store (`src/stores/dentalChart.ts`)

```typescript
export const useDentalChartStore = defineStore('dentalChart', () => {
  const chart = ref<DentalChart | null>(null)
  const selectedTooth = ref<string | null>(null)
  const selectedSurfaces = ref<ToothSurface[]>([])
  const isLoading = ref(false)
  
  async function loadChart(patientId: string): Promise<void> { ... }
  async function addCondition(condition: CreateConditionRequest): Promise<void> { ... }
  async function updateCondition(id: string, update: UpdateConditionRequest): Promise<void> { ... }
  async function deleteCondition(id: string): Promise<void> { ... }
  
  function selectTooth(toothNumber: string): void { ... }
  function toggleSurface(surface: ToothSurface): void { ... }
  
  return { chart, selectedTooth, selectedSurfaces, isLoading,
           loadChart, addCondition, updateCondition, deleteCondition,
           selectTooth, toggleSurface }
})
```

---

## Tests Required

```typescript
// src/components/dental-chart/__tests__/
ToothSVG.test.ts          // surface click events, color rendering, condition display
ToothChart.test.ts        // arch rendering, selection behavior
AddConditionForm.test.ts  // form validation, submit
useDentalChart.test.ts    // store actions, state changes
```

---

## Acceptance Criteria

- [ ] All 32 teeth rendered in correct anatomical positions (upper arch R→L, lower arch L→R from patient view)
- [ ] Each tooth has 5 clickable surface segments
- [ ] Clicking a surface highlights it and shows detail panel
- [ ] Conditions render with correct color-coded overlays per surface
- [ ] Missing/extracted teeth show X overlay
- [ ] Implant indicator appears in root area
- [ ] Root canal shows red lines through root
- [ ] Bridge connectors rendered between abutment teeth over pontics
- [ ] Right-click context menu with correct options
- [ ] Notation system toggle (Universal/FDI/Palmer) updates all tooth labels
- [ ] Dentition mode toggle switches between permanent/primary/mixed views
- [ ] Zoom control (50%–200%) works
- [ ] Keyboard navigation: arrow keys move between teeth
- [ ] Hover tooltip shows tooth name and current conditions
- [ ] All interactive elements have ARIA labels
- [ ] Component tests pass
