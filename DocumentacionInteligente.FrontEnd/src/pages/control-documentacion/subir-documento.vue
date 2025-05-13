<template>
  <div class="q-pa-md">
    <q-card class="q-pa-lg shadow-2">
      <div class="text-h4 q-mb-lg">Subir Documento Manualmente</div>

      <q-form @submit.prevent="subirDocumento" ref="formRef">
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
              class="q-mt-md"
            />
          </div>

        </div>
      </q-form>
    </q-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'

// Referencia al formulario para validación
const formRef = ref(null)

// Datos del formulario
const formData = ref({
  titulo: '',
  archivo: null,
  descripcion: '',
  categoria: null,
  estado: '',
  creadoIA: false
})

// Opciones de categorías
const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 },
  { label: 'Comercial', value: 3 }
]

// Opciones de estado
const estados = ['Borrador', 'Aprobado', 'Rechazado']

// Simula envío
function subirDocumento() {
  formRef.value.validate().then((ok) => {
    if (ok) {
      console.log('Datos del formulario:', formData.value)
      // axios
    } else {
      console.warn('Validación fallida')
    }
  })
}
</script>
