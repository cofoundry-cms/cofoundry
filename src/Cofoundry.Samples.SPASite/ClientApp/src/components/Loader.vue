<template>
    <div>
        <div class="loader" v-if="isLoading">
            <div class="lds-dual-ring"></div>
        </div>
        <slot  v-if="!isLoading">
        </slot>
    </div>
</template>

<script>

export default {
    name: "Loader",
    props: {
        isLoading: Boolean
    }
};
</script>

<style scoped lang="scss">

.loader {
    min-height: 300px;
    display: flex;

    .lds-dual-ring {
        margin: auto;
        text-align: center;
    }
}

/* Spinner adapted from https://loading.io/css/ */

.lds-dual-ring {
    display: inline-block;
    width: 64px;
    height: 64px;
}
.lds-dual-ring:after {
    content: " ";
    display: block;
    width: 46px;
    height: 46px;
    margin: 1px;
    border-radius: 50%;
    border: 5px solid #fff;
    border-color: #fff transparent #fff transparent;
    animation: lds-dual-ring 1.2s linear infinite;
}
@keyframes lds-dual-ring {
    0% {
        transform: rotate(0deg);
    }
    100% {
        transform: rotate(360deg);
    }
}
</style>
