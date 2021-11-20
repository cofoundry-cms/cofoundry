<template>
    <div class="grid" v-if="result">
        <cat-item v-for="cat in result.items" :cat="cat" :key="cat.catId"/>
    </div>
</template>

<script>
import CatItem from "@/components/CatItem";

export default {
    name: "CatGrid",
    components: {
        CatItem
    },
    props: {
        result: Object
    }
};
</script>

<style scoped lang="scss">
.grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
    grid-auto-rows: minmax(150px, auto);
    grid-gap: 2rem;
    
    margin: 2rem;
}
</style>
