<template>
    <div class="form-group">
        <label :for="id">{{title}}</label>
        <slot></slot>
    </div>
</template>

<script>

export default {
    name: "FormGroup",
    props: {
        title: String,
        id: String
    }
};
</script>

<style scoped lang="scss">

.form-group {
    max-width: 400px;
    margin-bottom: 1rem;
}

label {
    display: block;
    font-weight: bold;
    margin-bottom: 0.5rem;
}

input {
    border: 1px solid #ccc;
    padding: 0.3rem 0.8rem;
    width: 100%;
}
</style>
