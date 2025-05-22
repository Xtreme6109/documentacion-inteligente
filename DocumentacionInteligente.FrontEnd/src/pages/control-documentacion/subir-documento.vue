<template>
  <div class="q-pa-md">
    <q-card class="q-pa-lg shadow-2">
      <div class="text-h5 q-mb-lg">Subir Documento Manualmente</div>

        <div class="q-gutter-md">

          <!-- Título del documento -->
          <q-input
            v-model="formData.titulo"
            label="Título del documento"
            outlined
            clearable
            :rules="[val => !!val || 'El título es requerido']"
          />

          <!-- Archivo -->
          <q-file
            v-model="formData.archivo"
            label="Seleccionar archivo"
            outlined
            use-chips
            :rules="[val => !!val || 'Debe seleccionar un archivo']"
          />

          <!-- Descripción -->
          <q-input
            v-model="formData.descripcion"
            label="Descripción"
            type="textarea"
            outlined
            autogrow
            :rules="[val => !!val || 'La descripción es requerida']"
          />

          <!-- Categoría -->
          <q-select
            v-model="formData.categoria"
            label="Categoría"
            outlined
            :options="categorias"
            emit-value
            map-options
            :rules="[val => !!val || 'Seleccione una categoría']"
          />

          <!-- Estado -->
          <q-select
            v-model="formData.estado"
            label="Estado del documento"
            outlined
            :options="estados"
            emit-value
            map-options
            :rules="[val => !!val || 'Seleccione un estado']"
          />

          <!-- Creado con IA -->
          <q-checkbox
            v-model="formData.creadoIA"
            label="Documento creado por IA"
          />

          <!-- Botón -->
          <div class="row justify-end">
            <q-btn
              label="Subir Documento"
              color="primary"
              type="submit"
              :loading="subiendo"
              class="q-mt-md"
            />
          </div>
        </div>
    </q-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
//import { useQuasar } from 'quasar'
// import axios from 'axios'

//const $q = useQuasar()
//const formRef = ref(null)
const subiendo = ref(false)

// Datos del formulario
const formData = ref({
  titulo: '',
  archivo: null,
  descripcion: '',
  categoria: null,
  estado: null,
  creadoIA: false
})

// Opciones de categorías
const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 },
  { label: 'Comercial', value: 3 }
]

// Opciones de estado
const estados = [
  { label: 'Borrador', value: 'Borrador' },
  { label: 'Aprobado', value: 'Aprobado' },
  { label: 'Rechazado', value: 'Rechazado' }
]

/*function subirDocumento() {
  formRef.value.validate().then(async (ok) => {
    if (!ok) {
      $q.notify({ type: 'negative', message: 'Por favor completa todos los campos obligatorios' })
      return
    }

    subiendo.value = true

    try {
      const payload = new FormData()
      payload.append('titulo', formData.value.titulo)
      payload.append('archivo', formData.value.archivo)
      payload.append('descripcion', formData.value.descripcion)
      payload.append('categoria', formData.value.categoria)
      payload.append('estado', formData.value.estado)
      payload.append('creadoIA', formData.value.creadoIA ? '1' : '0')

      // await axios.post('/api/subir-documento', payload)

      await new Promise(resolve => setTimeout(resolve, 1000)) // Simulación

      $q.notify({ type: 'positive', message: 'Documento subido correctamente' })

      // Limpiar formulario
      formRef.value.resetValidation()
      formData.value = {
        titulo: '',
        archivo: null,
        descripcion: '',
        categoria: null,
        estado: null,
        creadoIA: false
      }

    } catch (err) {
      $q.notify({ type: 'negative', message: 'Error al subir el documento' + ' ' + err.message })
    } finally {
      subiendo.value = false
    }
  })
}*/
</script>
