<template>
    <nav class="wrapper">
        <div class="container">
            <router-link to="/" class="logo">SPA Cats</router-link>

            <ul class="menu">
                <li>
                    <router-link to="/" class="menu-link">Home</router-link>
                </li>
                <li>
                    <router-link to="/login" class="menu-link" v-if="!member">Login</router-link>
                </li>
                <li>
                    <router-link to="/register" class="menu-link" v-if="!member">Register</router-link>
                </li>
                <li>
                    <button class="menu-link" v-if="member" @click="signOut">Logout</button>
                </li>
            </ul>
        </div>
    </nav>
</template>

<script>
import { mapState, mapActions } from "vuex";

export default {
    name: "SiteNav",
    computed: mapState("auth", ["member"]),
    methods: mapActions("auth", ["signOut"])
};
</script>

<style scoped lang="scss">
.wrapper {
    background-color: $color-primary;
}

.container {
    
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 0 2rem;

    @include respond-min($tablet) {
        flex-direction: row;
        justify-content: space-between;
        max-width: $layout-max-width;
        margin: 0 auto;
    }
}

.menu {
    list-style-type: none;
    margin: 0;
    padding: 0;
    
    li {
        display: inline-block;
    }

    &-link {
        display: inline-block;
        background-color: transparent;
        border: 0;
        color: #fff;
        padding: 0.5rem 1rem;
        margin: 0.5rem 0;
        text-decoration: none;
        cursor: pointer;
        line-height: 1.5;
        
        &:hover {
            background-color: $color-secondary;
        }
    }

    @include respond-min($tablet) {

        &-link {
            padding: 0 1rem;
            margin: 0;
            line-height: 60px;
        }
    }
}

.logo {
    background: url("../assets/spacats-logo.png") no-repeat left top;
    background-size: cover;
    display: inline-block;
    width: 187px;
    height: 36px;
    margin-top: 1rem;

    @include hide-text();
    
    @include respond-min($tablet) {
        margin-top: 0;
    }
}
</style>
