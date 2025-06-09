<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h5">Extraer Puntos Clave</div>
        <div class="text-subtitle2">Selecciona un documento y obtén los puntos clave generados por IA</div>
      </q-card-section>

      <q-card-section class="q-gutter-md">
        <q-select
          v-model="documentoSeleccionado"
          :options="listaDocumentos"
          :option-label="docLabel"
          option-value="id"
          label="Selecciona un documento"
          filled
          emit-value
          map-options
          :rules="[val => !!val || 'Debes seleccionar un documento']"
        />

        <div v-if="documentoSeleccionado">
          <q-btn
            label="Ver documento"
            icon="attach_file"
            color="secondary"
            flat
            class="q-mb-md"
            :href="urlDocumentoSeleccionado"
            target="_blank"
          />

          <q-btn
            label="Extraer puntos clave"
            color="primary"
            :disable="cargando"
            :loading="cargando"
            @click="extraerPuntosClave"
          />
        </div>

        <q-separator spaced />

        <q-card v-if="puntosClave" flat bordered class="q-pa-md q-mt-md">
          <div class="text-h5">Puntos Clave</div>
          <div class="puntos-clave" v-html="htmlPuntosClave"></div>
        </q-card>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'

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

const $q = useQuasar()

const listaDocumentos = ref([])
const documentoSeleccionado = ref(null)
const puntosClave = ref('')
const cargando = ref(false)

const urlDocumentoSeleccionado = computed(() => {
  const doc = listaDocumentos.value.find(d => d.id === documentoSeleccionado.value)
  return doc ? `http://localhost:5168/${doc.rutaArchivo}` : '#'
})

function docLabel(doc) {
  return `${doc.titulo} - v${doc.versionActual}`
}

onMounted(async () => {
  try {
    const res = await api.get('/Documentos/AllDocuments')
    listaDocumentos.value = res.data
  } catch (error) {
    console.error('Error al obtener documentos:', error)
    $q.notify({ type: 'negative', message: 'Error al cargar documentos' })
  }
})

async function extraerPuntosClave() {
  if (!documentoSeleccionado.value) return;

  cargando.value = true;
  puntosClave.value = '';

  try {
    // 1. Obtener el texto completo del documento
    const resTexto = await api.get(`/Documentos/TextoDocumento/${documentoSeleccionado.value}`);
    const textoDocumento = resTexto.data?.texto || resTexto.data; // ajustar si la propiedad es directa

    if (!textoDocumento) {
      throw new Error('El documento no contiene texto válido.');
    }

    // 2. Enviar el texto a la API para extraer los puntos clave
    const res = await api.post('/PuntosClave/extraer', { texto: textoDocumento });

    puntosClave.value = res.data?.puntosClave || 'No se extrajeron puntos clave.';
  } catch (err) {
    console.error('Error al extraer puntos clave:', err);
    $q.notify({ type: 'negative', message: 'Error al extraer puntos clave' });
  } finally {
    cargando.value = false;
  }
}

const htmlPuntosClave = computed(() => {
  if (!puntosClave.value) return ''

  let texto = puntosClave.value

  // Escape HTML por seguridad (opcional)
  // texto = texto.replace(/</g, '&lt;').replace(/>/g, '&gt;')

  // Convertir títulos principales **texto** => <h2>texto</h2>
  texto = texto.replace(/\*\*(.+?)\*\*/g, '<h4>$1</h4>')

  // Convertir subtítulos *texto* => <h3>texto</h3>
  texto = texto.replace(/\*(.+?)\*/g, '<h5>$1</h5>')

  // Procesar viñetas en listas:
  // Dividir por saltos de línea
  const lineas = texto.split('\n')

  let htmlFinal = ''
  let enLista = false

  lineas.forEach(linea => {
    const trimmed = linea.trim()
    if (trimmed.startsWith('-')) {
      if (!enLista) {
        enLista = true
        htmlFinal += '<ul>'
      }
      // Quitar el guion y espacio inicial
      const contenido = trimmed.substring(1).trim()
      htmlFinal += `<li>${contenido}</li>`
    } else {
      if (enLista) {
        htmlFinal += '</ul>'
        enLista = false
      }
      // Si línea vacía, poner salto, si no, párrafo simple
      if (trimmed === '') {
        htmlFinal += '<br/>'
      } else {
        htmlFinal += `<p>${trimmed}</p>`
      }
    }
  })

  // Cerrar lista si queda abierta
  if (enLista) htmlFinal += '</ul>'

  return htmlFinal
})


</script>

<style scoped>
.puntos-clave ul {
  padding-left: 1.25em;
  list-style-type: disc;
}

.puntos-clave strong {
  font-weight: bold;
  font-size: 1.1em;
}
</style>
