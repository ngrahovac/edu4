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

async function getSubmittedApplications(accessToken, projectId, sort) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/applications/sent`;

        var queryParams = [];

        if (projectId != undefined)
            queryParams["projectId"] = projectId;

        if (sort != undefined)
            queryParams["sort"] = sort;

        if (Object.keys(queryParams).length > 0) {
            requestUri += "?";

            for (let parameter in queryParams)
                if (queryParams[parameter] != undefined)
                    requestUri += `${parameter}=${encodeURI(queryParams[parameter])}&`

            requestUri = requestUri.slice(0, -1);
        }

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            let body = await response.json();

            return {
                outcome: successResult,
                message: "Submitted applications retrieved successfully!",
                payload: body
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

async function getIncomingApplications(accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var response = await getAsync(`${apiRootUri}/applications/received`, accessToken);

        if (response.ok) {
            let body = await response.json();

            return {
                outcome: successResult,
                message: "Received applications retrieved successfully!",
                payload: body
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

async function revokeApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/applications/${applicationId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application revoked successfully!"
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

async function rejectApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/applications/${applicationId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application rejected successfully!"
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

async function acceptApplication(applicationId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await putAsync(
            `${apiRootUri}/applications/${applicationId}`,
            {
                status: "Accepted"
            },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Application accepted successfully!"
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

async function getSubmittedApplicationsProjectIds(accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var response = await getAsync(`${apiRootUri}/applications/sent/projects`, accessToken);

        if (response.ok) {
            let body = await response.json();

            return {
                outcome: successResult,
                message: "Submitted applications project ids retrieved successfully!",
                payload: body
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
    submitApplication,
    getSubmittedApplications,
    getIncomingApplications,
    revokeApplication,
    rejectApplication,
    acceptApplication,
    getSubmittedApplicationsProjectIds
}