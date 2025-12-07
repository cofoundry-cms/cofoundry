<template>
    <div class="wrapper">
        <div class="content">
            <slot></slot>
        </div>
    </div>
</template>

<script>

export default {
    name: "ContentPanel"
};

</script>

<style scoped lang="scss">

.wrapper {
    margin: 2rem 0;
    flex-grow: 1;
}

.content {
    
    background-color: #fff;
    padding: 1rem;
    margin: 1rem;
    border-radius: 3px;

    @include respond-min($tablet) {
        max-width: $layout-max-width;
        margin: 0 auto;
    }
}

*:first-child {
    margin-top: 0;
}

</style>
