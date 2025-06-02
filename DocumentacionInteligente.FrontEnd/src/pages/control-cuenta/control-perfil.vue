<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">Perfil</div>

    <q-card class="q-pa-md">
      <q-item>
        <q-item-section avatar>
          <q-avatar size="56px">
            <img :src="perfil.foto" alt="Avatar" />
          </q-avatar>
        </q-item-section>
        <q-item-section>
          <q-item-label class="text-h6">{{ perfil.nombre }}</q-item-label>
          <q-item-label>{{ perfil.correo }}</q-item-label>
        </q-item-section>
      </q-item>

      <q-separator />

      <q-card-actions align="right">
        <q-btn color="primary" label="Cambiar Contraseña" @click="dialogVisible = true" />
      </q-card-actions>
    </q-card>

    <!-- Modal para cambiar contraseña -->
    <q-dialog v-model="dialogVisible">
      <q-card style="min-width: 350px">
        <q-card-section>
          <div class="text-h6">Cambiar Contraseña</div>
        </q-card-section>

        <q-separator />

        <q-card-section>
          <q-input v-model="nuevaContrasena" label="Nueva Contraseña" type="password" />
          <q-input
            v-model="confirmarContrasena"
            label="Confirmar Contraseña"
            type="password"
            class="q-mt-md"
            :error="confirmarContrasena && !validarContrasena"
            error-message="Las contraseñas no coinciden"
          />
        </q-card-section>

        <q-separator />

        <q-card-actions align="right">
          <q-btn
            flat
            label="Cancelar"
            @click="() => { dialogVisible = false; nuevaContrasena = ''; confirmarContrasena = '' }"
          />

          <q-btn
            color="primary"
            :loading="guardando"
            label="Guardar Cambios"
            @click="cambiarContrasena"
            :disable="!validarContrasena"
          />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

// Instancia centralizada de Axios
const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})

const dialogVisible = ref(false)
const guardando = ref(false)

// Perfil cargado desde el backend
const perfil = ref({
  nombre: '',
  correo: '',
  foto: 'https://cdn.quasar.dev/img/avatar.png' // opcional
})

// Datos para cambiar contraseña
const nuevaContrasena = ref('')
const confirmarContrasena = ref('')

// Validación de contraseñas
const validarContrasena = computed(() =>
  nuevaContrasena.value === confirmarContrasena.value &&
  nuevaContrasena.value.length > 0
)

// Obtener datos del usuario autenticado
async function cargarPerfil() {
  try {
    const token = localStorage.getItem('token')
    if (!token) throw new Error('Token no encontrado')

    const response = await api.get('/user/perfil', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    const data = response.data
    perfil.value.nombre = data.nombre
    perfil.value.correo = data.correo
    // Si hay foto en el backend: perfil.value.foto = data.foto
  } catch (error) {
    console.error('Error al cargar el perfil:', error)
    alert('No se pudo cargar el perfil del usuario.')
  }
}

// Cambiar contraseña
async function cambiarContrasena() {
  if (!validarContrasena.value) {
    alert('Las contraseñas no coinciden o están vacías')
    return
  }

  guardando.value = true

  try {
    const token = localStorage.getItem('token')
    await api.put(
      '/user/cambiar-contrasena',
      { nuevaContrasena: nuevaContrasena.value },
      {
        headers: {
          Authorization: `Bearer ${token}`
        }
      }
    )

    alert('Contraseña cambiada correctamente')
    dialogVisible.value = false
    nuevaContrasena.value = ''
    confirmarContrasena.value = ''
  } catch (error) {
    console.error('Error al cambiar contraseña:', error)
    alert('Error al cambiar la contraseña')
  } finally {
    guardando.value = false
  }
}

// Cargar el perfil al montar el componente
onMounted(() => {
  cargarPerfil()
})
</script>
