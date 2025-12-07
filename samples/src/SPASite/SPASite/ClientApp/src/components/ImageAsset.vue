<template>
    <img :src="url" :title="image.title">
</template>

<script>
export default {
    name: "ImageAsset",
    props: {
        image: Object,
        width: Number,
        height: Number
    },
    computed: {
        url() {
            let url = this.image.url;

            if (this.width > 0 || this.height > 0) {
                let parts = [];

                if (this.width > 0) parts.push("width=" + this.width);
                if (this.height > 0) parts.push("height=" + this.height);

                url += "?" + parts.join("&");
            }

            return url;
        }
    }
};
</script>
