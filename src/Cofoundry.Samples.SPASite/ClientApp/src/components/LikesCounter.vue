<template>
    <span class="likes">{{ numLikes }}</span>
</template>

<script>
export default {
    name: "LikesCounter",
    props: {
        numLikes: Number
    }
};
</script>

<style scoped lang="scss">
.likes {
    color: $color-text;
    font-weight: bold;

    &:before {
        content: "";
        background: url("../assets/heart-icon.png") no-repeat left top;
        background-size: contain;
        width: 0.8em;
        height: 0.8em;
        display: inline-block;
        margin-right: 0.5em;
        top: 0.1em;
        position: relative;
    }
}
</style>
