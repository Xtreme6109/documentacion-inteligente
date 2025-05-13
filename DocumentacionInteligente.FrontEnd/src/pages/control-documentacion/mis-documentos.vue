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
          :rows="documentos"
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
              <q-btn dense flat icon="visibility" @click="verDocumento(props.row)" />
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
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useQuasar } from 'quasar'
// import axios from 'axios'

const $q = useQuasar()

// Mock de filtros
const filtros = ref({
  titulo: '',
  categoria: null,
  estado: null,
  creadoIA: false
})

const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 },
  { label: 'Interno', value: 3 }
]

const estados = ['Borrador', 'Aprobado', 'Rechazado']

const columnas = [
  { name: 'TITULO', label: 'Título', field: 'TITULO', sortable: true },
  { name: 'DESCRIPCION', label: 'Descripción', field: 'DESCRIPCION' },
  { name: 'CATEGORIA', label: 'Categoría', field: 'CATEGORIA' },
  { name: 'CREATE_DATE', label: 'Creado el', field: 'CREATE_DATE', format: val => new Date(val).toLocaleDateString() },
  { name: 'ESTADO', label: 'Estado', field: 'ESTADO' },
  { name: 'VERSION_ACTUAL', label: 'Versión', field: 'VERSION_ACTUAL' },
  { name: 'CREADO_IA', label: 'IA', field: 'CREADO_IA' },
  { name: 'acciones', label: 'Acciones', field: 'ID' }
]

// Documentos de prueba
const documentos = ref([])
const modalAbierto = ref(false)
const doc = ref({})

function cargarMock() {
  documentos.value = [
    {
      ID: 1,
      TITULO: 'Contrato laboral',
      DESCRIPCION: 'Documento de contrato estándar.',
      CATEGORIA: 1,
      CREATE_DATE: '2025-05-10',
      ESTADO: 'Aprobado',
      VERSION_ACTUAL: 3,
      CREADO_IA: true
    },
    {
      ID: 2,
      TITULO: 'Manual técnico',
      DESCRIPCION: 'Instrucciones para el sistema.',
      CATEGORIA: 2,
      CREATE_DATE: '2025-05-11',
      ESTADO: 'Borrador',
      VERSION_ACTUAL: 1,
      CREADO_IA: false
    }
  ]
}

function buscar() {
  // Aquí iría la lógica para filtrar desde el backend
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

function guardarDocumento() {
  if (!doc.value.TITULO || !doc.value.DESCRIPCION || !doc.value.CATEGORIA || !doc.value.ESTADO) {
    $q.notify({ type: 'negative', message: 'Faltan campos requeridos' })
    return
  }

  if (doc.value.ID) {
    // axios.put(`/api/documentos/${doc.value.ID}`, doc.value)
    //   .then(() => cargarMock())
    const index = documentos.value.findIndex(d => d.ID === doc.value.ID)
    documentos.value[index] = { ...doc.value }
    $q.notify({ message: 'Documento actualizado', color: 'positive' })
  } else {
    doc.value.ID = Date.now()
    doc.value.CREATE_DATE = new Date().toISOString()
    doc.value.VERSION_ACTUAL = 1
    documentos.value.push({ ...doc.value })
    $q.notify({ message: 'Documento creado', color: 'positive' })
  }

  modalAbierto.value = false
}

function verDocumento(doc) {
  $q.dialog({
    title: doc.TITULO,
    message: doc.DESCRIPCION,
    ok: 'Cerrar'
  })
}

function descargarArchivo(doc) {
  // Simulación
  $q.notify({ message: `Descargando ${doc.TITULO}`, color: 'info' })
}

function eliminarDocumento(doc) {
  $q.dialog({
    title: 'Confirmar',
    message: `¿Eliminar "${doc.TITULO}"?`,
    cancel: true,
    persistent: true
  }).onOk(() => {
    documentos.value = documentos.value.filter(d => d.ID !== doc.ID)
    $q.notify({ message: 'Eliminado', color: 'negative' })
  })
}

onMounted(() => {
  cargarMock()
})
</script>
