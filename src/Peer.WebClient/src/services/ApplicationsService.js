import { deleteAsync, getAsync, postAsync, putAsync } from './ApiService'
import {
    successResult,
    failureResult,
    errorResult
} from './RequestResult'

async function submitApplication(projectId, positionId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var payload = {
            projectId: projectId,
            positionId: positionId
        }
        var response = await postAsync(`${apiRootUri}/applications`, payload, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application submitted successfully!"
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

export {
    submitApplication
}