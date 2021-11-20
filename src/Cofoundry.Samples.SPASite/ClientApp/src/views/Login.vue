<template>
    <content-panel>
        <h1>Login</h1>

        <form
            @submit.prevent="submitLogin"
            v-if="!loginComplete"
        >
            <form-group :title="'Email'" :id="'inputEmail'">
                <input
                    type="email"
                    class="form-control"
                    id="inputEmail"
                    placeholder="Email"
                    required
                    v-model="command.email"
                >
            </form-group>

            <form-group :title="'Password'" :id="'inputPassword'">
                <input
                    type="password"
                    class="form-control"
                    id="inputPassword"
                    placeholder="Password"
                    required
                    v-model="command.password"
                >
            </form-group>

            <validation-summary :errors="errors"/>

            <form-actions>
                <submit-button title="Login"/>
            </form-actions>
        </form>

        <div class="message" v-if="loginComplete">
            <p>Login successful!</p>
            <p><router-link to="/">View the cats</router-link></p>
        </div>
    </content-panel>
</template>

<script>
import accountApi from "@/api/auth";
import ValidationSummary from "@/components/ValidationSummary";
import FormGroup from "@/components/FormGroup";
import ContentPanel from "@/components/ContentPanel";
import FormActions from "@/components/FormActions";
import SubmitButton from "@/components/SubmitButton";

export default {
    name: "login",
    components: {
        ValidationSummary,
        FormGroup,
        ContentPanel,
        FormActions,
        SubmitButton
    },
    data() {
        return {
            loginComplete: false,
            command: {},
            errors: []
        };
    },
    methods: {
        submitLogin() {
            const me = this;

            this.$store
                .dispatch("auth/login", this.command)
                .then(loginComplete)
                .catch(loginFailed);

            function loginComplete() {
                me.loginComplete = true;
                me.errors = [];
            }

            function loginFailed(errors) {
                me.errors = errors;
            }
        }
    }
};
</script>

<style scoped lang="scss">

</style>
