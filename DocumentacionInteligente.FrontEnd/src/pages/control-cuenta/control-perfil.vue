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
          <q-input v-model="confirmarContrasena" label="Confirmar Contraseña" type="password" class="q-mt-md" />
        </q-card-section>

        <q-separator />

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" v-close-popup />
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
import { ref, computed } from 'vue'
// import axios from 'axios' // Descomenta si usarás peticiones reales

const dialogVisible = ref(false)
const guardando = ref(false)

const perfil = ref({
  nombre: 'Juan Pérez',
  correo: 'juan.perez@empresa.com',
  foto: 'https://cdn.quasar.dev/img/avatar.png'
})

// Datos para cambiar contraseña
const nuevaContrasena = ref('')
const confirmarContrasena = ref('')

// Valida que la nueva contraseña y la confirmación sean iguales
const validarContrasena = computed(() => nuevaContrasena.value === confirmarContrasena.value && nuevaContrasena.value.length > 0)

function cambiarContrasena() {
  if (!validarContrasena.value) {
    alert('Las contraseñas no coinciden o están vacías')
    return
  }

  guardando.value = true

  // Simulación de retardo como si se llamara al backend
  setTimeout(() => {
    // Aquí iría una llamada real a la API para actualizar la contraseña:
    /*
    axios.post('/api/perfil/cambiar-contrasena', { nuevaContrasena: nuevaContrasena.value })
      .then(response => {
        alert('Contraseña cambiada correctamente')
      })
      .catch(error => {
        console.error('Error al cambiar contraseña:', error)
      })
    */

    // Simulación de éxito
    alert('Contraseña cambiada correctamente')

    guardando.value = false
    dialogVisible.value = false
    nuevaContrasena.value = ''
    confirmarContrasena.value = ''
  }, 1200)
}
</script>
