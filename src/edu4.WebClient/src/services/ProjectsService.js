import { postAsync } from './ApiService'
import {
    successResult,
    failureResult,
    errorResult
} from './RequestResult'

async function publish(publishModel, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects`, publishModel, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Signup successfully completed!"
            };
        } else {
            var responseMessage = await response.text();

            return {
                outcome: failureResult,
                message: responseMessage
            };
        }
    } catch (ex) {
        return {
            outcome: errorResult,
            message: "The request failed. Please check your connection and try again."
        };
    }
}

export { publish }