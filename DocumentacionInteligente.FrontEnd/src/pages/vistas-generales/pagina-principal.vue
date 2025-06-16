<template>
  <q-page class="q-pa-md">
    <div
      class="grid-container"
      style="display: grid; grid-template-columns: 1fr 1fr; grid-template-rows: 1fr 1fr; gap: 16px; height: 70vh;"
    >
      <q-card class="q-pa-md" @click="GraficosCategoriaLink">
        <q-card-section>
          <canvas ref="chart1" style="height: 250px;"></canvas>
        </q-card-section>
      </q-card>

      <q-card class="q-pa-md" @click="GraficosTokenLink">
        <q-card-section>
          <canvas ref="chart4" style="height: 250px;"></canvas>
        </q-card-section>
      </q-card>

      <q-card class="q-pa-md" @click="GraficosFechaLink">
        <q-card-section>
          <canvas ref="chart3" style="height: 250px;"></canvas>
        </q-card-section>
      </q-card>

      <q-card class="q-pa-md" @click="GraficosEstadoLink">
        <q-card-section>
          <canvas ref="chart2" style="height: 100px;"></canvas>
        </q-card-section>
      </q-card>
    </div>
  </q-page>
</template>


<script setup>
import { ref, onMounted } from 'vue'
import Chart from 'chart.js/auto'
import axios from 'axios'
import { useQuasar } from 'quasar'

// Quasar y Axios
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

// Referencias a los canvas
const chart1 = ref(null)
const chart2 = ref(null)
const chart3 = ref(null)
const chart4 = ref(null)

// Funciones de agrupación
function agruparPorCategoria(documentos) {
  const conteo = {}
  for (const doc of documentos) {
    const key = (doc.nombreCategoria || 'Desconocida')
    conteo[key] = (conteo[key] || 0) + 1
  }
  return conteo
}

function agruparPorEstado(documentos) {
  const conteo = {}
  for (const doc of documentos) {
    const key = doc.estado || 'Desconocido'
    conteo[key] = (conteo[key] || 0) + 1
  }
  return conteo
}

function agruparPorFecha(documentos) {
  const conteo = {}
  for (const doc of documentos) {
    const fecha = new Date(doc.createDate).toLocaleDateString()
    conteo[fecha] = (conteo[fecha] || 0) + 1
  }
  return conteo
}

onMounted(async () => {
  try {
    const documentos = (await api.get('/Documentos/AllDocuments')).data

    // 1) Documentos por Categoría
    const porCategoria = agruparPorCategoria(documentos)
    new Chart(chart1.value, {
      type: 'bar',
      data: {
        labels: Object.keys(porCategoria),
        datasets: [{
          label: 'Documentos',
          data: Object.values(porCategoria),
          backgroundColor: ['black', 'orange', 'purple']
        }]
      }
    })

    // 2) Documentos por Estado
    const porEstado = agruparPorEstado(documentos)
    new Chart(chart2.value, {
      type: 'pie',
      data: {
        labels: Object.keys(porEstado),
        datasets: [{
          label: 'Documentos',
          data: Object.values(porEstado),
          backgroundColor: ['cyan', 'yellow', 'red']
        }],
        options: {
          maintainAspectRatio: false,
          responsive: true
        }
      }
    })

    // 3) Documentos por Fecha
    const porFecha = agruparPorFecha(documentos)
    new Chart(chart3.value, {
      type: 'line',
      data: {
        labels: Object.keys(porFecha),
        datasets: [{
          label: 'Documentos por fecha',
          data: Object.values(porFecha),
          fill: false,
          borderColor: 'orange'
        }]
      }
    })

    // 4) Tokens por usuario
    const tokensUsu = (await api.get('/Reporte/TokensPorUsuario')).data
    new Chart(chart4.value, {
      type: 'bar',
      data: {
        labels: tokensUsu.map(x => x.nombreUsuario),
        datasets: [{
          label: 'Tokens Totales',
          data: tokensUsu.map(x => x.tokensTotales),
          backgroundColor: 'purple'
        }]
      }
    })

  } catch (error) {
    console.error('Error cargando los gráficos:', error)
    $q.notify({ type: 'negative', message: 'Error al cargar gráficos' })
  }
})

function GraficosCategoriaLink(){
  window.location.href = 'http://localhost:5168/Graficos/GraficosCategoria';
}

function GraficosEstadoLink(){
  window.location.href = 'http://localhost:5168/Graficos/GraficosEstado';
}

function GraficosFechaLink(){
  window.location.href = 'http://localhost:5168/Graficos/GraficosFecha';
}

function GraficosTokenLink(){
  window.location.href = 'http://localhost:5168/Graficos/GraficosTokens';
}
</script>
