const menuComponent = () => import('src/layouts/integraciones-vistas/menu-principal.vue')

const routes = [
  //LOGIN
  {
    path: '/',
    children: [
      { path: '', component: () => import('src/pages/vistas-generales/pagina-login.vue') },
    ],
  },

  //MENU DE INICIO
  {
    path: '/inicio',
    component: menuComponent,
    children: [
      { path: '', component: () => import('src/pages/vistas-generales/pagina-principal.vue') },
    ],
  },

  //CONTROL DE DOCUMENTACION
  {
    path: '/control-documentacion',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-documentacion/crear-documento.vue'),
      },
    ],
  },
  {
    path: '/subir-documento',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-documentacion/subir-documento.vue'),
      },
    ],
  },
  {
    path: '/mis-documentos',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-documentacion/mis-documentos.vue'),
      },
    ],
  },
  {
    path: '/versiones-documento',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-documentacion/versiones-documento.vue'),
      },
    ],
  },

  //ASISTENTE GPT
  {
    path: '/redactar-contenido',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/asistente-gpt/redactar-contenido.vue'),
      },
    ],
  },
  {
    path: '/resumir-documento',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/asistente-gpt/resumir-documento.vue'),
      },
    ],
  },
  {
    path: '/clasificar-por-tema',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/asistente-gpt/clasificar-tema.vue'),
      },
    ],
  },

  //REPORTES
  {
    path: '/documentos-por-usuario',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-reportes/reportes-usuario.vue'),
      },
    ],
  },
  {
    path: '/documentos-por-categoria',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-reportes/reportes-categoria.vue'),
      },
    ],
  },

  //CONTROL DE ADMINITRACIÓN
  {
    path: '/control-usuarios',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-administracion/control-usuarios.vue'),
      },
    ],
  },
  {
    path: '/crear-categoria',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-administracion/control-categorias.vue'),
      },
    ],
  },

    //CONTROL DE CUENTA
    {
      path: '/perfil-usuario',
      component: menuComponent,
      children: [
        {
          path: '',
          component: () =>
            import('src/pages/control-cuenta/control-perfil.vue'),
        },
      ],
    },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('src/pages/vistas-generales/pagina-no-encontrada.vue'),
  },
]

export default routes
