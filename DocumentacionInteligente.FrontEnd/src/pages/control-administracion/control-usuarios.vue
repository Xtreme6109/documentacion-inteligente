<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section class="row items-center justify-between">
        <div class="text-h6">Control de Usuarios</div>
        <q-btn label="Agregar usuario" icon="person_add" color="primary" @click="abrirModal()" />
      </q-card-section>

      <q-separator />

      <q-card-section>
        <q-table
          :rows="usuarios"
          :columns="columnas"
          row-key="id"
          flat
          dense
          :loading="cargando"
          loading-label="Cargando usuarios..."
        >
          <template #body-cell-acciones="props">
            <q-td align="center">
              <q-btn flat icon="edit" color="primary" @click="abrirModal(props.row)" size="sm" />
              <q-btn flat icon="delete" color="negative" @click="confirmarEliminar(props.row)" size="sm" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>

    <!-- Modal -->
    <q-dialog v-model="modalAbierto">
      <q-card style="min-width: 400px">
        <q-card-section>
          <div class="text-h6">{{ usuario.id ? 'Editar' : 'Nuevo' }} Usuario</div>
        </q-card-section>

        <q-separator />

        <q-card-section class="q-gutter-md">
          <q-input v-model="usuario.nombre" label="Nombre" outlined dense />
          <q-input v-model="usuario.correo" label="Correo" type="email" outlined dense />
          <q-select
            v-model="usuario.rol"
            label="Rol"
            outlined
            dense
            :options="rolesOptions"
            option-label="nombre"
            option-value="nombre"
            emit-value
            map-options
          />
          <q-input
            v-if="!usuario.id"
            v-model="usuario.password"
            label="Contraseña"
            type="password"
            outlined
            dense
          />
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="grey" v-close-popup />
          <q-btn color="primary" label="Guardar" @click="guardarUsuario" />
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

// Crear instancia axios con interceptor
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
const rolesOptions = ref([])
const cargando = ref(false)
const modalAbierto = ref(false)

const usuario = ref({
  id: null,
  nombre: '',
  correo: '',
  rol: '',
  password: ''
})

const columnas = [
  { name: 'nombre', label: 'Nombre', field: 'nombre', align: 'left' },
  { name: 'correo', label: 'Correo', field: 'correo', align: 'left' },
  { name: 'rol', label: 'Rol', field: 'rol', align: 'left' },
  { name: 'acciones', label: 'Acciones', align: 'center' }
]

function cargarUsuarios() {
  cargando.value = true
  api.get('/User/load-users')
    .then(res => {
      usuarios.value = res.data.map(u => ({
        id: u.id,
        nombre: u.nombre,
        correo: u.correo,
        rol: u.rol
      }))
    })
    .catch(err => {
      console.error(err)
      $q.notify({ type: 'negative', message: 'Error cargando usuarios' })
    })
    .finally(() => {
      cargando.value = false
    })
}

function cargarRoles() {
  api.get('/roles')
    .then(res => {
      rolesOptions.value = res.data
        .filter(r => r.estado === true)
        .map(r => ({ id: r.id, nombre: r.nombre }))
    })
    .catch(err => {
      console.error(err)
      $q.notify({ type: 'negative', message: 'Error cargando roles' })
    })
}

function abrirModal(usuarioExistente = null) {
  if (usuarioExistente) {
    usuario.value = { ...usuarioExistente, password: '' }
  } else {
    usuario.value = {
      id: null,
      nombre: '',
      correo: '',
      rol: '',
      password: ''
    }
  }
  modalAbierto.value = true
}

function guardarUsuario() {
  if (!usuario.value.nombre || !usuario.value.correo || !usuario.value.rol) {
    $q.notify({ type: 'negative', message: 'Todos los campos excepto la contraseña son obligatorios' });
    return;
  }

  if (!usuario.value.id && !usuario.value.password) {
    $q.notify({ type: 'negative', message: 'La contraseña es obligatoria para crear usuario' });
    return;
  }

  const payload = {
    Nombre: usuario.value.nombre,
    Correo: usuario.value.correo,
    Password: usuario.value.password,
    Rol: usuario.value.rol
  }

  if (usuario.value.id) {
    api.put(`/User/update-user/${usuario.value.id}`, payload)
      .then(() => {
        $q.notify({ type: 'positive', message: 'Usuario actualizado' })
        cargarUsuarios()
        modalAbierto.value = false
      })
      .catch(() => $q.notify({ type: 'negative', message: 'Error al actualizar usuario' }))
  } else {
    api.post('/User/register', payload)
      .then(() => {
        $q.notify({ type: 'positive', message: 'Usuario creado' })
        cargarUsuarios()
        modalAbierto.value = false
      })
      .catch(() => $q.notify({ type: 'negative', message: 'Error al crear usuario' }))
  }
}

function confirmarEliminar(u) {
  $q.dialog({
    title: 'Confirmar',
    message: `¿Deseas eliminar a ${u.nombre}?`,
    cancel: true,
    persistent: true
  }).onOk(() => {
    api.delete(`/User/delete-user/${u.id}`)
      .then(() => {
        $q.notify({ type: 'positive', message: 'Usuario eliminado' })
        cargarUsuarios()
      })
      .catch(() => $q.notify({ type: 'negative', message: 'Error al eliminar usuario' }))
  })
}


onMounted(() => {
  cargarUsuarios()
  cargarRoles()
})
</script>
