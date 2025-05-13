<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">
      Reportes por Usuario
    </div>

    <!-- Filtro de selección usando divs y clases flexbox -->
    <div class="flex flex-wrap q-gutter-md">
      <!-- Usuario -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-select
          v-model="usuarioSeleccionado"
          :options="usuarios"
          label="Seleccionar Usuario"
          option-label="nombre"
          option-value="id"
          outlined
          emit-value
          map-options
        />
      </div>

      <!-- Fecha de Inicio -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaInicio"
          label="Fecha de Inicio"
          type="date"
          outlined
        />
      </div>

      <!-- Fecha de Fin -->
      <div class="q-mb-md col-12 col-sm col-md">
        <q-input
          v-model="fechaFin"
          label="Fecha de Fin"
          type="date"
          outlined
        />
      </div>
    </div>

    <!-- Botón para generar el reporte -->
    <q-btn
      color="primary"
      label="Generar Reporte"
      @click="generarReporte"
      class="q-mt-md full-width"
    />
  </div>
</template>

<script setup>
import { ref } from 'vue'
import jsPDF from 'jspdf'

const usuarios = ref([
  { id: 1, nombre: 'Juan Pérez' },
  { id: 2, nombre: 'Ana Gómez' },
  { id: 3, nombre: 'Luis Rodríguez' }
])


const usuarioSeleccionado = ref(null)
const fechaInicio = ref('')
const fechaFin = ref('')

function generarReporte() {
  // Validar que los campos necesarios estén seleccionados
  if (!usuarioSeleccionado.value || !fechaInicio.value || !fechaFin.value) {
    alert("Por favor, complete todos los campos.")
    return
  }

  // Crear el PDF de preueba nada más aqui es con asp
  const doc = new jsPDF()

  // Agregar los textos al PDF
  doc.text('Reporte de Usuario', 10, 10)
  doc.text('Usuario: ' + usuarioSeleccionado.value.nombre, 10, 20)
  doc.text('Fecha de Inicio: ' + fechaInicio.value, 10, 30)
  doc.text('Fecha de Fin: ' + fechaFin.value, 10, 40)

  // Generar el archivo PDF
  doc.save('reporte_usuario.pdf')
}
</script>

<style scoped>


</style>
