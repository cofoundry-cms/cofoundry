import Vue from 'vue'
import Router from 'vue-router'
import Home from './views/Home.vue'

Vue.use(Router)

export default new Router({
    mode: 'history',
    base: process.env.BASE_URL,
    routes: [{
        path: '/',
        name: 'home',
        component: Home
    }, {
        path: '/cat/:id',
        name: 'catDetails',
        component: () => import(/* webpackChunkName: "CatDetails" */ './views/CatDetails.vue')
    }, {
        path: '/login',
        name: 'login',
        component: () => import(/* webpackChunkName: "Login" */ './views/Login.vue')
    }, {
        path: '/register',
        name: 'register',
        component: () => import(/* webpackChunkName: "Register" */ './views/Register.vue')
    }, {
        path: '*',
        name: 'NotFound',
        component: () => import(/* webpackChunkName: "404" */ './views/NotFound.vue')
    } ]
})
