<template>
  <div class="q-pa-md">
    <q-form ref="formRef" @submit.prevent="subirDocumento">
      <q-card class="q-pa-lg shadow-2">
        <div class="text-h5 q-mb-lg">Subir Documento Manualmente</div>

        <div class="q-gutter-md">

          <!-- Selector / input híbrido para título -->
          <q-select
            v-model="formData.titulo"
            label="Título del documento"
            outlined
            use-input
            input-debounce="300"
            :options="titulos"
            clearable
            :rules="desactivarRules ? [] : [val => !!val || 'El título es requerido']"
            :hide-dropdown-icon="false"
            @new-value="agregarTituloNuevo"
            new-value-mode="add"
          />



          <!-- Campo para versión -->
          <q-input
            v-model="formData.version"
            label="Versión del documento"
            outlined
            clearable
            :rules="desactivarRules ? [] : [val => !!val || 'La versión es requerida']"
          />

          <q-file
            v-model="formData.archivo"
            label="Seleccionar archivo"
            outlined
            use-chips
            :rules="desactivarRules ? [] : [val => !!val || 'Debe seleccionar un archivo']"
          />

          <q-input
            v-model="formData.descripcion"
            label="Descripción"
            type="textarea"
            outlined
            autogrow
            :rules="desactivarRules ? [] : [val => !!val || 'La descripción es requerida']"
          />

          <q-select
            v-model="formData.categoria"
            label="Categoría"
            outlined
            :options="categorias"
            emit-value
            map-options
            :rules="desactivarRules ? [] : [val => !!val || 'Seleccione una categoría']"
          />

          <q-select
            v-model="formData.estado"
            label="Estado del documento"
            outlined
            :options="estados"
            emit-value
            map-options
            :rules="desactivarRules ? [] : [val => !!val || 'Seleccione un estado']"
          />

          <q-checkbox
            v-model="formData.creadoIA"
            label="Documento creado por IA"
          />

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
    </q-form>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useQuasar } from 'quasar'
import axios from 'axios'
import { watch } from 'vue'

const $q = useQuasar()

const api = axios.create({
  baseURL: 'http://localhost:5168/api'
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(error)
})

const subiendo = ref(false)
const desactivarRules = ref(false)

const titulos = ref([])
const formData = ref({
  titulo: '',
  version: '',
  archivo: null,
  descripcion: '',
  categoria: null,
  estado: null,
  creadoIA: false
})

const categorias = [
  { label: 'Legal', value: 1 },
  { label: 'Técnico', value: 2 },
  { label: 'Comercial', value: 3 }
]

const estados = [
  { label: 'Borrador', value: 'Borrador' },
  { label: 'Aprobado', value: 'Aprobado' },
  { label: 'Rechazado', value: 'Rechazado' }
]

async function subirDocumento() {
  // Validaciones simples (puedes mejorar con formRef.validate() si quieres)
  if (!formData.value.titulo) {
    $q.notify({ type: 'negative', message: 'El título es obligatorio' })
    return
  }
  if (!formData.value.version) {
    $q.notify({ type: 'negative', message: 'La versión es obligatoria' })
    return
  }
  if (!formData.value.archivo) {
    $q.notify({ type: 'negative', message: 'Debe seleccionar un archivo' })
    return
  }
  if (!formData.value.descripcion) {
    $q.notify({ type: 'negative', message: 'La descripción es obligatoria' })
    return
  }
  if (!formData.value.categoria) {
    $q.notify({ type: 'negative', message: 'Debe seleccionar una categoría' })
    return
  }
  if (!formData.value.estado) {
    $q.notify({ type: 'negative', message: 'Debe seleccionar un estado' })
    return
  }

  subiendo.value = true

  try {
    const payload = new FormData()
    payload.append('titulo', formData.value.titulo)
    payload.append('version', formData.value.version)
    payload.append('archivo', formData.value.archivo)
    payload.append('descripcion', formData.value.descripcion)
    payload.append('categoriaId', formData.value.categoria)
    payload.append('estado', formData.value.estado)
    payload.append('creadoIA', formData.value.creadoIA ? 'true' : 'false')

    const response = await api.post('/DocumentosSubida/subir', payload, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })

    $q.notify({ type: 'positive', message: response.data.mensaje || 'Documento subido exitosamente' })

    desactivarRules.value = true
    // Resetear formulario
    formData.value = {
      titulo: '',
      version: '',
      archivo: null,
      descripcion: '',
      categoria: null,
      estado: null,
      creadoIA: false
    }

    setTimeout(() => {
      desactivarRules.value = false
    }, 200)
  } catch (error) {
    console.error(error)
    $q.notify({ type: 'negative', message: 'Error al subir documento: ' + (error.response?.data || error.message) })
  } finally {
    subiendo.value = false
  }
}

async function cargarTitulos() {
  try {
    const response = await api.get('/Documentos/titulos')
    titulos.value = response.data
  } catch (error) {
    console.error('Error al cargar títulos:', error)
    $q.notify({ type: 'negative', message: 'No se pudieron cargar los títulos' })
  }
}

/*function limpiarDuplicados(array) {
  return [...new Set(array.map(x => x.toLowerCase()))].map(x => {
    return array.find(y => y.toLowerCase() === x)
  })
}*/
function agregarTituloNuevo(val, done) {
  const nuevoTitulo = val.trim()
  if (!nuevoTitulo) {
    done()
    return
  }

  if (!titulos.value.some(t => t.toLowerCase() === nuevoTitulo.toLowerCase())) {
    titulos.value.push(nuevoTitulo)
  }

  done(nuevoTitulo)
}


onMounted(() => {
  cargarTitulos()
})

watch(() => formData.value.titulo, async (nuevoTitulo) => {
  if (!nuevoTitulo) {
    // Limpiar versión y demás si título vacío
    formData.value.version = ''
    // puedes limpiar más campos si quieres
    return
  }

  try {
    const response = await api.get('/Documentos/version-mayor', {
      params: { titulo: nuevoTitulo }
    })

    const doc = response.data

    if (doc) {
      console.log('Documento encontrado:', doc)
      formData.value.version = doc.versioN_ACTUAL ? (doc.versioN_ACTUAL + 1).toString() : '1'
    }
  } catch (error) {
    if (error.response && error.response.status === 404) {
      // No existe documento con ese título, limpiar versión
      formData.value.version = ''
    } else {
      console.error('Error al obtener versión mayor:', error)
      $q.notify({ type: 'negative', message: 'Error al consultar documento' })
    }
  }
})
</script>
