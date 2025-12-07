<template>
    <div v-if="errors.length" class="container">
        <span v-if="errors.length === 1">{{errors[0].message}}</span>
        <ul v-if="errors.length > 1">
            <li v-for="error in errors" :key="error.message">{{error.message}}</li>
        </ul>
    </div>
</template>

<script>
export default {
    name: "ValidationSummary",
    props: {
        errors: Array
    }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
.container {
    padding: 1rem 2rem;
    color: red;
    border: solid 1px rgb(255, 129, 129);
    background-color: rgb(255, 238, 238);
    margin: 2rem 0;
}
</style>
