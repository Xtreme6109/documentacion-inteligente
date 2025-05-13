<template>
  <div class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h6">Mis documentos</div>
      </q-card-section>

      <q-separator />

      <!-- Filtros -->
      <q-card-section class="q-gutter-md row items-center">
        <q-input v-model="filtros.titulo" label="Buscar por título" dense clearable />
        <q-select v-model="filtros.categoria" :options="categorias" label="Categoría" dense clearable />
        <q-select v-model="filtros.estado" :options="estados" label="Estado" dense clearable />
        <q-checkbox v-model="filtros.creadoIA" label="Solo creados con IA" />
        <q-btn label="Buscar" color="primary" @click="buscar" />
      </q-card-section>

      <q-separator />

      <!-- Tabla de documentos -->
      <q-card-section>
        <q-table
          :rows="documentos"
          :columns="columnas"
          row-key="ID"
          :filter="filtros.titulo"
          flat
        >
          <template v-slot:body-cell-acciones="props">
            <q-td>
              <q-btn dense flat icon="visibility" @click="verDocumento(props.row)" />
              <q-btn dense flat icon="edit" @click="editarDocumento(props.row)" />
              <q-btn dense flat icon="download" @click="descargarArchivo(props.row)" />
              <q-btn dense flat icon="delete" color="negative" @click="eliminarDocumento(props.row)" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>
  </div>
</template>


<script setup>
import { ref } from 'vue'

// Filtros
const filtros = ref({
  titulo: '',
  categoria: null,
  estado: null,
  creadoIA: false
})

// Opciones de filtro
const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 }

]

const estados = ['Borrador', 'Aprobado', 'Rechazado']

// Columnas de tabla
const columnas = [
  { name: 'TITULO', label: 'Título', field: 'TITULO', sortable: true },
  { name: 'DESCRIPCION', label: 'Descripción', field: 'DESCRIPCION' },
  { name: 'CATEGORIA', label: 'Categoría', field: 'CATEGORIA' },
  { name: 'CREATE_DATE', label: 'Creado el', field: 'CREATE_DATE', format: val => new Date(val).toLocaleDateString() },
  { name: 'ESTADO', label: 'Estado', field: 'ESTADO' },
  { name: 'VERSION_ACTUAL', label: 'Versión', field: 'VERSION_ACTUAL' },
  { name: 'CREADO_IA', label: 'IA', field: row => row.CREADO_IA ? 'Sí' : 'No' },
  { name: 'acciones', label: 'Acciones', field: 'ID' }
]

// Datos simulados o traídos con Axios
const documentos = ref([])

function buscar() {
  // Lógica para buscar documentos del usuario con filtros
}

// function verDocumento(doc) {
//
// }

// function editarDocumento(doc) {
//
// }

// function descargarArchivo(doc) {
//
// }

// function eliminarDocumento(doc) {
//
// }
</script>

