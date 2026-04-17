<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const activeMenu = computed(() => route.path)
</script>

<template>
  <el-container class="app-layout">
    <el-aside width="240px" class="app-layout__aside">
      <div class="app-layout__brand">
        <span class="app-layout__brand-mark">🦷</span>
        <div>
          <h1>DentalChart</h1>
          <p>Practice foundation</p>
        </div>
      </div>

      <el-menu :default-active="activeMenu" router class="app-layout__menu">
        <el-menu-item index="/">Dashboard</el-menu-item>
        <el-menu-item index="/patients">Patients</el-menu-item>
        <el-menu-item index="/treatment-plans">Treatment Plans</el-menu-item>
      </el-menu>
    </el-aside>

    <el-container>
      <el-header class="app-layout__header">
        <div>
          <h2>{{ route.meta.title ?? 'Dental Chart' }}</h2>
          <p>Vue 3 + TypeScript + Vite + Element Plus</p>
        </div>
      </el-header>

      <el-main class="app-layout__main">
        <RouterView />
      </el-main>
    </el-container>
  </el-container>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
}

.app-layout__aside {
  border-right: 1px solid #dcdfe6;
  background: #ffffff;
  padding: 1.5rem 1rem;
}

.app-layout__brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
}

.app-layout__brand h1 {
  font-size: 1.125rem;
  font-weight: 700;
}

.app-layout__brand p {
  color: #606266;
  font-size: 0.875rem;
}

.app-layout__brand-mark {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.5rem;
  height: 2.5rem;
  border-radius: 999px;
  background: #ecf5ff;
  font-size: 1.25rem;
}

.app-layout__menu {
  border-right: 0;
}

.app-layout__header {
  display: flex;
  align-items: center;
  padding: 1.5rem 2rem;
  background: #ffffff;
  border-bottom: 1px solid #dcdfe6;
}

.app-layout__header h2 {
  font-size: 1.375rem;
  font-weight: 700;
  margin-bottom: 0.25rem;
}

.app-layout__header p {
  color: #606266;
}

.app-layout__main {
  padding: 2rem;
}
</style>
