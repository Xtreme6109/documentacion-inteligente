const menuComponent = () => import('src/layouts/integraciones-vistas/menu-principal.vue')

const routes = [
  //LOGIN
  {
    path: '/',
    children: [
      { path: '', component: () => import('src/pages/vistas-generales/pagina-login.vue') },
    ],
  },

  //CERRAR SESION
  {
    path: '/cerrar-sesion',
    component: () => import('src/components/componentes-generales/cerrar-sesion.vue'),
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
          import('src/pages/control-documentacion/creacion-documentacion/crear-documento.vue'),
      },
    ],
  },
  {
    path: '/peticion-documento-ia',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-documentacion/creacion-documentacion/peticion-documento-ia.vue'),
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
    path: '/puntos-clave-gpt',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/asistente-gpt/puntos-clave-gpt.vue'),
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
  {
    path: '/reporte-consumo-tokens',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-reportes/reporte-tokens.vue'),
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
    {
    path: '/crear-roles',
    component: menuComponent,
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/control-administracion/control-roles.vue'),
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
