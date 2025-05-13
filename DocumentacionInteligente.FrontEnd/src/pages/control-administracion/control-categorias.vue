<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Categorías</div>
        <q-btn label="Nueva categoría" color="primary" icon="add" @click="abrirModal()" />
      </q-card-section>

      <q-separator />

      <!-- Tabla -->
      <q-card-section>
        <q-table
          :rows="categorias"
          :columns="columnas"
          row-key="id"
          flat
          dense
        >
          <template #body-cell-acciones="props">
            <q-td>
              <q-btn dense flat icon="edit" @click="abrirModal(props.row)" />
              <q-btn dense flat icon="delete" color="negative" @click="eliminarCategoria(props.row)" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>

    <!-- Modal -->
    <q-dialog v-model="modalAbierto">
      <q-card style="min-width: 400px">
        <q-card-section>
          <div class="text-h6">{{ categoria.id ? 'Editar categoría' : 'Nueva categoría' }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section class="q-gutter-md">
          <q-input v-model="categoria.nombre" label="Nombre de la categoría" outlined dense :rules="[val => !!val || 'Requerido']" />
          <q-input v-model="categoria.descripcion" label="Descripción" type="textarea" outlined dense />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey" v-close-popup />
          <q-btn label="Guardar" color="primary" @click="guardarCategoria" />
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

const categorias = ref([])
const columnas = [
  { name: 'nombre', label: 'Nombre', field: 'nombre', sortable: true },
  { name: 'descripcion', label: 'Descripción', field: 'descripcion' },
  { name: 'acciones', label: 'Acciones', field: 'id' }
]

const categoria = ref({})
const modalAbierto = ref(false)

function cargarMock() {
  categorias.value = [
    { id: 1, nombre: 'Legal', descripcion: 'Documentos jurídicos y contratos' },
    { id: 2, nombre: 'Técnico', descripcion: 'Manual, instrucciones, procedimientos técnicos' },
    { id: 3, nombre: 'Interno', descripcion: 'Uso exclusivo del personal interno' }
  ]
}

function abrirModal(cat = null) {
  categoria.value = cat
    ? { ...cat }
    : {
        nombre: '',
        descripcion: ''
      }
  modalAbierto.value = true
}

function guardarCategoria() {
  if (!categoria.value.nombre) {
    $q.notify({ type: 'negative', message: 'El nombre es obligatorio' })
    return
  }

  if (categoria.value.id) {
    const index = categorias.value.findIndex(c => c.id === categoria.value.id)
    categorias.value[index] = { ...categoria.value }
    $q.notify({ message: 'Categoría actualizada', color: 'positive' })
    // axios.put(`/api/categorias/${categoria.value.id}`, categoria.value)
  } else {
    categoria.value.id = Date.now()
    categorias.value.push({ ...categoria.value })
    $q.notify({ message: 'Categoría creada', color: 'positive' })
    // axios.post('/api/categorias', categoria.value)
  }

  modalAbierto.value = false
}

function eliminarCategoria(cat) {
  $q.dialog({
    title: 'Eliminar categoría',
    message: `¿Eliminar la categoría "${cat.nombre}"?`,
    cancel: true,
    persistent: true
  }).onOk(() => {
    categorias.value = categorias.value.filter(c => c.id !== cat.id)
    $q.notify({ message: 'Eliminado', color: 'negative' })
    // axios.delete(`/api/categorias/${cat.id}`)
  })
}

onMounted(() => {
  cargarMock()
})
</script>

