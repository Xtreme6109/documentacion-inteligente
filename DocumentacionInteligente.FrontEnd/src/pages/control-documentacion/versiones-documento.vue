<template>
  <div class="q-pa-md">
    <q-card>
      <q-card-section>
        <div class="text-h6">Historial de versiones de documentos</div>
      </q-card-section>

      <q-card-section>
        <div v-for="doc in documentos" :key="doc.ID" class="q-mb-md">
          <q-expansion-item
            :label="doc.TITULO"
            icon="description"
            expand-separator
          >
            <q-table
              :rows="doc.versiones"
              :columns="columnas"
              flat
              dense
              row-key="ID"
              no-data-label="Sin versiones disponibles"
            >
              <template v-slot:body-cell-acciones="props">
                <q-td>
                  <q-btn dense flat icon="visibility" @click="verVersion(props.row)" />
                  <q-btn dense flat icon="download" @click="descargarVersion(props.row)" />
                  <q-btn
                    dense flat icon="restore"
                    color="warning"
                    @click="restaurarVersion(props.row)"
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

const documentos = ref([])

const columnas = [
  { name: 'NUMERO_VERSION', label: 'Versión', field: 'NUMERO_VERSION', align: 'left' },
  { name: 'FECHA_CREACION', label: 'Fecha', field: row => new Date(row.FECHA_CREACION).toLocaleString() },
  { name: 'NOTAS', label: 'Notas', field: 'NOTAS', align: 'left' },
  { name: 'acciones', label: 'Acciones', field: 'ID' }
]

documentos.value = [
  {
    ID: 1,
    TITULO: 'Política de Seguridad',
    versiones: [
      { ID: 101, NUMERO_VERSION: 1, RUTA_ARCHIVO: '/docs/v1.pdf', FECHA_CREACION: '2024-01-01', NOTAS: 'Versión inicial' },
      { ID: 102, NUMERO_VERSION: 2, RUTA_ARCHIVO: '/docs/v2.pdf', FECHA_CREACION: '2024-02-01', NOTAS: 'Corrección de errores' }
    ]
  }
]

function verVersion(version) {
  window.open(version.RUTA_ARCHIVO, '_blank')
}

function descargarVersion(version) {
  window.open(version.RUTA_ARCHIVO, '_blank')
}

// function restaurarVersion(version) {
//  
//   // axios.post('/api/restaurar-version', { versionId: version.ID })
// }
</script>
