<template>
    <content-panel>
        <loader :is-loading="loading" />

        <div v-if="cat">

            <div class="heading">
                <h1 class="title">{{cat.name}}</h1>
                <likes-counter :num-likes="cat.totalLikes" class="num-likes"/>
            </div>

            <dl class="info">
                <dt v-if="cat.breed">Breed:</dt>
                <dd v-if="cat.breed">{{cat.breed.title}}</dd>

                <dt>Characteristics:</dt>
                <dd>{{formattedCharacteristics}}</dd>
                <dt>Description:</dt>
                <dd>{{cat.description}}</dd>
            </dl>
            
            <div class="actions">
                <button class="btn-love" @click="handleLike" v-if="member && !isLiked">Like</button>

                <button class="btn-love" @click="handleLike" v-if="member && isLiked">Un-like</button>
            </div>
                
            <div class="cat-images">
                <image-asset
                    v-for="image in cat.images"
                    :key="image.imageAssetId"
                    :image="image"
                    :width="640"
                    :height="480"
                />
            </div>

        </div>
    </content-panel>
</template>

<script>
import { mapState } from "vuex";
import catsApi from "@/api/cats";
import ImageAsset from "@/components/ImageAsset";
import Loader from "@/components/Loader";
import LikesCounter from "@/components/LikesCounter";
import ContentPanel from "@/components/ContentPanel";

export default {
    name: "catDetails",
    components: {
        ImageAsset,
        Loader,
        LikesCounter,
        ContentPanel
    },
    data() {
        return {
            loading: true,
            cat: null
        };
    },
    computed: {
        isLiked() {
            return this.cat !== null 
                && this.$store.state.cats.likedCatIds.indexOf(this.cat.catId) !== -1;
        },
        formattedCharacteristics() {
            if (!this.cat) return '';

            return this.cat.features.map(f => f.title).join(', ');
        },
        ...mapState("auth", ["member"])
    },
    created() {
        this.loadCat();
    },
    methods: {
        loadCat() {
            this.loading = true;
            catsApi.getCatById(this.$route.params.id).then(result => {
                this.cat = result;
                this.loading = false;
            });
        },
        handleLike() {
            const actionName = this.isLiked ? "unlike" : "like";
            const likeModifier = this.isLiked ? -1 : 1;

            this.$store
                .dispatch("cats/" + actionName, this.cat.catId)
                .then(() => {
                    this.cat.totalLikes += likeModifier;
                });
        }
    }
};
</script>

<style scoped lang="scss">

.heading {
    display: block;

    @include respond-min($tablet) {
        display: flex;
    }
}

.title {
    margin: 0;
    flex-grow: 1;
    font-size: 1.6rem;

    @include respond-min($tablet) {
        font-size: 2rem;
    }
}

.num-likes {
    font-size: 1.6rem;
    
    @include respond-min($tablet) {
        font-size: 2rem;
    }
}

.info {
    dt {
        margin-top: 1rem;
        font-weight: bold;
    }
    dd {
        margin: 0;
    }
}

.cat-images {
    display: flex;
    flex-direction: column;
    
    img {
        width: 100%;
        height: auto;
        margin-bottom: 1rem;
    }
    
    @include respond-min($tablet) {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
        grid-auto-rows: minmax(150px, auto);
        grid-gap: 2rem;

        img {
            margin-bottom: 0;
        }
    }
}

.actions {
    margin: 2rem 0;
}

.btn-love {
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