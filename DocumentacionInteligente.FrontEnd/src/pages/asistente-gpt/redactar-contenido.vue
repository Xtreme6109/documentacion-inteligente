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
            v-model="datos.tono"
            :options="tonos"
            label="Tono o estilo"
            outlined
            :rules="[val => !!val || 'Selecciona un tono']"
          />

          <q-input
            v-model="datos.descripcion"
            label="Descripción"
            outlined
            type="textarea"
            autogrow
          />

          <q-select
            v-model="datos.estado"
            :options="estados"
            label="Estado del documento"
            outlined
          />
          <div class="q-gutter-0">
            <div class="text-subtitle2">Contenido</div>
              <q-editor
                v-model="datos.contenido"
                :dense="$q.screen.lt.md"
                :toolbar="[
                  [
                    {
                      label: $q.lang.editor.align,
                      icon: $q.iconSet.editor.align,
                      fixedLabel: true,
                      list: 'only-icons',
                      options: ['left', 'center', 'right', 'justify']
                    },
                    {
                      label: $q.lang.editor.align,
                      icon: $q.iconSet.editor.align,
                      fixedLabel: true,
                      options: ['left', 'center', 'right', 'justify']
                    }
                  ],
                  ['bold', 'italic', 'strike', 'underline', 'subscript', 'superscript'],
                  ['token', 'hr', 'link', 'custom_btn'],
                  ['print', 'fullscreen'],
                  [
                    {
                      label: $q.lang.editor.formatting,
                      icon: $q.iconSet.editor.formatting,
                      list: 'no-icons',
                      options: [
                        'p',
                        'h1',
                        'h2',
                        'h3',
                        'h4',
                        'h5',
                        'h6',
                        'code'
                      ]
                    },
                    {
                      label: $q.lang.editor.fontSize,
                      icon: $q.iconSet.editor.fontSize,
                      fixedLabel: true,
                      fixedIcon: true,
                      list: 'no-icons',
                      options: [
                        'size-1',
                        'size-2',
                        'size-3',
                        'size-4',
                        'size-5',
                        'size-6',
                        'size-7'
                      ]
                    },
                    {
                      label: $q.lang.editor.defaultFont,
                      icon: $q.iconSet.editor.font,
                      fixedIcon: true,
                      list: 'no-icons',
                      options: [
                        'default_font',
                        'arial',
                        'arial_black',
                        'comic_sans',
                        'courier_new',
                        'impact',
                        'lucida_grande',
                        'times_new_roman',
                        'verdana'
                      ]
                    },
                    'removeFormat'
                  ],
                  ['quote', 'unordered', 'ordered', 'outdent', 'indent'],

                  ['undo', 'redo'],
                  ['viewsource']
                ]"
                :fonts="{
                  arial: 'Arial',
                  arial_black: 'Arial Black',
                  comic_sans: 'Comic Sans MS',
                  courier_new: 'Courier New',
                  impact: 'Impact',
                  lucida_grande: 'Lucida Grande',
                  times_new_roman: 'Times New Roman',
                  verdana: 'Verdana'
                }"
              />
          </div>


          <q-input
            v-model="datos.instrucciones"
            label="Instrucciones adicionales"
            type="textarea"
            outlined
            autogrow
          />

          <q-btn
            label="Generar contenido"
            icon="smart_toy"
            color="primary"
            :loading="cargando"
            @click="generarContenidoIA"
          />


          <q-btn
            label="Usar contenido generado"
            icon="content_paste"
            color="secondary"
            @click="usarContenidoGenerado"
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
          label="Guardar documento"
          icon="save"
          color="green"
          :loading="cargando"
          :disable="!datos.titulo || !datos.contenido"
          @click="guardarComoDocumento"
        />
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup>
import { ref } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'
const $q = useQuasar()


//const $q = useQuasar()

const datos = ref({
  titulo: '',
  descripcion: '',
  categoria: null,
  tono: null,
  contenido: '',
  instrucciones: '',
  estado: 'Borrador',
  creado_ia: true
})


// Simulaciones (reemplaza con tu API)
const categorias = ['Política', 'Manual', 'Técnico']
const tonos = ['Formal', 'Informal', 'Técnico', 'Creativo']
const estados = ['Borrador', 'Publicado', 'Archivado']
const documentData = ref({})
const systemMessage = `
Eres una herramienta para crear documentación inteligente.
Debes devolver siempre únicamente JSON puro, sin ningún texto adicional, sin comentarios, sin encabezados ni bloques de código.
No incluyas explicaciones, ni etiquetas, ni texto fuera del JSON.
El JSON debe tener formato válido y correcto para poder parsearse sin errores.
Si algún campo no tiene información, usa null.
Nunca devuelvas fragmentos de código o ejemplos que no sean JSON.
Al darte la intrucción en este prompt intenta recrear una estructura muy extensa de documentación, con múltiples secciones, subsecciones y detalles.
`;


const respuestaIA = ref('')
const cargando = ref(false)


const generarContenidoIA = async () => {
  const prompt = `
    Título: ${datos.value.titulo}
    Descripción: ${datos.value.descripcion}
    Instrucciones: ${datos.value.instrucciones}
    Tono: ${datos.value.tono || 'Formal'}
    Contenido: ${systemMessage}
  `.trim()

  if (!datos.value.titulo) {
    $q.notify({ type: 'warning', message: 'Especifica al menos un título o tema para generar contenido' })
    return
  }

  cargando.value = true
  await getInformation(prompt)
  respuestaIA.value = documentData.value.contenido || ''
  cargando.value = false
}



const getInformation = async (messageText) => {
  try {
    const response = await axios.post('http://localhost:5168/api/documentacion/generar', {
      prompt: (systemMessage + "\n\n" + messageText),
    })
    console.log('Respuesta de la IA:', response.data.text)
    console.log('Respuesta de la IA:', JSON.parse(response.data.text))
    documentData.value = JSON.parse(response.data.text)
  } catch (error) {
    console.error('Error al generar el documento:', error)
  }
}

const usarContenidoGenerado = () => {
  if (!documentData.value.titulo) {
    $q.notify({ type: 'warning', message: 'No hay contenido generado aún' })
    return
  }

  datos.value.titulo = documentData.value.titulo || ''
  datos.value.descripcion = documentData.value.descripcion || ''
  datos.value.contenido = documentData.value.contenido || ''
  datos.value.categoria = documentData.value.categoria || null
  datos.value.tono = documentData.value.tono || null
}

const guardarComoDocumento = async () => {
  if (!respuestaIA.value || respuestaIA.value.trim() === '') {
    $q.notify({
      type: 'warning',
      message: 'No hay contenido generado para guardar',
    })
    return
  }

  cargando.value = true

  const datosGenerados = {
    ...datos.value,
    contenido: respuestaIA.value,
    creado_ia: true
  }

  // Por si el usuario no llenó título, categoría o tono
  if (!datosGenerados.titulo) datosGenerados.titulo = 'Documento generado por IA'
  if (!datosGenerados.categoria) datosGenerados.categoria = 'General'
  if (!datosGenerados.tono) datosGenerados.tono = 'Formal'

  try {
    const response = await axios.post('http://localhost:5168/api/documentos', datosGenerados)
    console.log('Documento guardado:', response.data)

    $q.notify({
      type: 'positive',
      message: 'Documento generado guardado exitosamente',
    })

    // Opcional: limpiar campos
    datos.value = {
      titulo: '',
      descripcion: '',
      categoria: null,
      tono: null,
      contenido: '',
      instrucciones: '',
      estado: 'Borrador',
      creado_ia: true
    }

    respuestaIA.value = ''

  } catch (error) {
    console.error('Error al guardar contenido generado:', error)
    $q.notify({
      type: 'negative',
      message: 'Error al guardar contenido generado',
    })
  } finally {
    cargando.value = false
  }
}

</script>
