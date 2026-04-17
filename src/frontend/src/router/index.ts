import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'dashboard',
      component: () => import('@/views/DashboardView.vue'),
      meta: { title: 'Project Dashboard' },
    },
    {
      path: '/patients',
      name: 'patients',
      component: () => import('@/views/PatientsView.vue'),
      meta: { title: 'Patients' },
    },
    {
      path: '/treatment-plans',
      name: 'treatment-plans',
      component: () => import('@/views/TreatmentPlansView.vue'),
      meta: { title: 'Treatment Plans' },
    },
  ],
})

export default router
