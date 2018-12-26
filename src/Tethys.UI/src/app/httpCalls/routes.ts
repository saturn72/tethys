import { RouteConfig } from 'vue-router/types/router';

export const HttpCallRoutes: RouteConfig[] = [
  {
    name: 'list',
    path: '/httpcall',
    component: () => import(/* webpackChunkName: "counter" */ './HttpCallList.vue').then((m: any) => m.default),
  },
  {
    name: 'edit',
    path: '/httpcall/:id/edit',
    component: () => import(/* webpackChunkName: "counter" */ './HttpCallEdit.vue').then((m: any) => m.default),
  },
];
