<template>
    <main>

        <div class="hero">
            <div class="logo">
                <img src="../assets/heart-icon.png" alt="Heart symbol" />
            </div>
            <div class="intro">
                <p>Welcome to SPA Cats, the site where beautiful moggies SPA for your affections. We have some of the smartest, fluffiest kitties battling to become top cat.</p>
                <p>Register now and you can help your favourite cat in the race to become most loved.</p>
            </div>
        </div>

        <div class="cat-grid">
            <loader :is-loading="loading"/>
            <cat-grid v-if="searchResult" :result="searchResult"/>
        </div>
    </main>
</template>

<script>
import CatGrid from "@/components/CatGrid";
import catsApi from "@/api/cats";
import Loader from "@/components/Loader";

export default {
    name: "home",
    components: {
        CatGrid,
        Loader
    },
    data() {
        return {
            loading: false,
            searchResult: null
        };
    },
    created() {
        this.loadGrid();
    },
    watch: {
        $route: "loadGrid"
    },
    methods: {
        loadGrid() {
            this.loading = true;

            catsApi.searchCats().then(result => {
                this.loading = false;
                this.searchResult = result;
            });
        }
    }
};
</script>

<style scoped lang="scss">
.hero {
    padding: 0 2rem 2rem 2rem;
    background-color: $color-primary;
    color: #fff;
    display: flex;
    flex-direction: column;
    align-items: center;

    @include respond-min($tablet) {
        padding: 1rem 2rem 4rem 2rem;
    }
}
        
.logo {
    display: inline-block;
    margin-top: 2rem;

    @include respond-min($tablet) {
    }
}

.intro {
    display: inline-block;
    margin: 1rem 2rem 0 2rem;
    font-size: 1.2rem;
    text-align: center;

    @include respond-min($tablet) {
        max-width: 45rem;
    }
}

.cat-grid {
    
    @include respond-min($tablet) {
        max-width: $layout-max-width;
        margin: 0 auto;
    }
}
</style>
