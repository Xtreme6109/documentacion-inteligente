<template>
  <q-page class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h5">Redacción asistida por IA</div>
        <div class="text-subtitle2">Especifica el contenido que deseas generar</div>
      </q-card-section>

        <q-card-section class="q-gutter-md">

          <q-input
            v-model="datos.titulo"
            label="Tema o Título"
            outlined
            required
            :rules="[val => !!val || 'Este campo es obligatorio']"
          />

          <q-select
            v-model="datos.categoria"
            :options="categorias"
            label="Categoría"
            outlined
            emit-value
            map-options
            :rules="[val => !!val || 'Selecciona una categoría']"
          />

          <q-select
            v-model="datos.tipoDocumento"
            :options="tiposDocumento"
            label="Tipo de Documento"
            outlined
            :rules="[val => !!val || 'Selecciona un tipo de documento']"
          />

          <q-select
            v-model="datos.tono"
            :options="tonos"
            label="Tono o estilo"
            outlined
            :rules="[val => !!val || 'Selecciona un tono']"
          />

          <q-input
            v-model="datos.instrucciones"
            label="Instrucciones adicionales"
            type="textarea"
            outlined
            autogrow
          />

          <q-btn
            label="Generar contenido"
            color="primary"
            type="submit"
            :loading="cargando"
            :disable="cargando"
          />
        </q-card-section>

      <q-separator />

      <q-card-section v-if="respuestaIA">
        <div class="text-h6 q-mb-sm">Contenido generado</div>
        <q-editor
          v-model="respuestaIA"
          :fonts="['default', 'arial', 'times new roman']"
          :definitions="{
            bold: { icon: 'format_bold' },
            italic: { icon: 'format_italic' },
            underline: { icon: 'format_underlined' }
          }"
        />
        <q-btn
          class="q-mt-md"
          label="Guardar como documento"
          icon="save"
          color="secondary"
          @click="guardarComoDocumento"
        />
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref } from 'vue'
//import { useQuasar } from 'quasar'
// import axios from 'axios'

//const $q = useQuasar()

const datos = ref({
  titulo: '',
  categoria: null,
  tipoDocumento: null,
  tono: null,
  instrucciones: ''
})

// Simulaciones (reemplaza con tu API)
const categorias = ['Política', 'Manual', 'Técnico']
const tiposDocumento = ['Informe', 'Carta', 'Manual', 'Presentación']
const tonos = ['Formal', 'Informal', 'Técnico', 'Creativo']

const respuestaIA = ref('')
const cargando = ref(false)

// Generar contenido
/*async function generarContenido() {
  cargando.value = true
  const prompt = `
    Tema: ${datos.value.titulo}
    Tipo de documento: ${datos.value.tipoDocumento}
    Tono: ${datos.value.tono}
    Categoría: ${datos.value.categoria}
    Instrucciones: ${datos.value.instrucciones}
  `

  try {
    // Simulación — reemplazar con API real
    // const res = await axios.post('/api/redaccion-ia', datos.value)
    // respuestaIA.value = res.data.contenido
    const resultado = await fakeApiGenerarTexto(prompt)
    respuestaIA.value = resultado
  } catch (err) {
    console.error('Error al generar contenido:', err)
    $q.notify({ type: 'negative', message: 'Error al generar contenido' })
  } finally {
    cargando.value = false
  }
}

// Guardar contenido generado
function guardarComoDocumento() {
  console.log('Guardando documento con contenido:', respuestaIA.value)
  $q.notify({ type: 'positive', message: 'Documento guardado exitosamente (simulado)' })
}

// Simulación de API de generación de contenido
async function fakeApiGenerarTexto(prompt) {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(`📝 Contenido generado automáticamente para:\n${prompt.trim()}`)
    }, 1500)
  })
}*/
</script>
