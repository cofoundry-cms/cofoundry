<template>
    <button type="submit" class="btn"><slot>{{title}}</slot></button>
</template>

<script>

export default {
    name: "SubmitButton",
    props: {
        title: String
    }
};
</script>

<style scoped lang="scss">

.btn {
    background-color: $color-secondary;
    color: white;
    padding: 0.6rem 4rem;
    border: none;
    border-radius: 30px;
    transition: background-color 0.2s ease-out;
    width: 100%;
    cursor: pointer;
    
    &:hover {
        background-color: $color-primary;
        color: white;
    }
    
    @include respond-min($tablet) {
        width: unset;
    }
}
</style>
