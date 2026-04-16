# Task 07 — Frontend: Project Setup and Core Layout

**GitHub Issue Title**: `[Task-07] Frontend: Vue 3 Project Setup, Auth, and App Shell`  
**Labels**: `agent-task`, `frontend`  
**Priority**: 🔴 Critical  
**Depends On**: #1, #3

---

## Objective

Set up the Vue 3 frontend with authentication flow, app shell layout, patient list page, and the API client infrastructure.

---

## Deliverables

### 1. Project Configuration

`vite.config.ts`:
```typescript
export default defineConfig({
  plugins: [vue(), vueJsx()],
  resolve: { alias: { '@': fileURLToPath(new URL('./src', import.meta.url)) } },
  server: {
    proxy: {
      '/api': { target: 'http://localhost:8080', changeOrigin: true }
    }
  }
})
```

`tsconfig.json`: strict mode enabled.

### 2. API Client (`src/services/api.ts`)

```typescript
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'

const api = axios.create({ baseURL: '/api', timeout: 30000 })

// Request interceptor: attach JWT
api.interceptors.request.use((config) => {
  const auth = useAuthStore()
  if (auth.accessToken) {
    config.headers.Authorization = `Bearer ${auth.accessToken}`
  }
  return config
})

// Response interceptor: handle 401, refresh token, retry
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    if (error.response?.status === 401) {
      const auth = useAuthStore()
      const refreshed = await auth.refreshToken()
      if (refreshed) {
        return api.request(error.config)
      }
      auth.logout()
    }
    return Promise.reject(error)
  }
)

export default api
```

### 3. Auth Store (`src/stores/auth.ts`)

```typescript
export const useAuthStore = defineStore('auth', () => {
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))
  const user = ref<User | null>(JSON.parse(localStorage.getItem('user') || 'null'))
  
  const isAuthenticated = computed(() => !!accessToken.value)
  const isAdmin = computed(() => user.value?.role === 'admin')
  
  async function login(username: string, password: string): Promise<void> { ... }
  async function refreshToken(): Promise<boolean> { ... }
  function logout(): void { ... }
  
  return { accessToken, user, isAuthenticated, isAdmin, login, refreshToken, logout }
})
```

Persist access token and user info in `localStorage`. Clear on logout.

### 4. Router (`src/router/index.ts`)

```typescript
const routes: RouteRecordRaw[] = [
  { path: '/login', component: LoginView, meta: { requiresAuth: false } },
  {
    path: '/',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      { path: '', redirect: '/patients' },
      { path: 'patients', component: PatientListView },
      { path: 'patients/:id', component: PatientDetailView },
      { path: 'patients/:id/chart', component: DentalChartView },
      { path: 'patients/:id/perio', component: PerioChartView },
      { path: 'patients/:id/treatment', component: TreatmentPlanView },
      { path: 'calendar', component: CalendarView },
      { path: 'settings', component: SettingsView },
    ]
  }
]

// Navigation guard
router.beforeEach((to) => {
  const auth = useAuthStore()
  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return { path: '/login', query: { redirect: to.fullPath } }
  }
})
```

### 5. App Layout (`src/components/layout/AppLayout.vue`)

Left sidebar navigation:
```
🦷 DentalChart                    ← logo/brand
─────────────────
👥 Patients                       ← /patients
📅 Calendar                       ← /calendar  
─────────────────
⚙️  Settings                      ← /settings
─────────────────
👤 Current User Name
   Role: Dentist
   [Logout]
```

Main content area: `<router-view />`

Header bar: patient search box (global, quick search), current date/time.

### 6. Login Page (`src/views/LoginView.vue`)

- Username + password form
- Error message display (invalid credentials, account locked)
- Loading state on submit button
- Redirect to original URL after login (`?redirect=` param)

### 7. Patient List View (`src/views/PatientListView.vue`)

```
[ + New Patient ]                    [ 🔍 Search: ______________ ]

Name               DOB        Phone          Status    Provider    Actions
─────────────────────────────────────────────────────────────────────────
Smith, John        1985-03-15  555-0100       Active    Dr. Zhang   [Chart] [Edit]
Doe, Jane          1990-05-20  555-0200       Active    Dr. Li      [Chart] [Edit]
...

« Previous  Page 1 of 8  Next »
```

- Debounced search (300ms) hitting `GET /api/patients?search=`
- Filters: status, provider
- Sort by column headers
- Clicking [Chart] navigates to `/patients/:id/chart`
- [+ New Patient] opens modal/side panel

### 8. New Patient Modal

Form with fields: first name, last name, DOB (date picker), gender, mobile phone, email.  
Submit → `POST /api/patients` → redirect to patient detail.

### 9. Patient Detail View (`src/views/PatientDetailView.vue`)

Tab navigation:
```
[📋 Overview] [🦷 Dental Chart] [📊 Perio] [📝 Treatment Plan] [📅 Appointments]
```

Overview tab shows: demographics, medical alerts (red/orange banners), insurance summary, recent appointments.

### 10. TypeScript Types (`src/types/`)

```typescript
// src/types/patient.ts
export interface Patient { id: string; firstName: string; lastName: string; ... }
export interface MedicalHistory { hasDiabetes: boolean; alertFlags: AlertFlag[]; ... }

// src/types/api.ts
export interface PagedResponse<T> { data: T[]; pagination: Pagination }
export interface ApiError { error: string; message: string }
```

---

## Acceptance Criteria

- [ ] `/login` page works: successful login redirects to `/patients`
- [ ] Failed login shows error message
- [ ] Unauthenticated users redirected to `/login`
- [ ] Patient list shows with search, filtering, and pagination
- [ ] Patient search debounced (no request per keypress)
- [ ] New patient creation works end-to-end
- [ ] App layout sidebar navigation works for all routes
- [ ] JWT refresh token flow works (auto-refresh on 401)
- [ ] Logout clears state and redirects to `/login`
- [ ] TypeScript strict mode: no `any` types, no errors
- [ ] Unit tests for auth store: login, logout, token refresh
