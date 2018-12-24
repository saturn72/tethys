import { RouteConfig } from 'vue-router/types/router';

export const HttpCallRoutes: RouteConfig[] = [
  {
    path: '/httpcalls',
    component: () => import(/* webpackChunkName: "counter" */ './HttpCallList.vue').then((m: any) => m.default),
  },
];
