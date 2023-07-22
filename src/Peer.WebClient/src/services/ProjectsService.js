import { deleteAsync, getAsync, postAsync, putAsync } from './ApiService'
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
                message: "Project published successfully!"
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

async function addPositions(projectId, positions, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await postAsync(`${apiRootUri}/projects/${projectId}/positions`, positions, accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Positions added successfully!"
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

async function updateDetails(projectId, title, description, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await putAsync(
            `${apiRootUri}/projects/${projectId}/details`,
            { title: title, description: description },
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project details updated successfully!"
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

async function discover(keyword, sort, hat, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;

        var requestUri = `${apiRootUri}/projects`;

        var searchRefinemets = [];

        if (keyword != undefined)
            searchRefinemets["keyword"] = keyword;

        if (sort == "asc") {
            searchRefinemets["sort"] = "byDatePostedAsc";
        } else if (sort == "desc") {
            searchRefinemets["sort"] = "byDatePostedDesc";
        }


        if (hat != undefined) {
            searchRefinemets["hatType"] = hat.type;

            Object.keys(hat.parameters).forEach(k => searchRefinemets[k] = hat.parameters[k]);
        }

        if (Object.keys(searchRefinemets).length > 0) {
            requestUri += "?";

            for (let parameter in searchRefinemets)
                if (searchRefinemets[parameter] != undefined)
                    requestUri += `${parameter}=${encodeURI(searchRefinemets[parameter])}&`

            requestUri = requestUri.slice(0, -1);
        }

        var response = await getAsync(requestUri, accessToken);

        if (response.ok) {
            var body = await response.json();

            return {
                outcome: successResult,
                message: "Signup successfully completed!",
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

async function remove(projectId, accessToken) {
    try {
        const apiRootUri = process.env.REACT_APP_EDU4_API_ROOT_URI;
        var response = await deleteAsync(
            `${apiRootUri}/projects/${projectId}`,
            accessToken);

        if (response.ok) {
            return {
                outcome: successResult,
                message: "Project removed successfully!"
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
    publish,
    addPositions,
    discover,
    updateDetails,
    remove
}