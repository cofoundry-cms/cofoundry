import Vue from 'vue';
import Vuex from 'vuex';
import cats from './modules/cats';
import auth from './modules/auth';

Vue.use(Vuex);

export default new Vuex.Store({
    modules: {
        cats,
        auth
    }
});
