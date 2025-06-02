<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onMounted(() => {
  localStorage.removeItem('token')  
  router.push('/')                 
})
</script>

<template>
  <div>Cerrando sesión...</div>
</template>
