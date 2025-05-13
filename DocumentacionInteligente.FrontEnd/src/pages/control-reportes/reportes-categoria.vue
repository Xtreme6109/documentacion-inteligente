<template>
  <div class="q-pa-md">
    <div class="text-h5 q-mb-md">Reportes por Categoría</div>

    <!-- Filtro de selección usando divs y clases flexbox -->
    <div class="flex flex-wrap q-gutter-md">
      <!-- Categoría -->
      <div class="q-mb-md col-12 col-md col-sm">
        <q-select
          v-model="categoriaSeleccionada"
          :options="categorias"
          label="Seleccionar Categoría"
          option-label="nombre"
          option-value="nombre"
          outlined
          emit-value
          map-options
        />
      </div>

      <!-- Usuario -->
      <div class="q-mb-md col-12 col-md col-sm">
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
      <div class="q-mb-md col-12 col-md col-sm">
        <q-input
          v-model="fechaInicio"
          label="Fecha de Inicio"
          type="date"
          outlined
        />
      </div>

      <!-- Fecha de Fin -->
      <div class="q-mb-md col-12 col-md col-sm">
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

    <!-- Modal para mostrar detalles de reporte -->
    <q-dialog v-model="dialogVisible">
      <q-card style="min-width: 350px">
        <q-card-section>
          <div class="text-h6">Reporte de {{ categoriaSeleccionada }}</div>
        </q-card-section>

        <q-separator />

        <q-card-section>
          <div class="text-body2">
            <p><strong>Categoría:</strong> {{ categoriaSeleccionada }}</p>
            <p><strong>Usuario:</strong> {{ usuarioSeleccionado }}</p>
            <p><strong>Fecha de Inicio:</strong> {{ fechaInicio }}</p>
            <p><strong>Fecha de Fin:</strong> {{ fechaFin }}</p>
          </div>
        </q-card-section>

        <q-separator />

        <q-card-actions align="right">
          <q-btn flat label="Cerrar" v-close-popup />
        </q-card-actions>
      </q-card>
    </q-dialog>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { jsPDF } from 'jspdf'

const categorias = ref([
  { nombre: 'Ventas', descripcion: 'Reportes relacionados con las ventas' },
  { nombre: 'Inventarios', descripcion: 'Reportes sobre el estado del inventario' },
  { nombre: 'Finanzas', descripcion: 'Reportes de estados financieros' }
])

const usuarios = ref([
  { id: 1, nombre: 'Juan Pérez' },
  { id: 2, nombre: 'Ana Gómez' },
  { id: 3, nombre: 'Luis Rodríguez' }
])

const categoriaSeleccionada = ref(null)
const usuarioSeleccionado = ref(null)
const fechaInicio = ref('')
const fechaFin = ref('')
const dialogVisible = ref(false)

function generarReporte() {
  // Validar que los campos necesarios estén seleccionados
  if (!categoriaSeleccionada.value || !usuarioSeleccionado.value || !fechaInicio.value || !fechaFin.value) {
    alert("Por favor, complete todos los campos.")
    return
  }

  // Crear el PDF de preueba nada más aqui es con asp
  const doc = new jsPDF()

  doc.text('Reporte de ' + categoriaSeleccionada.value, 10, 10)
  doc.text('Usuario: ' + usuarioSeleccionado.value, 10, 20)
  doc.text('Fecha de Inicio: ' + fechaInicio.value, 10, 30)
  doc.text('Fecha de Fin: ' + fechaFin.value, 10, 40)

  // Generar el archivo PDF
  doc.save('reporte.pdf')
}
</script>

<style scoped>

</style>
