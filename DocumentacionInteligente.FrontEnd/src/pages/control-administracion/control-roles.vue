<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Roles</div>
        <q-btn label="Nuevo rol" color="primary" icon="add" @click="abrirModal()" />
      </q-card-section>

      <q-separator />

      <!-- Tabla -->
      <q-card-section>
        <q-table
          :rows="roles"
          :columns="columnas"
          row-key="id"
          flat
          dense
        >
          <template #body-cell-acciones="props">
            <q-td>
              <q-btn dense flat icon="edit" @click="abrirModal(props.row)" />
              <q-btn dense flat icon="delete" color="negative" @click="eliminarRol(props.row)" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>

    <!-- Modal -->
    <q-dialog v-model="modalAbierto">
      <q-card style="min-width: 400px">
        <q-card-section>
          <div class="text-h6">{{ rol.id ? 'Editar rol' : 'Nuevo rol' }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section class="q-gutter-md">
          <q-input v-model="rol.nombre" label="Nombre del rol" outlined dense :rules="[val => !!val || 'Requerido']" />
          <q-input v-model="rol.descripcion" label="Descripción" type="textarea" outlined dense />
          <q-toggle v-model="rol.estado" label="Activo" />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey" v-close-popup />
          <q-btn label="Guardar" color="primary" @click="guardarRol" />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'

const $q = useQuasar()
const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  console.log('Token: ' + localStorage.getItem('token'))
  const payload = JSON.parse(atob(token.split('.')[1]))
    console.log(payload)

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(error)
})

const roles = ref([])
const columnas = [
  { name: 'nombre', label: 'Nombre', field: 'nombre', sortable: true },
  { name: 'descripcion', label: 'Descripción', field: 'descripcion' },
  { name: 'estado', label: 'Estado', field: 'estado', format: val => val ? 'Activo' : 'Inactivo' },
  { name: 'acciones', label: 'Acciones', field: 'id' }
]

const rol = ref({})
const modalAbierto = ref(false)

async function cargarRoles() {
  try {
    const res = await api.get('/roles')
    roles.value = res.data
  } catch (err) {
    console.error(err)
    $q.notify({ type: 'negative', message: 'Error al cargar roles' })
  }
}

function abrirModal(r = null) {
  rol.value = r
    ? { ...r }
    : {
        nombre: '',
        descripcion: '',
        estado: true
      }
  modalAbierto.value = true
}

async function guardarRol() {
  if (!rol.value.nombre) {
    $q.notify({ type: 'negative', message: 'El nombre es obligatorio' })
    return
  }

  try {
    if (rol.value.id) {
      await api.put(`/roles/${rol.value.id}`, {
        id: rol.value.id,
        nombre: rol.value.nombre,
        descripcion: rol.value.descripcion,
        estado: rol.value.estado,
        permisosMenu: rol.value.permisosMenu || []
      })
      $q.notify({ message: 'Rol actualizado', color: 'positive' })
    } else {
      await api.post('/roles', {
        nombre: rol.value.nombre,
        descripcion: rol.value.descripcion,
        creatE_DATE: new Date().toISOString(),
        estado: rol.value.estado,
        permisosMenu: []
      })
      $q.notify({ message: 'Rol creado', color: 'positive' })
    }

    modalAbierto.value = false
    cargarRoles()
  } catch (err) {
    console.error(err)
    $q.notify({ type: 'negative', message: 'Error al guardar rol' })
  }
}

async function eliminarRol(r) {
  $q.dialog({
    title: 'Eliminar rol',
    message: `¿Eliminar el rol "${r.nombre}"?`,
    cancel: true,
    persistent: true
  }).onOk(async () => {
    try {
      await api.delete(`/roles/${r.id}`)
      $q.notify({ message: 'Rol eliminado', color: 'negative' })
      cargarRoles()
    } catch (err) {
      console.error(err)
      $q.notify({ type: 'negative', message: 'Error al eliminar rol' })
    }
  })
}

onMounted(() => {
  cargarRoles()
})
</script>
