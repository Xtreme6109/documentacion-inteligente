<template>
  <div class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h5">Historial de versiones de documentos</div>
      </q-card-section>

      <q-card-section>
        <div v-for="doc in documentos" :key="doc.ID" class="q-mb-md">
          <q-expansion-item
            expand-separator
            icon="description"
            :label="doc.TITULO"
            header-class="bg-grey-2 text-bold"
          >
            <q-table
              :rows="doc.versiones"
              :columns="columnas"
              row-key="ID"
              flat
              dense
              bordered
              no-data-label="Sin versiones disponibles"
              class="q-mt-md"
            >
              <template v-slot:body-cell-acciones="props">
                <q-td align="center">
                  <q-btn
                    dense flat round icon="visibility"
                    color="primary"
                    @click="verVersion(props.row)"
                    :title="'Ver versión ' + props.row.NUMERO_VERSION"
                  />
                  <q-btn
                    dense flat round icon="download"
                    color="secondary"
                    @click="descargarVersion(props.row)"
                    :title="'Descargar versión ' + props.row.NUMERO_VERSION"
                  />
                  <q-btn
                    dense flat round icon="restore"
                    color="warning"
                    :loading="restaurandoId === props.row.ID"
                    @click="restaurarVersion(props.row)"
                    :title="'Restaurar versión ' + props.row.NUMERO_VERSION"
                  />
                </q-td>
              </template>
            </q-table>
          </q-expansion-item>
        </div>
      </q-card-section>
    </q-card>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useQuasar } from 'quasar'
// import axios from 'axios'

const $q = useQuasar()
const restaurandoId = ref(null)

// Simulación de documentos con versiones
const documentos = ref([
  {
    ID: 1,
    TITULO: 'Política de Seguridad',
    versiones: [
      {
        ID: 101,
        NUMERO_VERSION: 1,
        RUTA_ARCHIVO: '/docs/v1.pdf',
        FECHA_CREACION: '2024-01-01',
        NOTAS: 'Versión inicial'
      },
      {
        ID: 102,
        NUMERO_VERSION: 2,
        RUTA_ARCHIVO: '/docs/v2.pdf',
        FECHA_CREACION: '2024-02-01',
        NOTAS: 'Corrección de errores'
      }
    ]
  },
  {
    ID: 2,
    TITULO: 'Manual de Usuario',
    versiones: [
      {
        ID: 201,
        NUMERO_VERSION: 1,
        RUTA_ARCHIVO: '/docs/manual_v1.pdf',
        FECHA_CREACION: '2024-01-10',
        NOTAS: 'Primera entrega'
      }
    ]
  }
])

// Columnas de la tabla de versiones
const columnas = [
  {
    name: 'NUMERO_VERSION',
    label: 'Versión',
    field: 'NUMERO_VERSION',
    align: 'left'
  },
  {
    name: 'FECHA_CREACION',
    label: 'Fecha',
    field: 'FECHA_CREACION',
    format: val => new Date(val).toLocaleDateString(),
    align: 'left'
  },
  {
    name: 'NOTAS',
    label: 'Notas',
    field: 'NOTAS',
    align: 'left'
  },
  {
    name: 'acciones',
    label: 'Acciones',
    field: 'ID',
    align: 'center'
  }
]

// Abrir archivo en nueva pestaña
function verVersion(version) {
  window.open(version.RUTA_ARCHIVO, '_blank')
}

// Descargar archivo
function descargarVersion(version) {
  const link = document.createElement('a')
  link.href = version.RUTA_ARCHIVO
  link.download = `documento_v${version.NUMERO_VERSION}.pdf`
  link.click()
}

// Restaurar versión
async function restaurarVersion(version) {
  restaurandoId.value = version.ID
  try {
    // await axios.post('/api/restaurar-version', { versionId: version.ID })

    await new Promise(resolve => setTimeout(resolve, 1000)) // Simulación

    $q.notify({
      type: 'positive',
      message: `Versión ${version.NUMERO_VERSION} restaurada correctamente`
    })
  } catch (error) {
    $q.notify({
      type: 'negative',
      message: 'No se pudo restaurar la versión: ' + (error.message || '')
    })
  } finally {
    restaurandoId.value = null
  }
}
</script>
