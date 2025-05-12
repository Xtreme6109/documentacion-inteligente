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
              <q-btn color="primary" label="Diseñar" />
            </div>
          </div>
        </div>
        <div class="col-12 col-md">
          <div class="border-box fit">
            <div class="column full-height q-pa-md">
              <div class="text-h4">Últimos Documentos Creados</div>
                <div style="overflow-y: auto; height: 40ch;" ref="scrollTarget2">
                  <q-infinite-scroll @load="loadMore" :scroll-target="scrollTarget2">
                    <div class="caption">
                      <q-list>
                        <q-item v-for="documento in documentos" :key="documento.id" clickable>
                          <q-item-section>
                            <q-item-label class="text-h6">{{ documento.headline }}</q-item-label>
                            <q-item-label caption>{{ documento.subtitle }}</q-item-label>
                            <q-item-label class="text-grey-7">{{ documento.title }}</q-item-label>
                          </q-item-section>
                          <q-item-section side>
                            <q-badge color="primary" outline>{{ documento.action }}</q-badge>
                          </q-item-section>
                        </q-item>
                      </q-list>
                    </div>
                  </q-infinite-scroll>
                </div>
              </div>
            </div>
        </div>
      </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const documentos = [
  {
    id: 1,
    action: '15 min',
    headline: 'Informe de Desempeño de Ventas',
    subtitle:
      'Este informe proporciona un análisis profundo de las cifras de ventas obtenidas durante el último trimestre, comparando los resultados reales con las metas establecidas por la gerencia. Además, incluye gráficos de rendimiento por producto, región y equipo de ventas, identificando áreas de mejora y oportunidades de crecimiento.',
    title: 'Sistema de Gestión Empresarial - Ventas',
  },
  {
    id: 2,
    action: '2 hr',
    headline: 'Estrategia de Marketing Anual',
    subtitle:
      'Documento integral que define los objetivos estratégicos de marketing para el próximo año, abordando campañas publicitarias, posicionamiento de marca, análisis del mercado objetivo, canales de distribución y métricas clave de rendimiento. Incluye una hoja de ruta trimestral para el seguimiento y ajuste de tácticas.',
    title: 'Sistema de Gestión Empresarial - Marketing',
  },
  {
    id: 3,
    action: '6 hr',
    headline: 'Estado de Proyectos y Tareas',
    subtitle:
      'Reporte completo que detalla el avance de todos los proyectos activos en la organización. Contiene el estado actual de cada tarea, responsables asignados, fechas estimadas de finalización y obstáculos detectados. Sirve como herramienta clave para la toma de decisiones gerenciales y la reasignación eficiente de recursos.',
    title: 'Sistema de Gestión Empresarial - Proyectos',
  },
  {
    id: 4,
    action: '12 hr',
    headline: 'Análisis y Evaluación Financiera',
    subtitle:
      'Análisis exhaustivo de los resultados financieros obtenidos durante el último año fiscal. Incluye el balance general, estado de resultados, flujo de efectivo, razones financieras clave y un apartado de proyecciones económicas a corto y mediano plazo. Se presenta con comentarios ejecutivos y recomendaciones contables.',
    title: 'Sistema de Gestión Empresarial - Finanzas',
  },
  {
    id: 5,
    action: '18 hr',
    headline: 'Manual de Procedimientos del Sistema ERP',
    subtitle:
      'Este manual contiene la documentación técnica y funcional necesaria para la operación correcta del sistema ERP. Describe paso a paso los procesos para cada módulo (ventas, compras, inventarios, contabilidad, RRHH), así como procedimientos de contingencia, usuarios responsables y políticas internas de uso.',
    title: 'Sistema de Gestión Empresarial - TI',
  },
]

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
const scrollTarget2 = ref(null)

function loadMore() {
  setTimeout(() => {}, 1000)
}
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
