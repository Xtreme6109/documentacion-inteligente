<template>
  <div class="q-pa-md">
    <div class="border-box">
      <div class="text-h4">Política de Privacidad</div>
      <div style="height: 250px; overflow-y: auto" ref="scrollTarget1">
        <q-infinite-scroll @load="loadMore" :scroll-target="scrollTarget1">
          <div class="caption">
            <q-list>
              <q-item v-for="politica in politicas" :key="politica.id">
                <q-item-section>
                  <q-item-label class="text-h6">{{ politica.headline }}</q-item-label>
                  <q-item-label caption>{{ politica.subtitle }}</q-item-label>
                  <q-item-label class="text-grey-7">{{ politica.title }}</q-item-label>
                </q-item-section>
                <q-item-section side>
                  <q-badge color="primary" outline>{{ politica.action }}</q-badge>
                </q-item-section>
              </q-item>
            </q-list>
          </div>
        </q-infinite-scroll>
      </div>
    </div>
  </div>
  <div class="q-pa-md">
      <div class="row q-col-gutter-sm row items-stretch">
        <div class="col-12 col-md">
          <div class="border-box fit">
            <div class="column justify-between full-height q-pa-md">
              <div class="text-h4">Crear Nuevo Documento</div>
              <div class="text-body1 q-mt-md">
                <p>
                  Con esta opción, puedes crear documentos utilizando la inteligencia artificial. Solo
                  tienes que usar las herramientas disponibles para generar y organizar tus documentos
                  de forma rápida y fácil.
                </p>
                <p>
                  Por favor, lea nuestra Política de Privacidad para conocer nuestras prácticas de privacidad con
                  respecto a la información utilizada para general los documentos.
                </p>
              </div>
              <q-btn color="primary" label="Diseñar" to="/peticion-documento-ia" />
            </div>
          </div>
        </div>
      </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const politicas = [
  {
    id: 1,
    action: 'No compartir',
    headline: 'Datos financieros y de ventas',
    subtitle:
      'Evitar compartir información relacionada con ingresos, costos, presupuestos, proyecciones o cualquier análisis financiero interno de la empresa.',
    title: 'Política de Seguridad: Información Financiera',
  },
  {
    id: 2,
    action: 'No divulgar',
    headline: 'Estrategias de negocio y marketing',
    subtitle:
      'Restringir el acceso a planes estratégicos, campañas de marketing, estudios de mercado o cualquier dato que pueda comprometer la ventaja competitiva.',
    title: 'Política de Seguridad: Estrategias Empresariales',
  },
  {
    id: 3,
    action: 'Confidencial',
    headline: 'Estado de proyectos y desarrollo interno',
    subtitle:
      'No compartir detalles sobre proyectos en curso, metodologías, cronogramas o cualquier información sobre el desarrollo de productos y servicios.',
    title: 'Política de Seguridad: Proyectos Internos',
  },
  {
    id: 4,
    action: 'Restringido',
    headline: 'Evaluaciones y reportes internos',
    subtitle:
      'Evitar divulgar reportes de auditoría, evaluaciones de desempeño, métricas clave o cualquier documento interno destinado solo para la empresa.',
    title: 'Política de Seguridad: Reportes Internos',
  },
  {
    id: 5,
    action: 'Prohibido compartir',
    headline: 'Documentación técnica y operativa',
    subtitle:
      'No compartir manuales internos, protocolos de seguridad, documentación sobre sistemas ERP o cualquier información técnica confidencial.',
    title: 'Política de Seguridad: Documentación Técnica',
  },
]

const scrollTarget1 = ref(null)


</script>

<style lang="scss" scoped>
.border-box {
  border: 1px solid #ccc;
  border-radius: 6px;
  padding: 16px;
}

.no-x-padding-items {
  padding: 8px 0px;
}
</style>
