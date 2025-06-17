<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">
      Reportes por Usuario
    </div>

    <!-- Filtro de selección usando divs y clases flexbox -->
    <div class="flex flex-wrap q-gutter-md">
      <!-- Usuario -->
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

      <!-- Fecha de Inicio -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaInicio"
          label="Fecha de Inicio"
          type="date"
          outlined
        />
      </div>

      <!-- Fecha de Fin -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaFin"
          label="Fecha de Fin"
          type="date"
          outlined
        />
      </div>
    </div>

    <!-- Botón para generar el reporte -->
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
      config.headers.Authorization = `Bearer ${token}`  // Esto es correcto
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
    // Si solo hay un usuario (normal), seleccionarlo automáticamente
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
    $q.notify({ type: 'negative', message: 'Por favor, seleccione un usuario.' });
    return;
  }

  if (!fechaInicio.value || !fechaFin.value) {
    $q.notify({ type: 'negative', message: 'Por favor, seleccione fechas de inicio y fin.' });
    return;
  }

  // Construir objeto con todas las propiedades (aunque vacías)
  const payload = {
  títuloDelDocumento: "",
  fechaDeEdición: new Date().toISOString(),
  version: "",
  códigoDelDocumento: "",
  elaboradoPor: "",
  revisadoPor: "",
  iObjetivo: "",
  iiAlcance: "",
  iiiResponsabilidades: {
    texto: "",
    lista: [],
    objeto: {
      "nodo1": {
        texto: "",
        lista: [],
        objeto: {}
      },
      "nodo2": {
        texto: "",
        lista: [],
        objeto: {}
      }
    }
  },
  ivDesarrollo: {
    texto: "",
    lista: [],
    objeto: {
      "nodo1": {
        texto: "",
        lista: [],
        objeto: {}
      },
      "nodo2": {
        texto: "",
        lista: [],
        objeto: {}
      }
    }
  },
  vVigencia: "",
  viReferenciasBibliográficas: "",
  viiHistorialDeCambioDeDocumentos: [
    {
      number: 0,
      date: new Date().toISOString(),
      description: ""
    }
  ],
  viiiFirmas: "",
  titulo: "",
  hoja: 0,
  totalHojas: 0,
  autorizadoPor: "",
  fechaDivulgacion: new Date().toISOString(),
  categoria: 0,
  nombreCategoria: "",
  usuarioCreadorId: 0,
  usuarioId: usuarioSeleccionado.value,
  fechaInicio: fechaInicio.value,
  fechaFin: fechaFin.value,
  nombreUsuarioCreador: ""
};

console.log('Payload para reporte:', payload);
  try {
    const response = await api.post('/Reporte/reporte-usuario', payload, {
      responseType: 'blob'
    });

    const pdfBlob = new Blob([response.data], { type: 'application/pdf' });
    const pdfUrl = URL.createObjectURL(pdfBlob);
    window.open(pdfUrl, '_blank');
    URL.revokeObjectURL(pdfUrl);

  } catch (error) {
    console.error('Error al generar reporte:', error);
    $q.notify({ type: 'negative', message: 'Error al generar reporte.' });
  }
}

</script>
