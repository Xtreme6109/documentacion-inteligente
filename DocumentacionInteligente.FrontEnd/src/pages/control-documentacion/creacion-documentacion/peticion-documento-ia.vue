<template>
  <div class="q-pa-md">
    <div class="border-box">
      <div class="text-h4">Consola de Generación de Documentos General</div>
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
        <q-btn
          label="Descargar documento generado"
          icon="download"
          color="secondary"
          class="q-ml-sm"
          :disable="Object.keys(documentData).length === 0"
          @click="downloadWordDocument"
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
import { useQuasar } from 'quasar'

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

const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})


const $q = useQuasar()

api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => Promise.reject(error)
)

const getInformation = async (messageText) => {
  cargando.value = true;
  try {
    const response = await api.post('/documentacion/generar', {
      prompt: systemMessage + "\n\n" + messageText
    })

    console.log('Respuesta de la IA:', response.data.text)

    const resultadoIA = JSON.parse(response.data.text)
    documentData.value = resultadoIA

    await registrarHistorial({
      UsuarioId: getUsuarioIdFromToken(),
      Prompt: messageText,
      CantidadPalabrasPrompt: messageText.split(/\s+/).length,
      TokensEntrada: messageText.split(/\s+/).length,
      TokensSalida: JSON.stringify(resultadoIA).length,
      Resultado: JSON.stringify(resultadoIA)
    })

  } catch (error) {
    console.error('Error al generar el documento:', error)
    $q.notify({ type: 'negative', message: 'Error al generar el documento: ' + error.message })
  } finally {
    cargando.value = false
  }
}
function mapearPropiedadesDinamicas(obj) {
  const resultado = {};
  Object.keys(obj).forEach((key) => {
    resultado[key] = obj[key];
  });
  return resultado;
}

function transformarDocumento(original) {
  return {
    títuloDelDocumento: original["Título del Documento"] || "",
    fechaDeEdición: original["Fecha de Edición"] ? new Date(original["Fecha de Edición"]).toISOString() : new Date().toISOString(),
    version: original["Versión"] || "",
    códigoDelDocumento: original["Código del Documento"] || "",
    elaboradoPor: original["Elaborado por"] || "",
    revisadoPor: original["Revisado por"] || "",
    iObjetivo: original["I. Objetivo"] || "",
    iiAlcance: original["II. Alcance"] || "",
    iiiResponsabilidades: mapearPropiedadesDinamicas(original["III. Responsabilidades"] || {}),
    ivDesarrollo: mapearPropiedadesDinamicas(original["IV. Desarrollo"] || {}),
    vVigencia: original["V. Vigencia"] || "",
    viReferenciasBibliográficas: original["VI. Referencias Bibliográficas"] || "",
    viiHistorialDeCambioDeDocumentos: (original["VII. Historial de cambio de Documentos"] || []).map(h => ({
      number: h.number || 0,
      date: h.date ? new Date(h.date).toISOString() : new Date().toISOString(),
      description: h.description || ""
    })),
    viiiFirmas: original["VIII. Firmas"] || "",

    // Campos adicionales obligatorios
    titulo: original["Título del Documento"] || "",
    hoja: 0,
    totalHojas: 0,
    autorizadoPor: original["Autorizado por"] || "string", // completar adecuadamente
    fechaDivulgacion: new Date().toISOString(),

    // Campos que faltaban en tu función
    categoria: original["Categoria"] || 0,
    nombreCategoria: original["Nombre Categoria"] || "string",
    usuarioCreadorId: original["Usuario Creador Id"] || 0,
    usuarioId: original["Usuario Id"] || 0,
    fechaInicio: original["Fecha Inicio"] ? new Date(original["Fecha Inicio"]).toISOString() : new Date().toISOString(),
    fechaFin: original["Fecha Fin"] ? new Date(original["Fecha Fin"]).toISOString() : new Date().toISOString(),
    nombreUsuarioCreador: original["Nombre Usuario Creador"] || "string",
  };
}



const downloadWordDocument = async () => {
  let data = transformarDocumento(documentData.value);
  console.log(transformarDocumento)
  console.log(documentData.value)
  if (!data || Object.keys(data).length === 0) {
    $q.notify({ type: 'warning', message: 'No hay documento para descargar' });
    return;
  }

  try {
    const response = await fetch('http://localhost:5168/api/reporte/reporte-documento-word', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });

    if (!response.ok) {
      throw new Error(`Error en la petición: ${response.status} ${response.statusText}`);
    }

    // Recibir el blob del archivo Word
    const blob = await response.blob();

    // Crear enlace de descarga
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;

    // Nombre para el archivo .docx (puedes ajustar el nombre)
    const fileName = (documentData.value.títuloDelDocumento || 'documento') + '.docx';
    link.download = fileName;

    document.body.appendChild(link);
    link.click();

    // Limpieza
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  } catch (error) {
    console.error(error);
    $q.notify({ type: 'negative', message: 'Error al descargar el documento: ' + error.message });
  }
}



const paginatedContent = computed(() => {
  const data = documentData.value
  if (Object.keys(data).length === 0) return []

  const content = []

  for (const [key, value] of Object.entries(data)) {
  if ((key === "IV. Desarrollo" || key === "III. Responsabilidades") && typeof value === 'object' && !Array.isArray(value)) {
    content.push({ title: key, content: "" });

    for (const [subkey, subvalue] of Object.entries(value)) {
      if (Array.isArray(subvalue)) {
        content.push({ title: subkey, content: "", indent: true });

        subvalue.forEach((item, i) => {
          if (typeof item === 'object' && item !== null) {
            content.push({
              title: `${i + 1}`,
              content: "",
              indent: true,
              deeperIndent: true
            });

            for (const [itemKey, itemValue] of Object.entries(item)) {
              if (Array.isArray(itemValue)) {
                // Subarray dentro del objeto (ej: Subreacciones)
                content.push({
                  title: itemKey,
                  content: "",
                  indent: true,
                  deeperIndent: true,
                  deepestIndent: true
                });

                itemValue.forEach((subItem, j) => {
                  if (typeof subItem === 'object' && subItem !== null) {
                    content.push({
                      title: `${j + 1}`,
                      content: "",
                      indent: true,
                      deeperIndent: true,
                      deepestIndent: true
                    });

                    for (const [k, v] of Object.entries(subItem)) {
                      content.push({
                        title: `${k}:`,
                        content: Array.isArray(v) ? v.join(', ') : v,
                        indent: true,
                        deeperIndent: true,
                        deepestIndent: true,
                        fourthIndent: true
                      });
                    }
                  }
                });

              } else {
                content.push({
                  title: `${itemKey}:`,
                  content: Array.isArray(itemValue) ? itemValue.join(', ') : itemValue,
                  indent: true,
                  deeperIndent: true,
                  deepestIndent: true
                });
              }
            }

            content.push({
              title: "",
              content: "",
              indent: true,
              deeperIndent: true,
              deepestIndent: true
            });
          }
        });

      } else if (typeof subvalue === 'object' && subvalue !== null) {
        // Subobjetos simples o anidados
        const isObjectOfObjects = Object.values(subvalue).every(
          v => typeof v === 'object' && v !== null && !Array.isArray(v)
        );

        content.push({ title: subkey, content: "", indent: true });

        if (isObjectOfObjects) {
          for (const [itemTitle, itemValue] of Object.entries(subvalue)) {
            content.push({
              title: itemTitle,
              content: "",
              indent: true,
              deeperIndent: true
            });

            for (const [innerKey, innerValue] of Object.entries(itemValue)) {
              content.push({
                title: `${innerKey}:`,
                content: Array.isArray(innerValue) ? innerValue.join(', ') : innerValue ?? "",
                indent: true,
                deeperIndent: true,
                deepestIndent: true
              });
            }
          }
        } else {
          for (const [innerKey, innerValue] of Object.entries(subvalue)) {
            content.push({
              title: `${innerKey}:`,
              content: Array.isArray(innerValue) ? innerValue.join(', ') : innerValue ?? "",
              indent: true,
              deeperIndent: true
            });
          }
        }

      } else {
        // Valores simples
        content.push({
          title: subkey,
          content: subvalue ?? "",
          indent: true
        });
      }
    }

  } else if (typeof value === 'string') {
    content.push({ title: key, content: value ?? "" });

  } else if (typeof value === 'object' && value !== null) {
    content.push({ title: key, content: value });

  } else {
    content.push({ title: key, content: "" });
  }
}


  // Paginación
  const pages = []
  let currentPage = []
  let currentHeight = 0
  const maxHeight = pageHeight - 100;

  content.forEach((section) => {
    let estimatedHeight = 0

    if (section.title === 'VII. Historial de cambio de Documentos') {
      const historyRows = Array.isArray(section.content) ? [...section.content] : []
      const rowHeight = 25
      while (historyRows.length) {
        const availableSpace = Math.floor((maxHeight - currentHeight) / rowHeight)
        const rowsToAdd = historyRows.splice(0, availableSpace)
        if (rowsToAdd.length === 0) break
        currentPage.push({ title: section.title, content: rowsToAdd })
        currentHeight += rowsToAdd.length * rowHeight
        if (currentHeight >= maxHeight) {
          pages.push(currentPage)
          currentPage = []
          currentHeight = 0
        }
      }
    } else {
      if (typeof section.content === 'string') {
        estimatedHeight = section.content.length * 0.6
      } else if (Array.isArray(section.content)) {
        estimatedHeight = section.content.length * 25
      } else {
        estimatedHeight = 50
      }

      if (currentHeight + estimatedHeight > maxHeight) {
        pages.push(currentPage)
        currentPage = []
        currentHeight = 0
      }
      currentPage.push(section)
      currentHeight += estimatedHeight
    }
  })

  if (currentPage.length) pages.push(currentPage)
  if (pages.length === 0) pages.push([])

  return pages
})



const registrarHistorial = async (datosHistorial) => {
  try {
    console.log('Enviando datosHistorial:', datosHistorial)

    const res = await api.post('/historialdocumentos/registrar', {
      UsuarioId: datosHistorial.UsuarioId,
      Prompt: datosHistorial.Prompt,
      CantidadPalabrasPrompt: datosHistorial.CantidadPalabrasPrompt,
      TokensEntrada: datosHistorial.TokensEntrada,
      TokensSalida: datosHistorial.TokensSalida,
      Resultado: datosHistorial.Resultado
    })

    console.log('Respuesta del servidor:', res.data)

    if (res.data.success) {
      $q.notify({ type: 'positive', message: 'Historial registrado con ID ' + res.data.id })
    } else {
      $q.notify({ type: 'warning', message: 'El servidor respondió pero no con éxito.' })
    }
  } catch (error) {
    console.error('Error al registrar historial:', error)
    if (error.response) {
      console.error('Respuesta del servidor con error:', error.response.data)
    }
    $q.notify({ type: 'negative', message: 'Error al registrar historial: ' + error.message })
  }
}

function getUsuarioIdFromToken() {
  const token = localStorage.getItem('token')
  if (!token) return null

  const payload = JSON.parse(atob(token.split('.')[1]))
  return parseInt(payload.UsuarioId)
}




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

.indent-1 {
  margin-left: 16px;
}
.indent-2 {
  margin-left: 32px;
}
.indent-3 {
  margin-left: 48px;
}
.indent-4 {
  margin-left: 64px;
}
</style>
