import authApi from '@/api/auth';

function loadAdditionalSessionData(context) {
    return context.dispatch('cats/loadSession', null, { root: true });
}

export default {
    namespaced: true,

    state: {
        member: null
    },

    mutations: {
        setMember(state, member) {
            state.member = member;
        }
    },

    actions: {
        loadSession(context) {
            loadAdditionalSessionData(context);

            return authApi
                .getSession()
                .then(member => {
                    context.commit('setMember', member);
                });
        },

        register(context, command) {
            return authApi
                .register(command)
                .then(member => {
                    loadAdditionalSessionData(context)
                        .then(() => {
                            context.commit('setMember', member);
                        });
                })
        },

        login(context, command) {
            return authApi
                .login(command)
                .then(member => {
                    loadAdditionalSessionData(context)
                        .then(() => {
                            context.commit('setMember', member);
                        });
                })
        },

        signOut(context) {
            return authApi
                .signOut()
                .then(member => {
                    context.dispatch('cats/clearSession', null, { root: true })
                        .then(() => {
                            context.commit('setMember', member);
                        });
                })
        }
    }
}
