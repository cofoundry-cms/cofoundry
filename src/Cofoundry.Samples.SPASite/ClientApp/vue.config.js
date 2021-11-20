module.exports = {
    css: {
        loaderOptions: {
            sass: {
                data: `
                  @import "@/scss/mixins.scss";
                  @import "@/scss/variables.scss";
                  @import "@/scss/normalize.scss";
                `
            }
        }
    }
}
