<template>
    <content-panel>
        <h1>Register</h1>

        <form
            @submit.prevent="submitRegistration"
            v-if="!registrationComplete"
        >
            <form-group :title="'First Name'" :id="'inputName'">
                <input
                    type="text"
                    name="firstname"
                    class="form-control"
                    id="inputName"
                    placeholder="First Name"
                    v-model="command.firstName"
                >
            </form-group>

            <form-group :title="'Last Name'" :id="'inputSurname'">
                <input
                    type="text"
                    name="lastname"
                    class="form-control"
                    id="inputSurname"
                    placeholder="Last Name"
                    v-model="command.lastName"
                >
            </form-group>

            <form-group :title="'Email address'" :id="'inputEmail'">
                <input
                    type="email"
                    name="email"
                    class="form-control"
                    id="inputEmail"
                    placeholder="Email"
                    v-model="command.email"
                >
            </form-group>

            <form-group :title="'Password'" :id="'inputPassword'">
                <input
                    type="password"
                    name="password"
                    class="form-control"
                    id="inputPassword"
                    placeholder="Password"
                    v-model="command.password"
                >
            </form-group>

            <validation-summary :errors="errors"/>

            <form-actions>
                <submit-button title="Register"/>
            </form-actions>
        </form>

        <div v-if="registrationComplete">
            <p>Thank you for registering with SPA Cats! You now have access to extra features, such as favouriting the cats you love the most.</p>
            <p>Enjoy!</p>
            <p><router-link to="/">View the cats</router-link></p>
        </div>
    </content-panel>
</template>

<script>
import ValidationSummary from "@/components/ValidationSummary";
import FormGroup from "@/components/FormGroup";
import ContentPanel from "@/components/ContentPanel";
import FormActions from "@/components/FormActions";
import SubmitButton from "@/components/SubmitButton";

export default {
    name: "registration",
    components: {
        ValidationSummary,
        FormGroup,
        ContentPanel,
        FormActions,
        SubmitButton
    },
    data() {
        return {
            registrationComplete: false,
            command: {},
            errors: []
        };
    },
    methods: {
        submitRegistration() {
            const me = this;

            this.$store
                .dispatch("auth/register", this.command)
                .then(registrationComplete)
                .catch(registrationFailed);

            function registrationComplete() {
                me.registrationComplete = true;
                me.errors = [];
            }

            function registrationFailed(errors) {
                me.errors = errors;
            }
        }
    }
};
</script>
