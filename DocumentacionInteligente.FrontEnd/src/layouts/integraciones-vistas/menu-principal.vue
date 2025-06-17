<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />
        <q-toolbar-title> Documentación Inteligente </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <div class="column full-height">
        <q-list class="col">
            <q-item-label header> Menu Principal </q-item-label>
            <q-item clickable tag="router-link" to="/inicio">
            <q-item-section avatar>
              <q-icon name="home" />
            </q-item-section>
            <q-item-section>
              <q-item-label>Inicio</q-item-label>
            </q-item-section>
            </q-item>
          <GroupedLinks :links="linksList" />
        </q-list>

        <q-item clickable tag="router-link" to="/cerrar-sesion" class="q-mt-auto">
          <q-item-section avatar>
            <q-icon name="logout" />
          </q-item-section>
          <q-item-section>
            <q-item-label>Cerrar sesión</q-item-label>
          </q-item-section>
        </q-item>
      </div>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import GroupedLinks from 'src/components/componentes-generales/control-de-links-agrupados.vue'
import { useQuasar } from 'quasar'
import axios from 'axios'

const $q = useQuasar()
const leftDrawerOpen = ref(false)
const linksList = ref([])

const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})

api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => Promise.reject(error)
)

onMounted(async () => {
  try {
    const response = await api.get('/menu/menu-por-rol')
    linksList.value = response.data
    console.log('Menú cargado:', linksList.value)
  } catch (error) {
    console.error('Error al cargar el menú:', error)
    $q.notify({
      color: 'negative',
      message: 'Error al cargar menú',
      icon: 'error'
    })
  }
})

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
</script>
