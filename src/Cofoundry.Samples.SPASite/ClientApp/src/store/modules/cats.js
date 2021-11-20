import currentMemberApi from '@/api/currentMember';
import catsApi from '@/api/cats';

export default {
    namespaced: true,
    state: {
        likedCatIds: []
    },
    mutations: {
        setLikedCatIds(state, catIds) {
            state.likedCatIds = catIds;
        },
        setCatLiked(state, catId) {
            state.likedCatIds.push(catId);
        },
        setCatUnliked(state, catId) {
            state.likedCatIds = state.likedCatIds.filter(id => id !== catId);
        }
    },
    actions: {
        loadSession(context) {
            return currentMemberApi.getLikedCats()
                .then(cats => {
                    const catIds = cats ? cats.map(i => i.catId) : [];
                    context.commit('setLikedCatIds', catIds);
                });
        },

        clearSession(context) {
            context.commit('setLikedCatIds', []);

            return Promise.resolve();
        },

        like(context, catId) {
            if (context.state.likedCatIds.indexOf(catId) !== -1) return Promise.resolve();

            return catsApi.like(catId).then(() => {
                context.commit('setCatLiked', catId);
            });
        },

        unlike(context, catId) {
            if (context.state.likedCatIds.indexOf(catId) === -1) return Promise.resolve();

            return catsApi.unlike(catId).then(() => {
                context.commit('setCatUnliked', catId);
            });
        }
    }
}
