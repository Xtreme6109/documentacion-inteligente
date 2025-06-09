<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />

        <q-toolbar-title> Documentación Inteligente </q-toolbar-title>
      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <div class="column full-height">
        <q-list class="col">
          <q-item-label header> Menu Principal </q-item-label>
          <GroupedLinks :links="linksList" />
        </q-list>

        <q-item
          clickable
          tag="router-link"
          to="/cerrar-sesion"
          class="q-mt-auto"
        >
          <q-item-section avatar>
            <q-icon name="logout" />
          </q-item-section>

          <q-item-section>
            <q-item-label>Cerrar sesión</q-item-label>
          </q-item-section>
        </q-item>
      </div>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup>
import { ref } from 'vue'
import GroupedLinks from 'src/components/componentes-generales/control-de-links-agrupados.vue'
const linksList = [
  {
    title: 'Crear con IA',
    caption: 'Documentos',
    icon: 'auto_mode', // Icono para creación automática
    link: '/control-documentacion'
  },
  {
    title: 'Subir manualmente',
    caption: 'Documentos',
    icon: 'upload_file', // Representa carga manual
    link: '/subir-documento'
  },
  {
    title: 'Mis documentos',
    caption: 'Documentos',
    icon: 'folder_open', // Icono clásico para documentos
    link: '/mis-documentos'
  },
  /*{
    title: 'Versiones',
    caption: 'Documentos',
    icon: 'change_history', // Representa cambios o versiones
    link: '/versiones-documento'
  },*/
  {
    title: 'Redactar contenido',
    caption: 'Asistente GPT',
    icon: 'edit_note', // Ideal para redacción asistida
    link: '/redactar-contenido'
  },
  {
    title: 'Resumir documento',
    caption: 'Asistente GPT',
    icon: 'summarize', // Icono específico para resumen
    link: '/resumir-documento'
  },
  /*{
    title: 'Clasificar por tema',
    caption: 'Asistente GPT',
    icon: 'label', // Representa clasificación o etiquetas
    link: '/clasificar-por-tema'
  },*/
  {
    title: 'Documentos por usuario',
    caption: 'Reportes',
    icon: 'person', // Representa un usuario
    link: '/documentos-por-usuario'
  },
  {
    title: 'Documentos por categoría',
    caption: 'Reportes',
    icon: 'category', // Icono de categoría
    link: '/documentos-por-categoria'
  },
  {
    title: 'Reporte de consumo de Token',
    caption: 'Reportes',
    icon: 'token', // Icono representativo de token
    link: '/reporte-consumo-tokens'
  },
  {
    title: 'Usuarios',
    caption: 'Administración',
    icon: 'group', // Icono para gestión de usuarios
    link: '/control-usuarios'
  },
  {
    title: 'Categorías',
    caption: 'Administración',
    icon: 'local_offer', // Representa etiquetas/categorías
    link: '/crear-categoria'
  },
  {
    title: 'Roles',
    caption: 'Administración',
    icon: 'admin_panel_settings', // Icono recomendado para roles/permisos
    link: '/crear-roles'
  },
  {
    title: 'Perfil',
    caption: 'Cuenta',
    icon: 'account_circle', // Perfil de usuario
    link: '/perfil-usuario'
  }
];


// Ensure leftDrawerOpen is declared only once
const leftDrawerOpen = ref(false)

function toggleLeftDrawer() {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
</script>
