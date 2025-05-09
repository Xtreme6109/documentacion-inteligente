const routes = [
  {
    path: '/',
    children: [
      { path: '', component: () => import('src/pages/paginas-generales/pagina-login.vue') },
    ],
  },
  {
    path: '/inicio',
    component: () => import('src/layouts/vistas-generales/menu-principal.vue'),
    children: [
      { path: '', component: () => import('src/pages/paginas-generales/pagina-principal.vue') },
    ],
  },
  {
    path: '/control-documentacion',
    component: () => import('src/layouts/vistas-generales/menu-principal.vue'),
    children: [
      {
        path: '',
        component: () =>
          import('src/pages/paginas-control-documentacion/documentacion-principal.vue'),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('src/pages/paginas-generales/pagina-no-encontrada.vue'),
  },
]

export default routes
