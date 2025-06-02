<template>
  <div class="q-pa-md">
    <div class="border-box">
      <div class="text-h4">Consola de Generación de Documentos</div>
      <div class="tw-flex flex-column q-mt-md">
        <q-card
            class="q-pa-0 q-my-lg text-black"
          flat
        >
          <p class="text-instructions-title">Indicaciones</p>
          <p class="text-instructions">
            Para generar un documento con inteligencia artificial, proporciona una descripción detallada del contenido que deseas incluir. Especifica el tipo de documento (informe, ensayo, contrato, etc.), su estructura (encabezado, objetivos, secciones, conclusiones), el tono y estilo (formal, técnico, persuasivo), y cualquier detalle relevante como temas específicos, datos clave o formatos especiales. Cuanta más información y contexto proporciones en tu solicitud, más preciso y útil será el documento generado.
          </p>
        </q-card>

        <q-input
          bottom-slots
          v-model="documentDescription"
          type="textarea"
          label="Escribe qué datos necesita tu documento aquí ..."
          :rules="rules"
          autogrow
          counter
          outlined
          bg-color="white"
          class="q-mb-md"
          clearable
        >

          <template v-slot:hint>
            Describe claramente el contenido, tipo y formato del documento que deseas generar.
          </template>
        </q-input>
        <q-btn
            label="Generar contenido"
            icon="smart_toy"
            color="primary"
            :loading="cargando"
            @click="getInformation(documentDescription)"
        />


        <div class="contenido-paginas q-mt-xl" v-if="paginatedContent.length > 0">
          <p class="text-preview-title q-text-center">Vista Previa</p>
          <div class="contenido-individual">
            <div
              v-for="(page, index) in paginatedContent"
              :key="index"
              class="page-wrapper"
            >
              <q-card
                ref="documentPages"
                class="q-pa-xl q-my-lg"
                style="aspect-ratio: 8.5 / 11; width: 100%; max-width: 816px;"
              >
              <DocumentHeader
                :title="documentData['Título del Documento']"
                :edit_date="documentData['Fecha de Edición']"
                :code="documentData['Código del Documento']"
                :version="documentData['Versión']"
                :person_prepared="documentData['Elaborado por']"
                :person_reviewed="documentData['Revisado por']"
                :person_authorized="documentData['Autorizó']"
                :sheet="index + 1"
                :total_sheets="paginatedContent.length"
                class="q-mb-lg"
              />
              <div v-for="(section, idx) in page" :key="idx">
                <div
                  v-if="!['Título del Documento', 'Código del Documento', 'Versión', 'Fecha de Edición', 'Elaborado por', 'Revisado por', 'Autorizó'].includes(section.title)"
                >
                  <!-- Subsección con doble sangría -->
                  <div
                    v-if="section.indent && section.deeperIndent"
                    class="q-ml-xl q-my-xs row no-wrap items-start"
                  >
                    <div class="q-mr-sm">◦</div>
                    <div>
                      <div class="text-subtitle2">{{ section.title }}</div>
                      <p class="q-mt-xs text-body2">{{ section.content }}</p>
                    </div>
                  </div>

                  <!-- Subsección con una sangría -->
                  <div
                    v-else-if="section.indent"
                    class="q-ml-md q-my-sm row no-wrap items-start"
                  >
                    <div class="q-mr-sm">•</div>
                    <div>
                      <div class="text-subtitle2">{{ section.title }}</div>
                      <p class="q-mt-xs text-body2">{{ section.content }}</p>
                    </div>
                  </div>

                  <!-- Sección especial: Historial -->
                  <div v-else-if="section.title === 'VII. Historial de cambio de Documentos'">
                    <div class="text-h5">{{ section.title }}</div>
                    <HistoryChanges :content="section.content" />
                  </div>

                  <!-- Sección normal sin indentación -->
                  <div v-else>
                    <div class="text-h5">{{ section.title }}</div>
                    <p class="q-my-md">
                      {{ section.content }}
                    </p>
                  </div>
                </div>
              </div>

              <!-- Pie de página -->
              <div
                class="q-pt-md q-text-center q-mt-auto q-text-caption text-grey q-full-width full-width"
                style="position: absolute; bottom: 30px; right: 0; left: 0; width: 100%; text-align: center;"
              >
                Página {{ index + 1 }} de {{ paginatedContent.length }}
              </div>
              </q-card>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import DocumentHeader from './cabecera-documentacion.vue'
import HistoryChanges from './historial-cambios.vue'
import axios from 'axios'

const documentDescription = ref('')
const rules = [(v) => v.length <= 4096 || 'El máximo es de 4,096 caracteres']
const documentData = ref({})
const pageHeight = 770;
const today = new Date().toISOString().split('T')[0];
const systemMessage = `
Eres una herramienta para crear documentación inteligente.
Debes devolver siempre únicamente JSON puro, sin ningún texto adicional, sin comentarios, sin encabezados ni bloques de código.
No incluyas explicaciones, ni etiquetas, ni texto fuera del JSON.
El JSON debe tener formato válido y correcto para poder parsearse sin errores.
Si algún campo no tiene información, usa null.
Nunca devuelvas fragmentos de código o ejemplos que no sean JSON.
Reemplaza todos los valores 'null' o '...' por texto completo y apropiado para el contexto, utilizando formato clave-valor.
Si el campo se refiere a fechas, usa la fecha actual "${today}".
Si el campo es una versión y no se indica ninguna, usa la versión "1.0".
Si un campo requiere una lista o subestructura, inclúyela completa.
Requisitos:
- Todos los campos deben contener datos válidos y completos.
- El contenido debe estar redactado de forma técnica y educativa.
- Mantén la estructura JSON utilizando claves exactas como están.
- No omitas secciones.
- El resultado debe estar listo para usarse como un documento formal técnico.
- Recuerda siempre utilizar la fecha actual cuando se requiera una fecha.
- No incluyas ningún tipo de comentario o explicación fuera del JSON.
- La longitud máxima del texto es de 25000 caracteres.
- La longitud mínima del texto es de 15000 caracteres.
- En el historial de cambios, en el apartado de fecha, utiliza esta fecha exacta: "${today}".
La estructura que tomarás es la siguente:
{
  "Título del Documento": null,
  "Fecha de Edición": null,
  "Versión": null,
  "Código del Documento": null,
  "Elaborado por": null,
  "Revisado por": null,
  "I. Objetivo": null,
  "II. Alcance": null,
  "III. Responsabilidades":  {
    "...": null
  },
  "IV. Desarrollo": {
    "...": null
  },
  "V. Vigencia": null,
  "VI. Referencias Bibliográficas": null,
  "VII. Historial de cambio de Documentos": [
    {
      "number": null,
      "date": null,
      "description": null
    }
  ],
  "VIII. Firmas": null
}
`;
const cargando = ref(false)

const getInformation = async (messageText) => {
  cargando.value = true;
  try {
    const response = await axios.post('http://localhost:5168/api/documentacion/generar', {
      prompt: (systemMessage + "\n\n" + messageText),
    })
    console.log('Respuesta de la IA:', response.data.text)
    console.log('Respuesta de la IA:', JSON.parse(response.data.text))
    documentData.value = JSON.parse(response.data.text)
    cargando.value = false;
  } catch (error) {
    console.error('Error al generar el documento:', error)
  }
}

const paginatedContent = computed(() => {
  const data = documentData.value
  if (Object.keys(data).length === 0) return []

  const content = []

  for (const [key, value] of Object.entries(data)) {
    if ((key === "IV. Desarrollo" || key === "III. Responsabilidades") && typeof value === 'object' && !Array.isArray(value)) {
      content.push({ title: key, content: "" })

      for (const [subkey, subvalue] of Object.entries(value)) {
        if (typeof subvalue === 'object' && subvalue !== null && !Array.isArray(subvalue)) {
          // Estructura anidada adicional (ej. pasos numerados)
          content.push({
            title: subkey,
            content: "",
            indent: true
          })

          for (const [innerKey, innerValue] of Object.entries(subvalue)) {
            content.push({
              title: `${innerKey}.`,
              content: innerValue ?? "",
              indent: true,
              deeperIndent: true
            })
          }
        } else {
          // Subestructura normal
          content.push({
            title: subkey,
            content: subvalue ?? "",
            indent: true
          })
        }
      }
    } else if (typeof value === 'string') {
      content.push({ title: key, content: value ?? "" })
    } else if (typeof value === 'object' && value !== null) {
      content.push({ title: key, content: value })
    } else {
      content.push({ title: key, content: "" })
    }
  }

  const pages = []
  let currentPage = []
  let currentHeight = 0
  const maxHeight = pageHeight - 100;

  content.forEach((section) => {
    let estimatedHeight = 0;

    if (section.title === 'VII. Historial de cambio de Documentos') {
      const historyRows = Array.isArray(section.content) ? [...section.content] : [];
      const rowHeight = 25;
      while (historyRows.length) {
        const availableSpace = Math.floor((maxHeight - currentHeight) / rowHeight);
        const rowsToAdd = historyRows.splice(0, availableSpace);
        if (rowsToAdd.length === 0) break;
        currentPage.push({ title: section.title, content: rowsToAdd });
        currentHeight += rowsToAdd.length * rowHeight;
        if (currentHeight >= maxHeight) {
          pages.push(currentPage);
          currentPage = [];
          currentHeight = 0;
        }
      }
    } else {
      if (typeof section.content === 'string') {
        estimatedHeight = section.content.length * 0.6;
      } else if (Array.isArray(section.content)) {
        estimatedHeight = section.content.length * 25;
      } else {
        estimatedHeight = 50;
      }

      if (currentHeight + estimatedHeight > maxHeight) {
        pages.push(currentPage);
        currentPage = [];
        currentHeight = 0;
      }
      currentPage.push(section);
      currentHeight += estimatedHeight;
    }
  });

  if (currentPage.length) pages.push(currentPage)
  if (pages.length === 0) pages.push([]);

  return pages
})

</script>

<style scoped lang="scss">
.text-instructions {
  font-family: 'Avant Garde', sans-serif;
  text-rendering: optimizeLegibility;
  font-size: 18px;
  background: #000;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  overflow-y: scroll;
}

.text-instructions-title {
  font-family: 'Avant Garde', sans-serif;
  text-rendering: optimizeLegibility;
  font-size: 20px;
  font-weight: bold;
  background: #000;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.text-preview-title {
  font-family: 'Avant Garde', sans-serif;
  text-rendering: optimizeLegibility;
  font-size: 25px;
  font-weight: bold;
  background: #000;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}


.contenido-paginas .contenido-individual {
  display: flex;
  flex-direction: row;
  justify-content: center;
  gap: 2%;
  flex-wrap: wrap;
}

@media (max-width: 600px) {
  .contenido-paginas .contenido-individual {
    flex-direction: column;
  }
}
</style>
