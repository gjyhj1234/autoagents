import { computed, ref } from 'vue'
import { defineStore } from 'pinia'

export const useAppStore = defineStore('app', () => {
  const applicationName = ref('DentalChart')
  const applicationTagline = ref('Production-ready dental practice foundation')

  const pageTitle = computed(() => `${applicationName.value} · ${applicationTagline.value}`)

  return {
    applicationName,
    applicationTagline,
    pageTitle,
  }
})
