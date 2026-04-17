import { beforeEach, describe, expect, it } from 'vitest'
import { mount } from '@vue/test-utils'
import { createMemoryHistory, createRouter } from 'vue-router'
import ElementPlus from 'element-plus'

import AppLayout from '../AppLayout.vue'

const router = createRouter({
  history: createMemoryHistory(),
  routes: [
    { path: '/', component: { template: '<div>Dashboard page</div>' } },
    { path: '/patients', component: { template: '<div>Patients page</div>' } },
    { path: '/treatment-plans', component: { template: '<div>Treatment plans page</div>' } },
  ],
})

describe('AppLayout', () => {
  beforeEach(async () => {
    router.push('/')
    await router.isReady()
  })

  it('renders the primary navigation shell', () => {
    const wrapper = mount(AppLayout, {
      global: {
        plugins: [router, ElementPlus],
      },
    })

    expect(wrapper.text()).toContain('DentalChart')
    expect(wrapper.text()).toContain('Dashboard')
    expect(wrapper.text()).toContain('Patients')
    expect(wrapper.text()).toContain('Treatment Plans')
  })
})
