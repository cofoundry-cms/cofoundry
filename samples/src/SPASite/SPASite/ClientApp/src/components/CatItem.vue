<template>
    <router-link :to="{ name: 'catDetails', params: { id: cat.catId }}" class="cat">
        <div class="image">
            <image-asset :image="cat.mainImage" :width="263" :height="263"/>
        </div>
        <div class="details">
            <span class="name">{{ cat.name }}</span>
            <likes-counter :num-likes="cat.totalLikes"/>
        </div>
    </router-link>
</template>

<script>
import ImageAsset from "@/components/ImageAsset";
import LikesCounter from "@/components/LikesCounter";

export default {
    name: "CatItem",
    components: {
        ImageAsset,
        LikesCounter
    },
    props: {
        cat: Object
    }
};
</script>

<style scoped lang="scss">
.cat {

    background-color: #fff;
    border-radius: 3px;
    text-decoration: none;
    
    &:hover {
        img {
            transform: translate(-50%, -50%) scale(1.1);
        }

        .name {
            color: $color-secondary;
        }
    }
}

.image {
    width: 100%;
    padding-bottom: 100%;
    overflow: hidden;
    position: relative;
    border-radius: 3px 3px 0 0;

    img {
        position: absolute;
        left: 50%;
        top: 50%;
        width: auto;
        height: 100%;
        transform: translate(-50%, -50%);
        transform-origin: 50% 50%;
        transition: transform 1s linear;
    }
}

.details {

    padding: 1rem;
    display: flex;

    .name {
        color: $color-text;
        font-weight: bold;
        font-size: 1.125rem;
        flex-grow: 1;
    }

    .likes {
    }

    @include respond-min(992px) {
        .likes {
        }

        .name {
            display: inline-block;
        }
    }
}
</style>
