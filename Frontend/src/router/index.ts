import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/tech',
      name: 'tech',
      component: () => import('../views/TechView.vue')
    },
    {
      path: '/auto',
      name: 'auto',
      component: () => import('../views/AutoView.vue')
    },
    {
      path: '/models',
      name: 'models',
      component: () => import('../views/ModelsView.vue')
    },
    {
      path: '/contact-me',
      name: 'contact-me',
      component: () => import('../views/ContactMeView.vue')
    },
    {
      path: '/todo',
      name: 'todo',
      component: () => import('../components/ToDoList.vue')
    }
  ]
})

export default router
