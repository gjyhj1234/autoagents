<template>
  <section class="dashboard-view">
    <el-row :gutter="16">
      <el-col :xs="24" :md="12" :lg="8">
        <el-card shadow="hover">
          <template #header>
            <span>Backend API</span>
          </template>
          <p>Minimal .NET 10 API scaffold with an OpenAPI-enabled health endpoint.</p>
        </el-card>
      </el-col>
      <el-col :xs="24" :md="12" :lg="8">
        <el-card shadow="hover">
          <template #header>
            <span>Frontend Shell</span>
          </template>
          <p>Vue 3, Pinia, Vue Router, Element Plus, and Axios are ready for feature work.</p>
        </el-card>
      </el-col>
      <el-col :xs="24" :md="12" :lg="8">
        <el-card shadow="hover">
          <template #header>
            <span>Infrastructure</span>
          </template>
          <p>Docker Compose and PostgreSQL scripts provide the initial delivery foundation.</p>
        </el-card>
      </el-col>
    </el-row>
  </section>
</template>

<style scoped>
.dashboard-view {
  display: grid;
  gap: 1rem;
}
</style>
