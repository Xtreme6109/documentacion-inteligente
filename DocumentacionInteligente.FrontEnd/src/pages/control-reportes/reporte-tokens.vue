<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">
      Reporte de Consumo de Tokens por Usuario
    </div>

    <div class="flex flex-wrap q-gutter-md">
      <!-- Selector Usuario -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-select
          v-model="usuarioSeleccionado"
          :options="usuarios"
          label="Seleccionar Usuario"
          option-label="nombre"
          option-value="id"
          outlined
          emit-value
          map-options
        />
      </div>

      <!-- Fecha Inicio -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaInicio"
          label="Fecha de Inicio"
          type="date"
          outlined
        />
      </div>

      <!-- Fecha Fin -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaFin"
          label="Fecha de Fin"
          type="date"
          outlined
        />
      </div>
    </div>

    <q-btn
      color="primary"
      label="Generar Reporte"
      @click="generarReporte"
      class="q-mt-md full-width"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { useQuasar } from 'quasar'

const $q = useQuasar()

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

const usuarios = ref([])
const usuarioSeleccionado = ref(null)
const fechaInicio = ref('')
const fechaFin = ref('')

async function cargarUsuarios() {
  try {
    const res = await api.get('/User/usuarios-por-rol')
    usuarios.value = res.data
    if (usuarios.value.length === 1) {
      usuarioSeleccionado.value = usuarios.value[0].id
    }
  } catch (error) {
    console.error('Error al cargar usuarios:', error)
    $q.notify({ type: 'negative', message: 'Error al cargar usuarios.' })
  }
}

onMounted(() => {
  cargarUsuarios()
})

async function generarReporte() {
  if (!usuarioSeleccionado.value) {
    $q.notify({ type: 'negative', message: 'Por favor, seleccione un usuario.' })
    return
  }
  if (!fechaInicio.value || !fechaFin.value) {
    $q.notify({ type: 'negative', message: 'Por favor, seleccione fechas de inicio y fin.' })
    return
  }

  try {
    const response = await api.post('/Reporte/reporte-consumo-tokens', {
      UsuarioId: usuarioSeleccionado.value,
      FechaInicio: fechaInicio.value,
      FechaFin: fechaFin.value
    }, { responseType: 'blob' })

    const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
    window.open(url, '_blank')
    window.URL.revokeObjectURL(url)

  } catch (error) {
    console.error('Error al generar reporte:', error)
    $q.notify({ type: 'negative', message: 'Error al generar reporte.' })
  }
}
</script>
