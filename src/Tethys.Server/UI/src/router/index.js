import Vue from 'vue'
import Router from 'vue-router'

// Containers
const DefaultContainer = () =>
  import('@/containers/DefaultContainer')

// Views
const Dashboard = () =>
  import('@/views/Dashboard')

const HttpList = () =>
  import('@/views/http/HttpList')

Vue.use(Router)

export default new Router({
  mode: 'hash', // https://router.vuejs.org/api/#mode
  linkActiveClass: 'open active',
  scrollBehavior: () => ({
    y: 0
  }),
  routes: [{
    path: '/',
    redirect: '/dashboard',
    name: 'Home',
    component: DefaultContainer,
    children: [{
        path: 'dashboard',
        name: 'Dashboard',
        component: Dashboard
      },
      {
        path: 'http',
        name: 'http',
        component: {
          render(c) {
            return c('router-view')
          }
        },
        children: [{
          path: 'list',
          name: 'List',
          component: HttpList
        }]
      }
    ]
  }]
})
