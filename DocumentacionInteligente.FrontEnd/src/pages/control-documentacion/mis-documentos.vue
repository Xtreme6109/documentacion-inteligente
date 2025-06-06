<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Mis documentos</div>
      </q-card-section>

      <q-separator />

      <!-- Filtros -->
      <q-card-section class="q-gutter-md row items-center">
        <q-input v-model="filtros.titulo" label="Buscar por título" dense clearable />
        <q-select v-model="filtros.categoria" :options="categorias" label="Categoría" dense clearable emit-value map-options />
        <q-select v-model="filtros.estado" :options="estados" label="Estado" dense clearable />
        <q-checkbox v-model="filtros.creadoIA" label="Solo creados con IA" />
        <q-btn label="Buscar" color="primary" @click="buscar" />
      </q-card-section>

      <q-separator />

      <!-- Tabla -->
      <q-card-section>
        <q-table
          :rows="documentosFiltrados"
          :columns="columnas"
          row-key="ID"
          flat
          dense
          :filter="filtros.titulo"
        >
          <template #body-cell-CREADO_IA="props">
            <q-td>{{ props.row.CREADO_IA ? 'Sí' : 'No' }}</q-td>
          </template>
          <template #body-cell-acciones="props">
            <q-td>
              <q-btn
                dense
                flat
                icon="visibility"
                :disable="esDocx(props.row.rutaArchivo)"
                @click="verDocumento(props.row)"
              />
              <q-btn dense flat icon="edit" @click="abrirModal(props.row)" />
              <q-btn dense flat icon="download" @click="descargarArchivo(props.row)" />
              <q-btn dense flat icon="delete" color="negative" @click="eliminarDocumento(props.row)" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>

    <!-- Modal -->
    <q-dialog v-model="modalAbierto">
      <q-card style="min-width: 500px; max-width: 90vw;">
        <q-card-section>
          <div class="text-h6">{{ doc.ID ? 'Editar documento' : 'Nuevo documento' }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section class="q-gutter-md">
          <q-input v-model="doc.TITULO" label="Título" outlined dense :rules="[val => !!val || 'Requerido']" />
          <q-input v-model="doc.DESCRIPCION" label="Descripción" type="textarea" outlined dense :rules="[val => !!val || 'Requerido']" />
          <q-select v-model="doc.CATEGORIA" :options="categorias" label="Categoría" outlined dense emit-value map-options :rules="[val => !!val || 'Requerido']" />
          <q-select v-model="doc.ESTADO" :options="estados" label="Estado" outlined dense :rules="[val => !!val || 'Requerido']" />
          <q-toggle v-model="doc.CREADO_IA" label="¿Creado por IA?" />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey" v-close-popup />
          <q-btn label="Guardar" color="primary" @click="guardarDocumento" />
        </q-card-actions>
      </q-card>
    </q-dialog>


    <q-dialog v-model="modalVisible" persistent>
      <q-card style="min-width: 60%; max-width: 90%; min-height: 80%;">
        <q-card-section class="q-pa-md row q-col-gutter-md q-gutter-y-md flex-wrap">
          <div class="col-12 row items-center justify-between q-mb-sm">
            <div class="text-h5 text-weight-bold">
              {{ documentoSeleccionado?.titulo }}
            </div>
            <q-btn
              flat
              dense
              round
              icon="close"
              color="primary"
              @click="modalVisible = false"
              aria-label="Cerrar"
              class="q-ml-auto"
            />
          </div>

          <q-separator spaced class="col-12" />

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="primary" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Descripción
            </q-badge>
            <span class="text-body1">{{ documentoSeleccionado?.descripcion || '-' }}</span>
          </div>

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="secondary" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Categoría
            </q-badge>
            <span class="text-body1">{{ obtenerNombreCategoria(documentoSeleccionado?.categoria_id) || '-' }}</span>
          </div>

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="accent" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Estado
            </q-badge>
            <span class="text-body1">{{ documentoSeleccionado?.estado || '-' }}</span>
          </div>

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="teal" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Creado por IA
            </q-badge>
            <span class="text-body1">{{ documentoSeleccionado?.creado_ia ? 'Sí' : 'No' }}</span>
          </div>

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="grey-8" text-color="white" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Fecha creación
            </q-badge>
            <span class="text-body1">
              {{ documentoSeleccionado?.creatE_DATE ? new Date(documentoSeleccionado.creatE_DATE).toLocaleString() : '-' }}
            </span>
          </div>

          <div class="q-mb-xs col-xs-12 col-sm-6 col-md-4">
            <q-badge color="deep-orange" class="q-mr-sm text-subtitle2 q-pa-sm rounded-borders">
              Versión
            </q-badge>
            <span class="text-body1">{{ documentoSeleccionado?.versioN_ACTUAL || '-' }}</span>
          </div>
        </q-card-section>



        <q-card-section>
          <iframe
            v-if="documentoSeleccionado?.rutA_ARCHIVO"
            :src="obtenerURLVisualizacion(documentoSeleccionado.rutA_ARCHIVO)"
            width="100%"
            height="600px"
            frameborder="0"
          />

          <div v-else class="text-grey">No hay documento disponible.</div>
        </q-card-section>


      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup>
import { ref, computed,onMounted } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'

const $q = useQuasar()
const modalVisible = ref(false)
const documentoSeleccionado = ref(null)

// Axios con configuración de baseURL y token
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

// Filtros
const filtros = ref({
  titulo: '',
  categoria: null,
  estado: null,
  creadoIA: false
})

function esDocx(rutaArchivo) {
  console.log(rutaArchivo)
  if (!rutaArchivo) return false
  const ext = rutaArchivo.split('.').pop().toLowerCase()
  return ext === 'docx'
}

const documentosFiltrados = computed(() => {
  return documentos.value.filter(doc => {
    if (filtros.value.titulo && !doc.titulo.toLowerCase().includes(filtros.value.titulo.toLowerCase())) {
      return false
    }
    if (filtros.value.categoria && doc.categoria !== filtros.value.categoria) {
      return false
    }
    if (filtros.value.estado && doc.estado !== filtros.value.estado) {
      return false
    }
    if (filtros.value.creadoIA && !doc.creadoIA) {
      return false
    }
    return true
  })
})

const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 },
  { label: 'Interno', value: 4 },
  { label: 'Financiero', value: 5 },
]

const estados = ['Borrador', 'Aprobado', 'Rechazado']

const columnas = [
  { name: 'titulo', label: 'Título', field: 'titulo', sortable: true },
  { name: 'descripcion', label: 'Descripción', field: 'descripcion' },
  { name: 'categoriaLabel', label: 'Categoría', field: 'categoriaLabel' },
  { name: 'createDate', label: 'Creado el', field: 'createDate', format: val => new Date(val).toLocaleDateString() },
  { name: 'estado', label: 'Estado', field: 'estado' },
  { name: 'versionActual', label: 'Versión', field: 'versionActual' },
  { name: 'creadoIA', label: 'IA', field: 'creadoIA', format: val => val ? 'Sí' : 'No' },
  { name: 'acciones', label: 'Acciones', field: 'id' }
]


const documentos = ref([])
const modalAbierto = ref(false)
const doc = ref({})

async function fetchDocumentos() {
  try {
    const res = await api.get('/Documentos/AllDocuments')
    documentos.value = res.data.map(d => ({
      ...d,
      CATEGORIA_LABEL: obtenerNombreCategoria(d.categoria)
    }))
  } catch (err) {
    $q.notify({ type: 'negative', message: 'Error al cargar documentos: ' + err.message })
  }
}

function obtenerNombreCategoria(categoriaId) {
  const cat = categorias.find(c => c.value === categoriaId)
  return cat?.label || 'Sin categoría'
}

function buscar() {
  $q.notify({ message: 'Filtrado (simulado)', color: 'primary' })
}

function abrirModal(documento = null) {
  doc.value = documento
    ? { ...documento }
    : {
        TITULO: '',
        DESCRIPCION: '',
        CATEGORIA: null,
        ESTADO: '',
        CREADO_IA: false
      }
  modalAbierto.value = true
}

function obtenerURLVisualizacion(ruta) {
  const base = `http://localhost:5168/${ruta}`
  const ext = ruta.split('.').pop().toLowerCase()
  const encoded = encodeURIComponent(base)

  if (ext === 'pdf') {
    return base
  } else if (ext === 'docx' || ext === 'doc') {
    return `https://view.officeapps.live.com/op/embed.aspx?src=${encoded}`
  } else {
    return ''
  }
}

function guardarDocumento() {
  if (!doc.value.TITULO || !doc.value.DESCRIPCION || !doc.value.CATEGORIA || !doc.value.ESTADO) {
    $q.notify({ type: 'negative', message: 'Faltan campos requeridos' })
    return
  }

  if (doc.value.ID) {
    // Simulación de actualización local
    const index = documentos.value.findIndex(d => d.ID === doc.value.ID)
    if (index !== -1) {
      documentos.value[index] = {
        ...doc.value,
        CATEGORIA_LABEL: obtenerNombreCategoria(doc.value.CATEGORIA)
      }
    }
    $q.notify({ message: 'Documento actualizado', color: 'positive' })
  } else {
    doc.value.ID = Date.now()
    doc.value.CREATE_DATE = new Date().toISOString()
    doc.value.VERSION_ACTUAL = 1
    documentos.value.push({
      ...doc.value,
      CATEGORIA_LABEL: obtenerNombreCategoria(doc.value.CATEGORIA)
    })
    $q.notify({ message: 'Documento creado', color: 'positive' })
  }

  modalAbierto.value = false
}

async function verDocumento(doc) {
  if (!doc?.id) {
    $q.notify({ type: 'negative', message: 'ID del documento no está definido.' })
    return
  }

  try {
    const res = await api.get(`/Documentos/${doc.id}`)
    documentoSeleccionado.value = res.data
    console.log('Documento cargado:', documentoSeleccionado.value)
    modalVisible.value = true
  } catch (error) {
    $q.notify({ type: 'negative', message: 'Error al cargar el documento: ' + error.message })
  }
}



function descargarArchivo(doc) {
  if (!doc.rutaArchivo) {
    $q.notify({ type: 'negative', message: 'No hay archivo para descargar.' })
    return
  }
  const url = `http://localhost:5168/${doc.rutaArchivo}`

  window.open(url, '_blank')
}



async function eliminarDocumento(doc) {
  console.log(doc)
  $q.dialog({
    title: 'Confirmar',
    message: `¿Eliminar "${doc.titulo}"?`,
    cancel: true,
    persistent: true
  }).onOk(async () => {
    try {
      await api.delete(`/Documentos/${doc.id}`)
      documentos.value = documentos.value.filter(d => d.id !== doc.id)
      $q.notify({ message: 'Documento eliminado', color: 'negative' })
    } catch (error) {
      $q.notify({ type: 'negative', message: 'Error al eliminar: ' + error.message })
    }
  })
}


onMounted(() => {
  fetchDocumentos()
})
</script>
