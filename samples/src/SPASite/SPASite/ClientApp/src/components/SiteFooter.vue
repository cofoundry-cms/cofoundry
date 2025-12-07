<template>
    <div class="wrapper">
        <footer class="container">
            <a href="https://www.cofoundry.org">
                <img 
                    src="../assets/cofoundry-logo.png" 
                    class="cofoundry-logo" 
                    alt="Cofoundry logo" />
            </a>
            <p>
                This sample is built using <a href="https://www.cofoundry.org">Cofoundry</a >, an open source .NET Core CMS.
            </p>
        </footer>
    </div>
</template>

<script>

export default {
    name: "SiteFooter",
};
</script>

<style scoped lang="scss">

.wrapper {
    background-color: $color-primary;
}

.container {
    
    display: flex;
    flex-direction: column;
    align-items: center;
    padding: 2rem;
    color: #fff;
    text-align: center;
    
    @include respond-min($tablet) {
        max-width: $layout-max-width;
        margin: 0 auto;
    }

    a {
        color: #fff;
    }
}

.cofoundry-logo {
    width: 200px;
}

</style>
