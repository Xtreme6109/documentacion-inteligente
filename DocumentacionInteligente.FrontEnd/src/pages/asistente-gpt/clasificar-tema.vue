<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h5">Clasificación automática por tema</div>
        <div class="text-subtitle2">Selecciona un documento para analizar su contenido</div>
      </q-card-section>

      <q-card-section class="q-gutter-md">
        <!-- Selector de documento -->
        <q-select
          v-model="documentoSeleccionado"
          :options="documentos"
          option-label="TITULO"
          option-value="ID"
          filled
          label="Documento a clasificar"
          emit-value
          map-options
        />

        <!-- Botón de clasificación -->
        <q-btn
          label="Clasificar con IA"
          color="primary"
          :disable="!documentoSeleccionado"
          @click="clasificarDocumento"
          :loading="cargando"
        />

        <q-separator spaced />

        <!-- Categoría sugerida -->
        <div v-if="categoriaSugerida">
          <div class="text-subtitle1">Categoría sugerida:</div>
          <q-select
            v-model="categoriaSeleccionada"
            :options="categorias"
            option-label="NOMBRE"
            option-value="ID"
            filled
          />
        </div>

        <!-- Palabras clave sugeridas -->
        <div v-if="palabrasClave.length">
          <div class="text-subtitle1 q-mt-md">Palabras clave sugeridas:</div>
          <q-chip
            v-for="(palabra, index) in palabrasClave"
            :key="index"
            color="secondary"
            text-color="white"
          >
            {{ palabra }}
          </q-chip>
        </div>

        <!-- Guardar clasificación -->
        <q-btn
          label="Guardar clasificación"
          color="positive"
          class="q-mt-md"
          @click="guardarClasificacion"
          :disable="!categoriaSeleccionada"
        />
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useQuasar } from 'quasar'
// import axios from 'axios'

const $q = useQuasar()

const documentos = ref([])
const categorias = ref([])
const documentoSeleccionado = ref(null)
const categoriaSeleccionada = ref(null)
const categoriaSugerida = ref(null)
const palabrasClave = ref([])
const cargando = ref(false)

// Cargar documentos simulados (aqui debe ir la API)
const cargarDocumentos = async () => {
  documentos.value = [
    { ID: 1, TITULO: 'Reglamento interno de seguridad' },
    { ID: 2, TITULO: 'Manual de operación técnica' },
    { ID: 3, TITULO: 'Política de privacidad de datos' }
  ]
  // const res = await axios.get('/api/documentos-sin-clasificar')
  // documentos.value = res.data
}

// Cargar categorías simuladas
const cargarCategorias = async () => {
  categorias.value = [
    { ID: 1, NOMBRE: 'Legal' },
    { ID: 2, NOMBRE: 'Técnico' },
    { ID: 3, NOMBRE: 'Administrativo' }
  ]
  // const res = await axios.get('/api/categorias')
  // categorias.value = res.data
}

// Simula clasificación por IA
const clasificarDocumento = async () => {
  cargando.value = true
  setTimeout(() => {
    const doc = documentos.value.find(d => d.ID === documentoSeleccionado.value)
    if (doc.TITULO.toLowerCase().includes('legal')) {
      categoriaSugerida.value = 1
      categoriaSeleccionada.value = 1
      palabrasClave.value = ['contrato', 'normativa', 'legal']
    } else if (doc.TITULO.toLowerCase().includes('técnico')) {
      categoriaSugerida.value = 2
      categoriaSeleccionada.value = 2
      palabrasClave.value = ['protocolo', 'mantenimiento', 'operación']
    } else {
      categoriaSugerida.value = 3
      categoriaSeleccionada.value = 3
      palabrasClave.value = ['personal', 'proceso', 'gestión']
    }
    cargando.value = false
  }, 1000)

  // Ejemplo real sería asi para cuando lo prueben:
  // const res = await axios.post('/api/clasificar-documento', { id: documentoSeleccionado.value })
  // categoriaSugerida.value = res.data.categoria_id
  // categoriaSeleccionada.value = res.data.categoria_id
  // palabrasClave.value = res.data.palabras_clave
}

// Simula guardar clasificación
const guardarClasificacion = async () => {
  $q.notify({ type: 'positive', message: 'Clasificación guardada correctamente' })

  // await axios.post('/api/guardar-clasificacion', {
  //   id_documento: documentoSeleccionado.value,
  //   id_categoria: categoriaSeleccionada.value,
  //   palabras_clave: palabrasClave.value
  // })
}

onMounted(() => {
  cargarDocumentos()
  cargarCategorias()
})
</script>
