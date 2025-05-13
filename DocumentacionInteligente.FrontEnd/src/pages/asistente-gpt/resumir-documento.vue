<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h5">Resumir un Documento</div>
        <div class="text-subtitle2">Selecciona un documento y obtén un resumen generado por IA</div>
      </q-card-section>

      <q-card-section class="q-gutter-md">
        <q-select
          v-model="documentoSeleccionado"
          :options="listaDocumentos"
          option-label="TITULO"
          option-value="ID"
          label="Selecciona un documento"
          filled
          emit-value
          map-options
          :rules="[val => !!val || 'Debes seleccionar un documento']"
        />

        <q-btn
          label="Resumir"
          color="primary"
          :disable="!documentoSeleccionado || cargando"
          :loading="cargando"
          @click="resumirDocumento"
        />

        <q-separator spaced />

        <q-card v-if="resumen" flat bordered class="q-pa-md q-mt-md">
          <div class="text-h5">Resumen generado</div>
          <q-editor
            v-model="resumen"
            :fonts="['default', 'arial', 'times new roman']"
            :definitions="{
              bold: { icon: 'format_bold' },
              italic: { icon: 'format_italic' },
              underline: { icon: 'format_underlined' }
            }"
          />
          <q-btn
            label="Guardar como resumen"
            icon="save"
            color="secondary"
            class="q-mt-md"
            @click="guardarResumen"
          />
        </q-card>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useQuasar } from 'quasar'
// import axios from 'axios'

const $q = useQuasar()

const listaDocumentos = ref([])
const documentoSeleccionado = ref(null)
const resumen = ref('')
const cargando = ref(false)

onMounted(async () => {
  // Reemplaza esto por tu llamada real a la API
  // const res = await axios.get('/api/mis-documentos')
  // listaDocumentos.value = res.data

  // Simulación de documentos
  listaDocumentos.value = [
    { ID: 1, TITULO: 'Informe de Seguridad' },
    { ID: 2, TITULO: 'Plan Estratégico 2025' },
    { ID: 3, TITULO: 'Resumen Ejecutivo del Proyecto' }
  ]
})

async function resumirDocumento() {
  cargando.value = true
  resumen.value = ''
  try {
    // Reemplazar con llamada real a la API
    // const res = await axios.post('/api/resumir', { idDocumento: documentoSeleccionado.value })
    // resumen.value = res.data.resumen

    const doc = listaDocumentos.value.find(d => d.ID === documentoSeleccionado.value)
    const simulacion = await fakeApiResumen(doc?.TITULO || 'Documento desconocido')
    resumen.value = simulacion
  } catch (err) {
    console.error('Error al resumir:', err)
    $q.notify({ type: 'negative', message: 'Error al resumir el documento' })
  } finally {
    cargando.value = false
  }
}

async function guardarResumen() {
  console.log('Resumen guardado:', resumen.value)
  $q.notify({ type: 'positive', message: 'Resumen guardado exitosamente (simulado)' })

  // Reemplazar por petición real
  // await axios.post('/api/guardar-resumen', {
  //   idDocumento: documentoSeleccionado.value,
  //   resumen: resumen.value
  // })
}

// Simulación de generación de resumen
async function fakeApiResumen(titulo) {
  return new Promise(resolve => {
    setTimeout(() => {
      resolve(`📄 Este es un resumen automático del documento titulado "${titulo}".`)
    }, 1500)
  })
}
</script>
