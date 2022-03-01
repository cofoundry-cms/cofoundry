export default {

    handleCommandError(error) {
        const response = error.response;

        if (response.request.method !== 'get' && response.status === 400) {
            return Promise.reject(response.data.errors);
        }
        else {
            alert('An unhandled error has occurred');
            return Promise.reject(error);
        }
    },

    handleQueryResponse(response) {
        return response.data.data;
    }
}