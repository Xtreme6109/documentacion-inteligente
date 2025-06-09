<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">Reportes por Categoría</div>

    <div class="flex flex-wrap q-gutter-md">
      <!-- Categoría -->
      <div class="q-mb-md col-12 col-md col-sm">
        <q-select
          v-model="categoriaSeleccionada"
          :options="categorias"
          label="Seleccionar Categoría"
          option-label="nombre"
          option-value="id" 
          outlined
          emit-value
          map-options
          :disable="!categorias.length"
        />
      </div>

      <!-- Fecha de Inicio -->
      <div class="q-mb-md col-12 col-md col-sm">
        <q-input
          v-model="fechaInicio"
          label="Fecha de Inicio"
          type="date"
          outlined
        />
      </div>

      <!-- Fecha de Fin -->
      <div class="q-mb-md col-12 col-md col-sm">
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
      :disable="!categoriaSeleccionada || !fechaInicio || !fechaFin"
    />

    <q-dialog v-model="dialogVisible">
      <q-card style="min-width: 350px">
        <q-card-section>
          <div class="text-h6">Reporte de {{ nombreCategoriaSeleccionada }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section>
          <div class="text-body2">
            <p><strong>Categoría:</strong> {{ nombreCategoriaSeleccionada }}</p>
            <p><strong>Fecha de Inicio:</strong> {{ fechaInicio }}</p>
            <p><strong>Fecha de Fin:</strong> {{ fechaFin }}</p>
          </div>
        </q-card-section>

        <q-separator />

        <q-card-actions align="right">
          <q-btn flat label="Cerrar" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

import { useQuasar } from 'quasar'

const $q = useQuasar()

const categorias = ref([])
const categoriaSeleccionada = ref(null)
const fechaInicio = ref('')
const fechaFin = ref('')
const dialogVisible = ref(false)

// Computed para mostrar el nombre legible en el diálogo
const nombreCategoriaSeleccionada = computed(() => {
  const cat = categorias.value.find(c => c.id === categoriaSeleccionada.value)
  return cat ? cat.nombre : ''
})


// Crear instancia axios con baseURL
const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})

// Agregar interceptor para agregar el token en el header Authorization
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


async function cargarCategorias() {
  try {
    const resp = await api.get('/Categorias')  // Ruta correcta con mayúscula
    categorias.value = resp.data
  } catch (error) {
    console.error('Error al cargar categorías:', error.response?.data || error.message)
    // Opcional: mostrar mensaje de error con Quasar
    $q.notify({ type: 'negative', message: 'Error al cargar categorías' })
  }
}
function generarReporte() {
  if (!categoriaSeleccionada.value || !fechaInicio.value || !fechaFin.value) {
    alert("Por favor, complete todos los campos.")
    return
  }

  api.post('/reporte/reporte-categoria', {
    Categoria: categoriaSeleccionada.value,
    FechaInicio: fechaInicio.value,
    FechaFin: fechaFin.value
  }, { responseType: 'blob' })
    .then(response => {
      // Crear un URL blob para el PDF
      const url = window.URL.createObjectURL(new Blob([response.data], { type: 'application/pdf' }))
      // Abrir en nueva pestaña
      window.open(url, '_blank')
    })
    .catch(err => {
      alert('Error al generar reporte: ' + err.message)
    })
}



onMounted(() => {
  cargarCategorias()
})
</script>
